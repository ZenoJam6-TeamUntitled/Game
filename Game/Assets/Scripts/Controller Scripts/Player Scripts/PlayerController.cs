using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    Rigidbody rb;

    Vector3 vertVelocity = Vector3.down;

    [Header("Movement")]
    public float jumpHeight;
    public float movementSpeed;
    public float jumpCooldown;
    public float groundDrag;

    
    public float grav;
    bool canJump = true;
    
    bool isGrounded;

    private void Awake()
    {
        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody>();

        playerInput.Player.Jump.performed += ctx => Jump();
        playerInput.Player.Crouch.performed += ctx => Crouch();
        playerInput.Player.Run.performed += ctx => Run();
    }


    private void update()
    {
        if(isGrounded)
        {
            rb.drag = groundDrag;
        }
        else{
            rb.drag = 0;
        }             
    }
    private void FixedUpdate()
    {
        
        Move();
       
    }

    private void Jump()
    {

        if (canJump && isGrounded)
        {
            // Calculate the upward jump force based on the jump height
            rb.velocity = new Vector3(rb.velocity.x,0f,rb.velocity.z);

            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);

            canJump = false;

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private void Move()
    {

        // Get the horizontal input value
        float horizontalInput = playerInput.Player.Movement.ReadValue<float>();

        // Only apply horizontal movement if the input value is greater than a small threshold
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            // Calculate the horizontal force to apply based on the movement speed and input value
            Vector3 horizontalForce = Vector3.right * horizontalInput * movementSpeed;

            // Apply the horizontal force to the player character
            rb.AddForce(horizontalForce, ForceMode.VelocityChange);
            rb.drag = Mathf.Clamp(5.0f - Mathf.Abs(horizontalInput), 0.0f, 5.0f);
        }
        else
        {
            // If there is no horizontal input, set the drag value to a larger value to slow down the player
            rb.drag = 3.0f;
        }
    }

    private void Crouch()
    {
        movementSpeed /= 2f;

    }

    private void Run()
    {
        movementSpeed *= 2f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGrounded = false;
        }
    }

    
    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
}