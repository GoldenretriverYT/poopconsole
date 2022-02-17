﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole.builtin
{
    public class MkDir : AbstractCommand
    {
        public override string name { get => "mkdir"; }

        public override void RunCommand(string[] args)
        {
            string addPath = args[1];

            if(Directory.Exists(Path.Combine(Program.path, addPath)))
            {
                Console.WriteLine("Error: Directory already exists.");
                return;
            }

            Directory.CreateDirectory(Path.Combine(Program.path, addPath));
            Console.WriteLine("Created directory " + addPath);
        }
    }
}
