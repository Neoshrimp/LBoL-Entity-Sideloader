using HarmonyLib;

namespace PatchWorkshop
{
    public static class PInfo
    {
        // each loaded plugin needs to have a unique GUID. usually author+generalCategory+Name is good enough
        public const string GUID = "neo.lbol.dev.patchWorkshop";
        public const string Name = "Patch Workshop";
        public const string version = "1.0.0";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
