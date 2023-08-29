using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LBoLEntitySideloader
{
    public partial class EntityManager
    {
        public class SideloaderUsers
        {
            public Dictionary<Assembly, UserInfo> userInfos = new Dictionary<Assembly, UserInfo>();


            public void AddUser(Assembly assembly, bool checkBepinex = true, bool lookForFactypes = true)
            {
                if (userInfos.ContainsKey(assembly))
                {
                    throw new Exception($"{assembly.GetName().Name} is already registered");

                }
                try
                {
                    userInfos.Add(assembly, ScanAssembly(assembly));

                }
                catch (Exception ex)
                {

                    log.LogError($"{assembly.GetName().Name}: {ex}");
                }

            }

            /*            public void UnregisterUser(Assembly assembly)
                        {
                            if (userInfos.ContainsKey(assembly))
                            {
                                userInfos[assembly] = new UserInfo();
                            }

                        }

                        public void ReRegisterUser(Assembly assembly)
                        {
                            if (userInfos.ContainsKey(assembly)) 
                            {
                                userInfos[assembly] = ScanAssembly(assembly);
                            }
                        }*/


            public UserInfo GetDefinitionUser(EntityDefinition entityDefinition)
            {
                if (userInfos.TryGetValue(entityDefinition.assembly, out UserInfo user))
                {
                    return user;
                }
                log.LogWarning($"{entityDefinition.assembly.GetName().Name} was not found among registered users");
                return null;
            }


            public Type GetEntityLogicType(Assembly assembly, Type definitionType)
            {
                if (userInfos.TryGetValue(assembly, out UserInfo user))
                { 
                    if(user.definition2customEntityLogicType.TryGetValue(definitionType, out Type entityType))
                    {
                        return entityType;
                    }
                    if (user.IsForOverwriting(definitionType) && user.definitionInfos.TryGetValue(definitionType, out EntityDefinition entityDefinition))
                    {
                        var typeDic = TypeFactoryReflection.AccessTypeDicts(entityDefinition.EntityType(), TypeFactoryReflection.TableFieldName.TypeDict);

                        if(typeDic().TryGetValue(entityDefinition.UniqueId, out Type logicType))
                            return logicType;
                    }
                }


                throw new ArgumentException($"{definitionType.Name} was not found in {assembly.GetName().Name} or {definitionType.Name} is not for overwriting");
            }

        }

    }
}
