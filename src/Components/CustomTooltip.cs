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
        private TextMeshProUGUI textBox;

        private void Awake()
        {
            textBox = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        public void Show(Vector2 position, string text)
        {
            this.textBox.text = text;
            this.transform.position = position;
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}