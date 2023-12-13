using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnityEngine;


namespace LBoLEntitySideloader.Resource
{
    /// <summary>
    /// Constants used as suffixes for PlayerImages.AutoLoad method
    /// </summary>
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

        public const string cardImprint = "CardImprint";


    }

    /// <summary>
    /// Container class for loading player unit images.
    /// Synchronous or asynchronous function can be used to load most of the sprites.
    /// Asynchronous methods are prioritized if both methods are provided.
    /// By far most efficient way of loading Sprites is by using AssetBundles. 
    /// However, for mod development it might more convenient to load raw image files.
    /// Therefore, packaging AssetBundles can be postponed to the final stage of development.
    /// Unity Editor 2021.3.28 and https://github.com/Unity-Technologies/AssetBundles-Browser are required to package AssetBundles
    /// </summary>
    public class PlayerImages
    {
        /// <summary>
        /// Quickly find and init PlayerImages if image file naming convention is followed.
        /// Convention is defined in PISuffixes constants.
        /// Suffixes are wrapped with provided arguments like this: {pathPrefix}{Id}{s}{fileSuffix} where 's' is the relevant suffix.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="loadingAction"></param>
        /// <param name="asyncAction"></param>
        /// <param name="useSame"></param>
        /// <param name="fileSuffix"></param>
        /// <param name="pathPrefix"></param>
        public void AutoLoad(string Id, Func<string, Sprite> loadingAction, [MaybeNull] Func<string, UniTask<Sprite>> asyncAction, UseSame useSame = UseSame.StandDeckAndWinStand, string fileSuffix = ".png", string pathPrefix = "")
        { 
            var pi = this;

            Func<string, string> Wrap = (string s) => $"{pathPrefix}{Id}{s}{fileSuffix}";

            pi.SetStartPanelStand(asyncAction?.Invoke(Wrap(PISuffixes.stand)), () => loadingAction?.Invoke(Wrap(PISuffixes.stand)));

            string deckStand = Wrap((int)useSame > 0 ? PISuffixes.stand : PISuffixes.deckStand);
            pi.SetDeckStand(asyncAction?.Invoke(deckStand), () => loadingAction?.Invoke(deckStand));
            pi.SetDefeatedStand(asyncAction?.Invoke(Wrap(PISuffixes.defeatedStand)), () => loadingAction?.Invoke(Wrap(PISuffixes.defeatedStand)));

            string winStand = Wrap((int)useSame > 1 ? PISuffixes.stand : PISuffixes.winStand);
            pi.SetWinStand(asyncAction?.Invoke(winStand), () => loadingAction?.Invoke(winStand));

            pi.SetInRunAvatarPic(() => loadingAction?.Invoke(Wrap(PISuffixes.avatar)));
            pi.SetCollectionIcon(() => loadingAction?.Invoke(Wrap(PISuffixes.collectionIcon)));
            pi.SetSelectionCircleIcon(() => loadingAction?.Invoke(Wrap(PISuffixes.selectionCircleIcon)));

            pi.SetPerfectWinIcon(asyncAction?.Invoke(Wrap(PISuffixes.perfectWinIcon)), () => loadingAction?.Invoke(Wrap(PISuffixes.perfectWinIcon)));
            pi.SetWinIcon(asyncAction?.Invoke(Wrap(PISuffixes.winIcon)), () => loadingAction?.Invoke(Wrap(PISuffixes.winIcon)));
            pi.SetDefeatedIcon(asyncAction?.Invoke(Wrap(PISuffixes.defeatedIcon)), () => loadingAction?.Invoke(Wrap(PISuffixes.defeatedIcon)));

            pi.SetCardImprint(() => loadingAction?.Invoke(Wrap(PISuffixes.cardImprint)));

        }


        public enum UseSame
        {
            None,
            StandAndDeck,
            StandDeckAndWinStand
        }



        public void SetStartPanelStand(UniTask<Sprite>? task, Func<Sprite> func = null) { startPanelStandTask = task; startPanelStandFunc = func; }
        public void SetDeckStand(UniTask<Sprite>? task, Func<Sprite> func = null) { deckStandTask = task; deckStandFunc = func; }
        public void SetWinStand(UniTask<Sprite>? task, Func<Sprite> func = null) { winStandTask = task; winStandFunc = func; }
        public void SetDefeatedStand(UniTask<Sprite>? task, Func<Sprite> func = null) { defeatedStandTask = task; defeatedStandFunc = func; }

        /// <summary>
        /// some scale of 846x688
        /// </summary>
        public void SetInRunAvatarPic(Func<Sprite> func) { inRunAvatarPic = func; }
        /// <summary>
        /// 448x306. top and bottom slightly cropped
        /// </summary>
        public void SetDefeatedIcon(UniTask<Sprite>? task, Func<Sprite> func = null) { defeatedIconTask = task; defeatedIconFunc = func; }
        /// <summary>
        /// 448x306. top and bottom slightly cropped
        /// </summary>
        public void SetWinIcon(UniTask<Sprite>? task, Func<Sprite> func = null) { winIconTask = task; winIconFunc = func;  }
        /// <summary>
        /// 448x306. top and bottom slightly cropped
        /// </summary>
        public void SetPerfectWinIcon(UniTask<Sprite>? task, Func<Sprite> func = null) { perfectWinIconTask = task; perfectWinIconFunc = func; }
        /// <summary>
        /// 970x236
        /// </summary>
        public void SetCollectionIcon(Func<Sprite> func) { collectionIcon = func; }
        /// <summary>
        /// 320x320 circle
        /// </summary>
        public void SetSelectionCircleIcon(Func<Sprite> func) { selectionCircleIcon = func; }
        /// <summary>
        /// 460x240
        /// </summary>
        public void SetCardImprint(Func<Sprite> func) { cardImprint = func; }



        internal Sprite LoadStartPanelStand()
        {
            var s = startPanelStandFunc?.Invoke();
            return s == null ? emptySprite : s;
        }
        internal Sprite LoadDeckStand()
        {
            var s = deckStandFunc?.Invoke();
            return s == null ? emptySprite : s;
        }

        internal Sprite LoadWinStand()
        {
            var s = winStandFunc?.Invoke();
            return s == null ? emptySprite : s;
        }
        internal Sprite LoadDefeatedStand()
        {
            var s = defeatedStandFunc?.Invoke();
            return s == null ? emptySprite : s;
        }

        internal Sprite LoadDefeatedIcon()
        {
            var s = defeatedIconFunc?.Invoke();
            return s == null ? emptySprite : s;
        }
        internal Sprite LoadWinIcon()
        {
            var s = winIconFunc?.Invoke();
            return s == null ? emptySprite : s;

        }
        internal Sprite LoadPerfectWinIcon()
        {
            var s = perfectWinIconFunc?.Invoke();
            return s == null ? emptySprite : s;

        }


        internal async UniTask<Sprite> LoadStartPanelStandAsync()
        {
            if (startPanelStandTask == null)
                return null;
            var s = await startPanelStandTask.Value;
            return s == null ? emptySprite : s;
        }
        internal async UniTask<Sprite> LoadDeckStandAsync()
        {
            if (deckStandTask == null)
                return null;
            var s = await deckStandTask.Value;
            return s == null ? emptySprite : s;
        }

        internal async UniTask<Sprite> LoadWinStandAsync()
        {
            if (winStandTask == null)
                return null;
            var s = await winStandTask.Value;
            return s == null ? emptySprite : s;
        }
        internal async UniTask<Sprite> LoadDefeatedStandAsync()
        {
            if (defeatedStandTask == null)
                return null;
            var s = await defeatedStandTask.Value;
            return s == null ? emptySprite : s;
        }

        internal async UniTask<Sprite> LoadDefeatedIconAsync()
        {
            if (defeatedIconTask == null)
                return null;
            var s = await defeatedIconTask.Value;
            return s == null ? emptySprite : s;

        }
        internal async UniTask<Sprite> LoadWinIconAsync()
        {
            if (winIconTask == null)
                return null;
            var s = await winIconTask.Value;
            return s == null ? emptySprite : s;

        }
        internal async UniTask<Sprite> LoadPerfectWinIconAsync()
        {
            if (perfectWinIconTask == null)
                return null;
            var s = await perfectWinIconTask.Value;
            return s == null ? emptySprite : s;

        }

        internal Sprite LoadInRunAvatarPic()
        {
            var s = inRunAvatarPic?.Invoke();
            return s == null ? emptySprite : s;
        }
        internal Sprite LoadCollectionIcon()
        {
            var s = collectionIcon?.Invoke();
            return s == null ? emptySprite : s;
        }
        internal Sprite LoadSelectionCircleIcon()
        {
            var s = selectionCircleIcon?.Invoke();
            // circle icon has default sprite
            return s == null ? null : s;
        }
        internal Sprite LoadCardBack()
        {
            var s = cardImprint?.Invoke();
            return s == null ? emptySprite : s;
        }


        internal UniTask<Sprite>? startPanelStandTask;
        internal Func<Sprite> startPanelStandFunc;


        internal UniTask<Sprite>? deckStandTask;
        internal Func<Sprite> deckStandFunc;


        internal UniTask<Sprite>? winStandTask;
        internal Func<Sprite> winStandFunc;
        internal UniTask<Sprite>? defeatedStandTask;
        internal Func<Sprite> defeatedStandFunc;

        internal Func<Sprite> inRunAvatarPic;
        internal Func<Sprite> cardImprint;


        internal UniTask<Sprite>? defeatedIconTask;
        internal Func<Sprite> defeatedIconFunc;
        internal UniTask<Sprite>? winIconTask;
        internal Func<Sprite> winIconFunc;
        internal UniTask<Sprite>? perfectWinIconTask;
        internal Func<Sprite> perfectWinIconFunc;


        internal Func<Sprite> collectionIcon;
        internal Func<Sprite> selectionCircleIcon;

        public readonly static Sprite emptySprite = Sprite.Create(new Texture2D(0, 0, TextureFormat.ARGB32, false), new Rect(), new Vector2(0.5f, 0.5f));

    }
}
