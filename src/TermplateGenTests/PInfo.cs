using HarmonyLib;

namespace TemplateGenTests
{
    public static class PInfo
    {
        // each loaded plugin needs to have a unique GUID. usually author+generalCategory+Name is good enough
        public const string GUID = "neo.lbol.examples.templateGen";
        public const string Name = "Template gen tests";
        public const string version = "1.0.0";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}