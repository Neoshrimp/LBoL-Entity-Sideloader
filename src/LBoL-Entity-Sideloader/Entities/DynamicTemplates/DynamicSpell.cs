using LBoL.ConfigData;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Entities.DynamicTemplates
{
    public class DynamicSpell : SpellTemplate, IDynamicTemplate<SpellTemplate>
    {
        public Func<IdContainer> CreateId { get; set; }

        public Func<LocalizationOption> CreateLoadLoc { get; set; }

        public Func<SpellConfig> CreateMakeConfig { get; set; }


        public SpellTemplate Create(IdContainer Id, UserInfo user)
        {
            IDynamicTemplate<SpellTemplate>.SetUser(this, user);
            this.CreateId = () => Id;
            try { EntityManager.Instance.RegisterConfig(this, user); } catch (Exception ex) { Log.log.LogError(ex); }
            try { this.Consume(LoadLocalization()); } catch (Exception ex) { Log.log.LogError(ex); }
            
            UniqueTracker.Instance.spellTemplates.TryAdd(userAssembly, new Dictionary<string, SpellTemplate>());
            UniqueTracker.Instance.spellTemplates[userAssembly].Add(GetId(), this);
            return this;
        }


        public override IdContainer GetId() => CreateId();

        public override LocalizationOption LoadLocalization() => CreateLoadLoc();

        public override SpellConfig MakeConfig() => CreateMakeConfig();

    }
}
