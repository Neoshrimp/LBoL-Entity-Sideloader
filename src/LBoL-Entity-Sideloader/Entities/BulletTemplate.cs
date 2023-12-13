using LBoL.ConfigData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LBoLEntitySideloader.Entities
{
    public abstract class BulletTemplate : EntityDefinition,
        IConfigProvider<BulletConfig>
    {
        public override Type ConfigType() => typeof(BulletConfig);

        public override Type EntityType() => throw new InvalidDataException();

        public override Type TemplateType() => typeof(BulletTemplate);


        /// <summary>
        /// Name: ;
        /// Widget: ;
        /// Launch: ;
        /// LaunchSfx: ;
        /// HitBody: ;
        /// HitBodySfx: ;
        /// HitShield: ;
        /// HitShieldSfx: ;
        /// HitBlock: ;
        /// HitBlockSfx: ;
        /// Graze: ;
        /// GrazeSfx: ;
        /// </summary>
        /// <returns></returns>
        public BulletConfig DefaultConfig()
        {
            var config = new BulletConfig(
               Name: "",
               Widget: "",
               Launch: "FireCommon",
               LaunchSfx: "CommonLaunchLight",
               HitBody: "HitBody",
               HitBodySfx: "CommonHitBody",
               HitShield: "HitShield",
               HitShieldSfx: "CommonHitShield",
               HitBlock: "HitBlock",
               HitBlockSfx: "CommonHitShield",
               Graze: "",
               GrazeSfx: "CommonGraze"
                );
            return config;
        }
        public abstract BulletConfig MakeConfig();

    }
}
