using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.ExtraFunc
{
    /// <summary>
    /// A helper class for extending IEnumerator result.
    /// Useful for patching coroutine methods.
    /// 
    /// Example:
    /// [HarmonyPatch(typeof(Exhibit), "TriggerGain")]
    /// class Exhibit_Patch
    /// {
    ///     static void Postfix(ref IEnumerator __result)
    ///     {
    ///         var extendedRez = new CoroutineExtender(__result);
    ///         extendedRez.preEnum = Pre();
    ///         for (int i = 0; i < 10; i++)
    ///             extendedRez.midEnums.Add(Mid(i));
    ///         extendedRez.postEnum = Post();
    ///         __result = extendedRez.GetEnumerator();
    ///     }
    /// 
    ///     static IEnumerator Pre()
    ///     {
    ///         log.LogDebug("deez");
    ///         yield break;
    ///     }
    /// 
    ///     static IEnumerator Mid(int i)
    ///     {
    ///         log.LogDebug("deeznuts " + i);
    ///         yield break;
    ///     }
    ///     static IEnumerator Post()
    ///     {
    ///         log.LogDebug("nuts");
    ///         yield break;
    ///     }
    /// }
    /// </summary>
    public class CoroutineExtender : IEnumerable
    {
        public IEnumerator target_enumerator;
        public IEnumerator preEnum;
        public IEnumerator postEnum;
        /// <summary>
        /// Each enumerator in the list is yielded after target_enumerator yields a result.
        /// That means last midEnum can sometimes be equivalent to postEnum.
        /// </summary>
        public List<IEnumerator> midEnums = new List<IEnumerator>();
        public CoroutineExtender() { }

        public CoroutineExtender(IEnumerator target_enumerator) { this.target_enumerator = target_enumerator; }

        public IEnumerator GetEnumerator()
        {
            while (preEnum.MoveNext()) yield return preEnum.Current;
            int i = 0;
            while (target_enumerator.MoveNext())
            {
                yield return target_enumerator.Current;
                if (i < midEnums.Count)
                {
                    if (midEnums[i] != null)
                        while (midEnums[i].MoveNext()) yield return midEnums[i].Current;
                }
                i++;
            }
            while (postEnum.MoveNext()) yield return preEnum.Current;
        }
    }


}
