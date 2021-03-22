using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Game
{
    /// <summary>
    /// State representing a component's data. Note: ALL inherited classes MUST implement the following method:
    /// public static int FromBytes(byte[] bytes, int startIndex, out ComponentState cs)
    /// </summary>
    public abstract class ComponentState
    {
        protected static Type[] componentTypes = new List<Type>(Assembly.GetExecutingAssembly().GetTypes()).FindAll(x => x.IsSubclassOf(typeof(ComponentState))).ToArray();

        protected const int HEADER_SIZE = 1;

        public abstract GameComponent ToComponent();

        public abstract int ToBytes(byte[] array, int startIndex);

        protected static int GetTypeIndex(Type t)
        {
            return Array.FindIndex(componentTypes, x => x.Equals(t));
        }

        protected void GetHeader(byte[] array, int index)
        {
            if (array.Length - index < HEADER_SIZE)
                throw new Exception("Invalid array given");

            array[index] = (byte)GetTypeIndex(GetType());
        }

        public static int FromBytes(byte[] array, int startIndex, out ComponentState result)
        {
            int index = startIndex;
            Type t = componentTypes[array[index++]];
            MethodInfo fromBytesMethod = t.GetMethod("FromBytes");
            if (fromBytesMethod == null)
            {
                Console.WriteLine("Invalid state given. Type: " + t);
                result = null;
                return index - startIndex;
            }

            object[] parameters = new object[] { array, index, null };
            index += (int)fromBytesMethod.Invoke(null, parameters);
            result = (ComponentState)parameters[2];
            //result = (ComponentState)t.GetConstructor(new Type[] { typeof(byte[]), typeof(int) })?.Invoke(new object[] { array, index });
            return index - startIndex;
        }
    }
}
