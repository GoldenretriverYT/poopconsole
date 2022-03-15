using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole.builtin
{
    public class Setvar : AbstractCommand
    {
        public override string name { get => "setvar"; }

        public override void RunCommand(string[] args, ref WrappedStreamWriter sw)
        {
            if(args.Length < 3)
            {
                sw.WriteLine("Syntax error");
                return;
            }

            Program.vars[args[1]] = args[2];
        }
    }
}
