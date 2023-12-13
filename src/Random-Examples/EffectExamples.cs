using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Presentation.Effect;
using LBoL.Presentation.UI;
using LBoL.Presentation.UI.Panels;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Random_Examples.BepinexPlugin;

namespace Random_Examples
{
    public abstract class BaseRedParticlesEffectDef : EffectTemplate
    {
        static GameObject ParticleGo;

        public override EffectWidgetData LoadEffectData() 
        {
            if (ParticleGo == null)
            {
                ParticleGo = EffectWidgetData.CreateEffect(effectsAB.LoadAsset<GameObject>("RedBlueBalls"), null);
            }

            return new EffectWidgetData() { effectGo = ParticleGo };
        }

    }

    public sealed class RedBlueCirclesEffect : BaseRedParticlesEffectDef
    {
        public override IdContainer GetId() => "RedBlueCircles";

        public override EffectConfig MakeConfig() => DefaultConfig();

    }

    [OverwriteVanilla]
    public abstract class RedParticleOverwrite : BaseRedParticlesEffectDef
    {
        [DontOverwrite]
        public override EffectConfig MakeConfig()
        {
            throw new NotImplementedException();
        }
    }


    public sealed class KoishiShootRedDef : RedParticleOverwrite
    {
        public override IdContainer GetId() => "DanmaBigHeart";
    }


    public sealed class ShootRedDef : RedParticleOverwrite 
    {
        public override IdContainer GetId() => "DanmaFish";
    }

    //gun = 凤翼天翔
    public sealed class DanmaAmuletRedDef : RedParticleOverwrite
    {
        public override IdContainer GetId() => "DanmaAmulet";

    }




}
