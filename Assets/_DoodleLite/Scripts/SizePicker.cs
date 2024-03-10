using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands;

public class SizePicker : HandUIElement
{
    private float selectedSize = 0.6f;
    public float SelectedSize 
    { 
        get
        {
            return selectedSize;
        }
    }

    public Transform sizeIndicator;
    
    private const float leftPosition = -200f;
    private const float rightPosition = 200f;
    
    public static SizePicker Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        HandGestureHandler.Instance.OnLeftPinch += UpdateSizePicker;
    }

    void UpdateSizePicker(Vector3 pinchPosition)
    {
        if (!HandUIManager.Instance.IsActive || !IsActive || !IsInteractable) return;

        Vector3 localPinchPosition = transform.InverseTransformPoint(pinchPosition);
        float horizontalPosition = Mathf.Clamp(localPinchPosition.x * 0.0005f, -0.1f, 0.1f);
        
        float sizeValue = Mathf.Lerp(.2f, 1f, Mathf.InverseLerp(-0.1f, 0.1f, horizontalPosition));

        selectedSize = sizeValue;

        float normalizedValue = (sizeValue - 0.2f) / (1f - 0.2f);
        float indicatorPosition = Mathf.Lerp(leftPosition, rightPosition, normalizedValue);
        Vector3 newPosition = new Vector3(indicatorPosition, sizeIndicator.localPosition.y, sizeIndicator.localPosition.z);

        sizeIndicator.localPosition = newPosition;
        sizeIndicator.localScale = Vector3.one * selectedSize * 100f;

        MeshDrawing.Instance.SetBrushSize(selectedSize);
    }

    void OnDestroy()
    {
        HandGestureHandler.Instance.OnLeftPinch -= UpdateSizePicker;
    }
}
