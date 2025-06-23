using LBoL.Base;
using LBoL.Base.Extensions;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoL.Core.Units;
using LBoL.Presentation;
using LBoL.Presentation.Bullet;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.TemplateGen;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static LBoLEntitySideloader.BepinexPlugin;
using static UnityEngine.UI.GridLayoutGroup;


namespace LBoLEntitySideloader.Entities
{
    public abstract class EnemyUnitTemplate : EntityDefinition,
        IConfigProvider<EnemyUnitConfig>,
        IGameEntityProvider<EnemyUnit>,
        IResourceConsumer<LocalizationOption>        
    {
        public override Type ConfigType() => typeof(EnemyUnitConfig);

        public override Type EntityType() => typeof(EnemyUnit);

        public override Type TemplateType() => typeof(EnemyUnitTemplate);

        /// <summary>
        /// icon for boss encounter node. Some circular dimension
        /// </summary>
        /// <param name="enemyUnitId"></param>
        /// <param name="getSprite"></param>
        /// <param name="callingAssembly"></param>
        public static void AddBossNodeIcon(string enemyUnitId, Func<Sprite> getSprite, Assembly callingAssembly = null)
        {
            if (callingAssembly == null)
                callingAssembly = Assembly.GetCallingAssembly();

            Action action = () => {
                var sprite = getSprite();
                ResourcesHelper.BossIcons.AlwaysAdd(enemyUnitId, sprite);
            };

            EntityManager.Instance.addBossIconsActions.AddAction(enemyUnitId, action, callingAssembly);
        }




        /// <summary>
        /// Id:
        /// RealName: for Death Note Exhibit
        /// OnlyLore: presumably for characters which don't appear as enemies. But actually don't appear to do anything.
        /// BaseManaColor: For rolling boss shining boss exhibits
        /// Order:
        /// ModleName:
        /// NarrativeColor:
        /// Type:
        /// IsPreludeOpponent:
        /// HpLength: hp bar visual??
        /// MaxHpAdd: extra max hp rolled from 0 to the specified number
        /// MaxHp:
        /// Damage1:
        /// Damage2:
        /// Damage3:
        /// Damage4:
        /// Power:
        /// Defend: extra block/barrier?
        /// Count1:
        /// Count2:
        /// MaxHpHard:
        /// Damage1Hard:
        /// Damage2Hard:
        /// Damage3Hard:
        /// Damage4Hard:
        /// PowerHard:
        /// DefendHard:
        /// Count1Hard:
        /// Count2Hard:
        /// MaxHpLunatic:
        /// Damage1Lunatic:
        /// Damage2Lunatic:
        /// Damage3Lunatic:
        /// Damage4Lunatic:
        /// PowerLunatic:
        /// DefendLunatic:
        /// Count1Lunatic:
        /// Count2Lunatic:
        /// PowerLoot:
        /// BluePointLoot:
        /// Gun1:
        /// Gun2:
        /// Gun3:
        /// Gun4:
        /// </summary>
        /// <returns></returns>
        public EnemyUnitConfig DefaultConfig()
        {
            var config = new EnemyUnitConfig(
                Id: "",
                RealName: false,
                OnlyLore: false,
                BaseManaColor: new List<ManaColor>() { },
                Order: 10,
                ModleName: "",
                NarrativeColor: "",
                Type: EnemyType.Normal,
                IsPreludeOpponent: false,
                HpLength: null,
                MaxHpAdd: null,
                MaxHp: 1,
                Damage1: null,
                Damage2: null,
                Damage3: null,
                Damage4: null,
                Power: null,
                Defend: null,
                Count1: null,
                Count2: null,
                MaxHpHard: null,
                Damage1Hard: null,
                Damage2Hard: null,
                Damage3Hard: null,
                Damage4Hard: null,
                PowerHard: null,
                DefendHard: null,
                Count1Hard: null,
                Count2Hard: null,
                MaxHpLunatic: null,
                Damage1Lunatic: null,
                Damage2Lunatic: null,
                Damage3Lunatic: null,
                Damage4Lunatic: null,
                PowerLunatic: null,
                DefendLunatic: null,
                Count1Lunatic: null,
                Count2Lunatic: null,
                PowerLoot: default(MinMax),
                BluePointLoot: new MinMax(5, 5),
                Gun1: new List<string>() { "Simple1" },
                Gun2: new List<string>() { "Simple2" },
                Gun3: new List<string>() { "Simple3" },
                Gun4: new List<string>() { "Simple4" }

                );

            return config;
        }

        public abstract EnemyUnitConfig MakeConfig();

        public abstract LocalizationOption LoadLocalization();


        public void Consume(LocalizationOption resource)
        {
            ProcessLocalization(resource, EntityType());
        }


    }
}
