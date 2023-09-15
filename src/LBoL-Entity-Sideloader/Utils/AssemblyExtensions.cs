using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LBoLEntitySideloader.Utils
{
    public static class AssemblyExtensions
    {
        public static bool IsLoadedFromDisk(this Assembly assembly) => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location);


    }
}
