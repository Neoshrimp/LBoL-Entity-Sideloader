using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class OverwriteVanilla : Attribute
    {
        private string idToOverwrite;

        private bool overideAllComponents = false;

        public OverwriteVanilla() { }

        public string IdToOverwrite { get => idToOverwrite; set => idToOverwrite = value; }

        public bool OverideAllComponents { get => overideAllComponents; set => overideAllComponents = value; }
    }
}
