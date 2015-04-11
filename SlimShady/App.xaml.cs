using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace SlimShady
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //AttachConsole(-1);
            AllocConsole();
            Console.WriteLine();

            String[] args = e.Args;
            //args = new String[] { "software", "list", "monitors" };
            //args = new String[] { "software", "get", "Brightness" };
            //args = new String[] { "software", "get", "Brightness", "DISPLAY1" };
            //args = new String[] { "software", "set", "Brightness", "\\\\.\\DISPLAY1()", "20" };
            //args = new String[] { "software", "set", "Brightness", "DISPLAY1", "80" };
            //Console.WriteLine("Args:");
            foreach (String arg in args)
                Console.WriteLine(@"- {0}", arg);

            // Temporarily disabled commands handling TODO: Enable commands (remove line below)
#if false

            // Don't run more than 1 'server' and 1 'client', 
            // because the 'client' is passing commands via single file and we can't write to a file simultanously
            // Also I can't think of any situation that 2 'clients' would be needed
            Process thisProc = Process.GetCurrentProcess();
            Process[] similarProc = Process.GetProcessesByName(thisProc.ProcessName);
            if (similarProc.Length > 2)
                Shutdown();

            // Leave a command for the server to execute
            CommandMgr.AppendNewCommand(args);

            // If we are a 'client', then we already left the command for the server so we can shutdown now
            if (similarProc.Length > 1)
                Shutdown();

            // So now we are the champions, um... server
            // So we start parsing commands
            CommandMgr commandMgr = new CommandMgr();
            DispatcherTimer dispatcher = new DispatcherTimer();
            dispatcher.Tick += (x, y) => commandMgr.ExecuteNextCommand();
            dispatcher.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcher.Start(); // Temporarily disable commands
#endif
        }

        public bool IsProcessOpen(string name)
        {
            return Process.GetProcesses().Any(clsProcess => clsProcess.ProcessName.Contains(name));
        }

        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);
        [DllImport("kernel32")]
        static extern bool AllocConsole();
    }
}
