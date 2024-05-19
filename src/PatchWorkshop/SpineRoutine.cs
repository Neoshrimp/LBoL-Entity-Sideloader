using Spine;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using System.Linq;
using AddWatermark;
using static PatchWorkshop.BepinexPlugin;

namespace PatchWorkshop
{
    public class SpineRoutine
    {

        

        public static IEnumerator TestBones(Skeleton skeleton, int? rS = null, int? rL = null, float delay = 1f)
        {
            var bCount = skeleton.Bones.Count;
            log.LogInfo($"bone count: {bCount}");

            if (rS is null || rL is null) { rS = 0; rL = bCount; }
            else {
                rS = Math.Clamp(rS.Value, 0, bCount);
                rL = Math.Clamp(rS.Value + rL.Value, 0, bCount) - rS.Value;
            }
            var i = rS.Value;
            foreach (var b in skeleton.Bones.GetRange(rS.Value, rL.Value))
            {
                var ogScaleX = b.ScaleX;
                var ogScaleY = b.ScaleY;
                log.LogInfo($"{i}: {b.Data.Name}");
                b.ScaleX *= 5;
                b.ScaleY *= 5;
                yield return new WaitForSeconds(delay);
                b.ScaleX = ogScaleX;
                b.ScaleY = ogScaleY;
                i++;
            }
            log.LogInfo($"Bones done");

        }
    }
}
