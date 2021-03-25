using KnueppelKampfBase.Game.Components;
using KnueppelKampfBase.Math;
using KnueppelKampfBase.Properties;
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
            type = Items.GUN;
            //für jetzt gibt es einfach nur ein item, und zwar die pistole
            image = Resources.pistol;
            damage = 2;
            size = new Vector(50, 50);
            AddComponent(new BoxComponent(OnPickup));
        }

        public Item(Items type)
        {
            this.Type = type;
            //TODO: get the image from the type
            this.image = null;
            this.damage = 2;
            this.size = new Vector(50, 50);
            this.AddComponent(new BoxComponent(OnPickup));
        }

        public Bitmap Image { get => image; set => image = value; }
        public Items Type { get => type; set => type = value; }
        public float Damage { get => damage; set => damage = value; }


        /// <summary>
        /// setzt das item zu dem spieler in die Hand und lässt das objekt verschwinden
        /// </summary>
        /// <param name="b"></param>
        private void OnPickup(BoxComponent b)
        {
            if (!(b.GameObject is Player))
                return;
            ItemComponent item = b.GameObject.GetComponent<ItemComponent>();
            if (item.Item == type)
                return;
            item.Item = type;
            this.Despawn = true;
        }

        public override void OnRender()
        {
            //zeichnet einfach nur das bild
            StateManager.DrawImage(image, position);
        }
    }

    /// <summary>
    /// hilfe um zu erkennen welche items es gibt und welches item der spieler hat
    /// </summary>
    public enum Items : int
    {
        HAND = 0,
        GUN = 1
    }
}
