using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnueppelKampfBase.Utils
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DontSerializeAttribute : Attribute
    {
        public DontSerializeAttribute()
        {

        }
    }
}
