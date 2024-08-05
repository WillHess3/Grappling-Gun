using UnityEngine;

public class CameraLook : MonoBehaviour {

    [SerializeField] private float _mouseSensitivity;

    private float _xRotation = 0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        // Adjust the x rotation (pitch) and clamp it to avoid flipping the camera
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        // Apply rotations
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }

}
