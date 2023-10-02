using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace Random_Examples
{
    public sealed class NewGunDef : GunTemplate
    {
        public static int IntId = 120000;

        public override IdContainer GetId() => "New Gun";
        

        public override GunConfig MakeConfig()
        {
            var config = new GunConfig(
                Id: IntId,
                Name: "",
                Spell: "",
                Sequence: "",
                Animation: "shoot1",
                ForceHitTime: null,
                ForceHitAnimation: false,
                ForceShowEndStartTime: null,
                Shooter: "Direct",
                ShakePower: 1f
            );
            return config;
        }
    }


    public sealed class NewPieceDef : PieceTemplate
    {
        public override IdContainer GetId() => ConvertGunId(NewGunDef.IntId);

        public override PieceConfig MakeConfig()
        {

            var config = DefaultConfig();
            config.Projectile = new RedBlueBullet().UniqueId;
            return config;
        }
    }

    public sealed class RedBlueBullet : BulletTemplate
    {
        public override IdContainer GetId() => "RedBlueBullet";

        public override BulletConfig MakeConfig()
        {       
            var config = new BulletConfig(
               Name: "",
               // 2do UniqueId might be load order dependent
               Widget: new RedBlueCirclesEffect().UniqueId,
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
    }


}
