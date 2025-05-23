using MGSC;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModConfigMenu.Components
{
    [UIView(GameLoopGroup.MainMenu, false, false)]
    public class ChangeModConfirmationPanel : MonoBehaviour
    {
        private Button acceptButton;
        private Button cancelButton;
        private Button discardButton;

        private TextMeshProUGUI captionText;
        private TextMeshProUGUI titleText;
        private void Awake()
        {
            Transform panelTransform = transform.Find("Panel");
            Transform buttonTransform = panelTransform.Find("Buttons");

            acceptButton = buttonTransform.Find("AcceptButton").GetComponent<Button>();
            cancelButton = buttonTransform.Find("CancelButton").GetComponent<Button>();
            discardButton = buttonTransform.Find("DiscardButton").GetComponent<Button>();

            captionText = panelTransform.Find("Caption").gameObject.GetComponent<TextMeshProUGUI>();
            titleText = panelTransform.Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        }

        public void Configure(string title, string caption, Action acceptAction, Action discardAction, Action cancelAction)
        {
            titleText.text = title;
            captionText.text = caption;
            cancelButton.onClick.AddListener(delegate { cancelAction?.Invoke(); });
            acceptButton.onClick.AddListener(delegate { acceptAction?.Invoke(); });
            discardButton.onClick.AddListener(delegate { discardAction?.Invoke(); });
        }

        public void OnDisable()
        {
            cancelButton.onClick.RemoveAllListeners();
            acceptButton.onClick.RemoveAllListeners();
            discardButton.onClick.RemoveAllListeners();
        }
    }
}