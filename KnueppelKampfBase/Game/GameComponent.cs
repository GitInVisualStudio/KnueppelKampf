using System;
using System.Collections.Generic;
using System.Text;

namespace KnueppelKampfBase.Game
{
    /// <summary>
    /// oberklasse für alle komponenten eines objektes
    /// verleiht dieses eigenschaften
    /// </summary>
    public abstract class GameComponent
    {
        private GameObject gameObject;
        public GameObject GameObject { get => gameObject; set => gameObject = value; }

        public virtual void Init()
        {

        }

        public abstract void OnRender();

        public abstract void OnUpdate();

        public abstract ComponentState GetState();

        public abstract void ApplyState(ComponentState state);
    }
}
