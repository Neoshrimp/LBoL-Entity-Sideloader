using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits.Common;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.PlayerUnits;
using LBoL.Presentation;
using LBoL.Presentation.UI;
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



    public static class CustomLoadouts
    {


        public static void AddLoadouts()
        {

            var cards = new List<string>() {
                        nameof(Shoot), nameof(Shoot), nameof(Boundary), nameof(Boundary),
                        nameof(ReimuAttackR),
                        nameof(ReimuAttackR),
                        nameof(ReimuAttackW),
                        nameof(ReimuBlockW),
                        nameof(ReimuBlockW),
                        nameof(ReimuBlockR),
                        // sunlight prayer
                        nameof(QiqingYishi)
                };

            PlayerUnitTemplate.AddLoadout(
                "Reimu",
                new ReimuUltRJabDef().UniqueId,
                nameof(CirnoU),
                cards,
                3
                );

            PlayerUnitTemplate.AddLoadout(
                "Reimu",
                new ReimuUltRJabDef().UniqueId,
                nameof(CirnoG),
                cards,
                1
                );


            PlayerUnitTemplate.AddLoadout(
                "Reimu",
                new ReimuUltRJabDef().UniqueId,
                nameof(Tuanzi),
                cards,
                2
                );

            PlayerUnitTemplate.AddLoadout(
                "Reimu",
                new ReimuUltRJabDef().UniqueId,
                nameof(CirnoG),
                cards,
                1
                );

            PlayerUnitTemplate.AddLoadout(
            "Reimu",
            new ReimuUltRJabDef().UniqueId,
            nameof(CirnoG),
            cards,
            1
            );

            PlayerUnitTemplate.AddLoadout(
            "Reimu",
            new ReimuUltRJabDef().UniqueId,
            nameof(CirnoG),
            cards,
            1
            );

            PlayerUnitTemplate.AddLoadout(
            "Reimu",
            new ReimuUltRJabDef().UniqueId,
            nameof(CirnoG),
            cards,
            1
            );

            PlayerUnitTemplate.AddLoadout(
            "Reimu",
            new ReimuUltRJabDef().UniqueId,
            nameof(CirnoG),
            cards,
            1
            );



            PlayerUnitTemplate.AddLoadout(
                "Marisa",
                new ReimuUltRJabDef().UniqueId,
                nameof(CirnoU),
                cards,
                3
                );

        }



       
    }
  
}
