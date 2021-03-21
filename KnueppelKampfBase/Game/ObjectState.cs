using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    public class ObjectState
    {
        private GameObject obj;
        private List<ComponentState> componentStates;

        public ObjectState(GameObject obj)
        {
            this.Obj = obj;
            lock (obj)
                componentStates = obj.Components.Select(component => component.GetState()).ToList();
        }

        public int Id => obj.Id;
        public List<ComponentState> ComponentStates { get => componentStates; set => componentStates = value; }
        public GameObject Obj { get => obj; set => obj = value; }

        public void Apply()
        {
            for (int i = 0; i < Obj.Components.Count; i++)
                Obj.Components[i].ApplyState(componentStates[i]);
        }
    }
}
