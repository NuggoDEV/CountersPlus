using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPALogger = IPA.Logging.Logger;

namespace CountersPlus.Utils
{
    public class Logger
    {
        private static IPALogger BSIPALogger;

        internal static void Init(IPALogger logger)
        {
            if (BSIPALogger == null) BSIPALogger = logger;
            else Log("Logger already initialized!", LogInfo.Warning);
        }

        public static void Log(string m) => Log(m, LogInfo.Info);
        public static void Log(string m, LogInfo l) => Log(m, l, null);
        public static void Log(string m, LogInfo l, string suggestedAction)
        {
            if (BSIPALogger == null) return;
            IPALogger.Level level = IPALogger.Level.Debug;
            switch (l)
            {
                case LogInfo.Info: level = IPALogger.Level.Debug; break;
                case LogInfo.Notice: level = IPALogger.Level.Notice; break;
                case LogInfo.Warning: level = IPALogger.Level.Warning; break;
                case LogInfo.Error: level = IPALogger.Level.Error; break;
                case LogInfo.Fatal: level = IPALogger.Level.Critical; break;
            }
            BSIPALogger.Log(level, m);
            if (suggestedAction != null)
                BSIPALogger.Log(level, $"Suggested Action: {suggestedAction}");
        }
    }
}