using HarmonyLib;

namespace EffectLib
{
    public static class PInfo
    {
        // each loaded plugin needs to have a unique GUID. usually author+generalCategory+Name is good enough
        public const string GUID = "neo.lbol.tools.EffectLib";
        public const string Name = "Effect Library";
        public const string version = "0.0.1";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
