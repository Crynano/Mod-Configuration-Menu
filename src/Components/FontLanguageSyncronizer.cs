using MGSC;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ModConfigMenu.Components
{
    public class FontLanguageSyncronizer : MonoBehaviour
    {
        private TextMeshProUGUI textObject;
        void Awake()
        {
            textObject = GetComponent<TextMeshProUGUI>();
        }

        private void UpdateFont()
        {
            var forceLang = Singleton<Localization>.Instance.currentLang;
            var fontPreset = SingletonMonoBehaviour<LocalizationFontKeeper>.Instance.FontPresets
                .Where(x => x.AvaialableLangs.IndexOf(forceLang) != -1)
                .FirstOrDefault();

            if (fontPreset == null)
            {
                Debug.LogError($"Error: Not found font preset for language {forceLang}.");
                return;
            }

            var font = fontPreset.FontAsset;
            if (textObject.font != font)
            {
                textObject.font = font;
            }
        }

        void OnEnable()
        {
            UpdateFont();
        }
    }
}
