﻿using System;
using MGSC;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using ModConfigMenu.Components;
using ModConfigMenu.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace ModConfigMenu
{
    [UIView(GameLoopGroup.MainMenu, false, true)]
    public class ModConfigMenu : MonoBehaviour
    {
        private GameObject ModButtonPrefab;

        private Transform ModListRoot;
        private Transform ModConfigRoot;
        private Transform ConfigAreaRoot;
        private Transform PrefabsRoot;
        private Transform ContentRoot;

        private GameObject boolButtonPrefab;
        private GameObject stringButtonPrefab;
        private GameObject rangeButtonPrefab;
        private GameObject colorButtonPrefab;
        private GameObject headerPrefab;
        private GameObject rootPrefab;

        private GameObject colorPickerPrefab;

        private CommonButton _backButton;
        private Button _saveButton;
        private Button _resetDefaultButton;

        private ConfigData lastActiveMod = null;
        private Transform lastActiveModRoot = null;
        private Dictionary<string, Transform> ModsRoot = new Dictionary<string, Transform>();

        public void Awake()
        {
            // We need the scroll bar for the mods working
            //var ModListGO = transform.Find("ModList");
            //var modListScrollBar = ModListGO.transform.Find("CommonScrollBar");
            //var scrollBarComponent = modListScrollBar.gameObject.AddComponent<CommonScrollBar>();
            //scrollBarComponent._canvasGroup = scrollBarComponent.GetComponent<CanvasGroup>();
            //scrollBarComponent._scrollRect = ModListGO.GetComponentInChildren<ScrollRect>();
            //scrollBarComponent._selectionBorder = modListScrollBar.transform.Find("SelectionBorder").GetComponent<Image>();
            //scrollBarComponent._handle = modListScrollBar.transform.Find("Sliding Area").GetComponentInChildren<Image>();

            // And we need the scroll bar for config working
            //var ConfigAreaGO = transform.Find("ConfigArea");
            //var configAreaScrollBar = ConfigAreaGO.Find("CommonScrollBar");
            //var configScrollBarComponent = configAreaScrollBar.gameObject.AddComponent<CommonScrollBar>();
            //configScrollBarComponent._canvasGroup = configScrollBarComponent.GetComponent<CanvasGroup>();
            //configScrollBarComponent._scrollRect = ConfigAreaGO.GetComponentInChildren<ScrollRect>();
            //configScrollBarComponent._selectionBorder = configAreaScrollBar.transform.Find("SelectionBorder").GetComponent<Image>();
            //configScrollBarComponent._handle = configAreaScrollBar.transform.Find("Sliding Area").GetComponentInChildren<Image>();

            // Gathering the gameSettings to get prefabs.
            var gameSettingsScreen = FindObjectOfType<GameSettingsScreen>(true);
            // Let's find a generic button to modify.
            ModButtonPrefab = gameSettingsScreen.transform.Find("Window").Find("Buttons").Find("BtnGeneral").gameObject;
            // Let's get the mod button prefab working
            ModListRoot = transform.Find("ModList").Find("ModsScroll").Find("Viewport").Find("Content");

            ConfigAreaRoot = transform.Find("ConfigArea");
            PrefabsRoot = ConfigAreaRoot.Find("Prefabs");
            ContentRoot = ConfigAreaRoot.Find("ContentRoot");

            ConfigureBoolButtonPrefab();
            ConfigureRangeButtonPrefab();
            ConfigureColorButtonPrefab();

            rootPrefab = ConfigAreaRoot.Find("Prefabs").Find("Root").gameObject;
            rootPrefab.SetActive(false);
            headerPrefab = ConfigAreaRoot.Find("Prefabs").Find("Header").gameObject;
            headerPrefab.SetActive(false);

            // We have back button solved.
            var bbPrefab = gameSettingsScreen.transform.Find("BackButton");
            _backButton = GameObject.Instantiate(bbPrefab, transform).GetComponent<CommonButton>();
            if (_backButton != null)
                _backButton.OnClick += delegate { UI.Back(); };

            _saveButton = ConfigAreaRoot.Find("SaveButton")?.GetComponent<Button>();
            if (_saveButton != null)
            {
                _saveButton.onClick.AddListener(SaveCurrentMod);
            }

            _resetDefaultButton = ConfigAreaRoot.Find("DefaultButton")?.GetComponent<Button>();
            if (_resetDefaultButton != null)
            {
                _resetDefaultButton.onClick.AddListener(ResetCurrentMod);
            }
        }

        public void Start()
        {
            CreateButtonsForEveryMod();
        }

        private void CreateButtonsForEveryMod()
        {
            foreach (var modName in ModConfigManager.ModsList)
            {
                var buttonObject = GameObject.Instantiate(ModButtonPrefab, ModListRoot);
                var objectButton = buttonObject.GetComponent<CommonButton>();
                objectButton.ChangeLabel(modName);
                objectButton.OnClick += delegate
                {
#if DEBUG
                    Debug.Log($"Clicked button to load {modName} config.");
#endif
                    ConfigData modConfig = ModConfigManager.GetModConfig(modName);
                    ModsRoot.TryGetValue(modName, out Transform root);
                    if (root != null)
                    {
                        SwitchMod(modConfig, root);
                    }
                    else
                    {
                        CreateNewMod(modName, modConfig);
                    }
                };
            }
        }

        private void ConfigureBoolButtonPrefab()
        {
            boolButtonPrefab = PrefabsRoot.Find("BoolConfig").gameObject;
            var boolToggle = boolButtonPrefab.transform.Find("Toggle").gameObject;
            boolToggle.AddComponent<OnClickSfx>();
            boolToggle.AddComponent<ToggleWrapper>();
            boolButtonPrefab.transform.Find("Label").gameObject.AddComponent<GenericHoverTooltip>();
            boolButtonPrefab.SetActive(false);
        }

        // Some way of instantiating the boolButton and start its components?
        private void ConfigureColorButtonPrefab()
        {
            colorButtonPrefab = PrefabsRoot.Find("ColourConfig").gameObject;
            colorButtonPrefab.transform.Find("Label").gameObject.AddComponent<GenericHoverTooltip>();
            colorButtonPrefab.SetActive(false);
        }

        private void ConfigureRangeButtonPrefab()
        {
            rangeButtonPrefab = PrefabsRoot.Find("RangeConfig").gameObject;
            var sliderObject = rangeButtonPrefab.transform.Find("Slider").gameObject;
            var sliderComponent = sliderObject.GetComponent<Slider>();

            SliderWrapper mgscSliderComponent = sliderObject.AddComponent<SliderWrapper>();
            mgscSliderComponent._visibleMode = SliderWrapper.VisibleMode.WholeNumbers;
            mgscSliderComponent._slider = sliderComponent;
            mgscSliderComponent._sliderFillBar = sliderObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
            mgscSliderComponent._valueText = sliderObject.transform.Find("SliderVal").GetComponent<TextMeshProUGUI>();
            mgscSliderComponent._barColor = new Color(0.5059f, 0.7098f, 0.4784f, 1f);
            mgscSliderComponent.Awake();

            rangeButtonPrefab.transform.Find("Label").gameObject.AddComponent<GenericHoverTooltip>();
            rangeButtonPrefab.SetActive(false);
        }

        private void SwitchMod(ConfigData newMod, Transform newOne)
        {
            // if currentMod is dirty, pop the menu asking to save.
            // if yes, save and execute following
            // If no, skip
            if (lastActiveModRoot != null && newOne.gameObject == lastActiveModRoot.gameObject) return;
            if (lastActiveMod != null && lastActiveMod.IsDirty)
            {
                // Popup
                ColorUtility.TryParseHtmlString("#FFFEC1", out Color letterColor);
                UI.Chain<ChangeModConfirmationPanel>().Show();
                UI.Get<ChangeModConfirmationPanel>().Configure(
                    "Unsaved Changes".ColorFirstLetter(letterColor),
                    "You still have unsaved changes.\nDo you want to save them before leaving this screen?",
                    SaveAndChangeMod,
                    DiscardChanges,
                    null
                );
            }
            else
            {
                ChangeMod();
            }
            return;

            void SaveAndChangeMod()
            {
                SaveCurrentMod();
                ChangeMod();
            }

            void DiscardChanges()
            {
                DiscardCurrentModChanges();
                DestroyDiscardedMod(lastActiveMod);
                ChangeMod();
            }

            void DestroyDiscardedMod(ConfigData oldMod)
            {
                lastActiveModRoot?.gameObject.SetActive(false);
                ModsRoot.Remove(oldMod.ModName);
                Destroy(lastActiveModRoot?.gameObject);
            }

            void ChangeMod()
            {
                if (lastActiveMod != null)
                {
                    lastActiveMod.OnConfigChanged -= EnableSaveButton;
                }
                lastActiveMod = newMod;
                lastActiveMod.OnConfigChanged += EnableSaveButton;
                lastActiveModRoot?.gameObject.SetActive(false);
                lastActiveModRoot = newOne;
                lastActiveModRoot.gameObject.SetActive(true);
            }
        }

        private void CreateNewMod(string modName, ConfigData modConfig)
        {
            var newRoot = BuildModConfig(modConfig);
            ModsRoot.Add(modName, newRoot);
            SwitchMod(modConfig, newRoot);
        }

        private void ReloadModRoot()
        {
            DiscardCurrentModChanges();
            string currentModName = lastActiveMod.ModName;
            lastActiveModRoot?.gameObject.SetActive(false);
            ModsRoot.Remove(lastActiveMod.ModName);
            Destroy(lastActiveModRoot?.gameObject);
            ConfigData modConfig = ModConfigManager.GetModConfig(currentModName);
            modConfig.ResetAllToDefault();
            CreateNewMod(currentModName, modConfig);
            SaveCurrentMod();
        }

        private void ResetCurrentMod()
        {
            if (lastActiveMod == null) return;

            ColorUtility.TryParseHtmlString("#FFFEC1", out Color letterColor);
            UI.Chain<ChangeModConfirmationPanel>().Show();
            UI.Get<ChangeModConfirmationPanel>().Configure(
                "Reset all to default.".ColorFirstLetter(letterColor),
                "Are you sure you want to reset all values to default?",
                ReloadModRoot,
                null,
                null
            );
        }

        private void DiscardCurrentModChanges()
        {
            if (lastActiveMod == null) return;
            lastActiveMod?.Discard();
            // Should discard all changes to the UI too.
            _saveButton?.gameObject.SetActive(false);
        }
        private void SaveCurrentMod()
        {
            if (lastActiveMod == null) return;
            lastActiveMod?.Save();
            _saveButton?.gameObject.SetActive(false);
        }

        private void EnableSaveButton()
        {
            _saveButton?.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Most important feature. UI Building!
        /// </summary>
        /// <param name="modData"></param>
        /// <returns></returns>
        private Transform BuildModConfig(ConfigData modData)
        {
            if (modData == null)
            {
                Debug.Log($"Mod can't be built!");
                return null;
            }

            Transform rootGameObject = GameObject.Instantiate(rootPrefab, ContentRoot).transform;
            //rootGameObject.transform.parent = ContentRoot;
            Transform thisContentRoot = rootGameObject.GetComponent<ScrollRect>().content;

            foreach (var headerEntry in modData.GetData())
            {
                var header = GameObject.Instantiate(headerPrefab, thisContentRoot);
                header.GetComponentInChildren<TextMeshProUGUI>().text = headerEntry.Key;
                header.SetActive(true);
                foreach (var categoryVariables in headerEntry.Value)
                {
                    var goToInstantiate = boolButtonPrefab;
                    GameObject instObj = null;
                    if (categoryVariables.GetValue() is bool boolValue)
                    {
                        goToInstantiate = boolButtonPrefab;
                        instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                        instObj.GetComponentInChildren<Toggle>().isOn = boolValue;
                        instObj.GetComponentInChildren<Toggle>().onValueChanged.AddListener(
                        delegate (bool a)
                        {
                            categoryVariables.SetUnstoredValue(a);
                        });
                    }
                    else if (categoryVariables.GetValue() is int intValue)
                    {
                        goToInstantiate = rangeButtonPrefab;
                        instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                        var objectSlider = instObj.GetComponentInChildren<Slider>();
                        objectSlider.minValue = categoryVariables.GetMin();
                        objectSlider.maxValue = categoryVariables.GetMax();
                        objectSlider.value = (float)intValue;
                        objectSlider.onValueChanged.AddListener(delegate (float newVal)
                        {
                            categoryVariables.SetUnstoredValue((int)newVal);
                        });
                        objectSlider.GetComponentInChildren<TextMeshProUGUI>().text = intValue.ToString(CultureInfo.CurrentCulture);
                    }
                    else if (categoryVariables.GetValue() is float floatValue)
                    {
                        goToInstantiate = rangeButtonPrefab;
                        instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                        var objectSlider = instObj.GetComponentInChildren<Slider>();
                        objectSlider.minValue = categoryVariables.GetMin();
                        objectSlider.maxValue = categoryVariables.GetMax();
                        objectSlider.value = floatValue;
                        objectSlider.onValueChanged.AddListener(delegate (float newVal)
                        {
                            categoryVariables.SetUnstoredValue(newVal);
                        });
                        objectSlider.GetComponentInChildren<TextMeshProUGUI>().text = floatValue.ToString(CultureInfo.CurrentCulture);
                    }
                    else if (categoryVariables.GetValue() is double doubleValue)
                    {
                        goToInstantiate = rangeButtonPrefab;
                        instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                        var objectSlider = instObj.GetComponentInChildren<Slider>();
                        objectSlider.minValue = categoryVariables.GetMin();
                        objectSlider.maxValue = categoryVariables.GetMax();
                        objectSlider.value = (float)doubleValue;
                        objectSlider.onValueChanged.AddListener(categoryVariables.SetUnstoredValue);
                        objectSlider.GetComponentInChildren<TextMeshProUGUI>().text = doubleValue.ToString(CultureInfo.CurrentCulture);
                    }
                    else if (categoryVariables.GetValue() is Color colore)// if (categoryVariables.Value is Color colorValue)
                    {
                        goToInstantiate = colorButtonPrefab;
                        instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                        var objectButton = instObj.GetComponentInChildren<Button>();
                        objectButton.transform.Find("ColorPreview").GetComponent<Image>().color = colore;
                        objectButton.onClick.AddListener(() =>
                        {
                            UI.Chain<ColorPickerController>().Show();
                            var currentColor = objectButton.transform.Find("ColorPreview").GetComponent<Image>().color;
                            UI.Get<ColorPickerController>().ConfigureButtons(currentColor, delegate(Color selectedColor)
                            {
                                categoryVariables.SetUnstoredValue(selectedColor);
                                objectButton.transform.Find("ColorPreview").GetComponent<Image>().color = selectedColor;
                            });
                        });
                    }
                    else if (ColorUtility.TryParseHtmlString(categoryVariables.Value.ToString().Replace("\"", string.Empty), out Color colorValue))// if (categoryVariables.Value is Color colorValue)
                    {
                        goToInstantiate = colorButtonPrefab;
                        instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                        var objectButton = instObj.GetComponentInChildren<Button>();
                        objectButton.transform.Find("ColorPreview").GetComponent<Image>().color = colorValue;
                        objectButton.onClick.AddListener(() =>
                        {
                            UI.Chain<ColorPickerController>().Show();
                            var currentColor = objectButton.transform.Find("ColorPreview").GetComponent<Image>().color;
                            UI.Get<ColorPickerController>().ConfigureButtons(currentColor, delegate(Color selectedColor)
                            {
                                categoryVariables.SetUnstoredValue($"\"#{ColorUtility.ToHtmlStringRGB(selectedColor)}\"");
                                objectButton.transform.Find("ColorPreview").GetComponent<Image>().color = selectedColor;
                            });
                        });
                    }
                    else
                    {
                        // Dafuq
                        Debug.Log("Dafuq");
                    }
                    // Here we instantiate the stuff!

                    if (instObj == null) continue;

                    // Maybe we should add a binding to restart each property?
                    // HURRRRRR

                    // Visual tooltip to aid in property description
                    instObj.GetComponentInChildren<GenericHoverTooltip>(true)?.Initialize(categoryVariables.GetDescription());

                    // Label for each object
                    var label = categoryVariables.GetLabel();
                    instObj.GetComponentInChildren<TextMeshProUGUI>().text = !string.IsNullOrEmpty(label) ? label : categoryVariables.Key;
                    instObj.SetActive(true);
                }
            }

            return rootGameObject;
        }
    }
}