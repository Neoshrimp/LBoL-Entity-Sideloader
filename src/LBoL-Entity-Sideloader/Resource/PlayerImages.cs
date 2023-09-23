using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    public static class PISuffixes
    {
        public const string stand = "Stand";
        public const string defeatedPic = "DefeatedStand";
        public const string deckPic = "DeckPic";
        public const string winPic = "WinPic";
        public const string avatar = "Avatar";

        public const string defeatedIcon = "DefeatedIcon";
        public const string winIcon = "WinIcon";
        public const string perfectWinIcon = "PerfectWinIcon";

        public const string collectionIcon = "CollectionIcon";
        public const string selectionCircleIcon = "SelectionCircleIcon";


    }

    public class PlayerImages
    {
        //const Sprite emptySprite = 

        public Func<Sprite> startPanelStandPic;
        public UniTask<Sprite> asyncStartPanelStandPic;
        public Sprite deckPic;

        public Sprite winPic;
        public Sprite defeatedPic;

        public Sprite inRunAvatarPic;


        public Sprite defeatedIcon;
        public Sprite winIcon;
        public Sprite perfectWinIcon;


        public Sprite collectionIcon;
        public Sprite selectionCircleIcon;


    }
}
