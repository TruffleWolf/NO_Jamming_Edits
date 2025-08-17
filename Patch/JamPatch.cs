using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace NO_Jamming_Edits.Patch
{
    [HarmonyPatch(typeof(CombatHUD))]
    internal class JamPatch
    {

        [HarmonyPatch("UpdateMarkers")]
        [HarmonyPrefix]
        static void SwapSound(ref AudioClip ___jammedSound)
        {
            if (JammingEditBase.soundReplaced)
            {
                ___jammedSound = JammingEditBase.newSound;
            }
        }

    }
}
