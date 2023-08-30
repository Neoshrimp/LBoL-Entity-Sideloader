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


            public void AddUser(Assembly assembly)
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

            public bool TryGetUserInfoViaAssemblyMap(Assembly genAssembly, out UserInfo userInfo)
            {
                if (userInfos.TryGetValue(genAssembly, out userInfo))
                    return true;

                UniqueTracker.Instance.gen2User.TryGetValue(genAssembly, out var assembly);
                if (assembly != null)
                {
                    if(userInfos.TryGetValue(assembly, out userInfo))
                        return true;
                }


                return false;

            }


            public Type GetEntityLogicType(Assembly assembly, Type definitionType)
            {
                //if (TryGetUserInfoViaAssemblyMap(assembly, out UserInfo user))
                if (userInfos.TryGetValue(assembly, out var user))
                {
                    if (user.definition2customEntityLogicType.TryGetValue(definitionType, out Type entityType))
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


                log.LogError($"{definitionType.Name} was not found in {assembly.GetName().Name} or {definitionType.Name} is not for overwriting");
                return null;
                //throw new ArgumentException($"{definitionType.Name} was not found in {assembly.GetName().Name} or {definitionType.Name} is not for overwriting");
            }

        }

    }
}
