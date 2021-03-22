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
        private Type objectType;

        public Dictionary<byte, object> ChangedProperties { get => changedProperties; set => changedProperties = value; }
        public List<ComponentDelta> ChangedComponents { get => changedComponents; set => changedComponents = value; }
        public int EntityId { get => entityId; }

        public ObjectDelta(ObjectState oldState, ObjectState newState)
        {
            entityId = oldState.Obj.Id;
            objectType = oldState.Obj.GetType();
            changedProperties = new Dictionary<byte, object>();
            object[] oldProperties = oldState.PropertyValues;
            object[] newProperties = newState.PropertyValues;
            for (int i = 0; i < oldState.PropertyValues.Length; i++)
            {
                object oldValue = oldProperties[i];
                object newValue = newProperties[i];
                if (newValue.GetType().IsValueType && ((newValue == null && oldValue != null) || !newValue.Equals(oldValue)))
                    changedProperties[(byte)i] = newValue;
            }
            changedComponents = new List<ComponentDelta>();
            for (int i = 0; i < oldState.ComponentStates.Count; i++)
            {
                ComponentDelta d = new ComponentDelta(oldState.ComponentStates[i], newState.ComponentStates[i]);
                if (d.ChangedProperties.Count > 0)
                    changedComponents.Add(d);
            }
        }   

        public ObjectDelta(byte[] bytes, int startIndex)
        {
            int index = startIndex;
            changedProperties = new Dictionary<byte, object>();
            changedComponents = new List<ComponentDelta>();
            entityId = BitConverter.ToInt32(bytes, index);
            index += sizeof(int);
            int typeIndex = bytes[index++];
            objectType = GameObject.ObjectTypes[typeIndex];

            // deserialize changed properties
            PropertyInfo[] properties = objectType.GetProperties();
            int length = bytes[index++];
            changedProperties = new Dictionary<byte, object>(length);
            for (int i = 0; i < length; i++)
            {
                byte key = bytes[index++];
                int size = bytes[index++];
                byte[] objBytes = new byte[size];
                Array.Copy(bytes, index, objBytes, 0, size);
                Type t = properties[key].PropertyType;
                changedProperties[key] = ByteUtils.FromBytes(objBytes, t);
                index += size;
            }

            // deserialize changed components
            length = bytes[index++];
            changedComponents = new List<ComponentDelta>(length);
            for (int i = 0; i < length; i++)
            {
                int size = bytes[index++];
                changedComponents.Add(new ComponentDelta(bytes, index));
                index += size;
            }
        }
        
        public int ToBytes(byte[] array, int startIndex)
        {
            int index = startIndex;
            BitConverter.GetBytes(entityId).CopyTo(array, index);
            index += sizeof(int);
            array[index++] = (byte)GameObject.GetTypeIndex(objectType);

            int propertyCountIndex = index++;
            int propertyCount = 0; // number of serialized properties
            // serialize changed properties
            PropertyInfo[] properties = objectType.GetProperties();
            foreach (int key in changedProperties.Keys)
            {
                PropertyInfo property = properties[key];
                Type t = property.PropertyType;
                if (!t.IsValueType || property.GetCustomAttribute<DontSerializeAttribute>() != null)
                    continue;
                object value = changedProperties[(byte)key];
                array[index++] = (byte)key;
                int size = ByteUtils.GetBytesAddSize(value, array, index);
                index += size;
                propertyCount++;
            }
            array[propertyCountIndex] = (byte)propertyCount;

            // serialize changed copmonents
            array[index++] = (byte)changedComponents.Count;
            foreach (ComponentDelta cd in changedComponents)
            {
                byte size = (byte)cd.ToBytes(array, index + 1);
                array[index++] = size;
                index += size;
            }
            return index - startIndex;
        }
    }
}
