using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeController : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Update()
    {
        // Get the main camera's transform
        Transform cameraTransform = Camera.main.transform;

        // Read input for rotation around X (pitch) and Z (roll) axes
        float horizontalInput = Input.GetAxis("Horizontal"); // A and D keys
        float verticalInput = Input.GetAxis("Vertical"); // W and S keys

        // Calculate rotation around X (pitch) and Z (roll) axes based on input
        float pitchRotation = verticalInput * rotationSpeed * Time.deltaTime;
        float rollRotation = -horizontalInput * rotationSpeed * Time.deltaTime; // Invert roll rotation

        // Combine pitch and roll rotations into a single rotation vector
        Vector3 rotationVector = new Vector3(pitchRotation, 0f, rollRotation);

        // Apply the rotation to the object relative to the camera's forward direction
        transform.rotation = Quaternion.Euler(rotationVector) * cameraTransform.rotation;
    }
}
