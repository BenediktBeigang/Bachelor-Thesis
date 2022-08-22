using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float movementMultiplier;
    public float turningMultiplier;
    InputMaster input;
    Vector2 leftThumb;
    Vector2 rightThumb;

    void Awake()
    {
        input = new InputMaster();
        input.Player.LeftThumb.performed += ctx => leftThumb = ctx.ReadValue<Vector2>();
        input.Player.LeftThumb.canceled += ctx => leftThumb = Vector2.zero;
        input.Player.RightThumb.performed += ctx => rightThumb = ctx.ReadValue<Vector2>();
        input.Player.RightThumb.canceled += ctx => rightThumb = Vector2.zero;
    }

    void Update()
    {
        Vector3 translation = new Vector3(0, 0, (leftThumb.y * movementMultiplier)) * Time.deltaTime;
        transform.Translate(translation, transform);

        Vector3 rotation = new Vector3(0, (rightThumb.x * turningMultiplier), 0) * Time.deltaTime;
        transform.Rotate(rotation);
    }

    void OnEnable()
    {
        input.Player.Enable();
    }

    void OnDisable()
    {
        input.Player.Disable();
    }
}
