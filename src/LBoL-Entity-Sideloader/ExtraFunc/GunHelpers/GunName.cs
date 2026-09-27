using System.Collections.Generic;
using System.Linq;
using LBoL.ConfigData;

namespace LBoLEntitySideloader.ExtraFunc.GunHelpers
{
    public static class GunNameID
    {
        private static readonly IReadOnlyList<GunConfig> gunConfig = GunConfig.AllConfig();

        /// <summary>
        /// <para>
        ///To get a list of all the in-game gun IDs:
        ///</para>
        /// 1) Install debug mode<br/>
        /// 2) Start an enemy encounter<br/>
        /// 3) Click F2<br/>
        /// 4) Select "gun test"<br/>
        /// 5) This will open a menu with all the gun options in the game. The IDs are located on the left of the gun names.<br/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetGunFromId(int id)
        {
            string gun_name = "";
            try
            {
                gun_name = (from config in gunConfig
                                where config.Id == id
                                select config.Name).ToList()[0];
            }
            catch
            {
                UnityEngine.Debug.Log("id: " + id + " doesn't exist. Check whether the ID is correct.");
                gun_name = "Instant";
            }                   
            return gun_name;
        }
    }
}