using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Entities.ConfigBuilders
{
    public class D2ArrayConverter<N> : ITwoWayConverter<D2ArrayEncode<N>, N[][]>
    {
        public N[][] ConvertTo(D2ArrayEncode<N> encoded)
        {

            switch (encoded.mode)
            {
                case D2ArrayEncode<N>.Mode.Zero:
                    return new N[0][];
                case D2ArrayEncode<N>.Mode.Direct:
                    return new N[][] { encoded.baseValue.ConvertSelf() };
                case D2ArrayEncode<N>.Mode.Group1:
                    return new N[][] { encoded.baseValue.ConvertSelf(), encoded.group1.ConvertSelf() };
                case D2ArrayEncode<N>.Mode.Group2:
                    return new N[][] { encoded.baseValue.ConvertSelf(), encoded.group1.ConvertSelf(), encoded.group2.ConvertSelf() };
                case D2ArrayEncode<N>.Mode.WayAndGroup:
                    return new N[][] { encoded.baseValue.ConvertSelf(), encoded.group1.ConvertSelf(), encoded.group2.ConvertSelf(), encoded.way.ConvertSelf() };
                default:
                    throw new ArgumentException("D2ArrayEncode<N>.mode is out of range");

            }
        }

        public D2ArrayEncode<N> ReverseConvert(N[][] array)
        {

            var converter = new NumberOption<N>();
            switch (array.Length)
            {
                case 0:
                    return new D2ArrayEncode<N>() { mode = D2ArrayEncode<N>.Mode.Zero };
                case 1:
                    return new D2ArrayEncode<N>() { mode = D2ArrayEncode<N>.Mode.Direct, baseValue = converter.ReverseConvert(array[0]) };
                case 2:
                    return new D2ArrayEncode<N>() { mode = D2ArrayEncode<N>.Mode.Group1, baseValue = converter.ReverseConvert(array[0]), group1 = converter.ReverseConvert(array[1]) };
                case 3:
                    return new D2ArrayEncode<N>() { mode = D2ArrayEncode<N>.Mode.Group2, baseValue = converter.ReverseConvert(array[0]), group1 = converter.ReverseConvert(array[1]), group2 = converter.ReverseConvert(array[2]) };
                case 4:
                    return new D2ArrayEncode<N>() { mode = D2ArrayEncode<N>.Mode.WayAndGroup, baseValue = converter.ReverseConvert(array[0]), group1 = converter.ReverseConvert(array[1]), group2 = converter.ReverseConvert(array[2]), way = converter.ReverseConvert(array[3]) };
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(D2ArrayConverter<N>)} {nameof(D2ArrayConverter<N>.ReverseConvert)} {typeof(N)} array is out of range");

            }
        }
    }

    public class D2ArrayEncode<N>
    {
        public Mode mode;
        public NumberOption<N> baseValue = new NumberOption<N>();
        public NumberOption<N> group1 = new NumberOption<N>();
        public NumberOption<N> group2 = new NumberOption<N>();
        public NumberOption<N> way = new NumberOption<N>();

        public enum Mode
        {
            Zero,
            Direct,
            Group1,
            Group2,
            WayAndGroup
        }
    }


    public class NumberOption<N> : ITwoWayConverter<NumberOption<N>, N[]>
    {
        public Mode mode;

        public N value;

        public N variance;

        public N[] ConvertTo(NumberOption<N> from)
        {
            switch (mode)
            {
                case Mode.None:
                    return new N[0];
                case Mode.Literal:
                    return new N[] { value };
                case Mode.Random:
                    return new N[] { value, variance };
                default:
                    throw new ArgumentException("NumberOption.mode is out of range");
            }

        }

        public N[] ConvertSelf() => ConvertTo(this);

        public NumberOption<N> ReverseConvert(N[] to)
        {
            switch (to.Length)
            {
                case 0:
                    return new NumberOption<N>() { mode = Mode.None };
                case 1:
                    return new NumberOption<N>() { mode = Mode.Literal, value = to[0] };
                case 2:
                    return new NumberOption<N>() { mode = Mode.Random, value = to[0], variance = to[1] };
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(NumberOption<N>)} {nameof(NumberOption<N>.ReverseConvert)} {typeof(N)} array is out of range");
            }
        }

        public enum Mode
        {
            None,
            Literal,
            Random
        }
    }
}
