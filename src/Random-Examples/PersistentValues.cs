using Cysharp.Threading.Tasks;
using HarmonyLib;
using LBoL.Base;
using LBoL.Core;
using LBoL.Core.SaveData;
using LBoL.Presentation;
using LBoLEntitySideloader;
using LBoLEntitySideloader.PersistentValues;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine.Rendering;
using YamlDotNet.Serialization;
using static Random_Examples.BepinexPlugin;

namespace Random_Examples
{

    public sealed class SuikaGrData : CustomGameRunSaveData
    {
        public override void Restore(GameRunController gameRun)
        {
            log.LogInfo("restoring");
            var player = gameRun.Player;
            if (player is Suika suika)
            {
                suika.drunkedness = drunkedness;
                suika.miniMes = miniMes;
                suika.rng = RandomGen.FromState(rngState);
            }
            PrintDebug();
        }

        public override void Save(GameRunController gameRun)
        {
            log.LogInfo("saving");
            var player = gameRun.Player;
            if (player is Suika suika)
            {
                drunkedness = suika.drunkedness;
                miniMes = suika.miniMes;
                rngState = suika.rng.State;
                suika.PrintDebug();
            }
        }

        public void PrintDebug()
        {
            log.LogInfo("Container class");
            log.LogInfo($"{drunkedness}");
            log.LogInfo($"{rngState}");
            miniMes.Do(i => log.LogInfo($"{i}"));
        }


        public int drunkedness;

        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public List<MiniMe> miniMes = new List<MiniMe>();

        public ulong rngState;



    }





}
