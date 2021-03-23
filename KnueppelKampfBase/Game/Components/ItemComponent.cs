using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnueppelKampfBase.Game.Objects;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;

namespace KnueppelKampfBase.Game.Components
{
    public class ItemComponent : GameComponent
    {
        private Item item;
        private float cooldown;
        public Item Item { get => item; set => item = value; }
        public float Cooldown { get => cooldown; set => cooldown = value; }

        public override void ApplyState(ComponentState state)
        {
            
        }

        public override ComponentState GetState()
        {
            return new ItemState() { Type = item == null ? -1 : (int)item.Type };
        }

        public override void OnRender()
        {
            if (item == null)
                return;
            StateManager.DrawImage(item.Image, default);
        }

        public void Attack()
        {
            if (Cooldown != 0)
                return;
            Cooldown = 10;
            if(item == null)
            {
                //TODO: nur mit der faust schlagen, so wie der herr
                foreach(HealthComponent enemy in GameObject.Manager.SelectComponents<HealthComponent>())
                {
                    if(enemy.GameObject != this.GameObject)
                    {
                        Vector dir = enemy.GameObject.Position - GameObject.Position;
                        if (dir > 100)
                            continue;
                        enemy.Health -= 2.0f;
                        MoveComponent move = enemy.GameObject.GetComponent<MoveComponent>();
                        if (move == null)
                            continue;
                        dir.Length = 5;
                        move.Velocity += dir;
                    }
                }
                return;
            }

            //TODO: add the projectile of the item
            GameObject.Manager.AddObject(new Projectile(GameObject, item.Damage));
        }

        public override void OnUpdate()
        {
            if (Cooldown != 0)
                Cooldown--;
        }
    }

    public class ItemState : ComponentState
    {
        private int type;

        public int Type { get => type; set => type = value; }

        public override int ToBytes(byte[] array, int startIndex)
        {
            GetHeader(array, startIndex);
            BitConverter.GetBytes(type).CopyTo(array, startIndex + HEADER_SIZE);
            return sizeof(int) + HEADER_SIZE;
        }

        public override GameComponent ToComponent()
        {
            return new ItemComponent(); // TODO @jamin
        }

        public static int FromBytes(byte[] bytes, int startIndex, out ItemState cs)
        {
            cs = new ItemState();
            cs.Type = BitConverter.ToInt32(bytes, startIndex);
            return sizeof(int);
        }
    }
}
