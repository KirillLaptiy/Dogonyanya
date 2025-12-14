using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementNewInput : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            float x = 0f;
            float y = 0f;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x = 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y = -1f;

            input = new Vector2(x, y).normalized;
        }
        else
        {
            input = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}
