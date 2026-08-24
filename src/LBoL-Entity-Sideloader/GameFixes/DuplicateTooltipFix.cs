using HarmonyLib;
using LBoL.Base;
using LBoL.Core;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib;
using System.Collections.Generic;
using System.Linq;
using System;


/// <summary>
/// Fixes a bug with multiple status effects having the same relative effect, resulting in duplicate tooltips of that effect appearing.
/// </summary>
[HarmonyPatch(typeof(Library), nameof(Library.InternalEnumerateDisplayWords))]
public static class DuplicateTooltipFix
{
    // We use a Prefix to replace the internal logic with a version that strictly checks the HashSet BEFORE enqueuing.
    static bool Prefix(GameRunController gameRun, Keyword keyword, IEnumerable<string> initEffects, bool verbose, Keyword? exceptKeywords, ref IEnumerable<IDisplayWord> __result)
    {
        __result = EnumerateDisplayFix(gameRun, keyword, initEffects, verbose, exceptKeywords);
        return false; // Skip original method
    }

    static IEnumerable<IDisplayWord> EnumerateDisplayFix(GameRunController gameRun, Keyword keyword, IEnumerable<string> initEffects, bool verbose, Keyword? exceptKeywords)
    {
        int counter = 0;
        // Strict tracker added to make sure nothing is duplicated
        HashSet<string> seenTypes = new HashSet<string>(); 
        Keyword traveledKeywords = Keyword.None;
        Queue<IDisplayWord> queue = new Queue<IDisplayWord>();

        // Handle Keywords
        foreach (Keyword k in Keywords.EnumerateComponents(keyword))
        {
            var dw = Keywords.GetDisplayWord(k);
            if (!dw.IsHidden && (!dw.IsVerbose || verbose))
                queue.Enqueue(dw);
        }

        // Handle Initial relative Effects
        if (initEffects != null)
        {
            foreach (string text in initEffects.Distinct())
            {
                if (seenTypes.Add(text)) // Add returns false if already present
                {
                    StatusEffect se = TypeFactory<StatusEffect>.CreateInstance(text);
                    if (verbose || !se.Config.IsVerbose)
                    {
                        se.GameRun = gameRun;
                        queue.Enqueue(se);
                    }
                }
            }
        }

        // Process Queue
        while (queue.Count > 0)
        {
            if (++counter > 99) throw new OverflowException("Too many references");

            IDisplayWord front = queue.Dequeue();

            // Keyword handling
            if (front is KeywordDisplayWord keywordDisplay)
            {
                if (exceptKeywords != null && exceptKeywords.Value.HasFlag(keywordDisplay.Keyword))
                    continue;

                traveledKeywords |= keywordDisplay.Keyword;
                yield return front;
            }
            // Status Effect handling
            else if (front is StatusEffect se)
            {
                yield return front;

                // Add sub-keywords
                foreach (Keyword subkeyword in Keywords.EnumerateComponents(se.Config.Keywords))
                {
                    // If the keyword hasn't been flagged already, add it to the queue.
                    if (!traveledKeywords.HasFlag(subkeyword))
                    {
                        traveledKeywords |= subkeyword;
                        var dw2 = Keywords.GetDisplayWord(subkeyword);
                        if (!dw2.IsHidden && (!dw2.IsVerbose || verbose))
                            queue.Enqueue(dw2);
                    }
                }

                // Add sub-effects (THE FIX IS HERE)
                foreach (string text2 in se.Config.RelativeEffects)
                {
                    // Check seenTypes BEFORE creating instance or enqueuing. Much simpler and robust than the original's conditions.
                    if (seenTypes.Add(text2))
                    {
                        StatusEffect seChild = TypeFactory<StatusEffect>.CreateInstance(text2);
                        if (verbose || !seChild.Config.IsVerbose)
                        {
                            seChild.GameRun = gameRun;
                            queue.Enqueue(seChild);
                        }
                    }
                }
            }
        }
    }
}