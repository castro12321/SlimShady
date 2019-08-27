namespace SlimShadyCore
{
    /*
    public class CommandMgr
    {
        private static String commandsFilePath = "command";

        public static void AppendNewCommand(String[] args, int tries = 3)
        {
            if (args.Length == 0) // No comment
                return;

            if (File.Exists(commandsFilePath))
            {
                if (tries == 0)
                {
                    Console.WriteLine(@"Maximum number of tries exceeded. Halting.");
                    return;
                }

                Console.WriteLine(@"A command is already awaiting for the execution. Retrying in 2 second");
                Thread.Sleep(2000);
                File.Delete(commandsFilePath); // If the file still exists, the server is either freezed or shutdown
                AppendNewCommand(args, tries - 1);
                return;
            }

            File.AppendAllText(commandsFilePath, args[0]);
            for (int i = 1; i < args.Length; ++i)
                File.AppendAllText(commandsFilePath, @" " + args[i]);
            File.AppendAllText(commandsFilePath, "\n");
        }

        public void ExecuteNextCommand()
        {
            if (!File.Exists(commandsFilePath))
                return;

            String[] file = File.ReadAllLines(commandsFilePath);
            if (file.Length == 0)
                return;
            foreach(String argsLine in file)
            {
                String[] args = argsLine.Split(' ');
                ExecuteCommand(args);
            }

            File.Delete(commandsFilePath);
        }

        private void ExecuteCommand(String[] args)
        {
            if (args.Length < 2)
            {
                usage();
                return;
            }

            // Process mandatory 'layer' argument
            MonitorManager brightnessManager;
            String argLayer = args[0].ToLowerInvariant();
            switch (argLayer)
            {
                case "hardware":
                    brightnessManager = HardwareMonitorManager.Instance;
                    break;
                case "software":
                    brightnessManager = SoftwareMonitorManager.Instance;
                    break;
                default:
                    Console.WriteLine(@"[ERROR] Wrong layer provided. Available values: {{hardware, software}}; got: {0}", argLayer);
                    return;
            }

            // Process mandatory 'command' argument
            String argCommand = args[1].ToLowerInvariant() + " " + args[2].ToLowerInvariant();

            // Process 'monitor' argument
            List<Monitor> argMonitors = brightnessManager.GetMonitorsList();
            if (args.Length >= 4)
            {
                String argMonitor = args[3];
                if (argMonitor != "all")
                    argMonitors.RemoveAll(x => x.ToString().Contains(argMonitor));
                //argMonitors.RemoveAll(x => x.Name != argMonitor && x.ToString() != argMonitor);
            }

            // Process 'value' argument
            int argValue = -1;
            try
            {
                if (args.Length >= 5)
                    argValue = int.Parse(args[4]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Cannot convert '{0}' to int because: {1}", args[4], ex.Message);
            }

            // Argument gathering done
            ExecuteCommand(argCommand, argMonitors, argValue);
        }

        /// <summary>
        /// </summary>
        /// <param name="argCommand"></param>
        /// <param name="argMonitors"></param>
        /// <param name="argValue"></param>
        private void ExecuteCommand(String argCommand, List<Monitor> argMonitors, int argValue)
        {
            switch (argCommand)
            {
                case "list monitors":
                    Console.WriteLine(@"Monitors:");
                    foreach (Monitor monitor in argMonitors)
                        Console.WriteLine(@"- {0}", monitor);
                    return;

                case "get Brightness":
                    Console.WriteLine(@"Brightness:");
                    foreach (Monitor monitor in argMonitors)
                        Console.WriteLine(@"- {0}: {1}", monitor, monitor.Brightness);
                    return;
                case "set Brightness":
                    foreach (Monitor monitor in argMonitors)
                        monitor.Brightness = argValue;
                    break;

                case "get Contrast":
                    Console.WriteLine(@"Contrast:");
                    foreach (Monitor monitor in argMonitors)
                        Console.WriteLine(@"- {0}: {1}", monitor, monitor.Contrast);
                    return;
                case "set Contrast":
                    foreach (Monitor monitor in argMonitors)
                        monitor.Contrast = argValue;
                    break;

                case "get Temperature":
                    Console.WriteLine(@"Temperature:");
                    foreach (Monitor monitor in argMonitors)
                        Console.WriteLine(@"- {0}: {1}", monitor, monitor.Temperature);
                    return;
                case "set Temperature":
                    foreach (Monitor monitor in argMonitors)
                        monitor.Temperature = argValue;
                    break;
            }
        }

        private void usage()
        {
            Console.WriteLine(@"Usage:");
            Console.WriteLine(@"	SlimShady - Control monitor Brightness");
            Console.WriteLine();

            Console.WriteLine(@"Synopsis:");
            Console.WriteLine(@"	SlimShady [layer] [command]");
            Console.WriteLine();

            Console.WriteLine(@"Layers:");
            Console.WriteLine(@"	hardware - Use DDC/CI standard for communication with monitor. May not work for TV screens and older monitors as they lack support for DCC/CI standard.");
            Console.WriteLine(@"	software - Emulate Brightness change using software only. Works on all monitors.");

            Console.WriteLine(@"Commands");
            Console.WriteLine(@"	list monitors - Prints available monitors for use by commands below.");
            Console.WriteLine(@"	set Brightness [monitor] [value]");
            Console.WriteLine(@"	get Brightness [monitor]");
            Console.WriteLine(@"	set Contrast [monitor] [value]");
            Console.WriteLine(@"	get Contrast [monitor]");
            Console.WriteLine(@"Tip: For [monitor] You may specify 'all' instead of monitor Name to select all monitors");
            Console.WriteLine(@"Tip2: You may also specify only a part of the monitor's Name. e.g. if your monitor is 'My SuperMonitor 5000x' you can specify 'SuperMonitor'");
            Console.WriteLine();

            Console.WriteLine(@"Examples:");
            Console.WriteLine();
        }
    }
    */
}
