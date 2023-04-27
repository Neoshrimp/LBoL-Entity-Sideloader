using LBoLEntitySideloader.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LBoLEntitySideloader
{
    public partial class EntityManager
    {
        internal class SideloaderUsers
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


            public Type GetEntityLogicType(Assembly assembly, Type definitionType)
            {
                if (this.userInfos.TryGetValue(assembly, out UserInfo user) && user.definition2EntityLogicType.TryGetValue(definitionType, out Type entityType))
                {
                    return entityType;
                }

                throw new ArgumentException($"{definitionType.Name} was not found in {assembly.GetName().Name}");
            }

        }

    }
}
