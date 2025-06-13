using MGSC;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

namespace ModConfigMenu.Services
{
    internal class Importer
    {
        public static T LoadFileFromBundle<T>(string bundlePath, string fileName) where T : class
        {
            if (string.IsNullOrEmpty(fileName))
            {
                //Logger.LogWarning($"Bundle fileName was empty or null");
                return null;
            }

            var completePath = Path.Combine(Plugin.ModAssemblyLocation, bundlePath);

            if (!File.Exists(completePath))
            {
                //Logger.LogWarning($"Could not find bundle with {bundlePath} at {completePath}");
                return null;
            }

            // If file doesnt have the correct extension?
            // if (!Path.HasExtension(completePath)) { Logger.LogError($"Incorrect path at {bundlePath}"); return null; }
            //Logger.LogInfo($"Loading from {bundlePath}");
            // We assume its right
            var loadedBundle = AssetBundle.LoadFromFile(completePath);
            var loadedAsset = loadedBundle.LoadAsset(fileName, typeof(T)) as T;
            loadedBundle.Unload(false);
            if (loadedAsset != null)
            {
                //Logger.LogInfo($"Loaded asset correctly! Returning {loadedAsset.GetType()}");
                return loadedAsset;
            }
            else
            {
                //Logger.LogWarning($"Asset {fileName} is missing from bundle: {bundlePath}");
                return null;
            }
        }

        // Thanks to "NBK_RedSpy" and "amazonochka utyty" from QM Discord!
        public static T LoadFileFromMemory<T>(string resourceName, string fileName) where T : class
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(resourceName))
            {
                return null;
            }

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                Logger.LogError("ASSETBUNDLE COULD NOT BE LOADED");
                return null;
            }

            AssetBundle loadedBundle = AssetBundle.LoadFromStream(stream);
            var loadedAsset = loadedBundle.LoadAsset(fileName, typeof(T)) as T;
            loadedBundle.Unload(false);

            stream.Position = 0;

            if (loadedAsset != null)
            {
                //Logger.LogInfo($"Loaded asset correctly! Returning {loadedAsset.GetType()}");
                return loadedAsset;
            }
            return null;
        }
    }
}
