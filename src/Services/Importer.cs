using MGSC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using JetBrains.Annotations;
using UnityEngine;

namespace ModConfigMenu.Services
{
    internal class Importer
    {
        public static T LoadFileFromBundle<T>(string bundlePath, string fileName) where T : class
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            var completePath = Path.Combine(Plugin.ModAssemblyLocation, bundlePath);

            if (!File.Exists(completePath))
            {
                return null;
            }
            
            var loadedBundle = AssetBundle.LoadFromFile(completePath);
            var loadedAsset = loadedBundle.LoadAsset(fileName, typeof(T)) as T;
            loadedBundle.Unload(false);
            return loadedAsset ?? null;
        }
        
        [CanBeNull]
        public static List<T> LoadFilesFromBundle<T>(string bundlePath, List<string> fileNames) where T : class
        {
            var loadedAssets = new List<T>();
            if (string.IsNullOrEmpty(bundlePath) || fileNames == null || fileNames.Count == 0)
            {
                return null;
            }

            var completePath = Path.Combine(Plugin.ModAssemblyLocation, bundlePath);

            if (!File.Exists(completePath))
            {
                return null;
            }
            
            var loadedBundle = AssetBundle.LoadFromFile(completePath);
            foreach (var fileName in fileNames)
            {
                var loadedAsset = loadedBundle.LoadAsset(fileName, typeof(T)) as T;
                if (loadedAsset != null)
                {
                    loadedAssets.Add(loadedAsset);
                }
            }
            loadedBundle.Unload(false);
            return loadedAssets;
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
                // Logger.LogError("ASSETBUNDLE COULD NOT BE LOADED");
                return null;
            }

            AssetBundle loadedBundle = AssetBundle.LoadFromStream(stream);
            var loadedAsset = loadedBundle.LoadAsset(fileName, typeof(T)) as T;
            loadedBundle.Unload(false);

            stream.Position = 0;

            return loadedAsset ?? null;
        }
    }
}
