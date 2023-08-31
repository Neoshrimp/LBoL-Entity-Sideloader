using Mono.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using static UnityEngine.GraphicsBuffer;

// from UnityExplorer

namespace LBoLEntitySideloader.TemplateGen
{
    public class ScriptEvaluator : Evaluator, IDisposable
    {
        // Token: 0x060003BD RID: 957 RVA: 0x0001B4EF File Offset: 0x000196EF
        public ScriptEvaluator(TextWriter tw)
            : base(ScriptEvaluator.BuildContext(tw))
        {
            this._textWriter = tw;
            this.ImportAppdomainAssemblies();
            ReferenceAssembly(typeof(EntityManager).Assembly);
        }

        // Token: 0x060003BE RID: 958 RVA: 0x0001B524 File Offset: 0x00019724
        public void Dispose()
        {
            this._textWriter.Dispose();
        }


        // Token: 0x060003C1 RID: 961 RVA: 0x0001B62C File Offset: 0x0001982C
        private static CompilerContext BuildContext(TextWriter tw)
        {
            ScriptEvaluator._reportPrinter = new StreamReportPrinter(tw);
            CompilerSettings compilerSettings = new CompilerSettings
            {
                Version = LanguageVersion.Experimental,
                GenerateDebugInfo = false,
                //GenerateDebugInfo = true,
                StdLib = true,
                Target = Mono.CSharp.Target.Library,
                WarningLevel = 0,
                EnhancedWarnings = false,
                Encoding = Encoding.UTF8,
            };
            return new CompilerContext(compilerSettings, ScriptEvaluator._reportPrinter);
        }

        // Token: 0x060003C2 RID: 962 RVA: 0x0001B688 File Offset: 0x00019888
        private void ImportAppdomainAssemblies()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string name = assembly.GetName().Name;
                bool flag = ScriptEvaluator.StdLib.Contains(name);
                if (!flag)
                {
                    try
                    {
                        base.ReferenceAssembly(assembly);
                    }
                    catch
                    {
                    }
                }
            }
        }

        // Token: 0x0400020D RID: 525
        internal TextWriter _textWriter;

        // Token: 0x0400020E RID: 526
        internal static StreamReportPrinter _reportPrinter;

        // Token: 0x0400020F RID: 527
        private static readonly HashSet<string> StdLib = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "mscorlib", "System.Core", "System", "System.Xml" };
    }
}
