using LBoLEntitySideloader.Entities;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace LBoLEntitySideloader.TemplateGen
{
    // 2do rename
    public abstract class TemplateGen<ED> where ED : EntityDefinition
    {

        public readonly static Assembly sideLoaderAss = typeof(BepinexPlugin).Assembly;

        public readonly Assembly originAssembly;

        public Assembly newAssembly;

        public string newAssName;

        public CodeNamespace newNameSpace;

        protected CodeCompileUnit compileUnit;



        public TemplateGen(Assembly originAssembly = null)
        {
            if(originAssembly == null )
                this.originAssembly = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;
            else
                this.originAssembly = originAssembly;

            // 2do error check
            newAssName = $"{originAssembly.GetName().Name}.{typeof(ED).Name}.Dynamic";


            var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, $"{newAssName}.dll", false);
            parameters.GenerateExecutable = false;



            if (newNameSpace == null)
            {
                compileUnit = new CodeCompileUnit();
                newNameSpace = new CodeNamespace($"{newAssName}");
                compileUnit.Namespaces.Add(newNameSpace);
                newNameSpace.Imports.Add(new CodeNamespaceImport("System"));


                newNameSpace.Imports.Add(new CodeNamespaceImport("LBoL.Base"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("LBoL.Base.Extensions"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("LBoL.ConfigData"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("LBoL.Core"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("LBoL.Core.Cards"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("LBoL.Presentation"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("LBoLEntitySideloader.Resource"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("System"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("System.Linq"));
                // 2do more imports
            }
        }


/*        static internal CodeMemberMethod WrapMethodInfo(MethodInfo methodInfo, bool dontOverwrite = false) 
        {
            methodInfo.GetParameters
        }*/



        protected void QueueGen(IdContainer Id, bool overwriteVanilla = false)
        {

            // 2do dup check
            var classType = new CodeTypeDeclaration($"{Id}Definition");
            classType.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
            newNameSpace.Types.Add(classType);


            classType.BaseTypes.Add(typeof(ED));
            if (overwriteVanilla)
                classType.CustomAttributes.Add(new CodeAttributeDeclaration());

/*            var getId = new CodeMemberMethod();

            getId.Attributes.*/



        }


        public Type FinalizeGen()
        {
            return null;
        }
    }
}
