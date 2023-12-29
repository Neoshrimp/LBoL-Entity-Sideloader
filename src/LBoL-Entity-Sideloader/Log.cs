using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LBoLEntitySideloader
{
    internal static class Log
    {
        private static readonly BepInEx.Logging.ManualLogSource BePinExlogger = BepinexPlugin.log;


        static public BepInEx.Logging.ManualLogSource log => BePinExlogger;

        public static BepInEx.Logging.ManualLogSource LogDev()
        {
            if (BepinexPlugin.devModeConfig.Value)
                return log;
            return null;
        }

        public static BepInEx.Logging.ManualLogSource LogDevExtra()
        {
            if (BepinexPlugin.devModeConfig.Value && BepinexPlugin.devExtraLoggingConfig.Value)
                return log;
            return null;
        }
    }
}
