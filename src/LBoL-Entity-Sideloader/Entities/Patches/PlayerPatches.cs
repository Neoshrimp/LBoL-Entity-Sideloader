using DG.Tweening;
using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Units;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.UIhelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static LBoLEntitySideloader.Entities.Patches.StartGamePanel_Patches;



namespace LBoLEntitySideloader.Entities.Patches
{

    internal class PlayerSpriteList
    {

        internal static void LoadForStartPanel(StartGamePanel startGamePanel)
        {
            Wrap((PlayerUnitTemplate puT) =>
            {
                var sprites = puT.LoadPlayerImages();
                EntityManager.HandleOverwriteWrap(() => startGamePanel.standPicList.AlwaysAdd(puT.UniqueId, sprites.startPanelStandPic()), puT, OverwriteName(PISuffixes.stand), puT.user);

                EntityManager.HandleOverwriteWrap(() => startGamePanel.headPicList.AlwaysAdd(puT.UniqueId, sprites.selectionCircleIcon), puT, OverwriteName(PISuffixes.selectionCircleIcon), puT.user);


            });

        }

        internal static void LoadForMuseumPanel(MuseumPanel museumPanel)
        {
            Wrap((PlayerUnitTemplate puT) =>
            {
                var sprites = puT.LoadPlayerImages();
                EntityManager.HandleOverwriteWrap(() => museumPanel.portraitList.AlwaysAdd(puT.UniqueId, sprites.collectionIcon), puT, OverwriteName(PISuffixes.collectionIcon), puT.user);

            });

        }

        internal static string OverwriteName(string name) => $"{nameof(PlayerUnitTemplate.LoadPlayerImages)}.{name}";

        internal static void Wrap(Action<PlayerUnitTemplate> action)
        {
            foreach (var puTlist in UniqueTracker.Instance.user2PlayerTemplates.Values)
            {
                foreach (var puT in puTlist)
                {
                    if (UniqueTracker.Instance.invalidRegistrations.Contains(puT.GetType()))
                        continue;

                    action(puT);    
                }

            }
        }
        
    }



    [HarmonyPatch(typeof(MuseumPanel), nameof(MuseumPanel.Awake))]
    class MuseumPanel_Patch
    {
        static void Prefix(MuseumPanel __instance)
        {
            PlayerSpriteList.LoadForMuseumPanel(__instance);

        }
    }




    public class StartGamePanel_Patches
    {

        static public GameObject scrollGameObject;

        static public RectTransform loadoutRectTransform;

        static public List<int> index2complexity = new List<int>();

        static float rectWidth = 4200f;

        static Vector3 contentLocalPos;

        [HarmonyPatch(typeof(StartGamePanel), nameof(StartGamePanel.Awake))]
        class Awake_Patch
        {

            static public async void Prefix(StartGamePanel __instance)
            {

                PlayerSpriteList.LoadForStartPanel(__instance);





                var loadoutGo = __instance.characterSetupRoot.gameObject;
                CreateScrollRect(loadoutGo);
            }

            static GameObject CreateScrollRect(GameObject loadoutGo)
            {
                var scrollGo = new GameObject("loadoutScroll");
                scrollGo.layer = 5; //UI
                scrollGo.transform.position = loadoutGo.transform.position;
                scrollGo.transform.SetParent(loadoutGo.transform.parent);
                // but why
                scrollGo.transform.localScale = new Vector3(1, 1, 1);

                // is regular transform but isn't currently used
                /*                    var viewPort = new GameObject("viewPort", typeof(RectTransform));
                                    viewPort.layer = 5;
                                    viewPort.transform.position = scrollGo.transform.position;
                                    viewPort.transform.SetParent(scrollGo.transform);

                                    viewPort.transform.localScale = new Vector3(1, 1, 1);

                                    loadoutGo.transform.SetParent(viewPort.transform);*/

                loadoutGo.transform.SetParent(scrollGo.transform);

                var scrollRectS = scrollGo.AddComponent<ScrollRect>();

                var loadoutRectT = loadoutGo.GetComponent<RectTransform>();

                var image = scrollGo.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0f);


                scrollGo.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
                scrollGo.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);

                scrollGo.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectWidth);
                scrollGo.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1206f);

                //scrollRectS.viewport = viewPort.GetComponent<RectTransform>();
                scrollRectS.content = loadoutRectT;
                scrollRectS.horizontal = true;
                scrollRectS.vertical = false;
                scrollRectS.scrollSensitivity = 25f;
                scrollRectS.elasticity = 0.08f;
                scrollRectS.movementType = ScrollRect.MovementType.Clamped;


                loadoutGo.transform.KeepPositionAfterAction(() =>
                {
                    loadoutRectT.anchorMax = new Vector2(0f, 0.5f);
                    loadoutRectT.anchorMin = new Vector2(0f, 0.5f);
                });

                foreach (var o in loadoutGo.transform)
                {
                    var rt = (RectTransform)o;
                    if (rt.TryGetComponent<StartSetupWidget>(out var _))
                    {
                        rt.KeepPositionAfterAction(() =>
                        {
                            rt.anchorMax = new Vector2(0.5f, 1f);
                            rt.anchorMin = new Vector2(0.5f, 1f);
                        });
                    }
                }

                // max scrolling is relative to ScrollRect size
                loadoutRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectWidth);

                scrollGameObject = scrollGo;

                loadoutRectTransform = loadoutRectT;

                contentLocalPos = loadoutGo.transform.localPosition;

                return scrollGo;

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



            internal static void TyrCreateLoadoutWidget(int index, UniqueTracker.CharLoadoutInfo loadoutInfo, StartGamePanel startGamePanel)
            {


                var startSetupWidgetA = startGamePanel.characterSetupList[0].gameObject;
                var startSetupWidgetB = startGamePanel.characterSetupList[1].gameObject;



                if (index == startGamePanel.characterSetupList.Count)
                {
                    GameObject baseWidget;
                    float dir = -1;
                    if (startGamePanel.characterSetupList.Count % 2 == 0)
                    {
                        baseWidget = startSetupWidgetA;
                        dir = -1;
                    }
                    else
                    {
                        baseWidget = startSetupWidgetB;
                        dir = 1;
                    }


                    var newWidget = GameObject.Instantiate(baseWidget, baseWidget.transform.parent);

                    newWidget.transform.position = baseWidget.transform.position;

                    var rt = newWidget.GetComponent<RectTransform>();
                    rt.KeepPositionAfterAction(() =>
                    {
                        rt.anchorMax = new Vector2(0.5f, 1f);
                        rt.anchorMin = new Vector2(0.5f, 1f);
                    });

                    newWidget.transform.localPosition += new Vector3(700f, 0, 0) * ((int)(startGamePanel.characterSetupList.Count - 2) / 2 + 1) * dir;

                    startGamePanel.characterSetupList.Add(newWidget.GetComponent<StartSetupWidget>());
                }

                var widget = startGamePanel.characterSetupList[index];

                widget.gameObject.SetActive(true);

                widget.GetComponent<Button>().onClick.RemoveAllListeners();

                widget.GetComponent<Button>().onClick.AddListener(delegate
                {
                    startGamePanel.SelectType(index);
                });

                widget.GetComponent<StartSetupWidget>().DeckButton.onClick.RemoveAllListeners();
                widget.GetComponent<StartSetupWidget>().DeckButton.onClick.AddListener(() =>
                {
                    startGamePanel.deckHolder.Clear();
                    loadoutInfo.deck.Do(c => startGamePanel.deckHolder.AddCardWidget(Library.CreateCard(c), true));
                    string text2 = startGamePanel._player.ShortName + "Game.Deck".Localize(true) + loadoutInfo.typeSuffix;
                    startGamePanel.deckHolder.SetTitle(text2, "Cards.Show".Localize(true));

                    CanvasGroup component = startGamePanel.deckHolder.GetComponent<CanvasGroup>();
                    component.DOKill(false);
                    component.DOFade(1f, 0.4f).From(0f, true, false).SetUpdate(true);
                    startGamePanel.deckHolder.gameObject.SetActive(true);
                    startGamePanel.deckReturnButton.interactable = true;
                }
                );
            }

        }


        [HarmonyPatch(typeof(StartGamePanel), nameof(StartGamePanel.SelectPlayer))]
        internal class SelectPlayer_Patch
        {

            static void Expand_typeCandidates(StartGamePanel startGamePanel)
            {

                var playerId = startGamePanel._player.Id;
                int numberOfWidgets = 2;

                index2complexity.Clear();
                index2complexity.Add(startGamePanel._player.Config.DifficultyA);
                index2complexity.Add(startGamePanel._player.Config.DifficultyB);

                if (UniqueTracker.Instance.loadoutInfos.ContainsKey(playerId))
                {
                    var i = 2;
                    foreach (var loadoutInfo in UniqueTracker.Instance.loadoutInfos[playerId])
                    {

                        Awake_Patch.TyrCreateLoadoutWidget(i, loadoutInfo, startGamePanel);

                        startGamePanel._typeCandidates = startGamePanel._typeCandidates.AddToArray(new StartGamePanel.TypeCandidate()
                        {
                            Name = loadoutInfo.typeName,
                            Us = Library.CreateUs(loadoutInfo.ultimateSkill),
                            Exhibit = Library.CreateExhibit(loadoutInfo.exhibit),
                            Deck = loadoutInfo.deck.Select(c => Library.CreateCard(c)).ToArray()

                        });

                        index2complexity.Add(loadoutInfo.complexity);


                        i++;
                    }

                    numberOfWidgets += UniqueTracker.Instance.loadoutInfos[playerId].Count;
                }


                startGamePanel.characterSetupList.Where((w, i) => i + 1 > numberOfWidgets).Do(w => w.gameObject.SetActive(false));

                // resets pos?
                loadoutRectTransform.gameObject.transform.localPosition = contentLocalPos;
                loadoutRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectWidth + ((numberOfWidgets - 2) * 650f));

                var rs = scrollGameObject.GetComponent<ScrollRect>();

                if (numberOfWidgets > 2)
                {
                    rs.movementType = ScrollRect.MovementType.Elastic;
                }
                else
                {
                    rs.movementType = ScrollRect.MovementType.Clamped;
                }

            }


            static void Prefix(StartGamePanel __instance)
            {
                var standGo = __instance.characterStandPicList[0].gameObject;
                for (var i = 0; i < __instance._players.Length - __instance.characterStandPicList.Count; i++)
                {
                    var newStandGo = GameObject.Instantiate(standGo, standGo.transform.parent);
                    newStandGo.transform.localScale = Vector3.one;
                    __instance.characterStandPicList.Add(newStandGo.GetComponent<CanvasGroup>());
                    newStandGo.SetActive(false);
                }
            }


            static int PlayerNumber(StartGamePanel startGamePanel)
            {
                return startGamePanel._players.Length;
            }

            static int PlayerNumberMinus1(StartGamePanel startGamePanel)
            {
                return startGamePanel._players.Length - 1;
            }

            static internal int ModuloStandNum(int i, StartGamePanel startGamePanel)
            {
                return i % startGamePanel._standPicAlpha.Length;
            }



            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {

                return new CodeMatcher(instructions)
                    // main loop
                    .MatchForward(false, new CodeMatch[] { OpCodes.Ldc_I4_5, OpCodes.Blt })
                    .SetAndAdvance(OpCodes.Ldarg_0, null)
                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SelectPlayer_Patch), nameof(SelectPlayer_Patch.PlayerNumber))))
                    // on button press
                    .MatchBack(false, new CodeMatch[] { OpCodes.Ldloc_3, OpCodes.Ldc_I4_4, OpCodes.Bne_Un })
                    .Advance(1)
                    .SetAndAdvance(OpCodes.Ldarg_0, null)
                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SelectPlayer_Patch), nameof(SelectPlayer_Patch.PlayerNumberMinus1))))
                    //.Start()
                    .Wrap_i(true)
                    .Wrap_i()

                    .Wrap_i(true)
                    .Wrap_i()
                    .Wrap_iPlusNum()
                    .Wrap_i(true)
                    .Wrap_i()
                    .Wrap_iPlusNum()
                    .Wrap_i(true)
                    .Wrap_i()
                    .Wrap_iPlusNum()
                    .Wrap_i(true)
                    .Wrap_i()
                    .Wrap_iPlusNum()
                    // setting _typeCandidates
                    .End()
                    .MatchBack(false, new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(StartGamePanel), nameof(StartGamePanel._typeCandidates))))
                    .Advance(1)
                    .Insert(new CodeInstruction(OpCodes.Ldarg_0))
                    .Advance(1)
                    .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SelectPlayer_Patch), nameof(SelectPlayer_Patch.Expand_typeCandidates))))
                    .InstructionEnumeration();

            }



        }



        [HarmonyPatch(typeof(StartGamePanel), nameof(StartGamePanel.SelectType))]
        class SetLoadoutComplexity_Patch
        {

            static int GetComplexity(int index)
            {
                try
                {
                    return index2complexity[index];
                }
                catch (Exception ex)
                {
                    Log.log.LogError($"Get loadout complexity: {ex}");
                }
                return 2;
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                return new CodeMatcher(instructions, generator)
                    .End()
                    .MatchBack(false, new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(StartStatusWidget), nameof(StartStatusWidget.SetSetup))))
                    .CreateLabel(out var popLabel)
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Pop).WithLabels(popLabel))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SetLoadoutComplexity_Patch), nameof(SetLoadoutComplexity_Patch.GetComplexity))))
                    .MatchBack(false, OpCodes.Br)
                    .Set(OpCodes.Br, popLabel)
                    .InstructionEnumeration();
            }

        }

    }

    [HarmonyPatch(typeof(ShowCardsPanel), "OnShowing")]
    class FixLoadDeckStand_Patch
    {

        static void Prefix(ShowCardsPanel __instance)
        {
            __instance._currentCharacterIndex = -999;
        }
    }



    static internal class MiniMatcherExtension
    {
        static internal CodeMatcher Wrap_i(this CodeMatcher _this, bool justMatch = false)
        {
            _this.MatchForward(false, new CodeMatch[] { OpCodes.Ldloc_3, OpCodes.Ldelem_R4 });
            if (!justMatch)
                _this.Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SelectPlayer_Patch), nameof(SelectPlayer_Patch.ModuloStandNum))));
            return _this;
        }

        static internal CodeMatcher Wrap_iPlusNum(this CodeMatcher _this)
        {

            return _this.MatchForward(true, new CodeMatch[] { OpCodes.Ldloc_3, OpCodes.Ldloc_0, OpCodes.Add, OpCodes.Ldelem_R4 })
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SelectPlayer_Patch), nameof(SelectPlayer_Patch.ModuloStandNum))));
        }
    }
}
