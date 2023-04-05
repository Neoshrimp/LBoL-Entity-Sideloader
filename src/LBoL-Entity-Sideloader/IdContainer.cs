using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LBoLEntitySideloader
{

    public struct IdContainer
    {
        public string sId;
        public int iId;

        private readonly IdType idType;


        public static implicit operator string(IdContainer id) => id.sId;

        //public static implicit operator int?(IdContainer id) => id.iId;

        public static implicit operator int(IdContainer id) => id.iId;

        public static implicit operator IdContainer(string s) => new IdContainer(s);

        public static implicit operator IdContainer(int i) => new IdContainer(i);



        public IdContainer(string s) : this()
        {
            this.sId = s;
            this.idType =  IdType.String;
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
                    return a.sId == b.sId;
                case IdType.Int:
                    return a.iId == b.iId;
                default:
                    return a.sId == b.sId;
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
                    return a.sId != b.sId;
                case IdType.Int:
                    return a.iId != b.iId;
                default:
                    return a.sId != b.sId;
            }
        }

        public override bool Equals(object obj)
        {
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
                    return sId.GetHashCode();
                case IdType.Int:
                    return iId.GetHashCode();
                default:
                    return sId.GetHashCode();
            }
        }

        public override string ToString()
        {
            switch (idType)
            {
                case IdType.String:
                    return sId.ToString();
                case IdType.Int:
                    return iId.ToString();
                default:
                    return sId.ToString();
            }
        }
    



        public enum IdType
        {
            String,
            Int
        }

    }
}
