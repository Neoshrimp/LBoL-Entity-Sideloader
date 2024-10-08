﻿using LBoL.Base.Extensions;
using LBoL.Core;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Rendering;
using YamlDotNet.RepresentationModel;

namespace LBoLEntitySideloader.Resource
{
    public abstract class LocalizationOption
    {

        internal static void FillLocalizationTables(Dictionary<string, Dictionary<string, object>>  termDic, Type facType, bool mergeTerms)
        {
            if (termDic != null)
            {
                foreach (var term in termDic)
                {
                    if (term.Value.Empty())
                        LocalizationFiles.MissingValueError(term.Key);
                    if (mergeTerms)
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


        public static YamlMappingNode TermDic2YamlMapping(Dictionary<string, Dictionary<string, object>> termDic)
        { 
            var mapping = new YamlMappingNode();
            foreach (var id in termDic.Keys)
            {
                var node = new YamlMappingNode();
                foreach (var termsKv in termDic[id])
                {
                    if (termsKv.Value.GetType() != typeof(string) &&
                        termsKv.Value.GetType().GetInterfaces()
                        .Any(i => (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        || i.GetType() == typeof(IEnumerable)))
                    {
                        node.Add(termsKv.Key, new YamlSequenceNode());
                        foreach (var item in (IEnumerable)termsKv.Value)
                        {
                            (node[termsKv.Key] as YamlSequenceNode).Add(new YamlScalarNode(item?.ToString()));
                        }
                    }
                    else
                    {
                        node.Add(termsKv.Key, new YamlScalarNode(termsKv.Value?.ToString()));
                    }
                }
                mapping.Add(id, node);
            }

            return mapping;
        }


        internal static void FillUnitNameTable(YamlMappingNode yamlKeyValue, bool mergeTerms, HashSet<IdContainer> keysToInclude = null)
        {

            foreach (KeyValuePair<YamlNode, YamlNode> keyValuePair in yamlKeyValue)
            {
                keyValuePair.Deconstruct(out var yamlNodeKey, out var yamlNodeValue);
                if (!(yamlNodeKey is YamlScalarNode yamlScalarKey))
                {
                    Log.log.LogError($"[Localization] UnitName key {yamlNodeKey} is not scalar");
                    continue;
                }

                if(keysToInclude != null && !keysToInclude.Contains(yamlScalarKey.Value)) 
                { continue; }


                if (!(yamlNodeValue is YamlMappingNode yamlNodeMap))
                {
                    Log.log.LogError($"[Localization] UnitName value of {yamlScalarKey.Value} is not mapping: {yamlNodeValue}");
                    continue;
                }

                if (mergeTerms)
                {
                    var unitName = new UnitName(yamlNodeMap);
                    UnitNameTable._table.TryAdd(yamlScalarKey.Value, unitName);

                    UnitNameTable._table[yamlScalarKey.Value]._table.Merge(unitName._table);

                }
                else
                {
                    UnitNameTable._table.AlwaysAdd(yamlScalarKey.Value, new UnitName(yamlNodeMap));
                }
            }
        }





     

    }
}
