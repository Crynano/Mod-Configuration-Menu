using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ModConfigMenu.Components
{
    public class CustomTooltip : MonoBehaviour
    {
        //private TextMeshProUGUI textBox;
        private LocalizableLabel label;

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
            this.transform.position = position;
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}