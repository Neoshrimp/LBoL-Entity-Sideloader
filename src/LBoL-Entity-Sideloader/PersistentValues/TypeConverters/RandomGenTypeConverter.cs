using LBoL.Base;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace LBoLEntitySideloader.PersistentValues.TypeConverters
{
    public class RandomGenTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(RandomGen);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            parser.Consume<MappingStart>();
            ulong state = 0;
            while (parser.Current is Scalar scalar)
            {
                var propertyName = scalar.Value;
                parser.MoveNext();

                if (propertyName == nameof(RandomGen.State))
                {
                    state = ulong.Parse(((Scalar)parser.Current).Value);
                    parser.MoveNext();
                }
            }
            parser.Consume<MappingEnd>();

            return RandomGen.FromState(state);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var randomGen = (RandomGen)value;
            var state = randomGen?.State ?? 0;

            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar(nameof(RandomGen.State)));
            emitter.Emit(new Scalar(state.ToString()));
            emitter.Emit(new MappingEnd());
        }
    }
}
