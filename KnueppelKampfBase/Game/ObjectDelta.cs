using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    public class ObjectDelta
    {
        private int entityId;
        private Dictionary<byte, object> changedProperties;
        private List<ComponentDelta> changedComponents;

        public Dictionary<byte, object> ChangedProperties { get => changedProperties; set => changedProperties = value; }
        public List<ComponentDelta> ChangedComponents { get => changedComponents; set => changedComponents = value; }
        public int EntityId { get => entityId; }

        public ObjectDelta(ObjectState oldState, ObjectState newState)
        {
            entityId = oldState.Obj.Id;
            changedProperties = WorldDelta.GetChangedProperties(oldState, newState);
            changedComponents = new List<ComponentDelta>();
            for (int i = 0; i < oldState.ComponentStates.Count; i++)
            {
                ComponentDelta d = new ComponentDelta(oldState.ComponentStates[i], newState.ComponentStates[i]);
                if (d.ChangedProperties.Count > 0)
                    changedComponents.Add(d);
            }
        }   

        public ObjectDelta(byte[] bytes, int startIndex, int endIndex)
        {
            int index = startIndex;
            changedProperties = new Dictionary<byte, object>();
            changedComponents = new List<ComponentDelta>();
            entityId = BitConverter.ToInt32(bytes, index);
            index += sizeof(int);
            PropertyInfo[] properties = typeof(ObjectState).GetProperties();
            while (index < endIndex)
            {
                int size = bytes[index++];
                byte key = bytes[index++];
                byte[] objBytes = new byte[size];
                Array.Copy(bytes, index, objBytes, 0, size);
                Type t = properties[key].PropertyType;
                changedProperties[key] = ByteUtils.FromBytes(objBytes, t);
            }
        }
        
        public int ToBytes(byte[] array, int startIndex)
        {
            int index = startIndex;
            BitConverter.GetBytes(entityId).CopyTo(array, index);
            index += sizeof(int);

            // serialize changed properties
            foreach (int key in changedProperties.Keys)
            {
                object value = changedProperties[(byte)key];
                array[index++] = (byte)key;
                int size = ByteUtils.GetBytesAddSize(value, array, index);
                index += size;
            }

            // serialize changed copmonents // TODO
            return index - startIndex;
        }
    }
}
