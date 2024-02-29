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
            handGestureHandler.OnPinchDown += StartDrawing;
            handGestureHandler.OnPinch += AddPoint;
            handGestureHandler.OnPinchRelease += EndDrawing;
        }
    }

    private void OnDisable()
    {
        if (handGestureHandler != null)
        {
            handGestureHandler.OnPinchDown -= StartDrawing;
            handGestureHandler.OnPinch -= AddPoint;
            handGestureHandler.OnPinchRelease -= EndDrawing;
        }
    }

     private void Update()
    {
        // if (currentLineRenderer != null)
        // {
        //     AddPoint(handGestureHandler.transform.position);
        // }
    }

    public void StartDrawing(Vector3 startPosition)
    {
        GameObject lineObj = Instantiate(linePrefab, startPosition, Quaternion.identity);
        currentLineRenderer = lineObj.GetComponent<LineRenderer>();

        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.positionCount = 1;
        currentLineRenderer.SetPosition(0, startPosition);
    }

    public void AddPoint(Vector3 position)
    {
        if (currentLineRenderer == null || Vector3.Distance(currentLineRenderer.GetPosition(currentLineRenderer.positionCount - 1), position) < 0.001f) return;

        currentLineRenderer.positionCount++;
        currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, position);
    }

    public void EndDrawing(Vector3 position)
    {
        currentLineRenderer = null;
    }

    public void EndDrawing()
    {
        currentLineRenderer = null;
    }
}
