using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;
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
        if (InputManager.CurrentInputMode != InputManager.InputMode.Player) return;

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
