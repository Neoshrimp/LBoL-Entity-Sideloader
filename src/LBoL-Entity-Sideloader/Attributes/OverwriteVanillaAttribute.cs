using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader.Attributes
{

    /// <summary>
    /// Used on template(definition) classes. 
    /// Indicates that changes defined in the template will be applied to a vanilla entity with Id returned by GetId().
    /// Which entity components should be overwritten can be controlled with [DontOverwrite] attribute.
    /// For entities which have EntityLogic type component entity logic will not be changed if a type with [EntityLogic] is omitted
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class OverwriteVanilla : Attribute
    {
        private string idToOverwrite;

        private bool overideAllComponents = false;

        public OverwriteVanilla() { }

        public string IdToOverwrite { get => idToOverwrite; set => idToOverwrite = value; }

        public bool OverideAllComponents { get => overideAllComponents; set => overideAllComponents = value; }
    }
}
