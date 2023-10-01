using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits.Common;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.PlayerUnits;
using LBoL.EntityLib.StatusEffects.Cirno;
using LBoL.Presentation;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.UIhelpers;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    [OverwriteVanilla]
    public sealed class StatusEffectExamples : StatusEffectTemplate
    {
        public override IdContainer GetId() => nameof(Cold);

        [DontOverwrite]
        public override LocalizationOption LoadLocalization()
        {
            throw new NotImplementedException();
        }

        [DontOverwrite]
        public override Sprite LoadSprite()
        {
            throw new NotImplementedException();
        }

        public override StatusEffectConfig MakeConfig()
        {
            var config = StatusEffectConfig.FromId(GetId());
            config.IsStackable = false;
            return config;
        }
    }
}
