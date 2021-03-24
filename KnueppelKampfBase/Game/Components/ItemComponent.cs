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
        private Items item;
        private float cooldown;
        public float Cooldown { get => cooldown; set => cooldown = value; }
        public Items Item { get => item; set => item = value; }

        public override void ApplyState(ComponentState state)
        {
            ItemState var1 = (ItemState)state;
            this.Item = (Items)var1.Type;
        }

        public override ComponentState GetState()
        {
            return new ItemState() { Type = (int)Item };
        }

        public override void OnRender()
        {
            if (Item == Items.HAND)
                return;
            //StateManager.DrawImage(item.Image, default);
        }

        public void Attack()
        {
            if (Cooldown != 0)
                return;
            Cooldown = 10;
            const float reduce = 5f;
            if(Item == Items.HAND)
            {
                //TODO: nur mit der faust schlagen, so wie der herr
                foreach(HealthComponent enemy in GameObject.Manager.SelectComponents<HealthComponent>())
                {
                    if(enemy.GameObject != this.GameObject)
                    {
                        Vector dir = enemy.GameObject.Position - GameObject.Position;
                        if (dir > 100)
                            continue;
                        ControlComponent control = enemy.GameObject.GetComponent<ControlComponent>();
                        enemy.Health -= control.Blocking ? 2 / reduce : 2.0f;
                        MoveComponent move = enemy.GameObject.GetComponent<MoveComponent>();
                        if (move == null)
                            continue;
                        dir.Length = control.Blocking ? 5 / reduce : 5;
                        move.Velocity += dir;
                    }
                }
                return;
            }
            //TODO: add the projectile of the item
            GameObject.Manager.AddObject(new Projectile(GameObject, 2));
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
            return new ItemComponent()
            {
                Item = (Items)Type
            }; // TODO @jamin
        }

        public static int FromBytes(byte[] bytes, int startIndex, out ItemState cs)
        {
            cs = new ItemState();
            cs.Type = BitConverter.ToInt32(bytes, startIndex);
            return sizeof(int);
        }
    }
}
