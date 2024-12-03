using HarmonyLib;
using JetBrains.Annotations;
using LBoL.Core.Cards;
using LBoL.Core.StatusEffects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LBoLEntitySideloader.CustomKeywords
{
    /// <summary>
    /// Keyword position in card's description.
    /// </summary>
    public enum KwDescPos
    {
        DoNotDisplay,
        First,
        Last
    }

    public enum CloningMethod
    {
        DoesntMatter,
        NonBattle,
        Copy,
        TwiceToken
    }


    /// <summary>
    /// Core class for custom keywords. Meant to be subclassed if keyword needs extra properties.
    /// </summary>
    public class CardKeyword
    {
        /// <summary>
        /// Identity of the keyword. Should be an Id of an existing StatusEffect which is used as localization middleman.
        /// Safety checks if SE actually exists are NOT performed.
        /// </summary>
        public readonly string kwSEid;

        public readonly KwDescPos descPos;

        public readonly bool isVerbose;


        /// <summary>
        /// How keyword properties should be cloned. Returning null means keyword is not clonable.
        /// </summary>
        /// <param name="cloningMethod">Optionally filter by method which performs the cloning.</param>
        /// <returns></returns>
        [return: MaybeNull]
        public virtual CardKeyword Clone(CloningMethod cloningMethod) 
        { 
            return new CardKeyword(kwSEid, descPos, isVerbose); 
        }


        /// <summary>
        /// How should current keyword be merged with another of the same type. Only matters if keyword has extra properties. 
        /// Performed on cloning and KeywordManager.AddCustomKeyword
        /// </summary>
        /// <param name="other"></param>
        public virtual void Merge(CardKeyword other) { }

        public CardKeyword(string kwSEid, KwDescPos descPos = KwDescPos.Last, bool isVerbose = false)
        {
            this.kwSEid = kwSEid;
            this.descPos = descPos;
            this.isVerbose = isVerbose;
        }


        public override bool Equals(object obj)
        {
            return obj is CardKeyword keyword &&
                   kwSEid == keyword.kwSEid;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(kwSEid);
        }
    }


}
