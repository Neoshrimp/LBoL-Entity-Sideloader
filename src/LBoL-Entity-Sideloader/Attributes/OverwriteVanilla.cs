using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OverwriteVanilla : Attribute
    {
        public string orginalId;

        public OverwriteVanilla(string orginalId)
        {
            this.orginalId = orginalId;
        }
    }
}
