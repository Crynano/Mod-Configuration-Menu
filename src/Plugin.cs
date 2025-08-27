using MGSC;
using ModConfigMenu.Components;
using ModConfigMenu.Objects;
using ModConfigMenu.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ModConfigMenu
{
    public static class Plugin
    {
        public static string ModAssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string AllModsConfigPath = $"{Application.persistentDataPath}/../Quasimorph_ModConfigs";

        public static string MCMConfigPath = Path.Combine(AllModsConfigPath, "MCM");

        public const string MCM_CONTROLLED_SUFFIX = "_mcm";
        
        [Hook(ModHookType.BeforeBootstrap)]
        public static void BeforeBootstrap(IModContext context)
        {
            Directory.CreateDirectory(MCMConfigPath);
        }

        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void AfterConfig(IModContext context)
        {
            // Do nothing here.
        }

        [Hook(ModHookType.MainMenuStarted)]
        public static void MainMenuButton(IModContext context)
        {
            // Get object from main menu
            // Acces menubuttons child
            // Get a copy of a button
            // Change button of that copy to open our custom UI
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
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains(nameof(ModConfigMenu)))
                {
                    var mcm = Importer.LoadFileFromMemory<GameObject>("ModConfigMenu.Resources.mcmassets", "MCM");
                    mcm.AddComponent<ModConfigMenu>();
                    mcm.gameObject.SetActive(false);
                    return mcm;
                }
                else if (path.Contains(nameof(ColorPickerController)))
                {
                    var obj = Importer.LoadFileFromMemory<GameObject>("ModConfigMenu.Resources.mcmassets", "ColorPickerRoot");
                    obj.AddComponent<ColorPickerController>();
                    obj.SetActive(false);
                    return obj;
                }
                else if (path.Contains(nameof(ChangeModConfirmationPanel)))
                {
                    var obj = Importer.LoadFileFromMemory<GameObject>("ModConfigMenu.Resources.mcmassets", "SaveModConfirmPanel");
                    obj.AddComponent<ChangeModConfirmationPanel>();
                    obj.gameObject.SetActive(false);
                    return obj;
                }
                //else if (path.Contains(nameof(CustomTooltip)))
                //{
                //    var obj = Importer.LoadFileFromMemory<GameObject>("ModConfigMenu.Resources.mcmassets", "CustomTooltipMessage");
                //    obj.AddComponent<CustomTooltip>();
                //    obj.gameObject.SetActive(false);
                //    return obj;
                //}
            }
            return null;
        }
    }
}
