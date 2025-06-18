using System;
using MGSC;
using ModConfigMenu.Components;
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

        private CustomTooltip _tooltip;

        public void Initialize(string tooltip, CustomTooltip customTooltip)
        {
            _tooltipText = tooltip;
            this._tooltip = customTooltip;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //SetSelectedVisual(value: true);
            if (!_createdTooltip && !string.IsNullOrEmpty(_tooltipText))
            {
                _createdTooltip = true;
                //SingletonMonoBehaviour<TooltipFactory>.Instance.ShowSimpleTextTooltip(_tooltipText);
                Vector3[] corners = new Vector3[4];
                ((RectTransform)this.transform).GetLocalCorners(corners);
                var offset = this.transform.TransformPoint(corners[0]);
                this._tooltip.Show(offset, _tooltipText);
                // Disable annoying shit. But not needed for thios
                SingletonMonoBehaviour<UI>.Instance._clickOnBackgroundHandler.gameObject.SetActive(false);
            }
            return;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //SetSelectedVisual(value: false);
            if (_createdTooltip)
            {
                _createdTooltip = false;
                //SingletonMonoBehaviour<TooltipFactory>.Instance.HideSimpleTextTooltip();
                //UI.Hide<CustomTooltip>();
                this._tooltip.Hide();
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