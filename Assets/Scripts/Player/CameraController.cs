using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] public float sensX;
    [SerializeField] public float sensY;

    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;
    float mouseX;
    float mouseY;
    float multiplier = 0.01f;
    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MyInput();
        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void MyInput()
    {
        var lookVector = InputManager.GetLookAxis();

        yRotation += lookVector.x * sensX * multiplier;
        xRotation -= lookVector.y * sensY * multiplier;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}
