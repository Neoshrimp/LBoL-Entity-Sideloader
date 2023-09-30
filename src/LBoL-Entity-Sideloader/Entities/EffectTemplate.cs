using HarmonyLib;
using LBoL.ConfigData;
using LBoL.Presentation;
using LBoL.Presentation.Effect;
using LBoLEntitySideloader.Entities.Patches;
using LBoLEntitySideloader.Resource;
using Mono.Cecil;
using Mono.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace LBoLEntitySideloader.Entities
{
    public abstract class EffectTemplate : EntityDefinition,
        IConfigProvider<EffectConfig>,
        IResourceConsumer<EffectData>
    {
        public override Type ConfigType() => typeof(EffectConfig);


        public override Type EntityType() => throw new InvalidDataException();


        public override Type TemplateType() => typeof(EffectTemplate);



        public EffectConfig DefaultConfig()
        {
            var config = new EffectConfig(
                Name: "",
                Path: "",
                Life: 0
                );

            return config;
        }

        abstract public EffectConfig MakeConfig();

        abstract public EffectData LoadEffectData();

        public void Consume(EffectData effectData)
        {

            if (effectData.effectGo.TryGetComponent<EffectWidget>(out var ew))
            {
                EffectManager.Instance._effectDictionary.AlwaysAdd(EffectConfig.FromName(UniqueId).Path, ew);
                return;
            }
            Log.log.LogWarning($"Failed to get {nameof(EffectWidget)} for Effect with Name: {UniqueId}");
        }

/*        [HarmonyPatch(typeof(ResourcesHelper), nameof(ResourcesHelper.LoadEffect))]
        class LoadEffect_Patch
        {
            static bool Prefix(string path, ref EffectWidget __result)
            {
                var Name = path;

                if (UniqueTracker.Instance.IsLoadedOnDemand(typeof(EffectTemplate), Name, out var entityDefinition))
                {
                    if (entityDefinition is EffectTemplate et && EntityManager.HandleOverwriteWrap(() => { }, et, nameof(LoadEffectData), et.user))
                    {

                        if (et.LoadEffectData().effectGo.TryGetComponent<EffectWidget>(out var ew))
                        {
                            //ew.name = entityDefinition.UniqueId;
                            __result = ew;
                            return false;
                        }
                        throw new ArgumentException($"Failed to get {nameof(EffectWidget)} with Name: {Name}");
                    }
                    return true;
                }
                return true;
            }
        }*/


    }
}
