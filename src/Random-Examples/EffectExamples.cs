using LBoL.ConfigData;
using LBoL.Presentation.Effect;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Random_Examples.BepinexPlugin;

namespace Random_Examples
{
    [OverwriteVanilla]
    public sealed class RedParticlesEffectDef : EffectTemplate
    {
        public override IdContainer GetId() => "DanmaFish";

        [DontOverwrite]
        public override EffectConfig MakeConfig() => DefaultConfig();

        public override EffectData LoadEffectData() 
        {
            log.LogDebug("DEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEZ");

            //var effectGo = GameObject.Instantiate(EffectManager.Instance._effectDictionary.First().Value.gameObject);

            var effectGo = GameObject.Instantiate(EffectManager.Instance._effectDictionary["Bullet/Danmaku/Fish"].gameObject);

            var sr = effectGo.AddComponent<SpriteRenderer>();
            sr.sprite = ResourceLoader.LoadSprite("Youmu.png", directorySource, ppu: 750, 1, FilterMode.Bilinear);

            
            //var effectGo = GameObject.Instantiate(SceneManager.GetSceneByName("HideAndDontSave").GetRootGameObjects().FirstOrDefault(go => go.name == "Fish"));


            //GameObject.DontDestroyOnLoad(effectGo);
            effectGo.hideFlags = HideFlags.HideAndDontSave;


            //effectGo.gameObject.layer


            effectGo.name = UniqueId;
/*            foreach (var c in effectGo.transform)
            {
                GameObject.Destroy(((Transform)c).gameObject);
            }*/
            var ew = effectGo.GetComponent<EffectWidget>();



            var particleGo = effectsAB.LoadAsset<GameObject>(effectsAB.GetAllAssetNames().First());
            particleGo.transform.position = Vector3.zero;
            particleGo.layer = effectGo.layer;
            particleGo.transform.SetParent(effectGo.transform);

            ew.particleSystemElements = new EffectWidget.ParticleSystemElement[] { new EffectWidget.ParticleSystemElement() { 
                particleSystem = particleGo.GetComponent<ParticleSystem>(), 
                changeColor = false, 
                dieType = EffectWidget.DieType.Inactivate, 
                lowPerformance = false} 
            };


            ew.trailRendererElements = new EffectWidget.TrailRendererElement[0];

            ew._particleSystemRawLifetimeColors = new ParticleSystem.MinMaxGradient[ew.particleSystemElements.Length];
            ew._particleSystemRawStartColors = new ParticleSystem.MinMaxGradient[ew.particleSystemElements.Length];
            ew._trailRenderRawColors = new Gradient[ew.trailRendererElements.Length];


            ew.ResetColors();

            return new EffectData() { effectGo = effectGo };
        }


    }
}
