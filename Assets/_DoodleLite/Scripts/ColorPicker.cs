using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands;

public class ColorPicker : HandUIElement
{
    private Color selectedColor = new Color(1f, 1f, 1f, 1f);
    public Color SelectedColor 
    { 
        get
        {
            return selectedColor;
        }
    }

    public Image selectedColorIndicator;
    public Transform colorPickerUI;
    public Transform colorSelector;
    
    public static ColorPicker Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        HandGestureHandler.Instance.OnLeftPinch += RotateColorSelector;
    }

    void RotateColorSelector(Vector3 pinchPosition)
    {
        if (!HandUIManager.Instance.IsActive || !IsActive || !IsInteractable) return;

        Vector3 localPinchPosition = transform.InverseTransformPoint(pinchPosition) * 0.0005f;
        
        if(localPinchPosition.magnitude < 0.05f)   return;

        float angle = Mathf.Atan2(-localPinchPosition.x, localPinchPosition.y) * Mathf.Rad2Deg;
        
        if (angle < 0) angle += 360f;

        colorSelector.localRotation = Quaternion.Euler(0, 0, angle);
        SetSelectedColor(angle);
    }
    
    private void SetSelectedColor(float angle)
    {
        float normalizedAngle = angle / 360f;

        float hue = 1f - normalizedAngle;
        float saturation = 1f;
        float value = 1f;

        selectedColor = Color.HSVToRGB(hue, saturation, value);

        selectedColorIndicator.color = selectedColor;
        
        // MeshDrawing.Instance.SetBrushColor(selectedColor);

        // Store the selected color in a variable if needed elsewhere
        // this.selectedColor = selectedColor;
    }

    void OnDestroy()
    {
        HandGestureHandler.Instance.OnLeftPinch -= RotateColorSelector;
    }
}
