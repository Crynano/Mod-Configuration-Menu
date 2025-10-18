using MGSC;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ModConfigMenu.Components
{
    public class CustomTooltip : MonoBehaviour
    {
        //private TextMeshProUGUI textBox;
        private LocalizableLabel label;

        private Coroutine AdjustPositionCoroutine;

        void Awake()
        {
            RegisterComponents();
        }

        private void RegisterComponents()
        {
            if (label != null) return;
            var textBox = GetComponentInChildren<TextMeshProUGUI>(true);
            label = textBox.gameObject.AddComponent<LocalizableLabel>();
            label._textMeshPro = textBox;
            label._labelContext = TextContext.None;
            label._coloredFirstLetter = false;
            label._convertBrToNewLine = false;
            label._forceUpperCase = false;
        }

        public void Show(Vector2 position, string text)
        {
            //this.textBox.text = text;
            RegisterComponents();
            label.ChangeLabel(text);
            this.gameObject.SetActive(true);
            this.transform.position = LimitLabelPositionToScreen(position, ((RectTransform)transform).rect);
        }

        private Vector3 LimitLabelPositionToScreen(Vector2 position, Rect rect)
        {
            var goodHeight = rect.height * 4f;
            position.y = Mathf.Clamp(position.y, goodHeight, Screen.height);
            return position;
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}