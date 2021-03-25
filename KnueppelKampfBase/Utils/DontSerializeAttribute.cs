using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Utils
{
    /// <summary>
    /// Attribute used to mark a property as non-serialized to prevent redundant information being sent
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DontSerializeAttribute : Attribute
    {
        public DontSerializeAttribute()
        {

        }
    }
}
