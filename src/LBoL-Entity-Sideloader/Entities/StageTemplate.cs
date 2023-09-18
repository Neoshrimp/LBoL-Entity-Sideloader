using LBoL.ConfigData;
using LBoL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Entities
{
    public abstract class StageTemplate : EntityDefinition,
        IConfigProvider<StageConfig>,
        IGameEntityProvider<Stage>
    {
        public override Type ConfigType() => typeof(StageConfig);
        public override Type EntityType() => typeof(Stage);
        public override Type TemplateType() => typeof(StageTemplate);


        /// <summary>
        /// Id : ,
        /// Obj0 : ,
        /// Obj1 : ,
        /// Level1 : ,
        /// Obj2 : ,
        /// Level2 : ,
        /// Obj3 : ,
        /// Level3 : ,
        /// Obj4 : ,
        /// Level4 : 
        /// </summary>
        /// <returns></returns>
        public StageConfig DefaultConfig()
        {
            var config = new StageConfig(
                Id : "",
                Obj0 : "BambooForest0",
                Obj1 : "",
                Level1 : 3,
                Obj2 : "",
                Level2 : 6,
                Obj3 : "",
                Level3 : 9,
                Obj4 : "",
                Level4 : 12
                );
            return config;
        }
        public abstract StageConfig MakeConfig();
    }
}
