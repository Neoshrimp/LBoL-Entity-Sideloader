using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace LBoLEntitySideloader
{
    public class PluginInfo
    {
        public const string GUID = "neo.lbol.frameworks.entitySideloader";
        public const string description = "Entity Sideloader";
        public const string version = "1.1.3";

        public static readonly Harmony harmony = new Harmony(GUID);
    }
}
