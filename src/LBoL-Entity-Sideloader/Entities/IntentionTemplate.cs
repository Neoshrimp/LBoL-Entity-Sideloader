using LBoL.Base.Extensions;
using LBoL.Core;
using LBoL.Core.Units;
using LBoL.Presentation;
using LBoLEntitySideloader.Entities.MockConfigs;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Entities
{
    public abstract class IntentionTemplate : EntityDefinition,
        IGameEntityProvider<Intention>,
        IResourceConsumer<LocalizationOption>,
        IResourceConsumer<IntentionImages>

    {
        public override Type ConfigType() => typeof(IntentionMockConfig);
        public override Type EntityType() => typeof(Intention);
        public override Type TemplateType() => typeof(IntentionTemplate);


        public static Intention CreateIntention(Type intentionType)
        {
            return TypeFactory<Intention>.TryCreateInstance(intentionType);
        }

        public static T CreateIntention<T>() where T : Intention
        {
            return (T)CreateIntention(typeof(T));
        }

        /// <summary>
        /// Intention can switch between several icons. Icons are identified by suffix keys supplied in IntentionImages.subSprites.
        /// </summary>
        /// <param name="intention">The custom intention</param>
        /// <returns>Suffix of a sub icon</returns>
        public virtual string SelectAltIconsSuffix(Intention intention)
        {
            return "";
        }

        public abstract LocalizationOption LoadLocalization();

        public abstract IntentionImages LoadSprites();

        public void Consume(LocalizationOption locOptions)
        {
            ProcessLocalization(locOptions, EntityType());
        }

        public void Consume(IntentionImages sprites)
        {
            if (sprites == null)
                return;

            var id = UniqueId.ToString();
            if (id.EndsWith("Intention"))
            {
                id = id.RemoveEnd("Intention");
            }

            if (sprites.main != null)
                ResourcesHelper.IntentionSprites.AlwaysAdd(id, sprites.main);
            else
                Log.LogDev()?.LogWarning($"Intention {id} of {this.GetType().Name} definition doesn't have the main sprite set.");

            foreach (var kv in sprites.LoadMany())
            {
                if (kv.Value != null)
                    ResourcesHelper.IntentionSprites.AlwaysAdd(id + kv.Key, kv.Value);
            }
        }
    }
}
