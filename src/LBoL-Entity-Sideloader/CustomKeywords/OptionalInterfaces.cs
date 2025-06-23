using LBoL.Core.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.CustomKeywords
{
    /// <summary>
    /// Execute extra logic when tooltip is displayed, i.e., move properties from keyword to SE.
    /// </summary>
    public interface IOnTooltipDisplay
    {
        public void OnTooltipDisplay(Card card);
    }


    /// <summary>
    /// Effectively override(via postfix) non-virtual StatusEffect.Brief getter. Brief description is used to display SE(keyword) tooltip.
    /// </summary>
    public interface IOverrideSEBrief
    {
        public string OverrideBrief(string rawBrief);
    }
}
