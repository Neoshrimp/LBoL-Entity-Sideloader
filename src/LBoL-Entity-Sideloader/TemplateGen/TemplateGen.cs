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
using UnityEngine.AddressableAssets.Initialization;
using LBoLEntitySideloader.ReflectionHelpers;

namespace LBoLEntitySideloader.TemplateGen
{

    public abstract class TemplateGen<ED> where ED : EntityDefinition
    {


        public readonly static Assembly sideLoaderAss = typeof(BepinexPlugin).Assembly;

        protected static Sequence dupNameSeq = new Sequence();

        static readonly StringBuilder evaluatorOutput;
        protected static readonly ScriptEvaluator scriptEvaluator = new ScriptEvaluator(new StringWriter(evaluatorOutput = new StringBuilder()));

        public readonly Assembly originAssembly;

        public Assembly newAssembly = null;

        public string newAssName;

        public CodeNamespace newNameSpace;


        protected CodeCompileUnit compileUnit;

        protected CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

        protected CodeGeneratorOptions codeGenOptions = new CodeGeneratorOptions();


        public Dictionary<IdContainer, CodeTypeDeclaration> generatedTypes = new Dictionary<IdContainer, CodeTypeDeclaration>();

        private bool properInnit = false;


        private Dictionary<string, Type> name2DefTypeCache = new Dictionary<string, Type>();


        // 2do add codeDom lib

        public TemplateGen(Assembly originAssembly = null)
        {
            if (originAssembly == null)
                this.originAssembly = new StackTrace().GetFrame(2).GetMethod().ReflectedType.Assembly;
            else
                this.originAssembly = originAssembly;

            // 2do error check
            newAssName = $"{this.originAssembly.GetName().Name}_{typeof(ED).Name}_Dynamic";

            if (UniqueTracker.Instance.methodCacheDic.ContainsKey(newAssName))
            {
                newAssName = newAssName + "_" + dupNameSeq.Next().ToString();
            }

            newAssName = LegalizeTypeName(newAssName);



            Log.LogDev()?.LogDebug($"Initializing generation of: {typeof(ED).Name}, {newAssName}");

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

            }


            UniqueTracker.Instance.methodCacheDic.TryAdd(newAssName, new MethodCache());


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


        internal CodeMemberMethod MakeMethod<R>(string name, Func<R> func, CodeTypeDeclaration targetClass, bool vanillaOverwrite = false)
        {



            CodeMemberMethod newMethod = new CodeMemberMethod();
            newMethod.Name = name;
            newMethod.ReturnType = new CodeTypeReference(typeof(R));
            newMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;


            if (vanillaOverwrite && func == null)
                newMethod.CustomAttributes = new CodeAttributeDeclarationCollection
                {
                    new CodeAttributeDeclaration(new CodeTypeReference(typeof(DontOverwriteAttribute)))
                };


            UniqueTracker.Instance.methodCacheDic.TryAdd(newAssName, new MethodCache());

            if (func == null)
                func = () => default(R);

            // 2do same delegate ref bug
            UniqueTracker.Instance.methodCacheDic[newAssName].AddMethod(typeof(ED), targetClass.Name, name, (Delegate)func.Clone());


            newMethod.Statements.Add(new CodeSnippetExpression(@$"
                var method = UniqueTracker.Instance.methodCacheDic[this.GetType().Assembly.GetName().Name].GetMethod(this.TemplateType(), this.GetType().Name, ""{name}"");
                return method.DynamicInvoke(null) as {provider.GetTypeOutput(newMethod.ReturnType)};
            "));

            targetClass.Members.Add(newMethod);
            return newMethod;
        }



        protected CodeTypeDeclaration MakeEntityLogic(IdContainer Id, CodeTypeDeclaration definitionClass, Type entityType)
        {
            var targetClass = new CodeTypeDeclaration(Id.ToString());
            targetClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
            newNameSpace.Types.Add(targetClass);

            targetClass.BaseTypes.Add(entityType);



            targetClass.CustomAttributes = new CodeAttributeDeclarationCollection() {
                new CodeAttributeDeclaration(new CodeTypeReference(typeof(EntityLogic)), new CodeAttributeArgument[] { new CodeAttributeArgument(new CodeSnippetExpression($"typeof({newNameSpace.Name}.{definitionClass.Name})"))})

            };

            return targetClass;

        }


        public static string MakeDefName(IdContainer Id) => $"{LegalizeTypeName(Id)}Definition";

        public static string LegalizeTypeName(string name) => "_" + name.Replace('-', '_').Replace(' ', '_').Replace('.', '_');

        protected CodeTypeDeclaration InnitDefintionType(IdContainer Id, bool overwriteVanilla = false)
        {



            if (!properInnit)
            {
                Log.log.LogError($"{this.GetType().Name} was not properly initialized");
                return null;
            }



            if (!generatedTypes.ContainsKey(Id))
            {
                var targetClass = new CodeTypeDeclaration(MakeDefName(Id));
                targetClass.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;
                newNameSpace.Types.Add(targetClass);


                targetClass.BaseTypes.Add(typeof(ED));
                if (overwriteVanilla)
                    targetClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(OverwriteVanilla))));

                generatedTypes.Add(Id, targetClass);

                MakeGetIdMethod(Id, targetClass);
                return targetClass;
            }
            else
            {
                Log.LogDevExtra()?.LogWarning($"{typeof(ED)} template gen: type {MakeDefName(Id)} was already generated. (ignore this warning if hot reloading)");
                return generatedTypes[Id];
            }


        }




        public void FinalizeGen()
        {
            if (newAssembly != null)
            {
                Log.log.LogWarning($"FinalizeGen: {newAssName} was already generated by {originAssembly.GetName().Name}. Skipping generation. FinalizeGen should only be called once. (ignore this warning if hot reloading)");
            }
            else
            { 
                Log.log.LogInfo($"{newAssName}: generating {typeof(ED)} definitions..");



                //var rez = scriptEvaluator.Compile(OutputCSharpCode().ToString(), out var compiled);

                var rez = scriptEvaluator.Compile(OutputCSharpCode().ToString(), out _, newAssName, out var evaluationInfo);


                if (!string.IsNullOrEmpty(rez))
                { 
                    Log.log.LogError("Got parsing errors.");
                    Log.log.LogError(rez);
                }
            

                if (ScriptEvaluator._reportPrinter.ErrorsCount > 0)
                {
                    Log.log.LogError("Got compile errors.");
                    scriptEvaluator._textWriter.ToString().Split('\n').ToList().ForEach(s => Log.log.LogError(s));
                    return;
                }

                newAssembly = evaluationInfo.assembly;
            
            }



            // populate UniqueTracker regardless because it might be reload
            UniqueTracker.Instance.generatedAssemblies.TryAdd(originAssembly, new List<Assembly>());

            UniqueTracker.Instance.generatedAssemblies[originAssembly].Add(newAssembly);

            UniqueTracker.Instance.gen2User.Add(newAssembly, originAssembly);

            if (TemplatesReflection.ExpectsEntityLogic(typeof(ED)))
                UniqueTracker.Instance.gen2FacType.Add(newAssembly, TemplatesReflection.Template2FacType(typeof(ED)));

        }


        public Func<Type> GetDefTypePromise(IdContainer Id)
        {
            return () => SearchForDefType(MakeDefName(Id));
        }


        private Type SearchForDefType(string name)
        {

            if (name2DefTypeCache.TryGetValue(name, out var defDype))
                return defDype;
            foreach (var t in newAssembly.GetTypes().ToList()) 
            {
                name2DefTypeCache.TryAdd(t.Name, t);
            }
            if (name2DefTypeCache.TryGetValue(name, out defDype))
                return defDype;
            return null;
        }


        public StringBuilder OutputCSharpCode(bool outputToFile = false)
        {


            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";


            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            
            provider.GenerateCodeFromCompileUnit(compileUnit, stringWriter, options);

            if(outputToFile) {
                var dir = $"generatedCode";
                Directory.CreateDirectory(dir);

                using FileStream fileStream = File.Open($"{dir}/{newAssName}-{typeof(ED).Name}.cs", FileMode.Create, FileAccess.Write, FileShare.None);

                using StreamWriter sourceWriter = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };

                sourceWriter.Write(stringBuilder.ToString());
            }

            return stringBuilder;

        }
    }
}
