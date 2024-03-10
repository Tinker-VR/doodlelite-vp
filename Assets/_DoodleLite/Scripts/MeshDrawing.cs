using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDrawing : MonoBehaviour
{
    [Header("Drawing Settings")]
    [SerializeField] private GameObject drawingPrefab;
    [SerializeField] [Range(0.001f, .02f)] private float lineWidth = 0.006f;
    [SerializeField] [Range(3, 20)] private int sides = 5;
    
    private List<GameObject> spawnedDrawings = new List<GameObject>();
    private List<GameObject> redoDrawings = new List<GameObject>();

    private GameObject currentDrawingObject;
    private Mesh currentMesh;
    private List<Vector3> points = new List<Vector3>();

    public static MeshDrawing Instance { get; private set; }

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (HandGestureHandler.Instance != null)
        {
            HandGestureHandler.Instance.OnRightPinchDown += StartDrawing;
            HandGestureHandler.Instance.OnRightPinch += AddPoint;
            HandGestureHandler.Instance.OnRightPinchRelease += EndDrawing;
        }
        else
        {
        }
    }

    private void OnDisable()
    {
        if (HandGestureHandler.Instance != null)
        {
            HandGestureHandler.Instance.OnRightPinchDown -= StartDrawing;
            HandGestureHandler.Instance.OnRightPinch -= AddPoint;
            HandGestureHandler.Instance.OnRightPinchRelease -= EndDrawing;
        }
    }

    public void StartDrawing(Vector3 startPosition)
    {
        ClearRedoList();

        currentDrawingObject = Instantiate(drawingPrefab, Vector3.zero, Quaternion.identity);
        currentMesh = new Mesh();
        currentDrawingObject.GetComponent<MeshFilter>().mesh = currentMesh;

        if(ColorPicker.Instance)
        {
            SetBrushColor(ColorPicker.Instance.SelectedColor);
        }

        points.Clear();
        points.Add(startPosition);
    }
    
    public void SetBrushColor(Color color)
    {
        if (currentDrawingObject != null)
        {
            MeshRenderer renderer = currentDrawingObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }
    
    public void SetBrushSize(float width)
    {
        lineWidth = Mathf.Lerp (0.001f, 0.02f, Mathf.InverseLerp (0.2f, 1f, width));
    }

    [Header("Optimization Settings")]
    [SerializeField] private int updateThreshold = 5; // Number of points before updating the mesh
    private int pointsSinceLastUpdate = 0;

    public void AddPoint(Vector3 position)
    {
        if (points.Count > 0 && Vector3.Distance(points[points.Count - 1], position) < lineWidth / 2)
        {
            return;
        }

        points.Add(position);
        pointsSinceLastUpdate++;

        // Update mesh only when threshold is reached
        if (pointsSinceLastUpdate >= updateThreshold)
        {
            UpdateMesh();
            pointsSinceLastUpdate = 0; // Reset counter
        }
    }

    public void EndDrawing(Vector3 position)
    {
        if (pointsSinceLastUpdate > 0)
        {
            UpdateMesh(); // Ensure the final points are added
        }

        spawnedDrawings.Add(currentDrawingObject);

        currentMesh = null;
        currentDrawingObject = null;
        pointsSinceLastUpdate = 0; // Reset for the next drawing
    }

    public void EndDrawing()
    {
        if (pointsSinceLastUpdate > 0)
        {
            UpdateMesh(); // Ensure the final points are added
        }

        spawnedDrawings.Add(currentDrawingObject);

        currentMesh = null;
        currentDrawingObject = null;
        pointsSinceLastUpdate = 0; // Reset for the next drawing
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

        currentMesh.Optimize();
    }

    public void UndoLastDrawing()
    {
        if (spawnedDrawings.Count > 0)
        {
            GameObject lastDrawing = spawnedDrawings[spawnedDrawings.Count - 1];
            spawnedDrawings.RemoveAt(spawnedDrawings.Count - 1);

            lastDrawing.SetActive(false);
            redoDrawings.Add(lastDrawing);
        }
    }

    public void RedoLastDrawing()
    {
        if (redoDrawings.Count > 0)
        {
            GameObject lastUndoneDrawing = redoDrawings[redoDrawings.Count - 1];
            redoDrawings.RemoveAt(redoDrawings.Count - 1);

            lastUndoneDrawing.SetActive(true);
            spawnedDrawings.Add(lastUndoneDrawing);
        }
    }

    private void ClearRedoList()
    {
        // Optionally, destroy all GameObjects in the redo list
        foreach (GameObject drawing in redoDrawings)
        {
            Destroy(drawing);
        }
        redoDrawings.Clear();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // Press Z to undo
    {
        UndoLastDrawing();
    }
    if (Input.GetKeyDown(KeyCode.Y)) // Press Y to redo
    {
        RedoLastDrawing();
    }
    }
}