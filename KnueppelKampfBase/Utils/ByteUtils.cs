using KnueppelKampfBase.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Utils
{
    /// <summary>
    /// Utils for converting structures to bytes and vice versa
    /// </summary>
    public class ByteUtils
    {
        public static int GetBytes(object obj, byte[] array, int index)
        {
            Type t = obj.GetType();
            if (!t.IsValueType)
                throw new Exception("Non-struct given");
            Type outputType;
            if (t.IsEnum) // enums need to be handled differently bc c# is not fun
            {
                outputType = typeof(int);
                obj = (int)obj;
            }
            else
                outputType = obj.GetType();
            int size = Marshal.SizeOf(outputType);

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, false);
            Marshal.Copy(ptr, array, index, size);
            Marshal.DestroyStructure(ptr, outputType);
            Marshal.FreeHGlobal(ptr);
            return size;
        }

        /// <summary>
        /// Writes given structure to array and prepends its size in bytes
        /// </summary>
        public static int GetBytesAddSize(object obj, byte[] array, int index)
        {
            int size = GetBytes(obj, array, index + 1);
            if (size > byte.MaxValue)
                throw new SerializedSizeTooLargeException(size);
            array[index] = (byte)size;
            return size + 1;
        }

        public static object FromBytes(byte[] bytes, Type t)
        {
            if (!t.IsValueType)
                throw new Exception("Non-struct type given");

            object obj;

            if (t.IsEnum)
            {
                obj = 0;
                t = typeof(int);
            }
            else
                obj = Activator.CreateInstance(t);

            int size = Marshal.SizeOf(obj);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(bytes, 0, ptr, size);

            obj = Marshal.PtrToStructure(ptr, t);
            Marshal.FreeHGlobal(ptr);

            return obj;
        }
    }
}
