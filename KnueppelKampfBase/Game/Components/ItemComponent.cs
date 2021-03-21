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
            return new ItemState() { Type = (int)item.Type };
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
                foreach(HealthComponent enemy in WorldManager.Instance.SelectComponents<HealthComponent>())
                {
                    if(enemy.GameObject != this.GameObject)
                    {
                        Vector dir = enemy.GameObject.Position - GameObject.Position;
                        if (dir > 100)
                            return;
                        enemy.Health -= 2.0f;
                        MoveComponent move = enemy.GameObject.GetComponent<MoveComponent>();
                        if (move == null)
                            return;
                        dir.Length = 5;
                        move.Velocity += dir;
                    }
                }
                return;
            }

            //TODO: add the projectile of the item
            WorldManager.Instance.Entities.Add(new Projectile(GameObject, item.Damage));
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
    }
}
