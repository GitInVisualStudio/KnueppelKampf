using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game.Objects
{
    public class Item : GameObject
    {
        private Items type;
        private Bitmap image;
        private float damage;

        public Item()
        {
            type = Items.TEST;
            image = null;
        }

        public Item(Items type)
        {
            this.Type = type;
            //TODO: get the image from the type
            this.image = null;
            this.damage = 2;
            this.size = new Vector(50, 50);
            this.AddComponent(new MoveComponent());
            this.AddComponent(new BoxComponent(OnPickup));
        }

        public Bitmap Image { get => image; set => image = value; }
        public Items Type { get => type; set => type = value; }
        public float Damage { get => damage; set => damage = value; }

        private void OnPickup(BoxComponent b)
        {
            if (!(b.GameObject is Player))
                return;
            ItemComponent item = b.GameObject.GetComponent<ItemComponent>();
            item.Item = this;
            this.Despawn = true;
        }

        public enum Items : int
        {
            TEST = 0,
        }

        public override void OnRender()
        {
            //base.OnRender();
            StateManager.DrawString("Ich bin ein item", position);
        }
    }
}
