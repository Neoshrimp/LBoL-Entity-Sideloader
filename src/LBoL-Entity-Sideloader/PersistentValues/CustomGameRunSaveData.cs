using LBoL.Core;
using LBoL.Presentation;
using LBoLEntitySideloader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;

namespace LBoLEntitySideloader.PersistentValues
{
    /// <summary>
    /// Template for persistent data container class.
    /// Extending types should contain fields for values to be stored.
    /// Logic for saving/restoring the values should be implemented in appropriate methods.
    /// 
    /// Note1: Values are encoded/decoded as yaml so some limitations apply. Example: LBoL.Core.SaveData.GameSaveData
    /// Note2: Binary yaml files, suffixed with profile index and ".modd", are stored in game's save data folder. The files are never deleted, only overwritten.
    /// </summary>
    public abstract class CustomGameRunSaveData
    {
        /// <summary>
        /// Gr for "gameRun".
        /// </summary>
        public const string filePrefix = "gr";

        /// <summary>
        /// Registers CustomGameRunSaveData.
        /// </summary>
        /// <param name="GUID">Must be a unique GUID. Best to use the same GUID as Bepinex plugin.</param>
        public static void RegisterCustomSaveData(CustomGameRunSaveData customSaveData, string GUID)
        {
            var saveDataID = customSaveData.GetID(GUID);

            var ass = Assembly.GetCallingAssembly();

            InternalRegisterCustomData(ass, saveDataID, customSaveData);


        }

        /// <summary>
        /// Registers self.
        /// </summary>
        /// <param name="GUID">Must be a unique GUID. Best to use the same GUID as Bepinex plugin.</param>
        public void RegisterSelf(string GUID) 
        {
            var saveDataID = this.GetID(GUID);

            var ass = Assembly.GetCallingAssembly();

            InternalRegisterCustomData(ass, saveDataID, this);
        }

        public SaveDataID GetID(string GUID) => new SaveDataID()
        {
            GUID = GUID,
            Name = this.Name,
            midfix = filePrefix
        };

        private static void InternalRegisterCustomData(Assembly assembly, SaveDataID id, CustomGameRunSaveData customData)
        {
            if (assembly.IsLoadedFromDisk())
                EntityManager.Instance.loadedFromDsikCustomGrSaveData.TryAdd(id, customData);

            if (!UniqueTracker.Instance.customGrSaveData.TryAdd(id, customData))
            {
                Log.log.LogError($"Failed to register custom save data {id}. Save data with the same name was already registered.");
            }

        }

        /// <summary>
        /// Optional override for unique name of save data file. Needed if using several CustomSaveData per mod.
        /// </summary>
        public virtual string Name
        {
            get => "";
        }

        /// <summary>
        /// Encode yaml save files to binary? (not implemented)
        /// </summary>
        internal virtual bool EncodeToBinary
        {
            get => true;
        }

        /// <summary>
        /// Should custom save file be deleted after gamerun has ended.
        /// </summary>
        public virtual bool DeleteFileOnGamerunEnd { get => true; }


        /// <summary>
        /// Provide type converters for yaml serialization/deserialization 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IYamlTypeConverter> TypeConverters()
        {
            yield break;
        }

        /// <summary>
        /// A good override for unique name.
        /// </summary>
        /// <returns></returns>
        public string ReasonableUniqueName() => this.GetType().Name.Replace(".", "_");


        /// <summary>
        /// One of two main requirements for data container class. The method is called when gameRun attempts to save. Should assign the relevant custom values from gameRun data to data container fields.
        /// </summary>
        /// <param name="gameRun"></param>
        public abstract void Save(GameRunController gameRun);

        /// <summary>
        /// One of two main requirements for data container class. The method is called when gameRun is being loaded. Should populate custom gameRun values with values from container fields.
        /// </summary>
        /// <param name="gameRun"></param>
        public abstract void Restore(GameRunController gameRun);

        /// <summary>
        /// Execute code after gamerun has ended. GameMaster.Instance.CurrentGameRun might still be available at that point.
        /// </summary>
        public virtual void OnGamerunEnded()
        {
        }
    }
}
