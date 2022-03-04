using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole.builtin
{
    public class Echo : AbstractCommand
    {
        public override string name { get => "echo"; }

        public override void RunCommand(string[] args, ref WrappedStreamWriter sw)
        {
            if(args.Length > 1)
                sw.WriteLine(args.Skip(1).ToArray().Join(" "));
        }
    }
}
