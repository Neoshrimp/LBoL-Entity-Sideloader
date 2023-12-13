using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UniverseLib.UI;

namespace GunDesigner.Reflection
{
    public static class UniverseLibReflection
    {
        public static AccessTools.FieldRef<object, Dictionary<string, UIBase>> registeredUIsRef = AccessTools.FieldRefAccess<Dictionary<string, UIBase>>(typeof(UniversalUI), "registeredUIs");

        public static AccessTools.FieldRef<object, List<UIBase>> uiBases = AccessTools.FieldRefAccess<List<UIBase>>(typeof(UniversalUI), "uiBases");

    }
}
