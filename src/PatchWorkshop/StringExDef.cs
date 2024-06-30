using LBoL.ConfigData;
using LBoL.Core;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace PatchWorkshop
{
    public /*sealed*/ class StringExDef : ExhibitTemplate
    {
        public override IdContainer GetId() => nameof(StringEx);

        public override LocalizationOption LoadLocalization() => BepinexPlugin.exBatchLoc.AddEntity(this);

        public override ExhibitSprites LoadSprite() => new ExhibitSprites(ResourcesHelper.Sprites[typeof(Exhibit)][nameof(Yuerang)]);

        public override ExhibitConfig MakeConfig()
        {
            var con = DefaultConfig();
            con.Value1 = 69;
            return con;
        }
    }

    [EntityLogic(typeof(StringExDef))]
    public /*sealed*/ class StringEx : Exhibit
    {
        public string Custom1
        {
            get 
            {
                string res;
                try
                {
                    res = LocalizeProperty(key: "Custom1", decorated: true, required: true).RuntimeFormat(FormatWrapper);
                }
                catch (Exception ex)
                {
                    res = "<Error>";
                }
                return res;
            }
        }
    }
}
