using LBoL.ConfigData;
using LBoL.Core.StatusEffects;
using System;


namespace LBoLEntitySideloader.Entities
{
    public abstract class StatusEffectTemplate : EntityDefinition,
        IConfigProvider<StatusEffectConfig>,
        IGameEntityProvider<StatusEffect>
    {

        public override Type ConfigType()
        {
            return typeof(StatusEffectConfig);
        }

        public override Type EntityType()
        {
            return typeof(StatusEffect);
        }

        public StatusEffectConfig DefaultConfig()
        {
            throw new NotImplementedException();
        }
        public abstract StatusEffectConfig MakeConfig();
    }
}
