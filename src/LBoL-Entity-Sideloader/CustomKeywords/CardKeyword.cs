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


        /// <summary>
        /// Not readonly but probably shouldn't be changed past construction.
        /// </summary>
        public KwDescPos descPos = KwDescPos.Last;


        /// <summary>
        /// Should tooltip be NOT displayed? Overridden by key SE config.IsVerbose
        /// </summary>
        public readonly bool isVerbose = false;

        /// <summary>
        /// Display customizable string when listing keyword in card description?
        /// Should be set only keyword construction only. If true keyword SE MUST implement IExtendedKeywordName. 
        /// </summary>
        public bool hasExtendedKeywordName = false;



        /// <summary>
        /// Wrapper method exposing card information when cloning a card with keyword.
        /// </summary>
        /// <param name="og"></param>
        /// <param name="other"></param>
        /// <param name="cloningMethod"></param>
        /// <returns></returns>
        public virtual CardKeyword CloneWithCard(Card og, [MaybeNull]Card other, CloningMethod cloningMethod)
        {
            return Clone(cloningMethod);
        }

        /// <summary>
        /// How keyword properties should be cloned. Returning null means keyword is not clonable.
        /// </summary>
        /// <param name="cloningMethod">Optionally filter by method which performs the cloning.</param>
        /// <returns></returns>
        [return: MaybeNull]
        public virtual CardKeyword Clone(CloningMethod cloningMethod) 
        { 
            return new CardKeyword(kwSEid, isVerbose) { descPos = this.descPos }; 
        }


        /// <summary>
        /// How should current keyword be merged with another of the same type. Only matters if keyword has extra properties. 
        /// Performed on cloning and KeywordManager.AddCustomKeyword
        /// </summary>
        /// <param name="other"></param>
        public virtual void Merge(CardKeyword other) { }

        public CardKeyword(string kwSEid, bool isVerbose = false)
        {
            this.kwSEid = kwSEid;
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
