using KnueppelKampfBase.Utils;
using KnueppelKampfBase.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    public class ObjectState
    {
        private GameObject obj;
        private List<ComponentState> componentStates;
        private object[] propertyValues;

        public ObjectState(GameObject obj = null)
        {
            this.obj = obj;
            componentStates = new List<ComponentState>();
            if (obj != null)
                lock (obj)
                {
                    PropertyInfo[] properties = obj.GetType().GetProperties();
                    propertyValues = new object[properties.Length];
                    for (int i = 0; i < properties.Length; i++)
                        propertyValues[i] = properties[i].GetValue(obj);

                    foreach (GameComponent c in obj.Components)
                    {
                        ComponentState s = c.GetState();
                        if (s != null)
                            componentStates.Add(s);
                    }
                }
        }

        public int Id => obj.Id;
        public List<ComponentState> ComponentStates { get => componentStates; set => componentStates = value; }
        public GameObject Obj { get => obj; set => obj = value; }
        public object[] PropertyValues { get => propertyValues; set => propertyValues = value; }

        public void Apply()
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
                properties[i].SetValue(Obj, propertyValues[i]);

            for (int i = 0; i < Obj.Components.Count; i++)
                Obj.Components[i].ApplyState(componentStates[i]);
        }

        private static int GetIndexForType(Type t)
        {
            return Array.FindIndex(GameObject.ObjectTypes, x => x.Equals(t));
        }

        public static int FromBytes(byte[] bytes, int startIndex, int length, out GameObject obj)
        {
            //obj = new GameObject();
            int index = startIndex;
            int endIndex = startIndex + length;
            int typeIndex = bytes[index++];
            Type objectType = GameObject.ObjectTypes[typeIndex];
            obj = (GameObject)Activator.CreateInstance(objectType);
            PropertyInfo[] properties = objectType.GetProperties();

            // deserialize properties
            for (int i = 0; i < properties.Length && index < endIndex; i++)
            {
                PropertyInfo prop = properties[i];
                Type t = prop.PropertyType;
                if (!t.IsValueType || prop.GetCustomAttribute<DontSerializeAttribute>() != null)
                    continue;
                int size = bytes[index++];
                byte[] objBytes = new byte[size];
                Array.Copy(bytes, index, objBytes, 0, size);
                object value = ByteUtils.FromBytes(objBytes, t);
                prop.SetValue(obj, value);
                index += size;
            }

            // deserialize components
            while (index < endIndex)
            {
                ComponentState cs;
                index += ComponentState.FromBytes(bytes, index, out cs);
                GameComponent gc = cs.ToComponent();
                obj.AddComponent(gc, false);
            }

            foreach (GameComponent gc in obj.Components)
                gc.Init();

            return index - startIndex;
        }

        public int ToBytes(byte[] array, int startIndex)
        {
            int index = startIndex;
            array[index++] = (byte)GetIndexForType(obj.GetType());
            index += PropertiesToBytes(array, index);
            index += ComponentsToBytes(array, index);            

            return index - startIndex;
        }

        private int PropertiesToBytes(byte[] array, int startIndex)
        {
            int index = startIndex;
            PropertyInfo[] properties = obj.GetType().GetProperties();
            for (int i = 0; i < propertyValues.Length; i++)
            {
                PropertyInfo property = properties[i];
                if (!property.PropertyType.IsValueType || property.GetCustomAttribute<DontSerializeAttribute>() != null)
                    continue;
                object value = propertyValues[i];
                int size = ByteUtils.GetBytes(value, array, index + 1);
                if (size > byte.MaxValue)
                    throw new SerializedSizeTooLargeException(size);
                array[index++] = (byte)size;
                index += size;
            }
            return index - startIndex;
        }

        private int ComponentsToBytes(byte[] array, int startIndex)
        {
            int index = startIndex;
            foreach (ComponentState cs in componentStates)
            {
                int size = cs.ToBytes(array, index);
                //array[index++] = (byte)size;
                index += size;
            }
            return index - startIndex;
        }
    }
}
