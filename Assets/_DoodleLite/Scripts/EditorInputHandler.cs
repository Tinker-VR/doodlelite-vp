using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorInputHandler : MonoBehaviour
{
    public MeshDrawing liteDrawing;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)); // Adjust 10 to your needs
            liteDrawing.StartDrawing(mousePos);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)); // Adjust 10 to your needs
            liteDrawing.AddPoint(mousePos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            liteDrawing.EndDrawing();
        }
    }
}
