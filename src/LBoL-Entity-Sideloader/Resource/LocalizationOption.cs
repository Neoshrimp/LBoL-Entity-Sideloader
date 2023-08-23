using LBoL.Base.Extensions;
using LBoL.Core;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Resource
{
    public abstract class LocalizationOption
    {

        internal static void FillLocalizationTables(Dictionary<string, Dictionary<string, object>>  termDic, Type facType, LocalizationFiles locFiles)
        {
            if (termDic != null)
            {
                foreach (var term in termDic)
                {
                    if (term.Value.Empty())
                        LocalizationFiles.MissingValueError(term.Key);
                    if (locFiles.mergeTerms)
                    {
                        TypeFactoryReflection.AccessTypeLocalizers(facType)().TryAdd(term.Key, new Dictionary<string, object>());
                        TypeFactoryReflection.AccessTypeLocalizers(facType)()[term.Key].Merge(term.Value);
                    }

                    else
                    {
                        TypeFactoryReflection.AccessTypeLocalizers(facType)().AlwaysAdd(term.Key, term.Value);
                    }
                }
            }
        }
    }
}
