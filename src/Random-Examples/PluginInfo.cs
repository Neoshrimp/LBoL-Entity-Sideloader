using HarmonyLib;

namespace Random_Examples
{
    public static class PluginInfo
    {
        // each loaded plugin needs to have a unique GUID. usually author+generalCategory+Name is good enough
        public const string GUID = "neo.lbol.examples.cardLoc";
        public const string Name = "Example card loc";
        public const string version = "1.0.0";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
