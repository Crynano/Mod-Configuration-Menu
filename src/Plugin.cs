using MGSC;
using ModConfigMenu.Components;
using ModConfigMenu.Services;
using System.IO;
using System.Reflection;
using UnityEngine;

#pragma warning disable IDE0060
namespace ModConfigMenu
{
    public static class Plugin
    {
        public static string ModAssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public readonly static string AllModsConfigPath = $"{Application.persistentDataPath}/../Quasimorph_ModConfigs";

        public readonly static string MCMConfigPath = Path.Combine(AllModsConfigPath, "MCM");

        public const string MCM_CONTROLLED_SUFFIX = "_mcm";
        
        public const string MCM_EMBEDDEDRESOURCE_FILENAME = "ModConfigMenu.mcmassets";
        
        public const string MCM_FILERESOURCE_FILENAME = "mcmassets";
        
        [Hook(ModHookType.BeforeBootstrap)]
        public static void BeforeBootstrap(IModContext context)
        {
            Directory.CreateDirectory(MCMConfigPath);
        }

        [Hook(ModHookType.MainMenuStarted)]
        public static void MainMenuButton(IModContext context)
        {
            LocalizationHelper.AddKeyToAllDictionaries("ui.mods.desc", "MODS");
            
            Logger.LogDebug("Main Menu Started");

            var mainMenu = GameObject.FindObjectOfType<MainMenuScreen>(true);
            var menuButtons = mainMenu.transform.Find("MenuButtons");
            var buttonPrefab = menuButtons.GetChild(0);
            var myButtonInstance = GameObject.Instantiate(buttonPrefab, menuButtons);
            myButtonInstance.SetSiblingIndex(1);
            
            var mainMenuModsCommonButton = myButtonInstance.GetComponent<CommonButton>();
            mainMenuModsCommonButton.ChangeLabel("ui.mods.desc");
            mainMenuModsCommonButton.OnClick -= mainMenu.StartGameBtnOnClick;
            mainMenuModsCommonButton.OnClick += delegate (CommonButton button, int amount)
            {
                UI.Chain<ModConfigMenu>().HideAll().Show();
            };
            Logger.Flush();
        }

        [Hook(ModHookType.ResourcesLoad)]
        public static object LoadCustomResource(System.String path)
        {
            return LoadFromFileResource(path);
        }

        private static object LoadFromFileResource(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (path.Contains(nameof(ModConfigMenu)))
            {
                var mcm = Importer.LoadFileFromBundle<GameObject>(MCM_FILERESOURCE_FILENAME, "MCM");
                mcm.AddComponent<ModConfigMenu>();
                mcm.gameObject.SetActive(false);
                return mcm;
            }
            else if (path.Contains(nameof(ColorPickerController)))
            {
                var colorPickerRoot = Importer.LoadFileFromBundle<GameObject>(MCM_FILERESOURCE_FILENAME, "ColorPickerRoot");
                colorPickerRoot.AddComponent<ColorPickerController>();
                colorPickerRoot.SetActive(false);
                return colorPickerRoot;
            }
            else if (path.Contains(nameof(ChangeModConfirmationPanel)))
            {
                var saveModConfirmPanel = Importer.LoadFileFromBundle<GameObject>(MCM_FILERESOURCE_FILENAME, "SaveModConfirmPanel");
                saveModConfirmPanel.AddComponent<ChangeModConfirmationPanel>();
                saveModConfirmPanel.gameObject.SetActive(false);
                return saveModConfirmPanel;
            }
            return null;
        }

        private static object LoadFromEmbeddedResource(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (path.Contains(nameof(ModConfigMenu)))
            {
                var mcm = Importer.LoadFileFromMemory<GameObject>(MCM_EMBEDDEDRESOURCE_FILENAME, "MCM");
                mcm.AddComponent<ModConfigMenu>();
                mcm.gameObject.SetActive(false);
                return mcm;
            }
            else if (path.Contains(nameof(ColorPickerController)))
            {
                var obj = Importer.LoadFileFromMemory<GameObject>(MCM_EMBEDDEDRESOURCE_FILENAME, "ColorPickerRoot");
                obj.AddComponent<ColorPickerController>();
                obj.SetActive(false);
                return obj;
            }
            else if (path.Contains(nameof(ChangeModConfirmationPanel)))
            {
                var obj = Importer.LoadFileFromMemory<GameObject>(MCM_EMBEDDEDRESOURCE_FILENAME, "SaveModConfirmPanel");
                obj.AddComponent<ChangeModConfirmationPanel>();
                obj.gameObject.SetActive(false);
                return obj;
            }
            return null;
        }
    }
}
#pragma warning restore IDE0060