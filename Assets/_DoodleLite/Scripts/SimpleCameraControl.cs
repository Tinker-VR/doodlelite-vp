using UnityEngine;

public class SimpleCameraControl : MonoBehaviour
{
    public float sensitivity = 100f;
    public Transform cameraTransform;

    private float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1)) // Right mouse button is held down
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        if (Input.GetMouseButtonUp(1)) // Right mouse button is released
        {
            Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
        }
    }
}
