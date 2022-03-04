using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole
{
    public class WrappedStreamWriter
    {
        private StreamWriter sw = null;
        public Stream BaseStream { get => sw.BaseStream; }

        public WrappedStreamWriter(StreamWriter sw)
        {
            this.sw = sw;
        }

        public void WriteLine(string line)
        {
            sw.BaseStream.Position = sw.BaseStream.Length;
            sw.WriteLine(line);
            sw.Flush();
            sw.BaseStream.Position = 0;
        }

        public void Write(string str)
        {
            sw.BaseStream.Position = sw.BaseStream.Length;
            sw.Write(str);
            sw.Flush();
            sw.BaseStream.Position = 0;
        }
    }
}
