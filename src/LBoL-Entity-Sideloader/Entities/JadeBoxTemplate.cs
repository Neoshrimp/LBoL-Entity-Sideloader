using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Entities
{
    public abstract class JadeBoxTemplate : EntityDefinition,
        IConfigProvider<JadeBoxConfig>,
        IGameEntityProvider<JadeBox>,
        IResourceConsumer<LocalizationOption>
    {
        public override Type ConfigType() => typeof(JadeBoxConfig);
        public override Type EntityType() => typeof(JadeBox);
        public override Type TemplateType() => typeof(JadeBoxTemplate);
        
        
        /// <summary>
        /// Group: restricts JadeBox selection. Only one JadeBox per group can be selected. Empty string list = no restrictions
        /// Keywords: should be RelativeKeywords. Cosmetic tooltip keywords.
        /// </summary>
        /// <returns></returns>
        public JadeBoxConfig DefaultConfig()
        {
            var config = new JadeBoxConfig(
                Index: 0,
                Id: "",
                Order: 10,
                Group: new List<string>() { },
                Value1: null,
                Value2: null,
                Value3: null,
                Mana: null,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { }
                );

            return config;
        }


        public abstract JadeBoxConfig MakeConfig();

        public abstract LocalizationOption LoadLocalization();

        public void Consume(LocalizationOption resource)
        {
            ProcessLocalization(resource, EntityType());
        }
    }
}
