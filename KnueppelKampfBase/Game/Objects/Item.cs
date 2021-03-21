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
        public Item(Items type)
        {
            this.Type = type;
            //TODO: get the image from the type
            this.image = null;
        }

        public Bitmap Image { get => image; set => image = value; }
        public Items Type { get => type; set => type = value; }
        public float Damage { get => damage; set => damage = value; }

        public enum Items : int
        {
            TEST = 0,
        }
    }
}
