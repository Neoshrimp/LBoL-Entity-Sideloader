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
using LBoL.Presentation;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.UIhelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Windows;
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
    class MoreVariants_Patch
    {

        //static public List<Card> blueReimuCards;

        static public void Prefix(StartGamePanel __instance)
        {
            var loadoutGo = __instance.characterSetupRoot.gameObject;
/*            var contentSizeFitter = loadoutGo.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;*/



            var blueReimuCards = new List<Type>() {
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





            var scrollGo = new GameObject("loadoutScroll");
            scrollGo.layer = 5; //UI
            scrollGo.transform.position = loadoutGo.transform.position;
            scrollGo.transform.SetParent(loadoutGo.transform.parent);
            // but why
            scrollGo.transform.localScale = new Vector3(1, 1, 1);


            var viewPort = new GameObject("viewPort");
            viewPort.layer = 5;
            viewPort.transform.position = scrollGo.transform.position;
            viewPort.transform.SetParent(scrollGo.transform);

            viewPort.transform.localScale = new Vector3(1, 1, 1);

            loadoutGo.transform.SetParent(viewPort.transform);

/*            var widgetHolder = new GameObject("widgetHolder");
            widgetHolder.layer = 5;
            widgetHolder.transform.position = scrollGo.transform.position;
            widgetHolder.transform.SetParent(scrollGo.transform);

            var holderRectT = widgetHolder.GetComponent<RectTransform>();*/
            


            var scrollRectS = scrollGo.AddComponent<ScrollRect>();

            var loadoutRectT = loadoutGo.GetComponent<RectTransform>();

            var image = scrollGo.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0f);

            //image.sprite = Resources.Load<Sprite>("UIMask");
            //image.sprite = ResourcesHelper.LoadUiBackground("Adventure");


            scrollGo.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            scrollGo.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);

            scrollGo.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 20000f);
            scrollGo.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1206f);

            //var mask = scrollGo.AddComponent<Mask>();

            // wtf it works?
            //scrollRectS.viewport = loadoutRectT;


            scrollRectS.viewport = viewPort.GetComponent<RectTransform>();
            scrollRectS.content = loadoutRectT;
            scrollRectS.horizontal = true;
            scrollRectS.vertical= false;
            scrollRectS.scrollSensitivity = 20;


            loadoutGo.transform.KeepPositionAfterAction(() =>
            {
                loadoutRectT.anchorMax = new Vector2(0f, 0.5f);
                loadoutRectT.anchorMin = new Vector2(0f, 0.5f);
/*                loadoutRectT.sizeDelta += new Vector2(1000, 0);
                loadoutRectT.anchoredPosition += new Vector2(1000, 0);*/
            }, true);


            /*loadoutRectT.sizeDelta += new Vector2(1000, 0);
            loadoutRectT.anchoredPosition += new Vector2(1000, 0);*/

            loadoutGo.transform.KeepPositionAfterAction(() =>
            {
                //loadoutRectT.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 20000f);

            }, true);

            //loadoutRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 5000f);


            //loadoutRectT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            //loadoutRectT.sizeDelta += new Vector2(0, 300);



            /*            var gridLayout = loadoutGo.AddComponent<GridLayoutGroup>();
                        gridLayout.constraint = GridLayoutGroup.Constraint.Flexible;*/


            /*            var startSetupWidgetB = __instance.characterSetupList[1].gameObject;

                        var newWidgetB = GameObject.Instantiate(startSetupWidgetB, startSetupWidgetB.transform.parent);
                        newWidgetB.transform.localPosition += new Vector3(-700f, 0, 0);
                        __instance.characterSetupList.Add(newWidgetB.GetComponent<StartSetupWidget>());*/


            var newWidgetA = GameObject.Instantiate(startSetupWidgetA, startSetupWidgetA.transform.parent);
            newWidgetA.transform.localPosition += new Vector3(700f, 0, 0);

            __instance.characterSetupList.Add(newWidgetA.GetComponent<StartSetupWidget>());
            newWidgetA.GetComponent<Button>().onClick.AddListener(delegate
            {
                __instance.SelectType(2);
            });
            newWidgetA.GetComponent<StartSetupWidget>().DeckButton.onClick.AddListener(() =>
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


            foreach (var o in loadoutGo.transform)
            {
                var rt = (RectTransform)o;

                if (rt.TryGetComponent<StartSetupWidget>(out var _))
                {
                    rt.KeepPositionAfterAction(() => {
                        rt.anchorMax = new Vector2(0.5f, 1f);
                        rt.anchorMin = new Vector2(0.5f, 1f);
                    }, true);
                }


            }

            loadoutRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 20000f);




        }

        /*        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
                {
                    // lol does nothing
                    return new CodeMatcher(instructions)
                        .MatchForward(true, new CodeMatch[] { new CodeMatch(new CodeInstruction(OpCodes.Ldloc_0)), new CodeMatch(new CodeInstruction(OpCodes.Ldc_I4_2)) })
                        .Set(OpCodes.Ldc_I4_2, null)
                        .InstructionEnumeration();
                }*/

    }


    // 2do maybe extend PlayerType enum
    [HarmonyPatch(typeof(StartGamePanel), "<Awake>b__68_9")]
    class StartRunButton_Patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch[] { 
                    new CodeMatch(new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(ArgumentOutOfRangeException), new Type[] { }))),
                    new CodeMatch(new CodeInstruction(OpCodes.Throw)) 
                })
                .Advance(1)
                .Set(OpCodes.Pop, null)
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


            var blueReimuCards = new List<Type>() {
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


            if (config.Id == CardTemplate.VanillaCharNames.Reimu)
            {
                startGamePanel._typeCandidates = startGamePanel._typeCandidates.AddToArray(new StartGamePanel.TypeCandidate()
                {
                    Name = "TypeC",
                    Us = LBoL.Core.Library.CreateUs(new ReimuUltRJabDef().UniqueId),
                    Exhibit = LBoL.Core.Library.CreateExhibit(typeof(CirnoU)),
                    Deck = blueReimuCards.ToArray()
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
