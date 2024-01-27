using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 2; // Speed of camera movement

    void Update()
    {

       
        if (Input.touchCount > 0) // If there are touches on the screen
        {
            HandleTouchInput();
        }
        else // If no touches, handle PC input
        {
            //HandlePCInput();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount < 2 ) // Single touch, drag camera
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                transform.Translate(-delta.x * dragSpeed * Time.deltaTime, -delta.y * dragSpeed * Time.deltaTime, 0);
            }
        }
        else if (Input.touchCount == 2) // Two touches, zoom camera (for example)
        {
            // Implement zoom logic here
        }
        // Add more conditions for handling different touch scenarios as needed
    }

    void HandlePCInput()
    {
        if (Input.GetMouseButton(0)) // Left mouse button, drag camera
        {
            Vector3 mouseDelta = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0);
            transform.Translate(mouseDelta * dragSpeed * Time.deltaTime);
        }
        else if (Input.GetMouseButton(1)) // Right mouse button, zoom camera (for example)
        {
            // Implement zoom logic here
        }
        // Add more conditions for handling different mouse button scenarios as needed
    }
}
