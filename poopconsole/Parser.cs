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
            ParserTask currently = ParserTask.Nothing;

            char previousChar = ' ';

            foreach(char c in input)
            {
                if(c == ' ' && currently == ParserTask.Nothing)
                {
                    args = args.Append(currentString).ToArray();
                    currentString = "";
                }
                else if (c == '"' && previousChar != '\\')
                {
                    if (currently == ParserTask.ReadingString)
                    {
                        currently = ParserTask.Nothing;
                        args = args.Append(currentString).ToArray();
                        currentString = "";
                    }
                    else if(currently == ParserTask.Nothing)
                    {
                        currently = ParserTask.ReadingString;
                        currentString = "";
                    }else
                    {
                        currentString += c;
                    }
                }
                else if (c == '%' && previousChar != '\\')
                {
                    if (currently == ParserTask.ReadingVariable)
                    {
                        currently = ParserTask.Nothing;

                        if(Program.vars.ContainsKey(currentString))
                        {
                            args = args.Append(Program.vars[currentString]).ToArray();
                        }else
                        {
                            throw new Exception("The variable " + currentString + " is not defined.");
                        }
                        
                        currentString = "";
                    }
                    else if(currently == ParserTask.Nothing)
                    {
                        currently = ParserTask.ReadingVariable;
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

    public enum ParserTask {
        Nothing,
        ReadingString,
        ReadingVariable
    }
}
