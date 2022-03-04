using poopconsole.builtin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
            foreach(AbstractCommand cmd in ReflectiveEnumerator.GetClassesOfType<AbstractCommand>())
            {
                commands.Add(cmd);
            }

            Console.WriteLine("Welcome to PoopConsole");

            while (true) {
                Console.Write(path + " > ");

                string input = Console.ReadLine();
                Run(input);
            }
        }

        public static void Run(string input)
        {
            string[] args = Parser.GetArgs(input);

            if (args.Length > 0)
            {
                if (args[0].StartsWith("./"))
                {
                    string programName = args[0].Split("./")[1];

                    if (File.Exists(Path.Combine(path, programName)))
                    {
                        string stringArgs = "";

                        foreach (string arg in args)
                        {
                            if (arg == args[0]) continue;
                            stringArgs += arg + " ";
                        }

                        if (stringArgs.Length > 0)
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

                        var _ = ConsumeReader(proc.StandardOutput);
                        var _2 = ConsumeReader(proc.StandardError);

                        while (!proc.HasExited)
                        {
                            while (Console.KeyAvailable == false)
                            {
                                if (proc.HasExited) goto ProcExit; // bla bla bla bad pratice go cry
                            }

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

                        ProcExit: ;
                    }
                    else
                    {
                        Console.WriteLine("Unable to find file");
                    }
                }
                else
                {
                    bool found = false;
                    foreach (AbstractCommand cmd in commands)
                    {
                        if (cmd.name != args[0].ToLower()) continue;

                        found = true;
                        try
                        {
                            CancellationTokenSource tokenSource = new CancellationTokenSource();
                            CancellationToken ct = tokenSource.Token;

                            MemoryStream ms = new MemoryStream();
                            WrappedStreamWriter sw = new StreamWriter(ms).Wrap();
                            Task _ = ConsumeReaderCancellable(new StreamReader(ms, true), ct);

                            cmd.RunCommand(args, ref sw);
                            tokenSource.Cancel();

                            while(_.Status != TaskStatus.RanToCompletion) { }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(cmd.name + " failed: " + ex.Message);
                        }

                        break;
                    }

                    if (!found)
                    {
                        Console.WriteLine("Invalid command");
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

        static async Task ConsumeReaderCancellable(TextReader reader, CancellationToken ct)
        {
            await Task.Run(async () =>
            {
                char[] buffer = new char[1];

                while (true)
                {
                    while ((await reader.ReadAsync(buffer, 0, 1)) > 0)
                    {
                        // process character...for example:
                        Console.Write(buffer[0]);
                    }

                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                }
            });
        }
    }
}
