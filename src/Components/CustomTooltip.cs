using MGSC;
using TMPro;
using UnityEngine;

namespace ModConfigMenu.Components
{
    public class CustomTooltip : MonoBehaviour
    {
        private LocalizableLabel label;

        private Vector2 _cachedPosition;

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
            RegisterComponents();
            label.ChangeLabel(text);
            _cachedPosition = position;
            transform.position = position;
            gameObject.SetActive(true);
        }

        void LateUpdate()
        {
            transform.position = LimitLabelPositionToScreen(_cachedPosition, ((RectTransform)transform).rect);
        }

        private static Vector3 LimitLabelPositionToScreen(Vector2 position, Rect rect)
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