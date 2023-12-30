/**
 * @file CameraMovement.cs
 * @brief Defines a class for handling camera movement in Unity.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class CameraMovement
 * @brief Manages the movement and rotation of the camera.
 */
public class CameraMovement : MonoBehaviour
{
    /// The movement speed of the camera.
    public float movementSpeed = 10f;

    /**
     * @brief Updates the camera's position based on user input.
     */
    void Update()
    {
        // Handle movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);

        // Handle rotation on key press
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(new Vector3(0, 180, 0));
        }
    }
}