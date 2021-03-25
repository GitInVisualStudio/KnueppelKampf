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
        private Type componentStateType;

        public Dictionary<byte, object> ChangedProperties { get => changedProperties; set => changedProperties = value; }
        public Type ComponentStateType { get => componentStateType; set => componentStateType = value; }

        public ComponentDelta(ComponentState oldState, ComponentState newState)
        {
            changedProperties = WorldDelta.GetChangedProperties(oldState, newState);
            ComponentStateType = oldState.GetType();
        }

        /// <summary>
        /// Generates a new ComponentDelta object from a byte array
        /// </summary>
        /// <param name="startIndex">The first index belonging to this object</param>
        public ComponentDelta(byte[] bytes, int startIndex)
        {
            int index = startIndex;
            int typeIndex = bytes[index++];
            ComponentStateType = ComponentState.ComponentTypes[typeIndex];
            int length = bytes[index++];
            changedProperties = new Dictionary<byte, object>(length);
            
            PropertyInfo[] properties = ComponentStateType.GetProperties();
            for (int i = 0; i < length; i++)
            {
                byte key = bytes[index++];
                int size = bytes[index++];
                Type t = properties[key].PropertyType;
                byte[] objBytes = new byte[size];
                Array.Copy(bytes, index, objBytes, 0, size);
                object value = ByteUtils.FromBytes(objBytes, t);
                changedProperties[key] = value;
                index += size;
            }
        }

        public int ToBytes(byte[] array, int startIndex)
        {
            int index = startIndex;
            array[index++] = (byte)ComponentState.GetTypeIndex(ComponentStateType);
            int changedPropertyCountIndex = index++;
            int changedPropertyCount = 0;
            PropertyInfo[] properties = componentStateType.GetProperties();
            foreach (byte key in changedProperties.Keys)
            { 
                object value = changedProperties[key];
                Type t = value.GetType();
                if (!t.IsValueType || properties[key].GetCustomAttribute<DontSerializeAttribute>() != null) // make sure only structs are serialized
                    continue;
                array[index++] = key;
                index += ByteUtils.GetBytesAddSize(value, array, index);
                changedPropertyCount++;
            }
            array[changedPropertyCountIndex] = (byte)changedPropertyCount;
            return index - startIndex;
        }
    }
}
