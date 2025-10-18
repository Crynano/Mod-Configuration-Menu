using MGSC;
using ModConfigMenu.Components;
using ModConfigMenu.Contracts;
using ModConfigMenu.Implementations;
using ModConfigMenu.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace ModConfigMenu
{
    [UIView(GameLoopGroup.MainMenu, false, true)]
    public class ModConfigMenu : MonoBehaviour
    {
        private const string DEFAULT_BUTTON_COLOR = "#FFFEC1";
        private const string QUASI_COLOR_STANDARD = "#1C3D2E";

        private readonly Color SELECTED_MOD_COLOR = new Color(0.288f, 0.6f, 0.31f, 1f);
        private readonly Color RANGE_BAR_COLOR = new Color(0.5059f, 0.7098f, 0.4784f, 1f);
        private Color QuasiStandardColor;

        public CustomTooltip _customTooltip;

        private GameObject ModButtonPrefab;

        private Transform ModListRoot;
        private Transform ConfigAreaRoot;
        private Transform PrefabsRoot;
        private Transform ContentRoot;

        private GameObject boolButtonPrefab;
        private GameObject rangeButtonPrefab;
        private GameObject colorButtonPrefab;
        private GameObject dropdownPrefab;
        private GameObject clickableButtonPrefab;

        private GameObject stringPrefab;

        private GameObject headerPrefab;
        private GameObject rootPrefab;

        private CommonButton _backButton;
        private Button _saveButton;
        private Button _resetDefaultButton;

        private ModConfig lastActiveMod = null;
        private Transform lastActiveModRoot = null;

        private Dictionary<string, Transform> ModsRoot = new Dictionary<string, Transform>();

        public void Awake()
        {
            // Gathering the gameSettings to get prefabs.
            var gameSettingsScreen = FindObjectOfType<GameSettingsScreen>(true);

            // Let's find a generic button to modify.
            //ModButtonPrefab = gameSettingsScreen.transform.Find("Window").Find("Buttons").Find("BtnGeneral").gameObject;

            ModListRoot = transform.Find("ModList").Find("ModsScroll").Find("Viewport").Find("Content");

            ConfigAreaRoot = transform.Find("ConfigArea");
            PrefabsRoot = ConfigAreaRoot.Find("Prefabs");
            ContentRoot = ConfigAreaRoot.Find("ContentRoot");

            ConfigureModButtonPrefab();
            ConfigureBoolButtonPrefab();
            ConfigureRangeButtonPrefab();
            ConfigureColorButtonPrefab();
            ConfigureClickableButtonPrefab();
            ConfigureDropdownPrefab();
            ConfigureStringPrefab();

            rootPrefab = ConfigAreaRoot.Find("Prefabs").Find("Root").gameObject;
            rootPrefab.SetActive(false);
            headerPrefab = ConfigAreaRoot.Find("Prefabs").Find("Header").gameObject;
            ConfigureLabel(headerPrefab.transform.Find("Label").gameObject, false);
            headerPrefab.SetActive(false);

            // We have back button solved.
            var bbPrefab = gameSettingsScreen.transform.Find("BackButton");
            _backButton = GameObject.Instantiate(bbPrefab, transform).GetComponent<CommonButton>();
            if (_backButton != null)
                _backButton.OnClick += delegate { UI.Back(); };

            _saveButton = ConfigAreaRoot.Find("SaveButton")?.GetComponent<Button>();
            _saveButton?.onClick.AddListener(SaveCurrentMod);

            _resetDefaultButton = ConfigAreaRoot.Find("DefaultButton")?.GetComponent<Button>();
            _resetDefaultButton?.onClick.AddListener(ResetCurrentMod);

            // Load custom tooltip from Assetbundle and instantiate.
            var tooltipToInstantiate =
                Importer.LoadFileFromMemory<GameObject>("ModConfigMenu.Resources.mcmassets", "CustomTooltipMessage");

            if (tooltipToInstantiate != null)
            {
                var instObject = Instantiate(tooltipToInstantiate,
                    SingletonMonoBehaviour<TooltipFactory>.Instance.transform);
                _customTooltip = instObject.AddComponent<CustomTooltip>();
                _customTooltip.name = $"Crynano's " + nameof(CustomTooltip);
                _customTooltip.gameObject.SetActive(false);
            }

            ColorUtility.TryParseHtmlString(QUASI_COLOR_STANDARD, out QuasiStandardColor);
        }

        public void Start()
        {
            Stopwatch crono = new Stopwatch();
            crono.Start();
            CreateButtonsForEveryMod();
            crono.Stop();
            Debug.Log($"MCM Load Time: {crono.ElapsedMilliseconds}ms");
        }

        private void CreateButtonsForEveryMod()
        {
            var orderedModList = ModConfigManager.ModsList.OrderBy(x => x).ToList();
            foreach (var modName in orderedModList)
            {
                GameObject modButton = null;
                try
                {
                    modButton = GameObject.Instantiate(ModButtonPrefab, ModListRoot);
                    modButton.name = $"[{modName.Replace(" ", string.Empty)}]";

                    var objectButton = modButton.GetComponent<Toggle>();
                    //objectButton.ChangeLabel(modName);
                    objectButton.GetComponentInChildren<TextMeshProUGUI>().text = modName.ColorFirstLetter(Colors.White);
                    objectButton.onValueChanged.AddListener((bool selected) =>
                    {
                        objectButton.transform.Find("Selected").gameObject.SetActive(selected);
                        SwitchToMod(modName);
                    });
                    modButton.SetActive(true);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Could not create UI for mod: {modName}" +
                        $"\n{ex.Message}" +
                        $"\n{ex.StackTrace}");

                    continue;
                }
            }
        }

        private void SwitchToMod(string modName)
        {
            ModConfig modConfig = ModConfigManager.GetModConfig(modName);
            ModsRoot.TryGetValue(modName, out Transform root);
            if (root != null)
            {
                CheckForChangesAndSwitchMod(modConfig, root);
            }
            else
            {
                CreateNewMod(modConfig);
            }
        }

        private void ConfigureModButtonPrefab()
        {
            // Instead of finding a generic button, we create our own.
            ModButtonPrefab = PrefabsRoot.Find("Mod").gameObject;
            ModButtonPrefab.SetActive(false);
        }

        private void ConfigureBoolButtonPrefab()
        {
            boolButtonPrefab = PrefabsRoot.Find("BoolConfig").gameObject;
            var boolToggle = boolButtonPrefab.transform.Find("Toggle").gameObject;
            boolToggle.AddComponent<OnClickSfx>();
            boolToggle.AddComponent<ToggleWrapper>();
            ConfigureLabel(boolButtonPrefab.transform.Find("Label").gameObject);
            boolButtonPrefab.SetActive(false);
        }

        // Some way of instantiating the boolButton and start its components?
        private void ConfigureColorButtonPrefab()
        {
            colorButtonPrefab = PrefabsRoot.Find("ColourConfig").gameObject;
            ConfigureLabel(colorButtonPrefab.transform.Find("Label").gameObject);
            colorButtonPrefab.SetActive(false);
        }

        private void ConfigureRangeButtonPrefab()
        {
            rangeButtonPrefab = PrefabsRoot.Find("RangeConfig").gameObject;
            var sliderObject = rangeButtonPrefab.transform.Find("Slider").Find("RangeComponent").gameObject;
            var sliderComponent = sliderObject.GetComponent<Slider>();

            SliderWrapper mgscSliderComponent = sliderObject.AddComponent<SliderWrapper>();
            mgscSliderComponent._visibleMode = SliderWrapper.VisibleMode.WholeNumbers;
            mgscSliderComponent._slider = sliderComponent;
            mgscSliderComponent._sliderFillBar = sliderObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
            mgscSliderComponent._valueText = sliderObject.transform.parent.Find("SliderValue").Find("Value").GetComponent<TextMeshProUGUI>();

            mgscSliderComponent._barColor = RANGE_BAR_COLOR;
            mgscSliderComponent.Awake();

            // Adding a wrapper to unselect automatically when the user cancels.
            var manualTextComponent = rangeButtonPrefab.GetComponentInChildren<TMP_InputField>(true);
            manualTextComponent.gameObject.AddComponent<InputTextWrapper>();

            ConfigureLabel(rangeButtonPrefab.transform.Find("Label").gameObject);
            rangeButtonPrefab.SetActive(false);
        }

        private void ConfigureClickableButtonPrefab()
        {
            clickableButtonPrefab = PrefabsRoot.Find("ClickableButton")?.gameObject;
        }

        private void ConfigureDropdownPrefab()
        {
            dropdownPrefab = PrefabsRoot.Find("Dropdown").gameObject;
            var labelGO = dropdownPrefab?.transform.Find("Label").gameObject;
            ConfigureLabel(labelGO);
            dropdownPrefab?.SetActive(false);
        }

        private void ConfigureStringPrefab()
        {
            stringPrefab = PrefabsRoot.Find("String").gameObject;
            var labelGO = stringPrefab?.transform.Find("Label").gameObject;
            ConfigureLabel(labelGO, false);
            stringPrefab?.SetActive(false);
        }

        private void ConfigureLabel(GameObject go, bool hoverable = true)
        {
            if (hoverable)
                go.AddComponent<GenericHoverTooltip>();

            ConfigureLocalizableLabel(go.AddComponent<LocalizableLabel>());
        }

        private void ConfigureLocalizableLabel(LocalizableLabel label)
        {
            label._coloredFirstLetter = false;
            label._convertBrToNewLine = false;
            label._firstLetterUpperCase = false;
            label._forceUpperCase = true;
            label._labelContext = TextContext.None;
        }

        private void CheckForChangesAndSwitchMod(ModConfig newMod, Transform newModRoot)
        {
            if (lastActiveModRoot != null && newModRoot.gameObject == lastActiveModRoot.gameObject) return;
            if (lastActiveMod != null && lastActiveMod.IsDirty)
            {
                // Popup
                ColorUtility.TryParseHtmlString(DEFAULT_BUTTON_COLOR, out Color letterColor);
                UI.Chain<ChangeModConfirmationPanel>().Show();
                SingletonMonoBehaviour<UI>.Instance._clickOnBackgroundHandler.gameObject.SetActive(false);
                // TODO Add Localization here.
                UI.Get<ChangeModConfirmationPanel>().Configure(
                    "Unsaved Changes".ColorFirstLetter(letterColor),
                    "You still have unsaved changes.\nDo you want to save them before leaving this screen?",
                    () => { SaveCurrentMod(); ChangeMod(newMod, newModRoot); },
                    () => { DiscardChanges(); ChangeMod(newMod, newModRoot); },
                    null
                );
            }
            else
            {
                ChangeMod(newMod, newModRoot);
            }
        }

        private void ChangeMod(ModConfig newMod, Transform newModRoot)
        {
            if (lastActiveMod != null)
            {
                lastActiveMod.OnConfigChanged -= EnableSaveButton;
            }

            lastActiveMod = newMod;
            lastActiveMod.OnConfigChanged += EnableSaveButton;
            lastActiveModRoot?.gameObject.SetActive(false);
            lastActiveModRoot = newModRoot;
            lastActiveModRoot.gameObject.SetActive(true);
        }

        private void DiscardChanges()
        {
            DiscardCurrentModChanges();
            lastActiveModRoot?.gameObject.SetActive(false);
            ModsRoot.Remove(lastActiveMod.ModName);
            Destroy(lastActiveModRoot?.gameObject);
        }

        private void CreateNewMod(ModConfig modConfig)
        {
            var newRoot = BuildModConfig(modConfig);
            ModsRoot.Add(modConfig.ModName, newRoot);
            CheckForChangesAndSwitchMod(modConfig, newRoot);
        }

        private void ReloadModRoot(bool resetDefaultValues = false)
        {
            DiscardCurrentModChanges();
            string currentModName = lastActiveMod.ModName;
            lastActiveModRoot?.gameObject.SetActive(false);
            ModsRoot.Remove(lastActiveMod.ModName);
            Destroy(lastActiveModRoot?.gameObject);
            ModConfig modConfig = ModConfigManager.GetModConfig(currentModName);
            if (resetDefaultValues)
                modConfig.ResetAllToDefault();
            CreateNewMod(modConfig);
        }

        private void ResetCurrentMod()
        {
            if (lastActiveMod == null) return;

            ColorUtility.TryParseHtmlString("#FFFEC1", out Color letterColor);
            UI.Chain<ChangeModConfirmationPanel>().Show();
            SingletonMonoBehaviour<UI>.Instance._clickOnBackgroundHandler.gameObject.SetActive(false);
            UI.Get<ChangeModConfirmationPanel>().Configure(
                "Reset all to default.".ColorFirstLetter(letterColor),
                "Are you sure you want to reset all values to default?",
                () =>
                {
                    ReloadModRoot(true);
                    SaveCurrentMod();
                },
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
            string errorMessage = string.Empty;
            if (lastActiveMod.Save(out errorMessage))
            {
                _saveButton?.gameObject.SetActive(false);
            }
            else
            {
                // Popup message.
                ColorUtility.TryParseHtmlString("#FFFEC1", out Color letterColor);
                UI.Chain<ChangeModConfirmationPanel>().Show();
                SingletonMonoBehaviour<UI>.Instance._clickOnBackgroundHandler.gameObject.SetActive(false);
                UI.Get<ChangeModConfirmationPanel>().Configure(
                    $"ERROR WHEN SAVING \"{lastActiveMod.ModName}\"".ColorFirstLetter(letterColor),
                    errorMessage,
                    () => { ReloadModRoot(false); },
                    null,
                    () => { ReloadModRoot(false); }
                );
            }
        }

        private void EnableSaveButton()
        {
            _saveButton?.gameObject.SetActive(true);
        }

        private void OnManualTextFocus(bool enable) //(string input, Slider objectSlider, TMP_InputField manualTextComponent)
        {
            SingletonMonoBehaviour<InputController>.Instance.enabled = enable;
            FindObjectOfType<MainMenuGameMode>().enabled = enable;
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
                Logger.LogError($"Mod data is empty and can't be built!");
                return null;
            }

            Transform rootGameObject = GameObject.Instantiate(rootPrefab, ContentRoot).transform;
            Transform thisContentRoot = rootGameObject.GetComponent<ScrollRect>().content;
            string currentHeader = string.Empty;

            // Perform grouping without ordering.
            var orderedModData = modData.GetData().GroupBy(x => x.Header).ToList().SelectMany(group => group);

            foreach (var currentDatablock in orderedModData)
            {
                var currentValue = currentDatablock.Value;
                // Header handling
                if (!string.Equals(currentHeader, currentDatablock.Header, StringComparison.Ordinal))
                {
                    currentHeader = currentDatablock.Header;
                    InstantiateHeader(currentHeader, thisContentRoot);
                }

                GameObject instObj = null;
                bool skipLabel = false;

                // Forced type. For example dropdown.
                if (currentValue is bool boolValue)
                {
                    instObj = GameObject.Instantiate(boolButtonPrefab, thisContentRoot);
                    var toggle = instObj.GetComponentInChildren<Toggle>();
                    toggle.isOn = boolValue;
                    toggle.onValueChanged.AddListener(
                        delegate (bool a) { currentDatablock.SetUnstoredValue(a); });
                }
                else if (currentDatablock is DropdownConfig dropdownConfig)
                {
                    instObj = GameObject.Instantiate(dropdownPrefab, thisContentRoot);
                    var dropdown = instObj.GetComponentInChildren<TMP_Dropdown>(true);

                    // Get options as objects but cast to int for display.
                    var dropdownOptions = currentDatablock.GetDropdownOptions();
                    if (dropdownOptions.Count <= 0)
                    {
                        Logger.LogError($"No valid dropdown options available one of the dropdowns in: {modData.ModName}");
                        continue;
                    }

                    dropdown.AddOptions(dropdownOptions.Select(x => x.ToString()).ToList());

                    var startingOptionIndex = dropdownOptions.FindIndex(x => x.Equals(dropdownConfig.Value));

                    if (startingOptionIndex >= 0)
                    {
                        dropdown.SetValueWithoutNotify(startingOptionIndex);
                    }
                    else
                    {
                        Logger.LogWarning($"Starting option for dropdown: {dropdownConfig.GetLabel()}, Value: \"{dropdownConfig.Value}\" could not be found in list. Trying the default value.");

                        var defaultValueIndex = dropdownOptions.FindIndex(x => x.Equals(dropdownConfig.GetDefault()));

                        if (defaultValueIndex < 0)
                        {
                            Logger.LogWarning($"Default option for dropdown: {dropdownConfig.GetLabel()}, Value: \"{dropdownConfig.GetDefault()}\" could not be found in list. Defaulting to first indexable value.");
                            defaultValueIndex = 0;
                        }
                        dropdown.SetValueWithoutNotify(defaultValueIndex);
                    }

                    dropdown.onValueChanged.AddListener(delegate (int newIndex)
                    {
                        var dropdownOption = dropdownOptions[newIndex];
                        currentDatablock.SetUnstoredValue(dropdownOption);
                    });
                }
                else if (currentValue is int intValue)
                {
                    // integer slider + manual input
                    instObj = CreateRangeControl(
                        currentDatablock,
                        initialValue: intValue,
                        wholeNumbers: true,
                        format: "F0",
                        thisContentRoot,
                        setUnstoredFloat: f => currentDatablock.SetUnstoredValue((int)Mathf.Round(f)));
                }
                else if (currentValue is float floatValue)
                {
                    instObj = CreateRangeControl(
                        currentDatablock,
                        initialValue: floatValue,
                        wholeNumbers: false,
                        format: "N2",
                        thisContentRoot,
                        setUnstoredFloat: f => currentDatablock.SetUnstoredValue((float)Math.Round(f, 2)));
                }
                else if (currentValue is double doubleValue)
                {
                    instObj = CreateRangeControl(
                        currentDatablock,
                        initialValue: (float)doubleValue,
                        wholeNumbers: false,
                        format: "N2",
                        thisContentRoot,
                        setUnstoredFloat: f => currentDatablock.SetUnstoredValue((double)f));
                }
                else if (currentValue is Color colore) // if (categoryVariables.Value is Color colorValue)
                {
                    instObj = GameObject.Instantiate(colorButtonPrefab, thisContentRoot);
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
                else if (ColorUtility.TryParseHtmlString(currentDatablock.Value.ToString().Replace("\"", string.Empty),
                             out Color colorValue)) // if (categoryVariables.Value is Color colorValue)
                {
                    instObj = GameObject.Instantiate(colorButtonPrefab, thisContentRoot);
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
                else if (currentValue is string currentString)
                {
                    // Accept strings and only do, string showcase.
                    skipLabel = true;
                    instObj = GameObject.Instantiate(stringPrefab, thisContentRoot);
                    var customLabel = currentString.Trim('"');
                    instObj.GetComponentInChildren<LocalizableLabel>()
                        .ChangeLabel(!string.IsNullOrEmpty(customLabel) ? customLabel : currentDatablock.Key);
                }
                else if (currentDatablock is StringConfig strConfig)
                {
                    skipLabel = true;
                    instObj = GameObject.Instantiate(stringPrefab, thisContentRoot);
                    var customLabel = strConfig.Value.ToString().Trim('"');
                    instObj.GetComponentInChildren<LocalizableLabel>()
                        .ChangeLabel(!string.IsNullOrEmpty(customLabel) ? customLabel : currentDatablock.Key);
                }
                else
                {
                    Logger.LogError(
                        $"Could not create UI. Value \"{currentValue}\" with Type \"{currentValue.GetType()}\" might not be supported, or an error has occurred.");
                }

                if (instObj == null) continue;

                // Visual tooltip to aid in property description
                instObj.GetComponentInChildren<GenericHoverTooltip>(true)
                    ?.Initialize(currentDatablock.GetTooltip(), _customTooltip);

                // Label for each object
                var label = currentDatablock.GetLabel();
                //instObj.GetComponentInChildren<TextMeshProUGUI>().text = !string.IsNullOrEmpty(label) ? label : currentDatablock.Key;
                if (!skipLabel && !string.IsNullOrEmpty(label))
                    instObj.GetComponentInChildren<LocalizableLabel>()
                        .ChangeLabel(!string.IsNullOrEmpty(label) ? label : currentDatablock.Key);
                instObj.SetActive(true);
            }

            return rootGameObject;
        }


        // Helper: creates range control (works for int/float/double)
        private GameObject CreateRangeControl(IConfigValue config, float initialValue, bool wholeNumbers, string format, Transform root, Action<float> setUnstoredFloat)
        {
            var inst = InstantiatePrefab(rangeButtonPrefab, root);
            if (inst == null) return null;

            var wrapper = inst.GetComponentInChildren<SliderWrapper>(true);
            if (wrapper != null)
            {
                // For numeric sliders we usually don't want the wrapper to bind the value text directly.
                wrapper._visibleMode = SliderWrapper.VisibleMode.Default;
                wrapper._valueText = null;
            }

            var manualText = inst.GetComponentInChildren<TMP_InputField>(true);
            var slider = inst.GetComponentInChildren<Slider>();
            slider.minValue = config.GetMin();
            slider.maxValue = config.GetMax();
            slider.wholeNumbers = wholeNumbers;
            slider.value = Mathf.Clamp(initialValue, config.GetMin(), config.GetMax());

            // Slider -> config + manual text
            slider.onValueChanged.AddListener((newVal) =>
            {
                float rounded = wholeNumbers ? Mathf.Round(newVal) : (float)Math.Round(newVal, 2);
                setUnstoredFloat(rounded);
                manualText.text = wholeNumbers ? rounded.ToString("F0", CultureInfo.InvariantCulture) : rounded.ToString(format, CultureInfo.InvariantCulture);
            });

            // Manual input -> slider + config
            manualText.text = wholeNumbers ? ((int)initialValue).ToString(CultureInfo.InvariantCulture) : initialValue.ToString(format, CultureInfo.InvariantCulture);
            manualText.onEndEdit.AddListener((s) =>
            {
                if (string.IsNullOrEmpty(s)) s = config.GetMin().ToString(CultureInfo.InvariantCulture);
                if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed)) return;
                float limited = Mathf.Clamp(parsed, config.GetMin(), config.GetMax());
                if (wholeNumbers) limited = Mathf.Round(limited);
                manualText.text = wholeNumbers ? ((int)limited).ToString(CultureInfo.InvariantCulture) : limited.ToString(format, CultureInfo.InvariantCulture);
                slider.value = limited;
                setUnstoredFloat(limited);
                OnManualTextFocus(true);
            });

            manualText.onSelect.AddListener((_) => OnManualTextFocus(false));

            return inst;
        }

        // Helper: header instantiation
        private void InstantiateHeader(string headerText, Transform parent)
        {
            if (string.IsNullOrEmpty(headerText)) return;
            var header = InstantiatePrefab(headerPrefab, parent);
            if (header == null) return;
            var lbl = header.GetComponentInChildren<LocalizableLabel>();
            if (lbl != null)
            {
                lbl._labelContext = TextContext.ButtonCaption;
                lbl.ChangeLabel(headerText);
            }
            header.SetActive(true);
        }

        // Helper: instantiate prefab safely
        private GameObject InstantiatePrefab(GameObject prefab, Transform parent)
        {
            if (prefab == null)
            {
                Logger.LogError("Prefab is null when trying to instantiate.");
                return null;
            }
            var o = GameObject.Instantiate(prefab, parent);
            return o;
        }

        // Helper: color button configuration
        private void ConfigureColorButton(GameObject instObj, Color initialColor, bool storeAsString, BaseConfig config)
        {
            var button = instObj.GetComponentInChildren<Button>();
            if (button == null) return;
            var preview = button.transform.Find("ColorPreview")?.GetComponent<Image>();
            if (preview != null) preview.color = initialColor;

            button.onClick.AddListener(() =>
            {
                UI.Chain<ColorPickerController>().Show();
                var currentColor = preview != null ? preview.color : initialColor;
                UI.Get<ColorPickerController>().ConfigureButtons(currentColor, (selectedColor) =>
                {
                    if (storeAsString)
                    {
                        config.SetUnstoredValue($"\"#{ColorUtility.ToHtmlStringRGB(selectedColor)}\"");
                    }
                    else
                    {
                        config.SetUnstoredValue(selectedColor);
                    }

                    if (preview != null) preview.color = selectedColor;
                });
            });
        }

        #region Helper Functions


        internal void SwitchModButtonLight(CommonButton lastSelectedModButton, CommonButton newSelectedButton)
        {
            if (lastSelectedModButton != null)
                lastSelectedModButton.background.color = Color.white;

            newSelectedButton.background.color = SELECTED_MOD_COLOR;
        }

        #endregion


        #region Unity Functions

        public void OnDisable()
        {
            if (lastActiveMod != null && lastActiveMod.IsDirty)
            {
                // Popup
                ColorUtility.TryParseHtmlString(DEFAULT_BUTTON_COLOR, out Color letterColor);
                UI.Chain<ChangeModConfirmationPanel>().Show();
                SingletonMonoBehaviour<UI>.Instance._clickOnBackgroundHandler.gameObject.SetActive(false);
                UI.Get<ChangeModConfirmationPanel>().Configure(
                    "Unsaved Changes".ColorFirstLetter(letterColor),
                    "You still have unsaved changes.\nDo you want to save them before leaving this screen?",
                       () => { SaveCurrentMod(); },
                    () => { DiscardChanges(); },
                    null
                );
            }
        }

        #endregion
    }
}