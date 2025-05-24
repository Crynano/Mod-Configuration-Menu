using MGSC;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ModConfigMenu
{
    [UIView(GameLoopGroup.MainMenu, false, false)]
    public class ColorPickerController : MonoBehaviour
    {
        public Button CancelButton;
        public Button AcceptButton;

        private ColorPicker colorPicker;
        private ColorPreview colorPreview;

        public void Awake()
        {
            CancelButton = transform.Find("BottomButtons").Find("Cancel").GetComponent<Button>();
            AcceptButton = transform.Find("BottomButtons").Find("Accept").GetComponent<Button>();

            colorPicker = transform.Find("ColorPicker").gameObject.AddComponent<ColorPicker>();
            colorPreview = transform.Find("Preview").gameObject.AddComponent<ColorPreview>();

            CancelButton.onClick.AddListener(UI.Hide<ColorPickerController>);
        }

        public void ConfigureButtons(Color currentColor, Action<Color> onSuccess)
        {
            colorPicker.color = currentColor;
            AcceptButton.onClick.RemoveAllListeners();
            AcceptButton.onClick.AddListener(() =>
            {
                onSuccess?.Invoke(colorPicker.color);
                UI.Hide<ColorPickerController>();
            });
        }
    }
}
