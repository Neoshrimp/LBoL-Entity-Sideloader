using HarmonyLib;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.EntityLib.Cards.Character.Sakuya;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Entities
{

    /// <summary>
    /// This template is only useful for setting enemy spell localization. 
    /// Player UltimateSkillTemplate should create 
    /// </summary>
    public abstract class SpellTemplate : EntityDefinition,
        IConfigProvider<SpellConfig>,
        IResourceConsumer<LocalizationOption>
        //IResourceConsumer<GameObject>
    {
        public override Type ConfigType() => typeof(SpellConfig);
        public override Type EntityType() => throw new InvalidDataException();
        public override Type TemplateType() => typeof(SpellTemplate);


        public const string OnCastTitle = "OnCastTitle";
        public const string OnCastName = "OnCastName";

        public SpellConfig DefaultConfig()
        {
            var config = new SpellConfig(
                    Id: "",
                    Resource: ""
                );
            return config;
        }

        public abstract SpellConfig MakeConfig();
        public abstract LocalizationOption LoadLocalization();

        public void Consume(LocalizationOption locOption)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (locOption is GlobalLocalization globalLoc)
            {
                if (globalLoc.LocalizationFiles.locTable.NotEmpty())
                {
                    UniqueTracker.Instance.spellEntriesLocFiles.TryAdd(userAssembly, new HashSet<LocalizationFiles>());

                    UniqueTracker.Instance.spellEntriesLocFiles[userAssembly].Add(globalLoc.LocalizationFiles);
/*                    if (!)
                    {

                        globalLoc.LocalizationFiles
                        Log.LogDev()?.LogWarning($"{userAssembly.GetName().Name}: {GetType()} tries to set global spell localization files but they've already been set by another {TemplateType().Name}.");
                    }*/
                }
                UniqueTracker.Instance.spellIdsToLocalize.TryAdd(userAssembly, new HashSet<string>());
                UniqueTracker.Instance.spellIdsToLocalize[userAssembly].Add(GetId());
                return;
            }
#pragma warning restore CS0618 // Type or member is obsolete
            if (locOption is BatchLocalization batchLocalization)
            {
                batchLocalization.RegisterSelf(userAssembly);
                return;
            }
        }



        internal static void LoadAllSpecialLoc(SpellPanel spellPanel = null)
        {
            foreach (var templates in UniqueTracker.Instance.spellTemplates.Values)
            {
                foreach (var spT in templates.Values)
                {
                    spT.LoadSpecialLoc(spT.LoadLocalization(), spellPanel);
                }
            }

        }

        /// <summary>
        /// on cast spell title
        /// </summary>
        /// <param name="locOption"></param>
        internal void LoadSpecialLoc(LocalizationOption locOption, SpellPanel spellPanel)
        {

#pragma warning disable CS0618 // Type or member is obsolete
            if (locOption is GlobalLocalization)
            {
                if (UniqueTracker.Instance.spellEntriesLocFiles.TryGetValue(user.assembly, out var spellLocfiles))
                {
                    foreach (var lf in spellLocfiles)
                    {
                        FillSpellPanelLocTable(lf.LoadLocTable(UniqueTracker.Instance.spellIdsToLocalize[user.assembly], addEmptyDic: false), lf.mergeTerms, spellPanel);
                    }
                }
                return;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            if (locOption is LocalizationFiles locFiles)
            {

                var termDic = locFiles.LoadLocTable(new string[] { GetId() });
                FillSpellPanelLocTable(termDic, locFiles.mergeTerms, spellPanel);
                return;
            }

            if (locOption is DirectLocalization rawLoc)
            {
                var termDic = rawLoc.WrapTermDic(UniqueId);
                FillSpellPanelLocTable(termDic, rawLoc.mergeTerms, spellPanel);
                return;
            }

            // actually loaded several times
            if (locOption is BatchLocalization batchLoc)
            {
                var lf = batchLoc.localizationFiles;
                FillSpellPanelLocTable(lf.LoadLocTable(batchLoc.entityIds, addEmptyDic: false), lf.mergeTerms, spellPanel);
                return;
            }
        }



        internal static void FillSpellPanelLocTable(Dictionary<string, Dictionary<string, object>> termDic, bool mergeTerms, SpellPanel spellPanel)
        {


            if (spellPanel == null)
            {
                try
                {
                    spellPanel = UiManager.GetPanel<SpellPanel>();
                }
                catch (InvalidOperationException)
                {
                    return;
                }
            }

            var table = (Dictionary<string, SpellPanel.Entry>)spellPanel._l10nTable;

            foreach (var kv in termDic)
            {
                var se = new SpellPanel.Entry() { Title = null, Name = null };

                var id = kv.Key;
                var terms = kv.Value;

                if (terms.TryGetValue(OnCastTitle, out var t1))
                    se.Title = t1.ToString() ?? "";
                else if (terms.TryGetValue("Title", out var t2))
                    se.Title = t2.ToString() ?? "";

                if (terms.TryGetValue(OnCastName, out var n1))
                    se.Name = n1.ToString() ?? "";
                else if (terms.TryGetValue("Name", out var n2))
                    se.Name = n2.ToString() ?? "";
                else if (terms.TryGetValue("Content", out var n3))
                    se.Name = n3.ToString() ?? "";


                /*                foreach (var tv in kv.Value)
                                {
                                    var tKey = tv.Key;
                                    var term = tv.Value;
                                    if (tKey == SpellTemplate.OnCastTitle)
                                    {
                                        se.Title = term.ToString() ?? "";
                                    }
                                    else if (tKey == SpellTemplate.OnCastName)
                                    {
                                        se.Name = term.ToString() ?? "";
                                    }
                                    else if (tKey == "Title" && !kv.Value.ContainsKey(SpellTemplate.OnCastTitle))
                                    {
                                        se.Title = term.ToString() ?? "";
                                    }
                                    else if (tKey == "Content" && !kv.Value.ContainsKey(SpellTemplate.OnCastName))
                                    {
                                        se.Name = term.ToString() ?? "";
                                    }
                                }*/

                if (mergeTerms)
                {
                    if (table.TryGetValue(id, out var cSe))
                    {
                        if (se.Title != null)
                            cSe.Title = se.Title;
                        if (se.Name != null)
                            cSe.Name = se.Name;
                    }
                    else
                    {
                        table.Add(id, se);
                    }
                }
                else
                {
                    table.AlwaysAdd(id, se);
                }
            }

        }

        // Not used
        //public virtual GameObject LoadResource() { return null; }


    }
}
