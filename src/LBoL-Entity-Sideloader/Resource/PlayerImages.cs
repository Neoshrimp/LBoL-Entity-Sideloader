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
    /// startPanelStandPic: some scale of 846x688
    /// </summary>
    public class PlayerImages
    {
        public readonly static Sprite emptySprite = Sprite.Create(new Texture2D(0, 0, TextureFormat.ARGB32, false), new Rect(), new Vector2(0.5f, 0.5f));

        public UniTask<Sprite> StartPanelStand { set => startPanelStand = value; }
        public UniTask<Sprite> DeckStand { set => deckStand = value; }
        public UniTask<Sprite> WinStand { set => winStand = value; }
        public UniTask<Sprite> DefeatedStand { set => defeatedStand = value; }
        public Func<Sprite> InRunAvatarPic { set => inRunAvatarPic = value; }
        public UniTask<Sprite> DefeatedIcon { set => defeatedIcon = value; }
        public UniTask<Sprite> WinIcon { set => winIcon = value; }
        public UniTask<Sprite> PerfectWinIcon { set => perfectWinIcon = value; }
        public Func<Sprite> CollectionIcon { set => collectionIcon = value; }
        public Func<Sprite> SelectionCircleIcon { set => selectionCircleIcon = value; }
        public Func<Sprite> CardBack { set => cardBack = value; }


        public async UniTask<Sprite> LoadStartPanelStand()
        {
            var s = await startPanelStand;
            return s == null ? emptySprite : s;

        }
        public async UniTask<Sprite> LoadDeckStand()
        {
            var s = await deckStand;
            return s == null ? emptySprite : s;

        }
        public async UniTask<Sprite> LoadWinStand()
        {
            var s = await winStand;
            return s == null ? emptySprite : s;

        }
        public async UniTask<Sprite> LoadDefeatedStand()
        {
            var s = await defeatedStand;
            return s == null ? emptySprite : s;

        }
        public Sprite LoadInRunAvatarPic()
        {
            var s = inRunAvatarPic?.Invoke();
            return s == null ? emptySprite : s;

        }
        public async UniTask<Sprite> LoadDefeatedIcon()
        {
            var s = await defeatedIcon;
            return s == null ? emptySprite : s;

        }
        public async UniTask<Sprite> LoadWinIcon()
        {
            var s = await winIcon;
            return s == null ? emptySprite : s;

        }
        public async UniTask<Sprite> LoadPerfectWinIcon()
        {
            var s = await perfectWinIcon;
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


        private UniTask<Sprite> startPanelStand;

        private UniTask<Sprite> deckStand;


        private UniTask<Sprite> winStand;
        private UniTask<Sprite> defeatedStand;

        private Func<Sprite> inRunAvatarPic;
        private Func<Sprite> cardBack;


        private UniTask<Sprite> defeatedIcon;
        private UniTask<Sprite> winIcon;
        private UniTask<Sprite> perfectWinIcon;


        private Func<Sprite> collectionIcon;
        private Func<Sprite> selectionCircleIcon;
    }
}
