namespace SlimShadyCore.Monitors
{
    public abstract class MonitorManagerProvider
    {
        public abstract string Id { get; }
        public abstract string DispName { get; }
        protected abstract MonitorManager Create();

        private MonitorManager mMonitorManager;
        public MonitorManager GetOrCreate()
        {
            if (mMonitorManager == null)
                mMonitorManager = Create();
            return mMonitorManager;
        }
    }
}
