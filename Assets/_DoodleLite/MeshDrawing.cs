using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDrawing : MonoBehaviour
{
    [Header("Drawing Settings")]
    [SerializeField] private GameObject drawingPrefab;
    [SerializeField] private float lineWidth = 0.01f;
    [SerializeField] [Range(3, 20)] private int sides = 5;
    
    [Header("Hand Settings")]
    [SerializeField] private HandGestureHandler handGestureHandler;
    
    private GameObject currentDrawingObject;
    private Mesh currentMesh;
    private List<Vector3> points = new List<Vector3>();
    
    private void Awake()
    {
    }

    private void OnEnable()
    {
        if (handGestureHandler != null)
        {
            handGestureHandler.OnPinchDown += StartDrawing;
            handGestureHandler.OnPinch += AddPoint;
            handGestureHandler.OnPinchRelease += EndDrawing;
            Debug.Log("MeshDrawing: Subscribed to HandGestureHandler events.");
        }
        else
        {
            Debug.LogError("MeshDrawing: HandGestureHandler reference is null.");
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

    public void StartDrawing(Vector3 startPosition)
    {
        currentDrawingObject = Instantiate(drawingPrefab, Vector3.zero, Quaternion.identity);
        currentMesh = new Mesh();
        currentDrawingObject.GetComponent<MeshFilter>().mesh = currentMesh;

        points.Clear();
        points.Add(startPosition);
        Debug.Log($"StartDrawing: Starting position: {startPosition}");
    }

    public void AddPoint(Vector3 position)
    {
        if (points.Count > 0 && Vector3.Distance(points[points.Count - 1], position) < lineWidth / 4)
        {
            Debug.Log("AddPoint: Ignored too close point.");
            return;
        }

        points.Add(position);

        Debug.Log($"AddPoint: Adding point. Position: {position}");
        UpdateMesh();
    }

    public void EndDrawing(Vector3 position)
    {
        currentMesh = null;
        Debug.Log("EndDrawing: Drawing ended.");
    }

    public void EndDrawing()
    {
        currentMesh = null;
        Debug.Log("EndDrawing: Drawing ended.");
    }

    private void UpdateMesh()
    {
        if (points.Count < 2) return;

        var meshVertices = new List<Vector3>();
        var meshTriangles = new List<int>();
        var meshNormals = new List<Vector3>();

        Vector3 lastForward = (points[1] - points[0]).normalized;
        Vector3 lastUp = Vector3.up;

        // Create vertices
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 forward = lastForward;
            if (i < points.Count - 1)
                forward = (points[i + 1] - points[i]).normalized;
            else if (i > 0)
                forward = (points[i] - points[i - 1]).normalized;

            Vector3 side = Vector3.Cross(lastUp, forward).normalized;
            Vector3 up = Vector3.Cross(forward, side).normalized;

            lastUp = up; // Update lastUp to the new up vector

            float angleStep = 360.0f / sides;
            Quaternion rotation = Quaternion.LookRotation(forward, up);

            for (int j = 0; j < sides; j++)
            {
                float angle = angleStep * j;
                Vector3 localPoint = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * lineWidth;
                Vector3 worldPoint = points[i] + rotation * localPoint;
                meshVertices.Add(worldPoint);
                meshNormals.Add(localPoint.normalized);
            }
            lastForward = forward; // Keep track of the last forward direction
        }

        // Create triangles
        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = 0; j < sides; j++)
            {
                int current = i * sides + j;
                int next = (j + 1) % sides + i * sides;
                int currentNext = current + sides;
                int nextNext = next + sides;

                meshTriangles.Add(current);
                meshTriangles.Add(next);
                meshTriangles.Add(nextNext);

                meshTriangles.Add(current);
                meshTriangles.Add(nextNext);
                meshTriangles.Add(currentNext);
            }
        }

        currentMesh.vertices = meshVertices.ToArray();
        currentMesh.triangles = meshTriangles.ToArray();
        currentMesh.normals = meshNormals.ToArray();
        currentMesh.RecalculateBounds();
        currentMesh.RecalculateNormals();
    }

}