using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole.builtin
{
    public class Repeat : AbstractCommand
    {
        public override string name { get => "repeat"; }

        public override void RunCommand(string[] args, ref WrappedStreamWriter sw)
        {
            if(args.Length < 3)
            {
                sw.WriteLine("Syntax error");
                return;
            }

            bool parseSuccess = Int32.TryParse(args[1], out int times);

            if(!parseSuccess)
            {
                sw.WriteLine("Argument times must be a valid 32-bit integer.");
                return;
            }

            for(var i = 0; i < times; i++)
            {
                string joinedArgs = args.Skip(2).ToArray().Join(" ");
                string[] commands = joinedArgs.Split(';');

                foreach(string cmd in commands)
                {
                    Program.Run(cmd, true);
                }
            }
        }
    }
}
