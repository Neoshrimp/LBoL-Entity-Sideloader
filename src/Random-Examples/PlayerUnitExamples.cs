using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.PlayerUnits;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static Random_Examples.BepinexPlugin;


namespace Random_Examples
{
    // doesn't work, Alice was never real
    [OverwriteVanilla]
    public /*sealed*/ class ShowAliceDef : PlayerUnitTemplate
    {
        public override IdContainer GetId() => nameof(Alice);

        public override PlayerUnitConfig MakeConfig()
        {
            var con = PlayerUnitConfig.FromId(nameof(Alice));
            con.IsSelectable = true;
            return con;
        }
    }



    [HarmonyPatch(typeof(StartGamePanel), nameof(StartGamePanel.Awake))]
    //[HarmonyDebug]
    class MoreVariants_Patch
    {

        static public List<Card> blueReimuCards;

        static void Prefix(StartGamePanel __instance)
        {
            blueReimuCards = new List<Type>() {
                        typeof(Shoot), typeof(Shoot), typeof(Boundary), typeof(Boundary),
                        typeof(ReimuAttackR),
                        typeof(ReimuAttackR),
                        typeof(ReimuAttackW),
                        typeof(ReimuBlockW),
                        typeof(ReimuBlockW),
                        typeof(ReimuBlockR),
                        // sunlight prayer
                        typeof(QiqingYishi)

                    }.Select(new Func<Type, Card>(LBoL.Core.Library.CreateCard)).ToList();

            var startSetupWidgetA = __instance.characterSetupList[0].gameObject;

            var newWidgetA = GameObject.Instantiate(startSetupWidgetA, startSetupWidgetA.transform.parent);
            newWidgetA.transform.localPosition += new Vector3(700f, 0, 0);

            __instance.characterSetupList.Add(newWidgetA.GetComponent<StartSetupWidget>());
            newWidgetA.GetComponent<Button>().onClick.AddListener(delegate
            {
                __instance.SelectType(2);
            });
            newWidgetA.GetComponent<StartSetupWidget>().DeckButton.onClick.AddListener( () =>
            { 
				__instance.deckHolder.Clear();
                blueReimuCards.Do(c => __instance.deckHolder.AddCardWidget(c, true));
                string text2 = __instance._player.ShortName + "Game.Deck".Localize(true) + "C";
                __instance.deckHolder.SetTitle(text2, "Cards.Show".Localize(true));

                CanvasGroup component = __instance.deckHolder.GetComponent<CanvasGroup>();
                component.DOKill(false);
                component.DOFade(1f, 0.4f).From(0f, true, false).SetUpdate(true);
                __instance.deckHolder.gameObject.SetActive(true);
                __instance.deckReturnButton.interactable = true;
            }
            );






            var startSetupWidgetB = __instance.characterSetupList[1].gameObject;

            var newWidgetB = GameObject.Instantiate(startSetupWidgetB, startSetupWidgetB.transform.parent);
            newWidgetB.transform.localPosition += new Vector3(-700f, 0, 0);
            __instance.characterSetupList.Add(newWidgetB.GetComponent<StartSetupWidget>());



        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // lol does nothing
            return new CodeMatcher(instructions)
                .MatchForward(true, new CodeMatch[] { new CodeMatch(new CodeInstruction(OpCodes.Ldloc_0)), new CodeMatch(new CodeInstruction(OpCodes.Ldc_I4_2)) })
                .Set(OpCodes.Ldc_I4_2, null)
                .InstructionEnumeration();
        }

    }



    /*[HarmonyPatch(typeof(StartGamePanel), "<Awake>b__68_9")]
    [HarmonyDebug]*/
    class StartRunButton_Patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch[] { 
                    new CodeMatch(new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(ArgumentOutOfRangeException), new Type[] { }))),
                    new CodeMatch(new CodeInstruction(OpCodes.Throw)) 
                })
                .RemoveInstructions(2)
                .InstructionEnumeration();
                
        }

    }




    [HarmonyPatch(typeof(StartGamePanel), nameof(StartGamePanel.SelectPlayer))]
    class SelectPlayer_Patch
    {

        static void ExpandVariants(StartGamePanel startGamePanel)
        {
            var config = startGamePanel._player.Config;
            log.LogDebug(config);

            if (config.Id == CardTemplate.VanillaCharNames.Reimu)
            {
                startGamePanel._typeCandidates = startGamePanel._typeCandidates.AddToArray(new StartGamePanel.TypeCandidate()
                {
                    Name = "TypeC",
                    Us = LBoL.Core.Library.CreateUs(new ReimuUltRJabDef().UniqueId),
                    Exhibit = LBoL.Core.Library.CreateExhibit(typeof(CirnoU)),
                    Deck = MoreVariants_Patch.blueReimuCards.ToArray()
                });
                startGamePanel._typeCandidates.ToList().Do(c => log.LogInfo(c));
            }
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            return new CodeMatcher(instructions)
                .End()
                .MatchBack(false, new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(StartGamePanel), nameof(StartGamePanel._typeCandidates))))
                .Advance(1)
                .Insert(new CodeInstruction(OpCodes.Ldarg_0))
                .Advance(1)
                .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SelectPlayer_Patch), nameof(SelectPlayer_Patch.ExpandVariants))))
                .InstructionEnumeration();

        }

    }
}
