using MGSC;
using ModConfigMenu.Components;
using ModConfigMenu.Services;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ModConfigMenu
{
    public static class Plugin
    {
        public static string ModAssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string AllModsConfigPath = $"{Application.persistentDataPath}/../Quasimorph_ModConfigs";

        public const string MCM_CONTROLLED_SUFFIX = "_mcm";

        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void AfterConfig(IModContext context)
        {
            // Do nothing?
        }

        [Hook(ModHookType.MainMenuStarted)]
        public static void MainMenuButton(IModContext context)
        {
            // Get object from main menu
            // Acces menubuttons child
            // Get a copy of a button
            // Change button of that copy to open our custom UI
            var mainMenu = GameObject.FindObjectOfType<MainMenuScreen>(true);
            var menuButtons = mainMenu.transform.Find("MenuButtons");
            var buttonPrefab = menuButtons.GetChild(0);
            var myButtonInstance = GameObject.Instantiate(buttonPrefab, menuButtons);
            myButtonInstance.SetSiblingIndex(1);
            //var captionGO = myButtonInstance.Find("Caption");
            //captionGO.GetComponent<TextMeshProUGUI>().text = "MODS".ColorFirstLetter(Color.yellow);

            LocalizationHelper.AddKeyToAllDictionaries("ui.mods.desc", "MODS");
            var mainMenuModsCommonButton = myButtonInstance.GetComponent<CommonButton>();
            mainMenuModsCommonButton.ChangeLabel("ui.mods.desc");
            mainMenuModsCommonButton.OnClick -= mainMenu.StartGameBtnOnClick;
            mainMenuModsCommonButton.OnClick += delegate (CommonButton button, int amount)
            {
                UI.Chain<ModConfigMenu>().HideAll().Show();
            };
        }

        [Hook(ModHookType.ResourcesLoad)]
        public static object LoadCustomResource(System.String path)
        {
            switch (string.IsNullOrEmpty(path))
            {
                //Debug.Log($"Trying to load custom resoruce \"{path}\"");
                case false when path.Contains(nameof(ModConfigMenu)):
                    {
                        // Get it from assetbundle
                        //var mcm = Importer.LoadFileFromBundle<GameObject>("mcmassets", "MCM");
                        var mcm = Importer.LoadFileFromMemory<GameObject>("ModConfigMenu.Resources.mcmassets", "MCM");
                        mcm.AddComponent<ModConfigMenu>();
                        return mcm;
                        //var menuSettingsScreen = GameObject.FindObjectOfType<GameSettingsScreen>(true).gameObject;
                        //var customSettingsInstance = GameObject.Instantiate(menuSettingsScreen);
                        //GameObject.DestroyImmediate(customSettingsInstance.GetComponent<GameSettingsScreen>());
                        //return customSettingsInstance;
                    }
                case false when path.Contains(nameof(ColorPickerController)):
                    {
                        var obj = Importer.LoadFileFromMemory<GameObject>("ModConfigMenu.Resources.mcmassets", "ColorPickerRoot");
                        obj.AddComponent<ColorPickerController>();
                        obj.SetActive(false);
                        return obj;
                    }
                case false when path.Contains(nameof(ChangeModConfirmationPanel)):
                    {
                        var obj = Importer.LoadFileFromMemory<GameObject>("ModConfigMenu.Resources.mcmassets", "SaveModConfirmPanel");
                        obj.AddComponent<ChangeModConfirmationPanel>();
                        return obj;
                    }
                default:
                    return null;
            }
        }
    }
}
