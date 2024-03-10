using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands;

public class HistoryWheel : HandUIElement
{
    public Transform undoWheelUI;
    public Transform undoWheelIndicator;

    private int lastStepIndex = -1;
    private float lastAngle = 0f;
    private const int degreesPerStep = 30;
    private const int totalSteps = 360 / degreesPerStep;

    public static HistoryWheel Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        HandGestureHandler.Instance.OnLeftPinch += RotateUndoWheel;
    }

    // void Start()
    // {
    // }

    void RotateUndoWheel(Vector3 pinchPosition)
    {
        if (!HandUIManager.Instance.IsActive || !IsActive || !IsInteractable) return;

        Vector3 localPinchPosition = transform.InverseTransformPoint(pinchPosition) * 0.0005f;
        
        if(localPinchPosition.magnitude < 0.05f)   return;

        float angle = Mathf.Atan2(-localPinchPosition.x, localPinchPosition.y) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;

        int currentStepIndex = Mathf.FloorToInt(angle / degreesPerStep);
        float snappedAngle = currentStepIndex * degreesPerStep;
        
        if (lastStepIndex != currentStepIndex)
        {
            int stepChange = currentStepIndex - lastStepIndex;

            if (stepChange <= -1)
            {
                PerformRedoOperation();
            }
            
            if (stepChange >= 1)
            {
                PerformUndoOperation();
            }
            // For redo, you would check for counterclockwise movement

            lastStepIndex = currentStepIndex;
        }

        undoWheelIndicator.localRotation = Quaternion.Euler(0, 0, snappedAngle);
        lastAngle = angle;
    }

    private void PerformUndoOperation()
    {
        MeshDrawing.Instance.UndoLastDrawing();
    }

    private void PerformRedoOperation()
    {
        MeshDrawing.Instance.RedoLastDrawing();
    }

    void OnDestroy()
    {
        HandGestureHandler.Instance.OnLeftPinch -= RotateUndoWheel;
    }
}
