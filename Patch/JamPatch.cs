using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static System.Net.WebRequestMethods;


namespace NO_Jamming_Edits.Patch
{
    [HarmonyPatch(typeof(CombatHUD))]
    internal class JamPatch
    {

        static VideoPlayer player = null;

        [HarmonyPatch("UpdateMarkers")]
        [HarmonyPrefix]
        static void SwapSound(ref AudioClip ___jammedSound, ref float ___jamAccumulation)
        {
            
            if (JammingEditBase.soundReplaced)
            {
                ___jammedSound = JammingEditBase.newSound;
            }

            if (player != null)
            {
                
                if (___jamAccumulation > 0f)
                {
                    
                    player.Play();
                    player.targetCameraAlpha = Mathf.Clamp(___jamAccumulation,0.1f,1.0f);
                }
                
                else
                {
                    player.Pause();
                    player.targetCameraAlpha = 0;
                }
                
            }
        }

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        static void AddVideo()
        {
            
            GameObject camera = Camera.allCameras[0].gameObject;
            VideoPlayer newPlayer = camera.gameObject.AddComponent<VideoPlayer>();
            newPlayer.playOnAwake = false;
            newPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;
            newPlayer.targetCameraAlpha = 0.5F;
            string AssetPath = JammingEditBase.FolderPath + "Assets" + Path.DirectorySeparatorChar+"clip.webm";
            newPlayer.url = AssetPath;
            newPlayer.isLooping = true;
            
            player = newPlayer;

        }

    }

    
}
