using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LBoLEntitySideloader
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
                userInfos.Add(assembly, EntityManager.ScanAssembly(assembly));
            }
            catch (Exception ex)
            {

                Log.log.LogError($"{assembly.GetName().Name}: {ex}");
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
            if (userInfos.TryGetValue(entityDefinition.userAssembly, out UserInfo user))
            {
                return user;
            }
            Log.log.LogWarning($"{entityDefinition.userAssembly.GetName().Name} was not found among registered users");
            return null;
        }




        public static Type GetEntityLogicType(Assembly assembly, Type definitionType)
        {
            //if (TryGetUserInfoViaAssemblyMap(assembly, out UserInfo user))

            var sideloaderUsers = new SideloaderUsers[] { EntityManager.Instance.sideloaderUsers, EntityManager.Instance.secondaryUsers };


            foreach(var sideloaderUser in sideloaderUsers) {
                if (sideloaderUser.userInfos.TryGetValue(assembly, out var user))
                {
                    if (user.definition2customEntityLogicType.TryGetValue(definitionType, out Type entityType))
                    {
                        return entityType;
                    }
                    if (user.IsForOverwriting(definitionType) && user.definitionInfos.TryGetValue(definitionType, out EntityDefinition entityDefinition))
                    {
                        var typeDic = TypeFactoryReflection.AccessTypeDicts(entityDefinition.EntityType(), TypeFactoryReflection.TableFieldName.TypeDict);

                        if (typeDic().TryGetValue(entityDefinition.UniqueId, out Type logicType))
                            return logicType;
                    }
                }


            }

            Log.log.LogError($"{definitionType.Name} was not found in {assembly.GetName().Name} or {definitionType.Name} is not for overwriting");
            return null;
            //throw new ArgumentException($"{definitionType.Name} was not found in {assembly.GetName().Name} or {definitionType.Name} is not for overwriting");
        }

    }

}
