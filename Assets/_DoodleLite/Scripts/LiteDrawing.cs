using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiteDrawing : MonoBehaviour
{
    [Header("Drawing Settings")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private float lineWidth = 0.01f;
    private LineRenderer currentLineRenderer;
    
    [Header("Hand Settings")]
    [SerializeField] private HandGestureHandler handGestureHandler;

    private void OnEnable()
    {
        if (handGestureHandler != null)
        {
            handGestureHandler.OnRightPinchDown += StartDrawing;
            handGestureHandler.OnRightPinch += AddPoint;
            handGestureHandler.OnRightPinchRelease += EndDrawing;
            Debug.Log("LiteDrawing: Subscribed to HandGestureHandler events.");
        }
        else
        {
            Debug.LogError("LiteDrawing: HandGestureHandler reference is null.");
        }
    }

    private void OnDisable()
    {
        if (handGestureHandler != null)
        {
            handGestureHandler.OnRightPinchDown -= StartDrawing;
            handGestureHandler.OnRightPinch -= AddPoint;
            handGestureHandler.OnRightPinchRelease -= EndDrawing;
        }
    }

    private void Update()
    {
    }

    public void StartDrawing(Vector3 startPosition)
    {
        Debug.Log($"StartDrawing: Starting position: {startPosition}");
        GameObject lineObj = Instantiate(linePrefab, startPosition, Quaternion.identity);
        currentLineRenderer = lineObj.GetComponent<LineRenderer>();

        if (currentLineRenderer == null)
        {
            Debug.LogError("StartDrawing: Failed to get LineRenderer component.");
            return;
        }

        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.positionCount = 1;
        currentLineRenderer.SetPosition(0, startPosition);
        Debug.Log("StartDrawing: Line initialized.");
    }

    public void AddPoint(Vector3 position)
    {
        if (currentLineRenderer == null)
        {
            Debug.LogError("AddPoint: No current LineRenderer.");
            return;
        }

        if (currentLineRenderer == null || Vector3.Distance(currentLineRenderer.GetPosition(currentLineRenderer.positionCount - 1), position) < 0.001f) return;

        Debug.Log($"AddPoint: Adding point. Position: {position}");
        
        currentLineRenderer.positionCount++;
        currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, position);
    }

    public void EndDrawing(Vector3 position)
    {
        Debug.Log("EndDrawing: Drawing ended.");
        currentLineRenderer = null;
    }

    public void EndDrawing()
    {
        currentLineRenderer = null;
    }
}
