using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game
{
    public abstract class GameComponent
    {
        private GameObject gameObject;
        public GameObject GameObject { get => gameObject; set => gameObject = value; }

        public virtual void Init()
        {

        }

        public abstract void OnRender();

        public abstract void OnUpdate();
    }
}
