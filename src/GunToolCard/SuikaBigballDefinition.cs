using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace GunToolCard
{
    [OverwriteVanilla]
    public sealed class SuikaBigballDefinition : CardTemplate
    {
        public override IdContainer GetId()
        {
            return "SuikaBigball";
        }

        public override CardImages LoadCardImages()
        {
            return null;
        }

        public override LocalizationOption LoadText()
        {
            return null;
        }

        public override CardConfig MakeConfig()
        {
            return CardConfig.FromId(GetId());
        }

        [EntityLogic(typeof(SuikaBigballDefinition))]
        public sealed class SuikaBigball : Card
        {
            protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
            {
                UnityEngine.Debug.Log("SUIKA DEEEEEZ");
                return base.Actions(selector, consumingMana, precondition);
            }
        }

    }
}
