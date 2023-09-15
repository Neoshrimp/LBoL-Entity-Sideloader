using HarmonyLib;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.Presentation.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.LowLevel;

namespace LBoLEntitySideloader.Entities
{
    public abstract class EnemyGroupTemplate : EntityDefinition,
        IConfigProvider<EnemyGroupConfig>
        // in theory could have custom type logic entity

    {
        public override Type ConfigType() => typeof(EnemyGroupConfig);


        public override Type EntityType() => throw new InvalidDataException();


        public override Type TemplateType() => typeof(EnemyGroupTemplate);



        // needs to be cleared manually
        public static HashSet<string> customFormations = new HashSet<string>();
        // 2do cache formation loaded from disk separately 


        /// <summary>
        /// Numbering should start from 0. First enemy from enemy list will go to slot 0, second to slot 1 and so on. Make sure enemy act order display is consistent with the game's (left to right first, then top to bottom).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enemyLocations"></param>
        static public void AddFormation(string name, Dictionary<int, Vector2> enemyLocations)
        {
            UniqueTracker.Instance.formationAddActions.Add(() => {

                

                if (!GameDirector.Instance.Formations.ContainsKey(name))
                {
                    var formation_go = new GameObject(name);
                    var enemyFormation = formation_go.AddComponent<EnemyFormation>();


                    customFormations.Add(name);
                    formation_go.transform.SetParent(GameDirector.Instance.unitRoot);
                    formation_go.transform.position = GameDirector.Instance.unitRoot.position;

                    GameDirector.Instance.Formations.Add(name, enemyFormation);


                    enemyFormation.enemyLocations = new AssociationList<int, Transform>();
                    foreach (var kv in enemyLocations)
                    {
                        var posGo = new GameObject(kv.Key.ToString());
                        posGo.transform.SetParent(enemyFormation.transform);
                        posGo.transform.localPosition = kv.Value;

                        enemyFormation.enemyLocations.Add(kv.Key, posGo.transform);
                    }

                }
                else
                {
                    Log.log.LogError($"Formation with name {name} is already present");
                }

                
            });
        }


        static internal void ReloadFormations()
        {
            var formationsToDestroy = EnemyGroupTemplate.FindFormationsToUnload();
            UnloadCustomFormations(formationsToDestroy);
            LoadCustomFormations();
        }

        static internal void LoadCustomFormations()
        {
            UniqueTracker.Instance.formationAddActions.Do(a => a.Invoke());
        }


        static internal List<GameObject> FindFormationsToUnload()
        {
            var formationsToDestroy = new List<GameObject>();
            foreach (var trans in GameDirector.Instance.unitRoot)
            {
                var go = (trans as Transform).gameObject;
                if (go.TryGetComponent<EnemyFormation>(out var _))
                {
                    if (customFormations.Contains(go.name))
                    {
                        formationsToDestroy.Add(go);
                    }
                }
            }
            customFormations.Clear();

            return formationsToDestroy;
        }

        static internal void UnloadCustomFormations(List<GameObject> formationsToDestroy)
        {
            foreach (var go in formationsToDestroy)
            {
                GameDirector.Instance.Formations.Remove(go.name);
                GameObject.Destroy(go);
            }
        }

        static public class VanillaFormations
        {
            public const string Single = "Single";
            public const string Heng = "Heng";
            public const string Shu = "Shu";
            public const string Triangle = "Triangle";
            public const string Triangle2 = "Triangle2";
            public const string Summoner = "Summoner";
            public const string Diamond = "Diamond";
            public const string Square = "Square";
            public const string Five = "Five";
            public const string Five2 = "Five2";
            public const string Lore = "Lore";
            public const string Reimu = "Reimu";
            public const string Yuyuko = "Yuyuko";
            public const string Siji = "Siji";
            public const string Doremy = "Doremy";
            public const string Seija = "Seija";
        }

        /// <summary>
        /// Id : should be a string. In vanilla strings which look like numbers are used but any string can be used
        /// Name : 
        /// FormationName : preset positions for all enemy units. Must be one of available formations. Custom formations can be added with EnemyGroupTemplate.AddFormation
        /// Enemies : List of enemy Ids. The list order determines enemy acting order with the first entry acting first, second acting second and so on. An entry "Empty" can be added to this list indicating that the slot is empty and can be filled with summoned unit.
        /// EnemyType : 
        /// DebutTime : 
        /// RollBossExhibit : 
        /// PlayerRoot : always Vector2(-4f, 0.5f)
        /// PreBattleDialogName : no way to add dialog yet
        /// PostBattleDialogName : no way to add dialog yet 
        /// </summary>
        /// <returns></returns>
        public EnemyGroupConfig DefaultConfig()
        {
            var config = new EnemyGroupConfig(
                    Id : "",
                    Name : "",
                    FormationName : VanillaFormations.Single,
                    Enemies : new List<string>() { },
                    EnemyType : EnemyType.Normal,
                    DebutTime : 1f,
                    RollBossExhibit : false,
                    PlayerRoot : new Vector2(-4f, 0.5f),
                    PreBattleDialogName : "",
                    PostBattleDialogName : ""
                );
            return config;
        }

        public abstract EnemyGroupConfig MakeConfig();


    }
}
