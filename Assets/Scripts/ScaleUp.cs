using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUp : MonoBehaviour
{
    // Public variables to access in the game engine
    public float scaleSpeed = 0.5f; // Speed of scaling
    public float minScale = 0.01f; // Minimum scale to prevent flipping
    public float maxScale = 0.5f; // Maximum scale

    //reference model controller script and declare type first
    public ModelsController modelController;

    private Vector3 scaleStep; // Step to change scale each frame

    // Start is called before the first frame update
    void Start()
    {
        // Initialize scaleStep based on scaleSpeed
        scaleStep = new Vector3(scaleSpeed, scaleSpeed, scaleSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // Determine the new scale based on user input
        Vector3 newScale = transform.localScale;


        // When you press the button "u" it should start scaling up
        if (Input.GetKey("u"))
        {
            newScale += scaleStep * Time.deltaTime;
        }

        // When you press the button "d" it should start scaling down
        if (Input.GetKey("d"))
        {
            newScale -= scaleStep * Time.deltaTime;
        }

        // Clamp the new scale to ensure it doesn't go below minScale or above maxScale
        newScale = new Vector3(
            Mathf.Clamp(newScale.x, minScale, maxScale),
            Mathf.Clamp(newScale.y, minScale, maxScale),
            Mathf.Clamp(newScale.z, minScale, maxScale)
        );

        // Apply the new scale to the transform
        transform.localScale = newScale;

        // Rotate the object when you press the button "r"
        if (Input.GetKey("r"))
        {
            // Rotate the object around the Y-axis
            transform.Rotate(Vector3.up, scaleSpeed * Time.deltaTime * 100); // Adjust rotation speed as needed
        }
    }
}
