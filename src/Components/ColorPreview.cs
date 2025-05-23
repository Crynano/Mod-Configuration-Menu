using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorPreview : MonoBehaviour
{
    public Graphic previewGraphic;

    public ColorPicker colorPicker;

    public void Awake()
    {
        previewGraphic = GetComponent<Image>();
        colorPicker = transform.parent.GetComponentInChildren<ColorPicker>();
    }

    public void Start()
    {
        previewGraphic.color = colorPicker.color;
        colorPicker.onColorChanged += OnColorChanged;
    }

    public void OnColorChanged(Color c)
    {
        previewGraphic.color = c;
    }

    private void OnDestroy()
    {
        if (colorPicker != null)
            colorPicker.onColorChanged -= OnColorChanged;
    }
}