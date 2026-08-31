using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float verticalSpeed = 2f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.15f;
    public float smoothSpeed = 12f;

    private float targetYaw;
    private float targetPitch;

    private float currentYaw;
    private float currentPitch;

    void Start()
    {
        targetYaw = transform.eulerAngles.y;
        targetPitch = transform.eulerAngles.x;

        currentYaw = targetYaw;
        currentPitch = targetPitch;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
            move += transform.forward;

        if (Keyboard.current.sKey.isPressed)
            move -= transform.forward;

        if (Keyboard.current.aKey.isPressed)
            move -= transform.right;

        if (Keyboard.current.dKey.isPressed)
            move += transform.right;

        if (Keyboard.current.qKey.isPressed)
            move += Vector3.down;

        if (Keyboard.current.eKey.isPressed)
            move += Vector3.up;

        if (
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.dKey.isPressed
        )
        {
            move.y = 0;
        }

        if (move.sqrMagnitude > 0)
        {
            transform.position +=
                move.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void HandleMouseLook()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();

            targetYaw += delta.x * mouseSensitivity;
            targetPitch -= delta.y * mouseSensitivity;

            targetPitch = Mathf.Clamp(targetPitch, -75f, 75f);
        }

        currentYaw = Mathf.Lerp(
            currentYaw,
            targetYaw,
            smoothSpeed * Time.deltaTime
        );

        currentPitch = Mathf.Lerp(
            currentPitch,
            targetPitch,
            smoothSpeed * Time.deltaTime
        );

        transform.rotation =
            Quaternion.Euler(currentPitch, currentYaw, 0f);
    }
}