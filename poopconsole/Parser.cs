using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poopconsole
{
    public class Parser
    {
        public static string[] GetArgs(string input)
        {
            string[] args = new string[] { };

            string currentString = "";
            bool stringing = false;

            char previousChar = ' ';

            foreach(char c in input)
            {
                if(c == ' ' && !stringing)
                {
                    args = args.Append(currentString).ToArray();
                    currentString = "";
                }else if(c == '"' && previousChar != '\\')
                {
                    if(stringing)
                    {
                        stringing = false;
                        args = args.Append(currentString).ToArray();
                        currentString = "";
                    }else
                    {
                        stringing = true;
                        currentString = "";
                    }
                }else
                {
                    currentString += c;
                }
            }

            if (!(currentString == "")) args = args.Append(currentString).ToArray();

            return args;
        }
    }
}
