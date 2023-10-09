using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using static GunDesigner.ConfigBuilders.Piece.PieceConverter;

namespace GunDesigner.ConfigBuilders.Converters
{
    public interface IConverter<From, To>
    {
        public To ConvertTo(From from);
    }

    public interface ITwoWayConverter<From, To> : IConverter<From, To>
    {
        public From ReverseConvert(To to);
    }

    public class IndentityConverter<T> : ITwoWayConverter<T, T>
    {
        public T ConvertTo(T from)
        {
            return from;
        }

        public T ReverseConvert(T to)
        {
            return to;
        }
    }

}
