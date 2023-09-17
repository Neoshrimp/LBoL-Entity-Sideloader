using LBoL.ConfigData;
using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Entities
{
    public abstract class StageTemplate : EntityDefinition
        //IConfigProvider<StageConfig>
    {
        public override Type ConfigType() => typeof(StageConfig);

        public override Type EntityType() => typeof(Stage);


        public override Type TemplateType() => typeof(StageTemplate);

    }
}
