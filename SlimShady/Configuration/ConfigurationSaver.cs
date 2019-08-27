using SlimShadyCore.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SlimShady.Configuration
{
    public class ConfigurationSaver
    {
        private readonly string path;
        private readonly BlockingCollection<string> saveQueue = new BlockingCollection<string>();

        private readonly Thread thread;
        private bool stop = false;


        public ConfigurationSaver(string path)
        {
            this.path = path;

            thread = new Thread(Main);
            thread.Name = "CfgSaver";
            thread.Start();
        }

        private void Main()
        {
            while(!stop)
            {
                try
                {
                    Tick();
                }
                catch(ThreadInterruptedException)
                {
                    continue;
                }
                catch(Exception ex)
                {
                    NkLogger.error("Thread saver error", ex);
                    Thread.Sleep(1000); // Retry after a bit of delay in order not to spam the logs too much in case of recurring exception
                }
            }
        }

        private void Tick()
        {
            string toSave = saveQueue.Take();
            string newToSave;
            while(saveQueue.TryTake(out newToSave, TimeSpan.FromSeconds(1)))
            {
                toSave = newToSave;
                // Quiescence - Wait for the save queue to "calm down".
            }

            using (NkTrace trace = NkLogger.trace("Actually saving config"))
            {
                File.WriteAllText(path, toSave);
            }
        }

        public void Save(ConfigurationNew configuration)
        {
            using (NkTrace trace = NkLogger.dbgTrace())
            {
                // We need to serialize the config instead of simply adding it to the queue.
                // Had we serialized the config on worker-thread just before writing to file, 
                // other application-threads could be modifying it concurrently (bad)
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationNew));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, configuration);
                    string toSave = writer.ToString();
                    saveQueue.Add(toSave);
                }
            }
        }

        private void Stop()
        {
            stop = true;
            thread.Interrupt();
        }
    }
}
