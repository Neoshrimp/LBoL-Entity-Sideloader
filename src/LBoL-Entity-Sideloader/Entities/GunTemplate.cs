using JetBrains.Annotations;
using LBoL.ConfigData;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using Spine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace LBoLEntitySideloader.Entities
{
    public abstract class GunTemplate : EntityDefinition,
        IConfigProvider<GunConfig>
    {
        public override Type ConfigType() => typeof(GunConfig);

        public override Type EntityType() => throw new InvalidDataException();

        public override Type TemplateType() => typeof(GunTemplate);

        /// <summary>
        /// Id : the most important parameter. Maps gun to one or more Pieces,
        /// Name : technically the Id of the GunCOnfig but in reality just a cosmetic name,
        /// Spell : ,
        /// Sequence : Sequence Id,
        /// Animation : "shoot1", "shoot2", "shoot3" or "shoot4",
        /// ForceHitTime : ,
        /// ForceHitAnimation : ,
        /// ForceShowEndStartTime : ,
        /// Shooter : always "Direct" ?,
        /// ShakePower : ,
        /// </summary>
        /// <returns></returns>
        public GunConfig DefaultConfig()
        {
            var config = new GunConfig(
                    Id : 120000,
                    Name : "",
                    Spell : "",
                    Sequence : "",
                    Animation : "shoot1",
                    ForceHitTime : null,
                    ForceHitAnimation : false,
                    ForceHitAnimationSpeed: 0f,
                    ForceShowEndStartTime : null,
                    Shooter : "Direct",
                    ShakePower : 1f
                );
            return config;  
        }

        public abstract GunConfig MakeConfig();


    }
}
