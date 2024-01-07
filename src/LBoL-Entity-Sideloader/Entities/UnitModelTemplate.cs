using LBoL.ConfigData;
using LBoL.EntityLib.EnemyUnits.Normal;
using Mono.CSharp.Linq;
using Mono.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEngine.InputSystem.HID;
using LBoL.Presentation.Units;
using UnityEngine;
using Cysharp.Threading.Tasks;
using LBoLEntitySideloader.Resource;
using HarmonyLib;
using LBoL.Presentation;
using Spine.Unity;
using DG.Tweening.Plugins.Options;
using LBoL.Base.Extensions;
using static Mono.CSharp.Argument;
using System.Data;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.Core;
using YamlDotNet.RepresentationModel;
using LBoL.Presentation.UI.Panels;

namespace LBoLEntitySideloader.Entities
{
    public abstract class UnitModelTemplate : EntityDefinition,
        IConfigProvider<UnitModelConfig>,
        IResourceConsumer<LocalizationOption>


    {
        public override Type ConfigType() => typeof(UnitModelConfig);

        public override Type EntityType() => throw new InvalidDataException();

        public override Type TemplateType() => typeof(UnitModelTemplate);
        /// <summary>
        /// Name : Id should be the same as EnemyUnit's;
        /// Type : model type: 0 - Sprite; 1 - Spine model; 3 - effect (like spirits or Kokoro masks);
        /// EffectName : effect Id if Type=2 is used;
        /// Offset : ;
        /// Flip : player models seem to get auto flipped?;
        /// Dielevel : 0, 1 or 2 0 - "UnitDeathSmall", 1 - "UnitDeath", 2 - "UnitDeathLarge";
        /// Box : collision box size. Player sized units it's (0.80f, 1.80f);
        /// Shield : barrier? effect radius;
        /// Block : block? effect radius;
        /// Hp : hp bar position;
        /// HpLength : hp bar length;
        /// Info : ;
        /// Select : ;
        /// ShootStartTime : ;
        /// ShootPoint : ;
        /// ShooterPoint : actual shoot position is the sum of these to point?;
        /// Hit : ;
        /// HitRep : ;
        /// GuardRep : ;
        /// Chat : ;
        /// ChatPortraitXY : ;
        /// ChatPortraitWH : ;
        /// HasSpellPortrait : should load extra sprite for bomb cast;
        /// SpellPosition : scale of this vector is in 100s;
        /// SpellScale : ;
        /// SpellColor : 4 color values or empty. If the list is empty UnitView.GetDefaultSpellColors will be used;
        /// 
        /// this config is for player sized models 2do
        /// </summary>
        /// <returns></returns>
        public UnitModelConfig DefaultConfig()
        {
            var config = new UnitModelConfig(
                    Name : "",
                    Type : (int)UnitView.ModelType.SingleSprite,
                    EffectName : "",
                    Offset : new Vector2(0, 0),
                    Flip : false,
                    Dielevel : 0,
                    Box : new Vector2(0.80f, 1.80f),
                    Shield : 1.2f,
                    Block : 1.3f,
                    Hp : new Vector2(0.00f, -1.30f),
                    HpLength : 480,
                    Info : new Vector2(0.00f, 1.20f),
                    Select : new Vector2(1.60f, 2.00f),
                    ShootStartTime : new List<float>() { 0.1f },
                    ShootPoint : new List<Vector2>() { new Vector2(0.60f, 0.30f) },
                    ShooterPoint : new List<Vector2>() { new Vector2(0.60f, 0.30f) },
                    Hit : new Vector2(0.30f, 0.00f),
                    HitRep : 0.1f,
                    GuardRep : 0.1f,
                    Chat : new Vector2(0.40f, 0.80f),
                    ChatPortraitXY : new Vector2(-0.80f, -0.58f),
                    ChatPortraitWH : new Vector2(0.60f, 0.50f),
                    HasSpellPortrait : false,
                    SpellPosition : new Vector2(0f, 0f),
                    SpellScale : 1,
                    SpellColor: new List<Color32>() { 
                        new Color32(101, 229, 71, 255), 
                        new Color32(131, 221, 117, 255), 
                        new Color32(137, 221, 117, 150), 
                        new Color32(153, 220, 127, 255) 
                    }
                );
            return config;
        }

        public abstract UnitModelConfig MakeConfig();

        public abstract ModelOption LoadModelOptions();

        // never be called if HasSpellPortrait is false
        public abstract UniTask<Sprite> LoadSpellSprite();

        /// <summary>
        /// Default:, Short: and Long: . Default is mandatory. 
        /// For name display above model and naming playable character 
        /// </summary>
        /// <returns></returns>
        public abstract LocalizationOption LoadLocalization();

        public void Consume(LocalizationOption locOption)
        {
            if (locOption == null) return;

#pragma warning disable CS0618 // Type or member is obsolete
            if (locOption is GlobalLocalization globalLoc)
            {
                if (globalLoc.LocalizationFiles.locTable.NotEmpty())
                {
                    if (!UniqueTracker.Instance.unitNamesGlobalLocalizationFiles.TryAdd(userAssembly, globalLoc.LocalizationFiles))
                    { 
                    Log.LogDev()?.LogWarning($"{userAssembly.GetName().Name}: {GetType()} tries to set global unit name localization files but they've already been set by another {TemplateType().Name}.");
                    }
                }
                UniqueTracker.Instance.unitIdsToLocalize.TryAdd(userAssembly, new HashSet<IdContainer>());
                UniqueTracker.Instance.unitIdsToLocalize[userAssembly].Add(GetId());
                return;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            if (locOption is LocalizationFiles locFiles)
            {

                var yamlMap = locFiles.Load(Localization.CurrentLocale);
                var keyNode = new YamlScalarNode(GetId());
                if (yamlMap.Children.TryGetValue(keyNode, out var yamlNode) && yamlNode is YamlMappingNode valuePairs)
                {
                    LocalizationOption.FillUnitNameTable(
                        new YamlMappingNode( new KeyValuePair<YamlNode, YamlNode>(keyNode, valuePairs)), locFiles.mergeTerms);
                }
                else
                {
                    locFiles.fileNames.TryGetValue(locFiles.GetAvailableLocale(), out var filename);
                    Log.log.LogWarning($"{GetId()} not found in {filename}");
                }
                return;
            }

            if (locOption is DirectLocalization rawLoc)
            {
                var termDic = rawLoc.WrapTermDic(UniqueId);

                LocalizationOption.FillUnitNameTable(LocalizationOption.TermDic2YamlMapping(termDic), rawLoc.mergeTerms);
                return;
            }

            // 2do test
            if (locOption is BatchLocalization batchLoc)
            {
                batchLoc.RegisterSelf(userAssembly);

                return;
            }

        }


        bool CheckModelOptions()
        {
            ModelOption modelOption = LoadModelOptions();
            UnitView.ModelType modelType = 0;
            try
            {
                modelType = (UnitView.ModelType)UnitModelConfig.FromName(UniqueId).Type;

            }
            catch (Exception ex)
            {
                Log.log.LogError($"UnitModelConfig.Type should be number from 0 to 2 : {ex}");
                return false;
            }

            if (modelType != modelOption.modelType)
            {
                Log.log.LogError($"{this.GetType().Name}: UnitModelConfig.Type mismatches options supplied by {typeof(ModelOption).Name}");
                return false;
            }
            return true;
        }



        [HarmonyPatch(typeof(ResourcesHelper), nameof(ResourcesHelper.LoadSpineUnitAsync))]
        class LoadSpineUnitAsync_Patch
        {
            static bool Prefix(string characterName, ref UniTask<SkeletonDataAsset> __result)
            {

                if (UniqueTracker.Instance.IsLoadedOnDemand(typeof(UnitModelTemplate), characterName, out var entityDefinition))
                {
                    if (entityDefinition is UnitModelTemplate umT && EntityManager.HandleOverwriteWrap(() => { }, umT, nameof(LoadModelOptions), umT.user) && umT.CheckModelOptions())
                    {
                        __result = umT.LoadModelOptions().loadSpine;
                        return false;

                    }
                    return true;
                }
                return true;
            }
        }


        [HarmonyPatch(typeof(ResourcesHelper), nameof(ResourcesHelper.LoadSimpleUnitSpriteAsync))]
        class LoadSimpleUnitSpriteAsync_Patch
        {
            static bool Prefix(string characterName, ref UniTask<Sprite> __result)
            {

                if (UniqueTracker.Instance.IsLoadedOnDemand(typeof(UnitModelTemplate), characterName, out var entityDefinition))
                {
                    if (entityDefinition is UnitModelTemplate umT && EntityManager.HandleOverwriteWrap(() => { }, umT, nameof(LoadModelOptions), umT.user) && umT.CheckModelOptions())
                    {
                        __result = umT.LoadModelOptions().loadSprite;
                        return false;

                    }
                    return true;
                }
                return true;
            }
        }




        bool CheckSpellSpriteEnabled()
        {
            if (!UnitModelConfig.FromName(UniqueId).HasSpellPortrait)
            {
                Log.log.LogWarning($"{this.GetType().Name}: UnitModelConfig.HasSpellPortrait is false but HasSpellPortrait is being loaded anyway");
                return false;
            }
            return true;
        }



        [HarmonyPatch(typeof(ResourcesHelper), nameof(ResourcesHelper.LoadSpellPortraitAsync))]
        class LoadSpellPortraitAsync_Patch
        {
            static bool Prefix(string characterName, ref UniTask<Sprite> __result)
            {
                if (UniqueTracker.Instance.IsLoadedOnDemand(typeof(UnitModelTemplate), characterName, out var entityDefinition) )
                {
                    if (entityDefinition is UnitModelTemplate umT && EntityManager.HandleOverwriteWrap(() => { }, umT, nameof(LoadSpellSprite), umT.user) && umT.CheckSpellSpriteEnabled())
                    {
                        __result = umT.LoadSpellSprite();
                        return false;

                    }
                    return true;
                }
                return true;
            }
        }




    }
}
