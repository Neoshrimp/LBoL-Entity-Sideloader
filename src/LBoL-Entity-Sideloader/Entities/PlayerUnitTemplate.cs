using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Units;
using System;
using System.Collections.Generic;
using System.Text;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.UI;
using UnityEngine;
using LBoL.Presentation.UI.Panels;
using HarmonyLib;
using System.Reflection.Emit;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.Core.Cards;
using Mono.CSharp;
using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.Presentation.UI.Widgets;
using LBoL.Presentation.UI;
using LBoLEntitySideloader.UIhelpers;
using System.Linq;
using DG.Tweening;
using LBoLEntitySideloader.Utils;
using LBoL.Base.Extensions;
using System.Reflection;
using Cysharp.Threading.Tasks;
using LBoL.Presentation;
using System.Diagnostics;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Entities.Patches;


namespace LBoLEntitySideloader.Entities
{
    // soon(tm)
    public abstract class PlayerUnitTemplate : EntityDefinition,
        IConfigProvider<PlayerUnitConfig>,
        IGameEntityProvider<PlayerUnit>,
        IResourceConsumer<LocalizationOption>,
        IResourceConsumer<PlayerImages>

    {
        public override Type ConfigType() => typeof(PlayerUnitConfig);

        public override Type EntityType() => typeof(PlayerUnit);

        public override Type TemplateType() => typeof(PlayerUnitTemplate);



        static public void AddLoadout(string charId, Type ultimateSkill, Type exhibit, List<Type> deck, int complexity, Assembly callingAssembly = null)
        {
            AddExtraLoadout(charId, ultimateSkill.Name, exhibit.Name, deck.Select(t => t.Name).ToList(), complexity, callingAssembly);
        }

        static public void AddExtraLoadout(string charId, string ultimateSkill, string exhibit, List<string> deck, int complexity, Assembly callingAssembly = null)
        {

            if (callingAssembly == null)
                callingAssembly = Assembly.GetCallingAssembly();

                

            Action action = () =>
            {
                var loadoutInfos = UniqueTracker.Instance.loadoutInfos;
                loadoutInfos.TryAdd(charId, new List<UniqueTracker.CharLoadoutInfo>());

                var typeSuffix = Numbers.DecimalToABC(2 + loadoutInfos[charId].Count);

                var typeName = "Type" + typeSuffix;

                loadoutInfos[charId].Add(new UniqueTracker.CharLoadoutInfo()
                {
                    ultimateSkill = ultimateSkill,
                    exhibit = exhibit,
                    deck = deck,
                    complexity = complexity,
                    typeName = typeName,
                    typeSuffix = typeSuffix
                });
            };

            if (callingAssembly.IsLoadedFromDisk())
                EntityManager.Instance.loadedFromDiskCharLoadouts.Add(action);


            UniqueTracker.Instance.populateLoadoutInfosActions.Add(action);

        }





        /// <summary> 
        /// Id : 
        /// ShowOrder : show order in game start panel (defacto index)
        /// Order : ordering priority for character's cards in collection // 2do make unique like index?
        /// UnlockLevel : should be 0 to make character available right away
        /// ModleName : always ""
        /// NarrativeColor : color hex code
        /// IsSelectable : show character filter in collection??
        /// MaxHp : 
        /// InitialMana : 
        /// InitialMoney : 
        /// InitialPower : 
        /// UltimateSkillA : 
        /// UltimateSkillB : 
        /// ExhibitA : 
        /// ExhibitB : 
        /// DeckA : 
        /// DeckB : 
        /// DifficultyA : number from 1 to 3
        /// DifficultyB : number from 1 to 3
        /// </summary>
        /// <returns></returns>
        public PlayerUnitConfig DefaultConfig()
        {
            var config = new PlayerUnitConfig(
                    Id : "",
                    ShowOrder : 0,
                    Order : 0,
                    UnlockLevel : 0,
                    ModleName : "",
                    NarrativeColor : "#f241a8",
                    IsSelectable : true,
                    MaxHp : 1,
                    InitialMana : new LBoL.Base.ManaGroup() { },
                    InitialMoney : 1,
                    InitialPower : 0,
                    UltimateSkillA : "",
                    UltimateSkillB : "",
                    ExhibitA : "",
                    ExhibitB : "",
                    DeckA : new List<string>() { },
                    DeckB : new List<string>() { },
                    DifficultyA : 1,
                    DifficultyB : 1
                
                );
            return config;
        }


        public abstract PlayerUnitConfig MakeConfig();

        public abstract PlayerImages LoadPlayerImages();


        public void Consume(PlayerImages resource)
        {
            UniqueTracker.Instance.user2PlayerTemplates.TryAdd(userAssembly, new List<PlayerUnitTemplate>());
            UniqueTracker.Instance.user2PlayerTemplates[userAssembly].Add(this);

            EntityManager.HandleOverwriteWrap(() => CardWidget._ownerSpriteTable.AlwaysAdd(
                UniqueId, resource.LoadCardBack()),
                this,
                PlayerSpriteLoader.OverwriteName(PISuffixes.cardBack),
                this.user
            );
            
        }

        public abstract LocalizationOption LoadLocalization();


        /// <summary>
        /// Name, Title and such
        /// </summary>
        /// <param name="locOptions"></param>
        public void Consume(LocalizationOption locOptions)
        {
            ProcessLocalization(locOptions, EntityType());
        }





        [HarmonyPatch(typeof(ResourcesHelper), nameof(ResourcesHelper.LoadCharacterAvatarSprite))]
        class LoadCharacterAvatarSprite_Patch
        {
            static bool Prefix(string characterName, ref Sprite __result)
            {
                if (UniqueTracker.Instance.IsLoadedOnDemand(typeof(PlayerUnitTemplate), characterName, out var entityDefinition))
                {
                    if (entityDefinition is PlayerUnitTemplate puT && EntityManager.HandleOverwriteWrap(() => { }, puT, PlayerSpriteLoader.OverwriteName(PISuffixes.avatar), puT.user))
                    {
                        __result = puT.LoadPlayerImages().LoadInRunAvatarPic();
                        return false;

                    }
                    return true;
                }
                return true;
            }
        }



   

    }











}
