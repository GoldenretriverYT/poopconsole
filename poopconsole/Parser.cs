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
            if(input == null) return new string[0];

            string[] args = new string[] { };

            string currentString = "";
            string currently = "nothing";

            char previousChar = ' ';

            foreach(char c in input)
            {
                if(c == ' ' && currently == "nothing")
                {
                    args = args.Append(currentString).ToArray();
                    currentString = "";
                }
                else if (c == '"' && previousChar != '\\')
                {
                    if (currently == "reading string")
                    {
                        currently = "nothing";
                        args = args.Append(currentString).ToArray();
                        currentString = "";
                    }
                    else if(currently == "nothing")
                    {
                        currently = "reading string";
                        currentString = "";
                    }else
                    {
                        currentString += c;
                    }
                }
                else if (c == '%' && previousChar != '\\')
                {
                    if (currently == "reading var")
                    {
                        currently = "nothing";

                        if(Program.vars.ContainsKey(currentString))
                        {
                            args = args.Append(Program.vars[currentString]).ToArray();
                        }else
                        {
                            throw new Exception("The variable " + currentString + " is not defined.");
                        }
                        
                        currentString = "";
                    }
                    else if(currently == "nothing")
                    {
                        currently = "reading var";
                        currentString = "";
                    }else
                    {
                        currentString += c;
                    }
                }
                else
                {
                    currentString += c;
                }
            }

            if (!(currentString == "")) args = args.Append(currentString).ToArray();

            return args;
        }
    }
}
