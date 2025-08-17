using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using NO_Jamming_Edits.Patch;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace NO_Jamming_Edits
{
    [BepInPlugin(modGUID,modName,modVersion)]
    public class JammingEditBase : BaseUnityPlugin
    {

        private const string modGUID = "Truffle.NOJammingEdits";
        private const string modName = "Nuclear Option Jamming Edits";
        private const string modVersion = "0.30.9.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        internal static JammingEditBase Instance;
        internal static ManualLogSource mls;
        internal static string FolderPath;

        internal static AudioClip newSound = null;
        internal static bool soundReplaced = false;

        internal static Image jam_icon = null;
        internal static bool iconReplaced = true;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            harmony.PatchAll(typeof(JammingEditBase));
            harmony.PatchAll(typeof(JamPatch));
            mls.LogInfo("JammingEdits Started");
            FolderPath = Instance.Info.Location;
            FolderPath = FolderPath.TrimEnd("NO_Jamming_Edits.dll".ToCharArray());
            PreloadAssets(FolderPath);
        }

        void PreloadAssets(string path)
        {
            mls.LogInfo("Basepath: " + path);
            string AssetPath = path + "Assets" + Path.DirectorySeparatorChar;

            string[] assetList = Directory.GetFiles(AssetPath);
            if (assetList.Length == 0) 
            {
                mls.LogInfo("Assets not found");
            }

            for (int i = 0; i < assetList.Length; i++)
            {
                AudioClip result = LoadNoise(assetList[i]);
                if (result != null)
                {
                    newSound = result;
                    soundReplaced = true;
                    mls.LogInfo("Jamming Audio replace with: "+assetList[i]);
                }
            }

        }

        AudioClip LoadNoise(string path)
        {

            var musicType = GetAudioType(path);
            if (musicType == AudioType.UNKNOWN)
            {
                return null;
            }
            var loader = UnityWebRequestMultimedia.GetAudioClip(path, musicType);
            loader.SendWebRequest();

            while (true)
            {
                if (loader.isDone) break;
            }

            if (loader.error != null)
            {
                mls.LogError(loader.error);
                return null;
            }
            var clip = DownloadHandlerAudioClip.GetContent(loader);
            if (clip && clip.loadState == AudioDataLoadState.Loaded)
            {
                clip.name = path.TrimStart(FolderPath.ToCharArray());
                return clip;
            }

            mls.LogError($"Failed to load clip:{path}");
            return null;

        }

        private static AudioType GetAudioType(string path)
        {
            var extension = Path.GetExtension(path).ToLower();

            if (extension == ".wav")
                return AudioType.WAV;
            if (extension == ".ogg")
                return AudioType.OGGVORBIS;
            if (extension == ".mp3")
                return AudioType.MPEG;

            mls.LogError($"Unsupported extension: {path}");
            return AudioType.UNKNOWN;
        }

        
    }
}
