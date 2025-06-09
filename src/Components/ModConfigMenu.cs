using MGSC;
using System.Collections.Generic;
using System.Globalization;
using ModConfigMenu.Components;
using ModConfigMenu.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using static MGSC.InputController;
using System.ComponentModel;
using System;

namespace ModConfigMenu
{
    [UIView(GameLoopGroup.MainMenu, false, true)]
    public class ModConfigMenu : MonoBehaviour
    {
        private GameObject ModButtonPrefab;

        private Transform ModListRoot;
        private Transform ConfigAreaRoot;
        private Transform PrefabsRoot;
        private Transform ContentRoot;

        private GameObject boolButtonPrefab;
        private GameObject stringButtonPrefab;
        private GameObject rangeButtonPrefab;
        private GameObject colorButtonPrefab;
        private GameObject dropdownPrefab;
        private GameObject clickableButtonPrefab;
        //private GameObject keybindPrefab;
        private GameObject headerPrefab;
        private GameObject rootPrefab;

        private GameObject colorPickerPrefab;

        private CommonButton _backButton;
        private Button _saveButton;
        private Button _resetDefaultButton;

        private ModConfig lastActiveMod = null;
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
            //ConfigureKeybindButtonPrefab();
            ConfigureClickableButtonPrefab();
            ConfigureDropdownPrefab();

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
                    ModConfig modConfig = ModConfigManager.GetModConfig(modName);
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

        private void ConfigureClickableButtonPrefab()
        {
            clickableButtonPrefab = PrefabsRoot.Find("ClickableButton")?.gameObject;
        }

        private void ConfigureDropdownPrefab()
        {
            dropdownPrefab = PrefabsRoot.Find("Dropdown").gameObject;
            dropdownPrefab?.transform.Find("Label").gameObject.AddComponent<GenericHoverTooltip>();
            dropdownPrefab?.SetActive(false);
        }

        //private void ConfigureKeybindButtonPrefab()
        //{
        //    keyBindPrefab = FindObjectOfType<KeybindingPage>(true)?.GetComponentInChildren<GameKeySetupPanel>(true)?.gameObject;
        //    // Label is keyBindPrefab.transform.Find("Label")
        //    if (keyBindPrefab == null)
        //    {
        //        Debug.LogError($"Could not find the keybind prefab for MCM");
        //        return;
        //    }
        //}

        private void SwitchMod(ModConfig newMod, Transform newOne)
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

            void DestroyDiscardedMod(ModConfig oldMod)
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

        private void CreateNewMod(string modName, ModConfig modConfig)
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
            ModConfig modConfig = ModConfigManager.GetModConfig(currentModName);
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
        private Transform BuildModConfig(ModConfig modData)
        {
            if (modData == null)
            {
                Debug.Log($"Mod can't be built!");
                return null;
            }

            Transform rootGameObject = GameObject.Instantiate(rootPrefab, ContentRoot).transform;
            //rootGameObject.transform.parent = ContentRoot;
            Transform thisContentRoot = rootGameObject.GetComponent<ScrollRect>().content;
            string currentHeader = string.Empty;

            foreach (var currentDatablock in modData.GetData())
            {
                if (!currentHeader.Equals(currentDatablock.Header))
                {
                    currentHeader = currentDatablock.Header;
                    var header = GameObject.Instantiate(headerPrefab, thisContentRoot);
                    header.GetComponentInChildren<TextMeshProUGUI>().text = currentHeader;
                    header.SetActive(true);
                }

                var goToInstantiate = boolButtonPrefab;
                GameObject instObj = null;

                // Forced type. For example dropdown.
                if (currentDatablock.GetValue() is bool boolValue)
                {
                    goToInstantiate = boolButtonPrefab;
                    instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                    var toggle = instObj.GetComponentInChildren<Toggle>();
                    toggle.isOn = boolValue;
                    toggle.onValueChanged.AddListener(
                    delegate (bool a)
                    {
                        currentDatablock.SetUnstoredValue(a);
                    });
                }
                else if (currentDatablock.GetValue() is int intValue)
                {
                    if (currentDatablock.GetTypeProp().ToLower().Equals("dropdown"))
                    {
                        // Create a dropdown but behave as an int.
                        // Get options, and their string counterparts.
                        goToInstantiate = dropdownPrefab;
                        instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                        var dropdown = instObj.GetComponentInChildren<TMP_Dropdown>(true);
                        dropdown.AddOptions(currentDatablock.GetDropdownOptions());
                        dropdown.SetValueWithoutNotify(intValue);
                        dropdown.onValueChanged.AddListener(delegate (int newIndex)
                        {
                            currentDatablock.SetUnstoredValue(Convert.ToInt32(newIndex));
                        });
                    }
                    else
                    {
                        goToInstantiate = rangeButtonPrefab;
                        instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                        var objectSlider = instObj.GetComponentInChildren<Slider>();
                        objectSlider.minValue = currentDatablock.GetMin();
                        objectSlider.maxValue = currentDatablock.GetMax();
                        objectSlider.value = (float)intValue;
                        objectSlider.onValueChanged.AddListener(delegate(float newVal)
                        {
                            currentDatablock.SetUnstoredValue(Convert.ToInt32(newVal));
                        });
                        objectSlider.GetComponentInChildren<TextMeshProUGUI>().text =
                            intValue.ToString(CultureInfo.CurrentCulture);
                    }
                }
                else if (currentDatablock.GetValue() is float floatValue)
                {
                    goToInstantiate = rangeButtonPrefab;
                    instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);

                    var wrapper = instObj.GetComponentInChildren<SliderWrapper>(true);
                    wrapper._visibleMode = SliderWrapper.VisibleMode.Default;
                    var bindingText = wrapper._valueText;
                    wrapper._valueText = null;

                    var objectSlider = instObj.GetComponentInChildren<Slider>();
                    objectSlider.minValue = currentDatablock.GetMin();
                    objectSlider.maxValue = currentDatablock.GetMax();
                    objectSlider.value = floatValue;
                    objectSlider.wholeNumbers = false;
                    objectSlider.onValueChanged.AddListener(delegate (float newVal)
                    {
                        float correctedVal = (float)Math.Round(newVal, 2);
                        currentDatablock.SetUnstoredValue(correctedVal);
                        // Discarding the decimal stuff with the wrapper
                        // TODO please devs add decimals to your wrapper.
                        bindingText.text = correctedVal.ToString("N2", CultureInfo.InvariantCulture);
                    });
                    objectSlider.GetComponentInChildren<TextMeshProUGUI>().text = floatValue.ToString("N2", CultureInfo.InvariantCulture);
                }
                else if (currentDatablock.GetValue() is double doubleValue)
                {
                    goToInstantiate = rangeButtonPrefab;
                    instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);

                    var wrapper = instObj.GetComponentInChildren<SliderWrapper>(true);
                    wrapper._visibleMode = SliderWrapper.VisibleMode.Default;
                    var bindingText = wrapper._valueText;
                    wrapper._valueText = null;

                    var objectSlider = instObj.GetComponentInChildren<Slider>();
                    objectSlider.minValue = currentDatablock.GetMin();
                    objectSlider.maxValue = currentDatablock.GetMax();
                    objectSlider.value = (float)doubleValue;
                    objectSlider.wholeNumbers = false;
                    objectSlider.onValueChanged.AddListener(delegate (float newVal)
                    {
                        float correctedVal = (float)Math.Round(newVal, 2);
                        currentDatablock.SetUnstoredValue(correctedVal);
                        bindingText.text = correctedVal.ToString("N2", CultureInfo.InvariantCulture);
                    });
                    objectSlider.GetComponentInChildren<TextMeshProUGUI>().text = doubleValue.ToString("N2", CultureInfo.InvariantCulture);
                }
                else if (currentDatablock.GetValue() is Color colore)// if (categoryVariables.Value is Color colorValue)
                {
                    goToInstantiate = colorButtonPrefab;
                    instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                    var objectButton = instObj.GetComponentInChildren<Button>();
                    objectButton.transform.Find("ColorPreview").GetComponent<Image>().color = colore;
                    objectButton.onClick.AddListener(() =>
                    {
                        UI.Chain<ColorPickerController>().Show();
                        var currentColor = objectButton.transform.Find("ColorPreview").GetComponent<Image>().color;
                        UI.Get<ColorPickerController>().ConfigureButtons(currentColor, delegate (Color selectedColor)
                        {
                            currentDatablock.SetUnstoredValue(selectedColor);
                            objectButton.transform.Find("ColorPreview").GetComponent<Image>().color = selectedColor;
                        });
                    });
                }
                else if (ColorUtility.TryParseHtmlString(currentDatablock.Value.ToString().Replace("\"", string.Empty), out Color colorValue))// if (categoryVariables.Value is Color colorValue)
                {
                    goToInstantiate = colorButtonPrefab;
                    instObj = GameObject.Instantiate(goToInstantiate, thisContentRoot);
                    var objectButton = instObj.GetComponentInChildren<Button>();
                    objectButton.transform.Find("ColorPreview").GetComponent<Image>().color = colorValue;
                    objectButton.onClick.AddListener(() =>
                    {
                        UI.Chain<ColorPickerController>().Show();
                        var currentColor = objectButton.transform.Find("ColorPreview").GetComponent<Image>().color;
                        UI.Get<ColorPickerController>().ConfigureButtons(currentColor, delegate (Color selectedColor)
                        {
                            currentDatablock.SetUnstoredValue($"\"#{ColorUtility.ToHtmlStringRGB(selectedColor)}\"");
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
                instObj.GetComponentInChildren<GenericHoverTooltip>(true)?.Initialize(currentDatablock.GetTooltip());

                // Label for each object
                var label = currentDatablock.GetLabel();
                instObj.GetComponentInChildren<TextMeshProUGUI>().text = !string.IsNullOrEmpty(label) ? label : currentDatablock.Key;
                instObj.SetActive(true);
            }


            /*var keyBindManager = GameObject.Instantiate(keyBindPrefab, thisContentRoot).GetComponent<GameKeySetupPanel>();
            if (keyBindManager != null)
            {
                var kTransform = keyBindManager._label.transform;
                kTransform.transform.position = kTransform.position - Vector3.left * 7f;
                keyBindManager._label.text = "Custom Test Label";
                keyBindManager._label.fontStyle = FontStyles.UpperCase & FontStyles.Normal;
                var tooltip = keyBindManager._label.gameObject.AddComponent<GenericHoverTooltip>();
                tooltip.Initialize("Testing the tooltip.");
                // The record here will be from ini
                // You can init everything with none?
                // you could default some key as keybind one
                // and then that's pretty much it.
                LocalizationHelper.AddKeyToAllDictionaries($"gamekey.Test_{modData.ModName}.desc", "TEST KEY!");
                GameKeyRecord record = new GameKeyRecord
                {
                    Id = $"Test_{modData.ModName}",
                    ContentDescriptor = null,
                    Layout = "",
                    OtherKeyIdToPress = "",
                    Bind1 = new List<KeyCode>()
                    {
                        KeyCode.Backslash
                    },
                    Bind2 = new List<KeyCode>()
                    {
                        KeyCode.None
                    },
                    ControllerBind1 = new List<ControllerAction>()
                    {
                        ControllerAction.None
                    },
                    ControllerBind2 = new List<ControllerAction>()
                    {
                        ControllerAction.None
                    },
                    ImmutableBind1 = false,
                    ImmutableBind2 = false,
                    ImmutableControllerBind1 = false,
                    ImmutableControllerBind2 = false,
                    AxisName = "",
                    MovementVector = default,
                    ExclusiveInputMode = new List<InputMode>()
                    {
                        InputMode.KeyboardAndMouse,
                        InputMode.KeyboardOnly
                    },
                    ForbiddenKeysToBind = new List<KeyCode>() { KeyCode.None }
                };

                // In-game key should be the amount of records there are right now
                int num = Data.Keybinding.Count;

                List<string> list = new List<string>();

                GameKey gameKey = new GameKey(record, num++);


                if (!list.Contains(record.Layout))
                {
                    list.Add(record.Layout);
                }

                var inputController = SingletonMonoBehaviour<InputController>.Instance;
                inputController._keys.Add(gameKey);
                inputController._idsToKeys.Add(record.Id, gameKey);

                keyBindManager.OnSlotClicked += delegate(GameKeySetupPanel panel, int arg2)
                {
                    UI.Chain<BindGameKeyWindow>().Invoke(delegate(BindGameKeyWindow v)
                    {
                        v.Configure(panel.GameKey);

                    }).Show();
                };

                keyBindManager.OnDeleteBind += delegate(GameKeySetupPanel arg1, int arg2)
                {
                    if (arg2 == 0)
                    {
                        if (!InputHelper.IsNotSet(arg1.GameKey.Bind1))
                        {
                            arg1.GameKey.Bind1.Clear();
                            arg1.GameKey.Bind1.Add(KeyCode.None);
                            keyBindManager.Initialize(arg1.GameKey);
                        }
                    }
                    else if (!InputHelper.IsNotSet(arg1.GameKey.Bind2))
                    {
                        arg1.GameKey.Bind2.Clear();
                        arg1.GameKey.Bind2.Add(KeyCode.None);
                        keyBindManager.Initialize(arg1.GameKey);
                    }

                    arg1.GameKey.Save();
                    SingletonMonoBehaviour<InputController>.Instance.RaiseKeyChange(arg1.GameKey.Record.Id);
                    PlayerPrefs.Save();
                };
                keyBindManager.Initialize(gameKey);

                gameKey.Load();
                var _player = Rewired.ReInput.players.GetPlayer(0);

                foreach (var item in list)
                {
                    // Check if there's an existing one.
                    if (inputController._keymaps.ContainsKey(item))
                    {
                        inputController._keymaps.TryGetValue(item, out Keymap modifiableKeymap);
                        modifiableKeymap?._keys.Add(gameKey);
                        modifiableKeymap?.Recalculate(inputController.Mode);
                    }
                    else
                    {
                        Keymap keymap = new Keymap(_player, item, inputController._keys);
                        inputController._keymaps.Add(item, keymap);
                        keymap.Recalculate(inputController.Mode);
                    }
                }

                keyBindManager.gameObject.SetActive(true);
            }
            */

            return rootGameObject;
        }
    }
}