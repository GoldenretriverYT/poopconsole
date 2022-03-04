using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole.builtin
{
    public abstract class AbstractCommand
    {
        public abstract string name
        {
            get;
        }

        public abstract void RunCommand(string[] args, ref WrappedStreamWriter sw);
    }
}
