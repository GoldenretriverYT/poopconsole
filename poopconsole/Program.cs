using poopconsole.builtin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace poopconsole
{
    internal class Program
    {
        public static string path = @"C:\";
        public static List<AbstractCommand> commands = new();

        public static Process proc;
        public static Thread commandThread;

        public static Dictionary<string, string> vars = new Dictionary<string, string>()
        {
            { "KILL_COMMAND_THREAD_ALLOWED", "false" }
        };

        static void Main(string[] launchArgs)
        {
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;

                try
                {
                    if (proc != null) proc.Kill();
                    if (commandThread != null) {
                        if (vars["KILL_COMMAND_THREAD_ALLOWED"].ToLower() == "true") commandThread.Interrupt();
                        if (vars["KILL_COMMAND_THREAD_ALLOWED"].ToLower() != "true") Console.WriteLine("Killing a command is currently disabled per default due to being extremly experimental and not working properly. Additionally, doing this will not free up CPU time. I don't know if I will ever fix it lol. Use 'setvar KILL_COMMAND_THREAD_ALLOWED true' to enable this experimental feature.");
                    }
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };

            foreach (AbstractCommand cmd in ReflectiveEnumerator.GetClassesOfType<AbstractCommand>())
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

        public static void Run(string input, bool subCommand = false)
        {
            string[] args = null;

            OutputType ot = OutputType.CONSOLE;
            string outputInfo = "";

            try
            {
                 args = Parser.GetArgs(input);
            }catch(Exception ex)
            {
                Console.WriteLine("Something went wrong: " + ex.Message);
                return;
            }

            if (args.Length > 2)
            {
                if (args[args.Length - 2] == "|f>")
                {
                    ot = OutputType.FILE;
                    outputInfo = args[args.Length - 1];
                    args = args.Take(args.Length - 2).ToArray();
                }
                else if (args[args.Length - 2] == "|v>")
                {
                    ot = OutputType.VARIABLE;
                    outputInfo = args[args.Length - 1];
                    args = args.Take(args.Length - 2).ToArray();
                }
            }

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
                                Arguments = stringArgs,
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
                            Task _ = ConsumeReaderCancellable(new StreamReader(ms, true), ct, ot, outputInfo);


                            Thread runningCommandThread = new Thread(() =>
                            {
                                try
                                {
                                    cmd.RunCommand(args, ref sw);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(cmd.name + " failed: " + ex.Message);
                                }
                            });

                            if (!subCommand)
                            {
                                commandThread = runningCommandThread;
                            }

                            runningCommandThread.Start();
                            runningCommandThread.Join();
                            
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

        static async Task ConsumeReaderCancellable(TextReader reader, CancellationToken ct, OutputType ot, string outputInfo)
        {
            string fullOutput = "";

            await Task.Run(async () =>
            {
                char[] buffer = new char[1];

                while (true)
                {
                    while ((await reader.ReadAsync(buffer, 0, 1)) > 0)
                    {
                        // process character...for example:
                        switch (ot)
                        {
                            case OutputType.CONSOLE:
                                Console.Write(buffer[0]);
                                break;
                            case OutputType.FILE:
                                File.AppendAllText(outputInfo, buffer[0].ToString());
                                break;
                            case OutputType.VARIABLE:
                                fullOutput += buffer[0].ToString();
                                break;
                        }
                    }

                    if (ct.IsCancellationRequested)
                    {
                        if(ot == OutputType.VARIABLE)
                        {
                            vars[outputInfo] = fullOutput.Trim();
                        }

                        break;
                    }
                }

                if (ot == OutputType.VARIABLE)
                {
                    vars[outputInfo] = fullOutput.Trim();
                }
            });
        }
    }
}
