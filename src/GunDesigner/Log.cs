using System;
using System.Collections.Generic;
using System.Text;

namespace GunDesigner
{
    internal static class Log
    {
        private static readonly BepInEx.Logging.ManualLogSource BePinExlogger = BepinexPlugin.log;


        static public BepInEx.Logging.ManualLogSource log => BePinExlogger;

    }
}
