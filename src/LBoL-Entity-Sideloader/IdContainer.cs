using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LBoLEntitySideloader
{

    public struct IdContainer : IEquatable<IdContainer>
    {
        private string sId;
        private int iId;

        public readonly IdType idType;

        public string SId { get => sId; set => sId = value; }
        public int IId { get => iId; set => iId = value; }

        public static implicit operator string(IdContainer id) => id.sId;

        //public static implicit operator int?(IdContainer id) => id.iId;

        public static implicit operator int(IdContainer id) => id.iId;

        public static implicit operator IdContainer(string s) => new IdContainer(s);

        public static implicit operator IdContainer(int i) => new IdContainer(i);

        public static IdContainer CastFromObject(object o)
        {
            if (o is string s)
            {
                return new IdContainer(s);
            }
            else if (o is int ii)
            {
                return new IdContainer(ii);
            }
            else
            {
                throw new ArgumentException($"IdContainer: {o.GetType()} is not a valid type to cast from");
            }

        }
               

        public IdContainer(string s) : this()
        {
            this.sId = s;
            this.idType = IdType.String;
        }

        public IdContainer(int i) : this()
        {
            this.iId = i;
            this.idType = IdType.Int;
        }

        public static bool operator ==(IdContainer a, IdContainer b)
        {
            if (a.idType != b.idType)
            {
                throw new ArgumentException("Can't compare IdContainers of different types");
            }

            switch (a.idType)
            {
                case IdType.String:
                    if (object.ReferenceEquals(null, a.sId))
                        return false;
                    return a.sId.Equals(b.sId);
                case IdType.Int:
                    return a.iId.Equals(b.iId);
                default:
                    if (object.ReferenceEquals(null, a.sId))
                        return false;
                    return a.sId.Equals(b.sId);
            }
        }


        public static bool operator !=(IdContainer a, IdContainer b)
        {
            if (a.idType != b.idType)
            {
                throw new ArgumentException("Can't compare IdContainers of different types");
            }

            switch (a.idType)
            {
                case IdType.String:
                    if (object.ReferenceEquals(null, a.sId))
                        return true;
                    return !a.sId.Equals(b.sId);
                case IdType.Int:
                    return !a.iId.Equals(b.iId);
                default:
                    if (object.ReferenceEquals(null, a.sId))
                        return true;
                    return !a.sId.Equals(b.sId);
            }
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            if (object.ReferenceEquals(null, obj))
                return false;

            switch (idType)
            {
                case IdType.String:
                    return sId.Equals(obj);
                case IdType.Int:
                    return iId.Equals(obj);
                default:
                    return sId.Equals(obj);
            }

        }

        public override int GetHashCode()
        {
            switch (idType)
            {
                case IdType.String:
                    if (object.ReferenceEquals(null, sId))
                        return 0;
                    return sId.GetHashCode();
                case IdType.Int:
                    return iId.GetHashCode();
                default:
                    if (object.ReferenceEquals(null, sId))
                        return 0;
                    return sId.GetHashCode();
            }
        }

        public override string ToString()
        {
            switch (idType)
            {
                case IdType.String:
                    return sId?.ToString();
                case IdType.Int:
                    return iId.ToString();
                default:
                    return sId?.ToString();
            }
        }

        public bool Equals(IdContainer other)
        {
            return this == other;
        }

        public enum IdType
        {
            String,
            Int
        }

    }
}
