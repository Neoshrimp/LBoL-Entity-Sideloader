using LBoL.Core;
using LBoL.EntityLib.Exhibits.Shining;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LBoLEntitySideloader.Resource
{
    public class DirectLocalization : LocalizationOption
    {
        public bool mergeTerms;

        public Dictionary<Locale, Dictionary<string, object>> termDic = new Dictionary<Locale, Dictionary<string, object>>();



        public Locale defaultLocale;
        public Locale? fallbackLocale;

        public DirectLocalization(Dictionary<string, object> termDic, Locale defaultLocale = Locale.En, Locale fallbackLocale = Locale.En, bool mergeTerms = false)
        {
            this.defaultLocale = defaultLocale;
            this.fallbackLocale = fallbackLocale;
            this.mergeTerms = mergeTerms;
            this.termDic.Add(defaultLocale, termDic);
        }

        public void AddLocalizedTerms(Locale locale, Dictionary<string, object> termDic)
        { 
            this.termDic.Add(locale, termDic);
        }

        public Dictionary<string, Dictionary<string, object>> WrapTermDic(IdContainer Id)
        {
            var tDic = new Dictionary<string, Dictionary<string, object>>();

            if (termDic.TryGetValue(Localization.CurrentLocale, out var dic))
            {
                tDic.Add(Id, dic);
                return tDic;

            }
            if (fallbackLocale != null && termDic.TryGetValue(fallbackLocale.Value, out dic))
            {
                tDic.Add(Id, dic);
                return tDic;
            }

            tDic.Add(Id, new Dictionary<string, object>());
            return tDic;

        }

    }
}
