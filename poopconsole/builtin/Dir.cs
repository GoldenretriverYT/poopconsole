﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole.builtin
{
    public class Dir : AbstractCommand
    {
        public override string name { get => "dir"; }

        public override void RunCommand(string[] args, ref WrappedStreamWriter sw)
        {
            string[] dirs = Directory.GetDirectories(Program.path);
            string[] files = Directory.GetFiles(Program.path);

            foreach(string dir in dirs)
            {
                sw.WriteLine("D | " + Path.GetFileName(dir));
            }

            foreach(string file in files)
            {
                sw.WriteLine("F | " + Path.GetFileName(file) + " | " + Math.Floor(new FileInfo(file).Length / 102.4d) * 10 + "kb");
            }
        }
    }
}
