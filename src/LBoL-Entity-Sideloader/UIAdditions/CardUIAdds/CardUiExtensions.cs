using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using LBoL.Base.Extensions;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using LBoL.Presentation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using RotateMode = DG.Tweening.RotateMode;
using Sequence = DG.Tweening.Sequence;

namespace LBoLEntitySideloader.UIAdditions.CardUIAdds
{
    public static class CardUiExtensions
    {

        static public BattleActionViewer<T> BAV_DelegateWrap<T>(Func<T, CardUi, IEnumerator> viewer, CardUi cardUi) where T : BattleAction => (T action) => viewer(action, cardUi);

        static public BattleActionViewer<AddCardsToExileAction> AddCardsToExileViewer(CardUi cardUi) => BAV_DelegateWrap<AddCardsToExileAction>(ViewAddCardsToExile, cardUi);

        static public IEnumerator ViewAddCardsToExile(AddCardsToExileAction action, CardUi cardUi)
        {
            float interval = 0.05f;
            Vector3? vector = cardUi._playBoard.FindActionSourceWorldPosition(action.Source);
            List<Card> list2 = action.Args.Cards.ToList();
            List<CardWidget> list = new List<CardWidget>();
            Vector3 localPosition = cardUi.ExileLocalPosition;
            Vector3 endPosition = cardUi.ExileLocalPosition;
            DG.Tweening.Sequence val = DOTween.Sequence();
            int count = list2.Count;
            if (count > 5)
            {
                Debug.LogWarning("Adding 5+ cards in one time, which may cause presentation weired.");
            }

            AudioManager.PlayUi("CardAppear");
            for (int i = 0; i < count; i++)
            {
                Card card = list2[i];
                CardWidget cardWidget = cardUi.CreateCardWidget(card, cardUi._rectTransform);
                list.Add(cardWidget);
                Transform transform = cardWidget.transform;
                if (vector.HasValue)
                {
                    Vector3 vector2 = (transform.position = vector.GetValueOrDefault());
                }
                else
                {
                    transform.localPosition = localPosition;
                }

                cardWidget.gameObject.SetActive(value: false);
                transform.localScale = new Vector3(0.2f, 0.2f);
                float num = interval * (float)i;
                TweenSettingsExtensions.InsertCallback(val, num, (TweenCallback)delegate
                {
                    cardWidget.gameObject.SetActive(value: true);
                });
                TweenSettingsExtensions.SetEase(TweenSettingsExtensions.Insert(val, num, (Tween)(object)ShortcutExtensions.DOLocalMove(transform, new Vector3(600f * ((float)i - (float)(count - 1) / 2f), 0f), 0.3f, false)), (Ease)3);
                TweenSettingsExtensions.Insert(val, num, (Tween)(object)ShortcutExtensions.DOScale(transform, Vector3.one, 0.3f));
            }

            yield return TweenExtensions.WaitForCompletion((Tween)(object)TweenSettingsExtensions.SetUpdate(TweenSettingsExtensions.AppendInterval(val, 0.3f), true));
            cardUi.RefreshAll();
            cardUi.StartCoroutine(EndRunner(cardUi));



            IEnumerator EndRunner(CardUi cardUi)
            {
                AudioManager.PlayUi("CardFly");
                var val2 = DOTween.Sequence();
                List<Transform> parents = new List<Transform>();
                foreach (var item3 in list.WithIndices())
                {
                    int item = item3.index;
                    CardWidget item2 = item3.elem;
                    float num2 = interval * (float)item;
                    Transform transform2 = item2.transform;
                    Transform transform3 = UnityEngine.Object.Instantiate(cardUi.cardFlyHelperPrefab, cardUi.cardEffectLayer);
                    parents.Add(transform3);
                    transform3.localPosition = transform2.localPosition;
                    transform2.SetParent(transform3);
                    GameObject gameObject = UnityEngine.Object.Instantiate(cardUi.cardFlyTrail, transform3);
                    TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)TweenSettingsExtensions.SetEase(ShortcutExtensions.DOLocalMove(transform3, endPosition, 0.4f, false), (Ease)2));
                    TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)ShortcutExtensions.DOLocalRotate(transform2, new Vector3(0f, 0f, -500f), 0.2f, (RotateMode)1));
                    TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)ShortcutExtensions.DOScale(transform2, new Vector3(0.2f, 0.2f, 1f), 0.2f));
                    int num3 = UnityEngine.Random.Range(200, 400);
                    TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)TweenSettingsExtensions.SetLoops(TweenSettingsExtensions.SetRelative(TweenSettingsExtensions.SetEase(ShortcutExtensions.DOLocalMoveY(transform2, (float)num3, 0.2f, false), (Ease)3), true), 2, (LoopType)1));
                    TweenSettingsExtensions.Insert(val2, num2, (Tween)(object)TweenSettingsExtensions.SetLoops(TweenSettingsExtensions.SetRelative(TweenSettingsExtensions.SetEase(ShortcutExtensions.DOLocalMoveY(gameObject.transform, (float)num3, 0.2f, false), (Ease)3), true), 2, (LoopType)1));
                }

                yield return TweenExtensions.WaitForCompletion((Tween)(object)TweenSettingsExtensions.SetUpdate(val2, true));
                foreach (CardWidget item4 in list)
                {
                    UnityEngine.Object.Destroy(item4.gameObject);
                }

                //CardUi.ImageBlink(cardUi.exileZoneButton.image);
                cardUi.ExileCount = cardUi.Battle.ExileZone.Count;
                foreach (Transform item5 in parents)
                {
                    UnityEngine.Object.Destroy(item5.gameObject, 0.2f);
                }
            }
        }
    }
}
