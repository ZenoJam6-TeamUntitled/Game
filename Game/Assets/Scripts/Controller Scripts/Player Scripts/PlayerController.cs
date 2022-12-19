using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour  
{


    PlayerInput playerInput;
    Rigidbody rb;

    [Header("Movement")]
    public float jumpHeight;
    public float movementSpeed;
    public float jumpCooldown;
    public float staminaCooldown;
    public float momentum;
    
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

    }

    private void update()
    {
        
    }
    private void FixedUpdate()
    {
       isGrounded = CheckTopCollision();
       isCollidingWithWall = CheckWallCollision();
       Move();
    }
    
    #region Player Movement

        
        private void Move()
        {
            
            // Get the horizontal input value
            float horizontalInput = playerInput.Player.Movement.ReadValue<float>();

            // Calculate the current movement speed based on the input value and the player's momentum
            float currentMovementSpeed = Mathf.Lerp(0, movementSpeed, Mathf.Abs(horizontalInput));

            // Only apply horizontal movement if the input value is greater than a small threshold
            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                // Calculate the horizontal velocity to set based on the current movement speed and input value
                Vector3 horizontalVelocity = Vector3.right * horizontalInput * currentMovementSpeed;

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
                // If there is no horizontal input, gradually reduce the player's horizontal velocity
                rb.velocity = new Vector3(rb.velocity.x * momentum, rb.velocity.y, rb.velocity.z);
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
                Invoke(nameof(Reset), staminaCooldown);
            }
        }


        private void Jump()
        {
    
            
            if (canJump && isGrounded)
            {
                // Calculate the upward jump force based on the jump height
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                
                canJump = false;

                Invoke(nameof(Reset), jumpCooldown);
            }
            
        }

        private void Reset()
        {
            if(!canJump)
            {
                canJump = true;
            }
            else if(!canRun)
            {
                movementSpeed = 8;
                canRun = true;
                staminaCooldown = 5;
            }
        }

    #endregion


    #region Collision Detection
        private bool CheckTopCollision()
        {
            // Set the origin of the ray to the top of the player character's collider
            Vector3 rayOrigin = transform.position + Vector3.up * GetComponent<Collider>().bounds.extents.y;
            

            // Set the direction of the ray to be downward
            Vector3 rayDirectionDOWN = Vector3.down;

            // Set the length of the ray to be slightly longer than the height of the player character's collider
            float rayLength = GetComponent<Collider>().bounds.extents.y + 1f;

            // Draw the ray in the Scene view for debugging purposes


            // Perform the raycast and store the result in a RaycastHit variable
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirectionDOWN, out hit, rayLength) && hit.collider.gameObject.tag == "Platform")
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