using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class OverwriteVanilla : Attribute
    {
        readonly IdContainer idToOverwrite;

        private bool overideAllComponents = false;

        public OverwriteVanilla(IdContainer orginalId)
        {
            idToOverwrite = orginalId;
        }

        public IdContainer IdToOverride => idToOverwrite;

        public bool OverideAllComponents { get => overideAllComponents; set => overideAllComponents = value; }
    }
}
