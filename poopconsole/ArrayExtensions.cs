using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole
{
    public static class ArrayExtensions
    {
        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        public static string Join<T>(this T[] source, string joiner)
        {
            string output = "";

            foreach(T val in source)
            {
                output += val.ToString() + joiner;
            }

            if (output.Length >= joiner.Length)
            {
                output = output.Substring(0, output.Length - joiner.Length);
            }else
            {
                output = "<empty>";
            }

            return output;
        }
    }
}
