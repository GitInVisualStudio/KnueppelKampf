using KnueppelKampfBase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public ComponentDelta(byte[] bytes, int startIndex)
        {
            int index = startIndex;
            int length = bytes[index++];
            changedProperties = new Dictionary<byte, object>(length);
            PropertyInfo[] properties = typeof(ComponentState).GetProperties();
            for (int i = 0; i < length; i++)
            {
                byte key = bytes[index++];
                int size = bytes[index++];
                Type t = properties[key].PropertyType;
                byte[] objBytes = new byte[size];
                Array.Copy(bytes, index, objBytes, 0, size);
                object value = ByteUtils.FromBytes(objBytes, t);
                changedProperties[key] = value;
            }
        }

        public int ToBytes(byte[] array, int startIndex)
        {
            int index = startIndex;
            array[index++] = (byte)changedProperties.Count;
            foreach (byte key in changedProperties.Keys)
            { 
                object value = changedProperties[key];
                Type t = value.GetType();
                if (!t.IsValueType)
                    continue;
                array[index++] = key;
                index += ByteUtils.GetBytesAddSize(value, array, index);
            }
            return index - startIndex;
        }
    }
}
