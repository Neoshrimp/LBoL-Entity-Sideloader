using LBoL.Core.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.CustomKeywords
{
    /// <summary>
    /// Meant to be implemented on keyword SE. Execute extra logic when tooltip is displayed, i.e., move properties from keyword to SE.
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

    /// <summary>
    /// Method controlling how extended keyword name should be displayed on card description.
    /// Meant to be implemented on keyword SE. CardKeyword needs hasExtendedKeywordName set for the method to be invoked.
    /// </summary>
    public interface IExtendedKeywordName
    {
        public string ExtendedKeywordName(Card card);
    }

    /// <summary>
    /// Extend Card cloning methods regardless of keywords. Happens after keywords are cloned.
    /// cloningMethod never has value CloningMethod.DoesntMatter supplied.
    /// </summary>
    public interface IExtendedCardClone
    {
        public void ExtendedCardClone(Card clonedCard, CloningMethod cloningMethod);
    }
}
