using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 300f;
    public float jumpForce = 8f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        Jump();
    }

    void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (movement.magnitude >= 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            Vector3 moveDirection = toRotation * Vector3.forward;
            rb.MovePosition(rb.position + moveDirection * speed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}


