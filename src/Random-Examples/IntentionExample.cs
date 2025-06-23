using HarmonyLib;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.EntityLib.StatusEffects.Enemy;
using LBoL.Presentation;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using YamlDotNet.Serialization.NodeDeserializers;

namespace Random_Examples
{
    public sealed class DeezIntentionDef : IntentionTemplate
    {
        public override IdContainer GetId() => nameof(DeezIntention);
        

        public override LocalizationOption LoadLocalization()
        {
            return BepinexPlugin.IntentionBatchLoc.AddEntity(this);
        }


        public override string SelectAltIconsSuffix(Intention intention)
        {
            var deezInt = (DeezIntention)intention;
            if (deezInt.Source is Doremy doremy)
            {
                var sleepLevel = doremy.Sleep.Level;
                if (sleepLevel <= 30)
                    return "";
                if (sleepLevel <= 50)
                    return "0";
                return "1";
            }
            return "";
        }

        public override IntentionImages LoadSprites()
        {
            return new IntentionImages() { main = ResourcesHelper.TryGetSprite<StatusEffect>(nameof(WindGirl)), 
                subSprites = new Dictionary<string, UnityEngine.Sprite>() { 
                    { "0", ResourcesHelper.TryGetSprite<StatusEffect>(nameof(JunkoPurify)) },
                    { "1", ResourcesHelper.TryGetSprite<StatusEffect>(nameof(Burst)) },

                }
            };
        }
    }

    [EntityLogic(typeof(DeezIntentionDef))]
    public sealed class DeezIntention : Intention
    {
        // this enum is never used. Intention can be identified by typeof(<intentionClass>)
        public override IntentionType Type => throw new NotImplementedException();
    }

    // replace Sleep intention with DeezIntention
    [HarmonyPatch(typeof(Doremy), nameof(Doremy.GetTurnMoves), MethodType.Enumerator)]
    class DoremyIntention_Patch
    {
        private static Intention CreateInt()
        {
            return IntentionTemplate.CreateIntention<DeezIntention>();
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchEndForward(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Intention), nameof(Intention.Sleep))))
                .SetInstruction(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DoremyIntention_Patch), nameof(DoremyIntention_Patch.CreateInt))))
                .InstructionEnumeration();
        }

    }




}
