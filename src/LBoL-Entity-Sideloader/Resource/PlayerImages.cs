using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LBoL.Core.CrossPlatformHelper;

namespace LBoLEntitySideloader.Resource
{
    public static class PISuffixes
    {
        public const string stand = "Stand";
        public const string defeatedStand = "DefeatedStand";
        public const string deckStand = "DeckStand";
        public const string winStand = "WinStand";
        public const string avatar = "Avatar";


        public const string defeatedIcon = "DefeatedIcon";
        public const string winIcon = "WinIcon";
        public const string perfectWinIcon = "PerfectWinIcon";

        public const string collectionIcon = "CollectionIcon";
        public const string selectionCircleIcon = "SelectionCircleIcon";

        public const string cardBack = "CardBack";


    }

    /// <summary>
    /// Container class for loading player unit images.
    /// startPanelStandPic: just works? at least with Suika.png
    /// </summary>
    public class PlayerImages
    {
        public readonly static Sprite emptySprite = Sprite.Create(new Texture2D(0, 0, TextureFormat.ARGB32, false), new Rect(), new Vector2(0.5f, 0.5f));

        public void SetStartPanelStand(UniTask<Sprite> task, Func<Sprite> func = null) { startPanelStandTask = task; startPanelStandFunc = func; }
        public void SetDeckStand(UniTask<Sprite> task, Func<Sprite> func = null) { deckStandTask = task; deckStandFunc = func; }
        public void SetWinStand(UniTask<Sprite> task, Func<Sprite> func = null) { winStandTask = task; winStandFunc = func; }
        public void SetDefeatedStand(UniTask<Sprite> task, Func<Sprite> func = null) { defeatedStandTask = task; defeatedStandFunc = func; }

        /// <summary>
        /// some scale of 846x688
        /// </summary>
        public void SetInRunAvatarPic(Func<Sprite> func) { inRunAvatarPic = func; }
        public void SetDefeatedIcon(UniTask<Sprite> task, Func<Sprite> func = null) { defeatedIconTask = task; defeatedIconFunc = func; }
        public void SetWinIcon(UniTask<Sprite> task, Func<Sprite> func = null) { winIconTask = task; winIconFunc = func;  }
        public void SetPerfectWinIcon(UniTask<Sprite> task, Func<Sprite> func = null) { perfectWinIconTask = task; perfectWinIconFunc = func; }
        public void SetCollectionIcon(Func<Sprite> func) { collectionIcon = func; }
        public void SetSelectionCircleIcon(Func<Sprite> func) { selectionCircleIcon = func; }
        /// <summary>
        /// 460x240
        /// </summary>
        public void SetCardBack(Func<Sprite> func) { cardBack = func; }


        public Sprite LoadStartPanelStand()
        {
            var s = startPanelStandFunc?.Invoke();
            return s == null ? emptySprite : s;
        }
        public Sprite LoadDeckStand()
        {
            var s = deckStandFunc?.Invoke();
            return s == null ? emptySprite : s;
        }

        public Sprite LoadWinStand()
        {
            var s = winStandFunc?.Invoke();
            return s == null ? emptySprite : s;
        }
        public Sprite LoadDefeatedStand()
        {
            var s = defeatedStandFunc?.Invoke();
            return s == null ? emptySprite : s;
        }

        public Sprite LoadDefeatedIcon()
        {
            var s = defeatedIconFunc?.Invoke();
            return s == null ? emptySprite : s;

        }
        public Sprite LoadWinIcon()
        {
            var s = winIconFunc?.Invoke();
            return s == null ? emptySprite : s;

        }
        public Sprite LoadPerfectWinIcon()
        {
            var s = perfectWinIconFunc?.Invoke();
            return s == null ? emptySprite : s;

        }
        /// <summary>
        /// ////
        /// </summary>
        /// <returns></returns>

        public async UniTask<Sprite> LoadStartPanelStandAsync()
        {
            var s = await startPanelStandTask;
            return s == null ? emptySprite : s;
        }
        public async UniTask<Sprite> LoadDeckStandAsync()
        {
            var s = await deckStandTask;
            return s == null ? emptySprite : s;
        }

        public async UniTask<Sprite> LoadWinStandAsync()
        {
            var s = await winStandTask;
            return s == null ? emptySprite : s;
        }
        public async UniTask<Sprite> LoadDefeatedStandAsync()
        {
            var s = await defeatedStandTask;
            return s == null ? emptySprite : s;
        }

        public async UniTask<Sprite> LoadDefeatedIconAsync()
        {
            var s = await defeatedIconTask;
            return s == null ? emptySprite : s;

        }
        public async UniTask<Sprite> LoadWinIconAsync()
        {
            var s = await winIconTask;
            return s == null ? emptySprite : s;

        }
        public async UniTask<Sprite> LoadPerfectWinIconAsync()
        {
            var s = await perfectWinIconTask;
            return s == null ? emptySprite : s;

        }

        public Sprite LoadInRunAvatarPic()
        {
            var s = inRunAvatarPic?.Invoke();
            return s == null ? emptySprite : s;
        }
        public Sprite LoadCollectionIcon()
        {
            var s = collectionIcon?.Invoke();
            return s == null ? emptySprite : s;
        }
        public Sprite LoadSelectionCircleIcon()
        {
            var s = selectionCircleIcon?.Invoke();
            return s == null ? emptySprite : s;
        }
        public Sprite LoadCardBack()
        {
            var s = cardBack?.Invoke();
            return s == null ? emptySprite : s;
        }


        internal UniTask<Sprite> startPanelStandTask;
        internal Func<Sprite> startPanelStandFunc;


        internal UniTask<Sprite> deckStandTask;
        internal Func<Sprite> deckStandFunc;


        internal UniTask<Sprite> winStandTask;
        internal Func<Sprite> winStandFunc;
        internal UniTask<Sprite> defeatedStandTask;
        internal Func<Sprite> defeatedStandFunc;

        internal Func<Sprite> inRunAvatarPic;
        internal Func<Sprite> cardBack;


        internal UniTask<Sprite> defeatedIconTask;
        internal Func<Sprite> defeatedIconFunc;
        internal UniTask<Sprite> winIconTask;
        internal Func<Sprite> winIconFunc;
        internal UniTask<Sprite> perfectWinIconTask;
        internal Func<Sprite> perfectWinIconFunc;


        internal Func<Sprite> collectionIcon;
        internal Func<Sprite> selectionCircleIcon;
    }
}
