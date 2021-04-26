using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotationSpeed;
    public float zoomSpeed;

    private Vector2 clickedPosition;
    
    // 0 = None, 1 = Horizontal, 2 = Vertical
    private int direction = 0;

    // Update is called once per frame
    void Update()
    {
        // Get anchor on screen to get speeds from
        if ( Input.GetMouseButtonDown(0) )
        {
            clickedPosition = Input.mousePosition;
        }

        // Get anchor on screen to get speeds from
        if (Input.GetMouseButtonUp(0))
        {
            direction = 0;
        }

        // Only rotate when mouse is pressed
        if (Input.GetMouseButton(0))
        {
            // Check if moving horizontal or vertically based on difference between current mouse position and anchored position
            if ( direction == 0)
            {
                if (Mathf.Abs(clickedPosition.x - Input.mousePosition.x) > Mathf.Abs(clickedPosition.y - Input.mousePosition.y))
                {
                    direction = 1;
                } 
                
                if (Mathf.Abs(clickedPosition.x - Input.mousePosition.x) < Mathf.Abs(clickedPosition.y - Input.mousePosition.y))
                {
                    direction = 2;
                }
            }

            if (direction == 1)
            {
                transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), (clickedPosition.x - Input.mousePosition.x) * rotationSpeed * Time.deltaTime, 0.0f);
            } 
            
            if (direction == 2)
            {
                // Check that it is between bounds (-29 && +30 in the X plane)
                if (transform.rotation.eulerAngles.x >= 331.0f || transform.rotation.eulerAngles.x <= 30.1f)
                {
                    transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f), (clickedPosition.y - Input.mousePosition.y) * rotationSpeed * Time.deltaTime);
                }

                // If overextends, reset to nearest limit
                if ( transform.rotation.eulerAngles.x > 30.0f && transform.rotation.eulerAngles.x <= 60.0f)
                {
                    Quaternion q = transform.rotation;
                    q.eulerAngles = new Vector3(30.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                    transform.rotation = q;
                }

                if (transform.rotation.eulerAngles.x < 331.0f && transform.rotation.eulerAngles.x >= 301.0f)
                {
                    Quaternion q = transform.rotation;
                    q.eulerAngles = new Vector3(331.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                    transform.rotation = q;
                }
            }
        }

        /// Move closer or farther based on wheel scroll
        if (Input.mouseScrollDelta.y != 0)
        {
            Transform cameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
            if (Input.mouseScrollDelta.y > 0)
                cameraTransform.localPosition += Vector3.forward * zoomSpeed * Time.deltaTime;
            if (Input.mouseScrollDelta.y < 0)
                cameraTransform.localPosition -= Vector3.forward * zoomSpeed * Time.deltaTime;
        }
    }
}
