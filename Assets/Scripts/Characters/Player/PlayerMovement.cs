using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float runningSpeedBoost = 3f;
    public float runningStaminaCostPerSecond = 25f;

    private Rigidbody2D rb;
    private Vector2 input;
    private PlayerStamina ps;

    private Vector2 lastPosition;
    private bool isWalking;
    private bool isRunning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ps = GetComponent<PlayerStamina>();

        lastPosition = rb.position;

        UpdateMoving();
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
        // Updates isWalking and isRunning
        UpdateMoving();

        float currentSpeed = speed;

        float stamina = ps.GetStamina();

        if (stamina > 0f && isRunning)
        {
            ps.SetStamina(stamina - runningStaminaCostPerSecond * Time.deltaTime);
            currentSpeed = speed + runningSpeedBoost;
        }

        rb.linearVelocity = input * currentSpeed;
    }

    private void UpdateMoving()
    {
        Vector2 currentPos = rb.position;

        float movedDistance = Vector2.Distance(currentPos, lastPosition);

        isWalking = movedDistance > 0.001f;

        isRunning = isWalking && Keyboard.current.shiftKey.isPressed;

        lastPosition = currentPos;
    }

    public bool IsWalking => isWalking;

    public bool IsRunning => isRunning;
}
