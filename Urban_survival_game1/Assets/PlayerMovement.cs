using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform playerCamera;

    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float Speed = 6.0f;

    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;

    [SerializeField] LayerMask ground;

    /// <summary> Stamina implementation 
    [SerializeField] private float maxSprintStamina = 100f;
    [SerializeField] float sprintDrainRate = 10f; // Stamina drained per second while sprinting
    [SerializeField] float sprintRecoveryRate = 5f; // Stamina recovered per second when not sprinting
    private float currentSprintStamina;

    /// </summary>

    public float jumpHeight = 6f;
    float velocityY;
    bool isGrounded;

    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;

    void Start()
    {

        currentSprintStamina = maxSprintStamina;
        controller = GetComponent<CharacterController>();

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    void Update() //update per frame 
    {
        UpdateMouse();
        UpdateMove();
    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;

        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraCap;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        float adjustedSpeed = Speed;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isWalkingSlowly = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (isSprinting && currentSprintStamina > 0)
        {
            adjustedSpeed *= 1.5f; // Increase speed for sprinting
            currentSprintStamina -= sprintDrainRate * Time.deltaTime; // Drain sprint stamina
            currentSprintStamina = Mathf.Max(currentSprintStamina, 0); // Prevent stamina from going below 0
        }
        else if (isWalkingSlowly)
        {
            adjustedSpeed *= 0.5f; // Halve the speed when Ctrl is pressed
        }

        if (!isSprinting)
        {
            currentSprintStamina += sprintRecoveryRate * Time.deltaTime; // Recover sprint stamina
            currentSprintStamina = Mathf.Min(currentSprintStamina, maxSprintStamina); // Prevent stamina from exceeding max
        }

        velocityY += gravity * 2f * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * adjustedSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (isGrounded! && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }

    public float GetCurrentSprintStamina()
    {
        return currentSprintStamina;
    }

    public float GetMaxSprintStamina()
    {
        return maxSprintStamina;
    }
}


