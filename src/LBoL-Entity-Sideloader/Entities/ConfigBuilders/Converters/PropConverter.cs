using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Entities.ConfigBuilders.Converters
{
    public abstract class PropConverter<From, To> : ITwoWayConverter<From, To>
    {
        public abstract To ConvertTo(From from);
        public abstract From ReverseConvert(To to);
    }
}
