using System;
using MGSC;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ModConfigMenu
{
    public class GenericHoverTooltip : MonoBehaviour, IPointerEnterHandler,
        IPointerExitHandler
    {
        private string _tooltipText = string.Empty;

        private bool _createdTooltip;

        //public override void Select()
        //{
        //    _selectedBorder.gameObject.SetActive(value: true);
        //    OnPointerEnter(null);
        //}

        //public override void Diselect()
        //{
        //    _selectedBorder.gameObject.SetActive(value: false);
        //    OnPointerExit(null);
        //}

        //public override void EvaluateConfirm()
        //{
        //    OnPointerClick(null);
        //}

        //public void Refresh(bool canDisassembly, CorpseInspectWindow.ActiveCorpseScreenPage activeCorpseScreenPage)
        //{
        //    base.gameObject.SetActive(canDisassembly);
        //    _createdTooltip = false;
        //    SingletonMonoBehaviour<TooltipFactory>.Instance.HideSimpleTextTooltip();
        //    SetSelectedVisual(value: false);
        //}

        //public void OnPointerClick(PointerEventData eventData)
        //{
        //    SingletonMonoBehaviour<SoundController>.Instance.PlayUiSound(SingletonMonoBehaviour<SoundsStorage>.Instance
        //        .ButtonClick);
        //    this.OnClicked();
        //    _createdTooltip = false;
        //    SingletonMonoBehaviour<TooltipFactory>.Instance.HideSimpleTextTooltip();
        //}

        public void Initialize(string tooltip)
        {
            _tooltipText = tooltip;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //SetSelectedVisual(value: true);
            if (!_createdTooltip && !string.IsNullOrEmpty(_tooltipText))
            {
                _createdTooltip = true;
                SingletonMonoBehaviour<TooltipFactory>.Instance.ShowSimpleTextTooltip(_tooltipText);
                var tooltip = SingletonMonoBehaviour<TooltipFactory>.Instance._simpleTextTooltip;
                var toolText = tooltip.GetComponentInChildren<TextMeshProUGUI>(true);
                toolText.wordWrappingRatios = 12f;
                toolText.enableWordWrapping = true;
                AdjustPosition(tooltip.transform);
            }
            return;

            void AdjustPosition(Transform stuff)
            {
                var currentPos = stuff.position;
                Vector3 correctedPosition = new Vector3()
                {
                    x = Mathf.Clamp(currentPos.x, 0f, Screen.width),
                    y = Mathf.Clamp(currentPos.y, 0f, Screen.height),
                    z = currentPos.z
                };
                stuff.position = correctedPosition;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //SetSelectedVisual(value: false);
            if (_createdTooltip)
            {
                _createdTooltip = false;
                SingletonMonoBehaviour<TooltipFactory>.Instance.HideSimpleTextTooltip();
            }
        }

        //private void SetSelectedVisual(bool value)
        //{
        //    if (value)
        //    {
        //        _icon.sprite = _activeIconSprite;
        //        _background.sprite = _activeBgSprite;
        //    }
        //    else
        //    {
        //        _icon.sprite = _inactiveIconSprite;
        //        _background.sprite = _inactiveBgSprite;
        //    }
        //}

        public void OnDisable()
        {
            OnPointerExit(null);
        }
    }

}