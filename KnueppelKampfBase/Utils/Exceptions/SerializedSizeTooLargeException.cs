using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Utils.Exceptions
{
    public class SerializedSizeTooLargeException : Exception
    {
        public SerializedSizeTooLargeException(int size) : base($"Serialized size of {size} bytes exceeded 256 bytes.")
        {

        }
    }
}
