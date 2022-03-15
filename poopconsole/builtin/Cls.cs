using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole.builtin
{
    public class Cls : AbstractCommand
    {
        public override string name { get => "cls"; }

        public override void RunCommand(string[] args, ref WrappedStreamWriter sw)
        {
            Console.Clear();
        }
    }
}
