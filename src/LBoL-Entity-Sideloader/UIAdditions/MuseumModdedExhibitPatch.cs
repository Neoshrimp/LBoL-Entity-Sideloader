using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI.Widgets;
using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LBoLEntitySideloader.Entities.ExhibitTemplate;

namespace LBoLEntitySideloader.UIAdditions
{
    [HarmonyPatch(typeof(MuseumPanel), nameof(MuseumPanel.EnsureExhibitsInitialized))]
    static class MuseumModdedExhibitPatch
    {
        public const int ModIndexStart = 900;

        /// <summary>
        /// The rules are as follow:
        /// <br>1. Shop only exhibits go to the shop.</br>
        /// <br>2. Shining exhibits always go to the shining pool.</br>
        /// <br>3.Rare/Uncommon/Common exhibits go to their respective buckets only if pooled.</br>
        /// <br>4. Everything else goes to events and other.</br>
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static MuseumSection AutoSection(ExhibitConfig cfg)
        {
            if (cfg.Appearance == AppearanceType.ShopOnly) return MuseumSection.Shop;
            switch (cfg.Rarity)
            {
                case Rarity.Mythic: return MuseumSection.EventsAndOther;
                case Rarity.Shining: return MuseumSection.Shining;
                case Rarity.Rare: return cfg.IsPooled ? MuseumSection.Rare : MuseumSection.EventsAndOther;
                case Rarity.Uncommon: return cfg.IsPooled ? MuseumSection.Uncommon : MuseumSection.EventsAndOther;
                case Rarity.Common: return cfg.IsPooled ? MuseumSection.Common : MuseumSection.EventsAndOther;
                default: return MuseumSection.EventsAndOther;
            }
        }

        static void Prefix(MuseumPanel __instance, out bool __state)
            => __state = !__instance._exhibitsInitialized;

        static void Postfix(MuseumPanel __instance, bool __state)
        {
            if (!__state) return;

            var templates = EntityManager.Instance.AllUsers
                .SelectMany(u => u.userInfo.definitionInstances.Values)
                .OfType<ExhibitTemplate>()
                .ToDictionary(t => t.UniqueId.ToString(), t => t);

            var modded = Library.EnumerateExhibitTypes()
                .Where(e => e.config.Index >= ModIndexStart && !e.config.IsDebug)
                .OrderBy(e => e.config.Index);

            foreach (var (type, cfg) in modded)
            {
                var section = MuseumSection.Auto;
                if (templates.TryGetValue(cfg.Id, out var t))
                    section = t.GetMuseumSection();
                if (section == MuseumSection.Auto)
                    section = AutoSection(cfg);

                if (!__instance._exhibitWidgetDic.TryGetValue((int)section, out var list))
                    continue;   // vanilla section missing, skip quietly

                var widget = UnityEngine.Object.Instantiate(
                    __instance.exhibitWidgetTemplate, __instance.exhibitsLayout);
                widget.Exhibit = Library.CreateExhibit(type);
                widget.exhibitWidget.ExhibitClicked += delegate
                {
                    __instance.OnMuseumExhibitClick(widget);
                };
                list.Add(widget);
            }
        }

        // Maybe add a new section for mods specifically in the future.
        //public const int ModsSectionKey = 9;
        //static List<MuseumExhibitWidget> CreateSection(MuseumPanel panel, int key)
        //{
        //    var label = UnityEngine.Object.Instantiate(panel.separatorLabelTemplate, panel.exhibitsLayout);
        //    panel._exhibitLabelDic.Add(key, label);
        //    var list = new List<MuseumExhibitWidget>();
        //    panel._exhibitWidgetDic.Add(key, list);
        //    return list;
        //}
    }
}
