using LBoL.Core;
using System.Collections.Generic;

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
            var saveDataID = new SaveDataID()
            {
                GUID = GUID,
                Name = customSaveData.Name,
                midfix = filePrefix
            };

            if (!UniqueTracker.Instance.customGrSaveData.TryAdd(saveDataID, customSaveData))
            {
                Log.log.LogError($"Failed to register custom save data {saveDataID}. Save data with the same name was already registered.");
            }
        }

        /// <summary>
        /// Registers self.
        /// </summary>
        /// <param name="GUID">Must be a unique GUID. Best to use the same GUID as Bepinex plugin.</param>
        public void RegisterSelf(string GUID) => RegisterCustomSaveData(this, GUID);

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
        internal virtual bool Encode
        {
            get => true;
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

    }
}
