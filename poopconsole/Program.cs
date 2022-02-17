using poopconsole.builtin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace poopconsole
{
    internal class Program
    {
        public static string path = @"C:\";
        public static List<AbstractCommand> commands = new();

        public static Process proc;

        static void Main(string[] launchArgs)
        {
            commands.Add(new ChangeDir());

            Console.WriteLine("welcom to poopconsol");
            Console.WriteLine("i moved you to C: C:");

            while (true) {
                Console.Write(path + " > ");

                string input = Console.ReadLine();
                string[] args = input.Split(' ');

                if(args.Length > 0)
                {
                    if (args[0].StartsWith("./"))
                    {
                        string programName = args[0].Split("./")[1];

                        if (File.Exists(Path.Combine(path, programName)))
                        {
                            string stringArgs = "";

                            foreach(string arg in args)
                            {
                                if (arg == args[0]) continue;
                                stringArgs += arg + " ";
                            }

                            if(stringArgs.Length > 0)
                                stringArgs = stringArgs.Substring(0, stringArgs.Length - 1);

                            proc = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = Path.Combine(path, programName),
                                    Arguments = "",
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    RedirectStandardInput = true,
                                    CreateNoWindow = true
                                }
                            };

                            proc.Start();

                            /*proc.OutputDataReceived += OnProcessOutputReceived;
                            proc.ErrorDataReceived += OnProcessOutputReceived;

                            proc.BeginOutputReadLine();
                            proc.BeginErrorReadLine();*/

                            var _ = ConsumeReader(proc.StandardOutput);
                            var _2 = ConsumeReader(proc.StandardError);

                            while (!proc.HasExited)
                            {
                                /*while(!proc.StandardOutput.EndOfStream)
                                    Console.Write((char)proc.StandardOutput.Read());

                                while (!proc.StandardError.EndOfStream)
                                    Console.Write((char)proc.StandardError.Read());*/

                                ConsoleKeyInfo cki = Console.ReadKey();
                                char procIn = cki.KeyChar;

                                if (cki.Key == ConsoleKey.Enter)
                                {
                                    proc.StandardInput.WriteLine();
                                }
                                else
                                {
                                    proc.StandardInput.Write(procIn);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("file not found dumbass");
                        }
                    }
                    else
                    {
                        bool found = false;
                        foreach(AbstractCommand cmd in commands)
                        {
                            if (cmd.name != args[0].ToLower()) continue;

                            found = true;
                            cmd.RunCommand(args);
                            break;
                        }

                        if(!found)
                        {
                            Console.WriteLine("Invalid command dumbass mf");
                        }
                    }
                }
            }
        }

        public static void OnProcessOutputReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        static async Task ConsumeReader(TextReader reader)
        {
            char[] buffer = new char[1];

            while ((await reader.ReadAsync(buffer, 0, 1)) > 0)
            {
                // process character...for example:
                Console.Write(buffer[0]);
            }
        }
    }
}
