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
    public float staminaCooldown;
    public float groundDrag;


   
    [SerializeField] private float stuckTimer = 0f;
    
    public float grav;
    bool canJump = true;
    bool canRun = true;
    bool canCrouch = true;
    
    [SerializeField] bool isGrounded;

    [SerializeField] bool isCollidingWithWall;

    private void Awake()
    {
        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody>();

        playerInput.Player.Jump.performed += ctx => Jump();
        playerInput.Player.Crouch.performed += ctx => Crouch();
        playerInput.Player.Run.performed += ctx => Run();
        playerInput.Player.Run.performed += ctx => Slide();
    }


    private void update()
    {
        if(isCollidingWithWall)
        {
            stamina();
        }        

        rb.AddForce(Vector3.down * grav, ForceMode.Force); 
    }
    private void FixedUpdate()
    {
        isGrounded = CheckTopCollision();
        // Check if the player character is colliding with a wall using raycasting
        isCollidingWithWall = CheckWallCollision();
        Move();
       
    }
    
    #region Player Movement

        private void Move()
        {
            // Get the horizontal input value
            float horizontalInput = playerInput.Player.Movement.ReadValue<float>();

            // Only apply horizontal movement if the input value is greater than a small threshold
            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                // Calculate the horizontal velocity to set based on the movement speed and input value
                Vector3 horizontalVelocity = Vector3.right * horizontalInput * movementSpeed;

                // Set the player's horizontal velocity
                rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, rb.velocity.z);

                // Change the player's orientation based on the horizontal input
                if (isGrounded)
                {
                    transform.rotation = Quaternion.Euler(0, horizontalInput < 0 ? 180 : 0, 0);
                }
                else
                {
                    Quaternion startRotation = transform.rotation;
                    Quaternion endRotation = Quaternion.Euler(0, horizontalInput < 0 ? 180 : 0, 0);
                }
            }
            else
            {
                // If there is no horizontal input, set the horizontal velocity to 0
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
        }
        private void Crouch()
        {
            movementSpeed = canCrouch && canRun ? movementSpeed / 2 : 8;
            canCrouch = !canCrouch;
        }

        private void Run()
        {
            if (canRun && isGrounded)
            {
                movementSpeed *= 1.5f;
                canRun = false;
                Invoke(nameof(ResetRun), staminaCooldown);
            }
        }

        private void Slide()
        {
            if (!canRun && isGrounded && canCrouch)
            {
                movementSpeed += 2f;
                canRun = false;
                staminaCooldown = 3;
                Invoke(nameof(ResetRun), staminaCooldown);
            }
        }


        private void ResetRun()
        {
            movementSpeed = 8;
            canRun = true;
            staminaCooldown = 5;
        }

        private void Jump()
        {
    
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            
            if (canJump && isGrounded)
            {
                // Calculate the upward jump force based on the jump height
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                
                canJump = false;

                Invoke(nameof(ResetJump), jumpCooldown);
            }
            else if(canJump && isCollidingWithWall)
            {
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                
                canJump = false;

                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void ResetJump()
        {
            canJump = true;
        }

        private void stamina()
        {
                
                if (stuckTimer > 0)
                    {
                        stuckTimer -= Time.deltaTime;

                        // Gradually reduce the player's velocity over time
                        rb.velocity = rb.velocity * 1f;
                    }
                    if (stuckTimer == 0)
                    {
                        // Reset the player's velocity when the timer expires
                        rb.velocity = Vector3.zero;
                        stuckTimer = staminaCooldown;
                    }
        }

        

    #endregion


    #region Collision Detection
        private bool CheckTopCollision()
        {
            // Set the origin of the ray to the top of the player character's collider
            Vector3 rayOrigin = transform.position + Vector3.up * GetComponent<Collider>().bounds.extents.y;

            // Set the direction of the ray to be downward
            Vector3 rayDirection = Vector3.down;

            // Set the length of the ray to be slightly longer than the height of the player character's collider
            float rayLength = GetComponent<Collider>().bounds.extents.y + 1f;

            // Draw the ray in the Scene view for debugging purposes
            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.yellow);

            // Perform the raycast and store the result in a RaycastHit variable
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength) && hit.collider.gameObject.tag == "Platform")
            {
                return true;
            }
            return false;
        }

        private bool CheckWallCollision()
        {
            // Set the origin of the ray to the center of the player character's collider
            Vector3 rayOrigin = transform.position;

            // Set the direction of the ray to be forward
            Vector3 rayDirection = new Vector3(transform.forward.x, transform.forward.y, 0);

            // Set the length of the ray to be slightly longer than the width of the player character's collider
            float rayLength = GetComponent<Collider>().bounds.extents.x + 1f;

            // Draw the ray in the Scene view for debugging purposes
            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red);

            // Perform the raycast and store the result in a RaycastHit variable
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength) && hit.collider.gameObject.tag == "Hoppable")
            {
                return true;
            }
            return false;
        }
    #endregion
    
    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
}