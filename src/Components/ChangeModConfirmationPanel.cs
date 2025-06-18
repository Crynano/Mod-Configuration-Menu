using MGSC; 
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModConfigMenu.Components
{
    [UIView(GameLoopGroup.MainMenu, false, true)]
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

            captionText = panelTransform.Find("CaptionArea").Find("Caption").gameObject.GetComponent<TextMeshProUGUI>();
            titleText = panelTransform.Find("TitleArea").Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        }

        public void Configure(string title, string caption, Action acceptAction, Action discardAction, Action cancelAction)
        {
            titleText.text = title;
            captionText.text = caption;
            cancelButton.onClick.AddListener(delegate { cancelAction?.Invoke(); });
            acceptButton.onClick.AddListener(delegate { acceptAction?.Invoke(); });
            if (discardAction == null)
            {
                discardButton.gameObject.SetActive(false);
            }
            else
            {
                discardButton.onClick.AddListener(delegate { discardAction?.Invoke(); });
                discardButton.gameObject.SetActive(true);
            }
        }

        public void OnEnable()
        {
            cancelButton.onClick.AddListener(UI.Hide<ChangeModConfirmationPanel>);
            acceptButton.onClick.AddListener(UI.Hide<ChangeModConfirmationPanel>);
            discardButton.onClick.AddListener(UI.Hide<ChangeModConfirmationPanel>);
        }

        public void OnDisable()
        {
            cancelButton.onClick.RemoveAllListeners();
            acceptButton.onClick.RemoveAllListeners();
            discardButton.onClick.RemoveAllListeners();
        }
    }
}