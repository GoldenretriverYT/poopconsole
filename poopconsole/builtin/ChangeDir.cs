using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole.builtin
{
    public class ChangeDir : AbstractCommand
    {
        public override string name { get => "cd"; }

        public override void RunCommand(string[] args)
        {
            string addPath = "";

            foreach (string arg in args)
            {
                if (arg == "cd") continue;

                addPath += arg;
            }

            if (addPath.Contains("/") || addPath.Contains("\\"))
            {
                if (Directory.Exists(addPath))
                {
                    Program.path = addPath;
                }
                else
                {
                    Console.WriteLine("Unable to find path");
                }
            }
            else
            {
                if (Directory.Exists(Path.Combine(Program.path, addPath)))
                {
                    Program.path = Path.GetFullPath(Path.Combine(Program.path, addPath));
                }
                else
                {
                    Console.WriteLine("Unable to find path");
                }
            }
        }
    }
}
