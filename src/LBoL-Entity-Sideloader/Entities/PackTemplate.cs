using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Mono.CSharp.Argument;
using LBoL.Core;
using UnityEngine;
using LBoL.Core.StatusEffects;
using LBoL.Presentation;

namespace LBoLEntitySideloader.Entities
{
    public abstract class PackTemplate : EntityDefinition,
        IConfigProvider<PackConfig>,
        IResourceConsumer<PackIcons>,
        IResourceConsumer<LocalizationOption>
    {

        public override Type TemplateType() => typeof(PackTemplate);

        public override Type ConfigType() => typeof(PackConfig);

        public override Type EntityType() => throw new InvalidDataException();

        // packs dont actually reference pack config in anyway
        public PackConfig DefaultConfig()
        {
            return new PackConfig("", new string[] { });
        }

        public PackConfig MakeConfig()
        {
            return DefaultConfig();
        }


        public abstract PackIcons LoadPackIcon();

        public abstract LocalizationOption LoadLocalization();

        public void Consume(PackIcons icons)
        {
            if (icons == null)
                return;
            
            if(icons.mainIcon != null)
                ResourcesHelper.StickerSprites.AlwaysAdd(GetId()+"Sticker", icons.mainIcon);

            if (icons.disabledIcon != null)
                ResourcesHelper.StickerSprites.AlwaysAdd(GetId() + "StickerOff", icons.disabledIcon);
        }


        public void Consume(LocalizationOption locOption)
        {

            if (locOption == null)
                return;


            if (locOption is GlobalLocalization)
                throw new InvalidOperationException($"{nameof(GlobalLocalization)} LocalizationOption not supported for {nameof(PackTemplate)}");


            if (locOption is LocalizationFiles locFiles)
            {
                var termDic = locFiles.LoadLocTable(new string[] { GetId() });
                FillPacksLocTable(termDic, locFiles.mergeTerms);
                return;

            }

            if (locOption is DirectLocalization rawLoc)
            {
                var termDic = rawLoc.WrapTermDic(UniqueId);
                FillPacksLocTable(termDic, rawLoc.mergeTerms);
                return;
            }

            if (locOption is BatchLocalization batchLocalization)
            {
                batchLocalization.RegisterSelf(userAssembly);
                return;
            }
        }



        internal static void FillPacksLocTable(Dictionary<string, Dictionary<string, object>> termDic, bool mergeTerms)
        {

            var table = Packs._table;

            foreach (var kv in termDic)
            {

                var id = kv.Key;
                var terms = kv.Value;


                var packDisplayWord = new Packs.PackDisplayWord(id, terms.GetValueOrDefault("Name") as string, terms.GetValueOrDefault("Description") as string);


                if (mergeTerms)
                {
                    if (table.TryGetValue(id, out var currentPackDisplay))
                    {
                        string nameToReplace = null;
                        if (packDisplayWord.Name != null)
                            nameToReplace = packDisplayWord.Name;
                        else
                            nameToReplace = currentPackDisplay.Name;


                        string descToReplace = null;
                        if (packDisplayWord.Description != null)
                            descToReplace = packDisplayWord.Description;
                        else
                            descToReplace = currentPackDisplay.Description;

                        table[id] = new Packs.PackDisplayWord(id, nameToReplace, descToReplace);

                    }
                    else
                    {
                        table.Add(id, packDisplayWord);
                    }
                }
                else
                {
                    table.AlwaysAdd(id, packDisplayWord);
                }
            }

        }


    }
}