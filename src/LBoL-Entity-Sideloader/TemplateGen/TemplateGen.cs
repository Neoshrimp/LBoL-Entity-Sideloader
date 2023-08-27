using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using HarmonyLib;
using LBoL.EntityLib.Exhibits.Shining;

namespace LBoLEntitySideloader.TemplateGen
{

    public abstract class TemplateGen<ED> where ED : EntityDefinition
    {


        public readonly static Assembly sideLoaderAss = typeof(BepinexPlugin).Assembly;

        static readonly StringBuilder evaluatorOutput;
        protected static readonly ScriptEvaluator scriptEvaluator = new ScriptEvaluator(new StringWriter(evaluatorOutput = new StringBuilder()));

        public readonly Assembly originAssembly;

        public Assembly newAssembly;

        public string newAssName;

        public CodeNamespace newNameSpace;


        protected CodeCompileUnit compileUnit;



        public Dictionary<IdContainer, CodeTypeDeclaration> generatedTypes = new Dictionary<IdContainer, CodeTypeDeclaration>();

        private bool properInnit = false;


        // 2do add codeDom lib

        public TemplateGen(Assembly originAssembly = null)
        {
            if(originAssembly == null )
                this.originAssembly = new StackTrace().GetFrame(2).GetMethod().ReflectedType.Assembly;
            else
                this.originAssembly = originAssembly;

            // 2do error check
            newAssName = $"{this.originAssembly.GetName().Name}.{typeof(ED).Name}.Dynamic";






/*            if (EntityManager.Instance?.sideloaderUsers?.userInfos?.TryGetValue(originAssembly, out user) == null)
            {
                Log.log.LogError($"{this.GetType().Name} must be instantiated after {nameof(EntityManager.RegisterSelf)} has been called.");
                return;
            }*/



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
                newNameSpace.Imports.Add(new CodeNamespaceImport("LBoLEntitySideloader"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("LBoLEntitySideloader.Resource"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("LBoLEntitySideloader.TemplateGen"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("System"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("System.Linq"));
                newNameSpace.Imports.Add(new CodeNamespaceImport("System.Reflection"));

                // 2do more imports
            }


            MethodCache.methodCacheDic.TryAdd(newAssName, new MethodCache());


            properInnit = true;
        }



        internal CodeMemberMethod MakeGetIdMethod(IdContainer Id, CodeTypeDeclaration targetClass)
        {
            CodeMemberMethod newMethod = new CodeMemberMethod();
            newMethod.Name = nameof(EntityDefinition.GetId);
            newMethod.ReturnType = new CodeTypeReference(typeof(IdContainer));
            newMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;


            newMethod.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(Id.ToString())));
            //new CodeSnippetExpression(@$"return {Id};")

            targetClass.Members.Add(newMethod);
            return newMethod;
        }


        internal CodeMemberMethod MakeMethod<R>(string name, Func<R> func, CodeTypeDeclaration targetClass, bool dontOverwrite = false) where R : class
        {

            MethodCache.methodCacheDic.TryAdd(newAssName, new MethodCache());

            MethodCache.methodCacheDic[newAssName].AddMethod(typeof(ED), targetClass.Name, name, func);

            /*            targetClass.Members.Add(new CodeMemberField() { Name = $"{name}_payload", Type = new CodeTypeReference(typeof(MethodInfo)), Attributes = MemberAttributes.Private });*/

            CodeMemberMethod newMethod = new CodeMemberMethod();
            newMethod.Name = name;
            newMethod.ReturnType = new CodeTypeReference(func.Method.ReturnType);
            newMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;


            if (dontOverwrite)
                newMethod.CustomAttributes = new CodeAttributeDeclarationCollection
                {
                    new CodeAttributeDeclaration(new CodeTypeReference(typeof(DontOverwriteAttribute)))
                };

            /*            foreach (var p in methodInfo.GetParameters())
                        {
                            newMethod.Parameters.Add(new CodeParameterDeclarationExpression(p.ParameterType, p.Name));
                        }*/




            newMethod.Statements.Add(new CodeSnippetExpression(@$"
                var method = MethodCache.methodCacheDic[this.GetType().Assembly.GetName().Name].GetMethod(this.TemplateType(), this.GetType().Name, ""{name}"");
                return method() as {func.Method.ReturnType.FullName};
            "));


            targetClass.Members.Add(newMethod);
            return newMethod;
        }



        protected CodeTypeDeclaration InnitDefintionType(IdContainer Id, bool overwriteVanilla = false)
        {



            if (!properInnit)
            {
                Log.log.LogError($"{this.GetType().Name} was not properly initialized");
                return null;
            }



            if (!generatedTypes.ContainsKey(Id))
            {
                var targetClass = new CodeTypeDeclaration($"{Id}Definition");
                targetClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
                newNameSpace.Types.Add(targetClass);


                targetClass.BaseTypes.Add(typeof(ED));
                if (overwriteVanilla)
                    targetClass.CustomAttributes.Add(new CodeAttributeDeclaration());

                generatedTypes.Add(Id, targetClass);

                MakeGetIdMethod(Id, targetClass);
                return targetClass;
            }
            else
            {
                Log.log.LogError($"{typeof(ED)} template gen: type {Id}Definition was already generated.");
                return null;
            }





        }




        public void FinalizeGen()
        {

            Log.log.LogInfo($"{newAssName}: generating {typeof(ED)} definitions..");



            //var rez = scriptEvaluator.Compile(OutputCSharpCode().ToString(), out var compiled);

            var rez = scriptEvaluator.Run(OutputCSharpCode().ToString());



            Log.log.LogInfo(rez);

            if (ScriptEvaluator._reportPrinter.ErrorsCount > 0)
            {
                Log.log.LogError("Got errors.");
                scriptEvaluator._textWriter.ToString().Split('\n').ToList().ForEach(s => Log.log.LogError(s));
            }


            /*            var newo = new object();
                        compiled.Invoke(newo);

                        Log.log.LogDebug(newo);
                        Log.log.LogDebug(newo.GetType());*/


            Log.log.LogInfo("------------------");

        }


        public StringBuilder OutputCSharpCode(bool outputToFile = false)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";


            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            
            provider.GenerateCodeFromCompileUnit(compileUnit, stringWriter, options);

            if(outputToFile) {
                var dir = $"generatedCode";
                Directory.CreateDirectory(dir);

                using FileStream fileStream = File.Open($"{dir}/{typeof(ED).Name}.cs", FileMode.Create, FileAccess.Write, FileShare.None);

                using StreamWriter sourceWriter = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };

                sourceWriter.Write(stringBuilder.ToString());
            }

            return stringBuilder;

        }
    }
}
