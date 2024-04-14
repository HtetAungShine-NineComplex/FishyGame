using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float rotationSpeed = 10f; // You can adjust this value as needed

    void Update()
    {
        // Rotate the game object around the Z axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
