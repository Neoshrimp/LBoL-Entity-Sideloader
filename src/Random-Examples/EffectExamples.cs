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
    [OverwriteVanilla]
    public abstract class BaseRedParticlesEffectDef : EffectTemplate
    {
        static GameObject ParticleGo;

        [DontOverwrite]
        public override EffectConfig MakeConfig() => DefaultConfig();


        public override EffectData LoadEffectData() 
        {
            log.LogDebug("DEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEZ");

            //var effectGo = GameObject.Instantiate(EffectManager.Instance._effectDictionary.First().Value.gameObject);

            //var effectGo = GameObject.Instantiate(EffectManager.Instance._effectDictionary["Bullet/Danmaku/Fish"].gameObject);

            //var effectGo = new GameObject();
/*            var sr = effectGo.AddComponent<SpriteRenderer>();
            sr.sprite = ResourceLoader.LoadSprite("Youmu.png", directorySource, ppu: 750, 1, FilterMode.Bilinear);*/

            
            //var effectGo = GameObject.Instantiate(SceneManager.GetSceneByName("HideAndDontSave").GetRootGameObjects().FirstOrDefault(go => go.name == "Fish"));


            //GameObject.DontDestroyOnLoad(effectGo);
            //effectGo.hideFlags = HideFlags.HideAndDontSave;

            ;
            //effectGo.gameObject.layer


            //effectGo.name = UniqueId;
            /*            foreach (var c in effectGo.transform)
                        {
                            GameObject.Destroy(((Transform)c).gameObject);
                        }*/

            //effectGo.AddComponent<EffectWidget>();
            //var ew = effectGo.GetComponent<EffectWidget>();
            //effectGo.layer = 10; // bullet 11- effect

            /*            var fireP = UiManager.GetPanel<UltimateSkillPanel>()?.fireParticle1;
                        if (fireP != null)
                        {
                            CopyComponent(effectGo, fireP);
                        }*/


            if (ParticleGo == null)
            { 
                ParticleGo = effectsAB.LoadAsset<GameObject>(effectsAB.GetAllAssetNames().First());
                log.LogInfo("p go name: " + ParticleGo.name);
                ParticleGo.transform.position = Vector3.zero;
                ParticleGo.layer = 10;
                var ew = ParticleGo.AddComponent<EffectWidget>();



                ParticleGo.GetComponent<ParticleSystemRenderer>().sortingLayerName = "Bullet";

                ew.particleSystemElements = new EffectWidget.ParticleSystemElement[] { new EffectWidget.ParticleSystemElement() { 
                    particleSystem = ParticleGo.GetComponent<ParticleSystem>(), 
                    changeColor = false, 
                    dieType = EffectWidget.DieType.Inactivate, 
                    lowPerformance = false} 

                };


                ew.trailRendererElements = new EffectWidget.TrailRendererElement[0];

                ew._particleSystemRawLifetimeColors = new ParticleSystem.MinMaxGradient[ew.particleSystemElements.Length];
                ew._particleSystemRawStartColors = new ParticleSystem.MinMaxGradient[ew.particleSystemElements.Length];
                ew._trailRenderRawColors = new Gradient[ew.trailRendererElements.Length];


                ew.ResetColors();
            }

            return new EffectData() { effectGo = ParticleGo };
        }


       

    }


    public sealed class KoishiShootRedDef : BaseRedParticlesEffectDef
    {
        public override IdContainer GetId() => "DanmaBigHeart";
    }


    public sealed class ShootRedDef : BaseRedParticlesEffectDef {
        public override IdContainer GetId() => "DanmaFish";

    }

    //gun = 凤翼天翔
    public sealed class DanmaAmuletRedDef : BaseRedParticlesEffectDef
    {
        public override IdContainer GetId() => "DanmaAmulet";

    }




}
