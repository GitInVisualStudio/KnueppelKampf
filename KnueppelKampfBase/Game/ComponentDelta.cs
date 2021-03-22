using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    public class ComponentDelta
    {
        private Dictionary<byte, object> changedProperties;

        public Dictionary<byte, object> ChangedProperties { get => changedProperties; set => changedProperties = value; }

        public ComponentDelta(ComponentState oldState, ComponentState newState)
        {
            changedProperties = WorldDelta.GetChangedProperties(oldState, newState);
        }
    }
}
