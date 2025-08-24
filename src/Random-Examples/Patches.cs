using HarmonyLib;
using LBoL.Core.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Random_Examples
{

    [HarmonyPatch(typeof(Card), nameof(Card.Loyalty), MethodType.Setter)]
    class OnUnityChanged_Patch
    {
        static void Prefix(Card __instance, out int __state)
        {
            __state = __instance._loyalty;
        }
        static void Postfix(Card __instance, int __state)
        {
            var card = __instance;
            var gr = card.GameRun;
            var battle = card.Battle;
            var unityBeforeChange = __state;

            if (battle == null)
                return;

            // maybe use this flag
            //battle.Player.IsInTurn;

        }



    }


}
