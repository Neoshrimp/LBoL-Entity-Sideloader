using LBoL.ConfigData;
using LBoL.Core.StatusEffects;
using LBoL.EntityLib.Cards.Neutral.Red;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

namespace LBoLEntitySideloader.Entities
{
    public abstract class LaserTemplate : EntityDefinition,
        IConfigProvider<LaserConfig>
    {
        public override Type ConfigType() => typeof(LaserConfig);

        public override Type EntityType() => throw new InvalidDataException();

        public override Type TemplateType() => typeof(LaserTemplate);


        /// <summary>
        /// Name : ,
        /// Widget : ,
        /// LaunchSfx : ,
        /// Size : ,
        /// Offset : ,
        /// Start : ,
        /// HitBody : effect Name,
        /// HitBodySfx : ,
        /// HitShield : ,
        /// HitShieldSfx : ,
        /// HitBlock : ,
        /// HitBlockSfx : ,
        /// Graze : ,
        /// GrazeSfx : ,
        /// </summary>
        /// <returns></returns>
        public LaserConfig DefaultConfig()
        {
            var config = new LaserConfig(
               Name : "",
               Widget : "",
               LaunchSfx : "Empty",
               Size : new Vector2(20f, 0f),
               Offset : new Vector2(10f, 0f),
               Start : 0,
               HitBody : "HitBody",
               HitBodySfx : "CommonHitBody",
               HitShield : "HitShield",
               HitShieldSfx : "CommonHitShield",
               HitBlock : "HitBlock",
               HitBlockSfx : "CommonHitShield",
               Graze : "",
               GrazeSfx : "CommonGraze"
                );
            return config;
        }
        public abstract LaserConfig MakeConfig();
        
    }
}
