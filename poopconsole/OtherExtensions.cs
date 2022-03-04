using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole
{
    public static class OtherExtensions
    {
        public static WrappedStreamWriter Wrap(this StreamWriter sw)
        {
            return new WrappedStreamWriter(sw);
        }
    }
}
