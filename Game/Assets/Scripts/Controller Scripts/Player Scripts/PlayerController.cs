using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour  
{   

    public enum PlayerStates{
        idle,
        running,
        walking,
        crouching,
        jumping,
        grappling
    }

    public PlayerStates playerState;
    // The character joint that will connect the player to the grapple point
    public PlayerInput playerInput;
    public Rigidbody rb;
    public LineRenderer lineRenderer;

    #region Movement

    [Header("Movement")]
    public float horizontalInput;
    public float jumpHeight;
    public float movementSpeed;
    public float jumpCooldown;
    public float staminaCooldown;
    public float momentum;
    #endregion

    #region Bool/Float Check
    [Header("BoolChecks")]
    public bool canJump = true;
    public bool canRun = true;
    public bool canCrouch = true;

    public float isJumping;
    public float isCrouching;
    public float isRunning;
    public float isIdle;
    public float isWalking;
    public bool isGrounded;
    public bool isCollidingWithWall;
    public float isGrappling;
    #endregion
    
    public float swingForce = 5;
    public Transform grapplePoint;
    
 
    public void Awake()
    {
        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody>(); 
        // lineRend = GetComponent<LineRenderer>();
        // springJnt = GetComponent<SpringJoint>();
        playerInput.Player.Enable();
        // lineRend.enabled = false;
        // springJnt.enableCollision = false;
    }

    public void Update()
    {
        horizontalInput = playerInput.Player.Move.ReadValue<float>();
        isJumping = playerInput.Player.Jump.ReadValue<float>();
        isRunning = playerInput.Player.Run.ReadValue<float>();
        isCrouching = playerInput.Player.Crouch.ReadValue<float>();
        isGrappling = playerInput.Player.Grapple.ReadValue<float>();
        
    }

    public void FixedUpdate()
    {
        isGrounded = CheckTopCollision();
        isCollidingWithWall = CheckWallCollision();
        UpdatePlayerState();
        switch (playerState)
        {
            case PlayerStates.idle:
                // Perform idle behavior
                
                break;
            case PlayerStates.walking:
                // Perform walking behavior
                
                Move();
                break;
            case PlayerStates.running:
                // Perform running behavior
                Run();
                break;
            case PlayerStates.jumping:
                // Perform jumping behavior
                
                Jump();
                break;
            case PlayerStates.crouching:
                // Perform crouching behavior
                
                Crouch();
                break;
            case PlayerStates.grappling:
                GrappleToPoint();
                lineRenderer.enabled = true;
                break;
        }
    }


    public void UpdatePlayerState()
    {
        if(isGrounded)
        {
            lineRenderer.enabled = false;
            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                    if (Mathf.Abs(isRunning)> 0.5f && playerState == PlayerStates.walking)
                    {
                        playerState = PlayerStates.running;
                    }
                    else if(Mathf.Abs(isJumping) > 0.5f)
                    {
                        playerState = PlayerStates.jumping;
                    }
                    else if(Mathf.Abs(isCrouching) > 0.5f && playerState == PlayerStates.walking)
                    {
                        playerState = PlayerStates.crouching;
                    }
                    else
                    {
                        playerState = PlayerStates.walking;
                    }
            }
            else if(Mathf.Abs(isJumping) > 0.5f)
            {
                playerState = PlayerStates.jumping;
            }
            else if(Mathf.Abs(isCrouching) > 0.5f) 
            {
                playerState = PlayerStates.crouching;
            }
            else
            {
                playerState = PlayerStates.idle;
            }
        }
        else if(!isGrounded)
        {
            lineRenderer.enabled = false;
            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                if (Mathf.Abs(isRunning)> 0.5f)
                {
                    playerState = PlayerStates.running;
                }
                else
                {
                    playerState = PlayerStates.walking;
                }
            }
            else if ((Vector3.Distance(transform.position, grapplePoint.position) < 15f && 
            Mathf.Abs(isGrappling) > 0.5f) || (Vector3.Distance(transform.position, grapplePoint.position) < 15f && 
            Mathf.Abs(isGrappling) > 0.5f && Mathf.Abs(horizontalInput) > 0.01f))
            {
                playerState = PlayerStates.grappling;
            }
            else
            {
                playerState = PlayerStates.idle;
            }
        }
        
    }


    #region Player Movement        
        public void Move()
        {
            // Get the horizontal input value

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
                if (isGrounded || !isGrounded)
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
    #endregion
   
    #region Player Actions
        public void Crouch()
        {
            // If the character is currently crouched, uncrouch them by setting their movement speed to the normal value
            if (canCrouch)
            {
                movementSpeed = 8;
                canCrouch = false;
            }
            // If the character is not currently crouched, crouch them by setting their movement speed to half the normal value
            else
            {
                movementSpeed /= 2;
                canCrouch = true;
            }
        }

        public void Run()
        {
            if(canCrouch && canRun)
            {
                movementSpeed *= 1.5f;
                canRun = false;
                Invoke(nameof(Reset), staminaCooldown);
            }
        }


        public void Jump()
        {
            if(isGrounded && canJump)
                // Calculate the upward jump force based on the jump height
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                canJump = false;
                Invoke(nameof(Reset), jumpCooldown); 
        }


    
    public void Reset()
        {
            if(!canJump)
            {
                canJump = true;
            }
            else if(!canRun)
            {
                movementSpeed = 8;
                canRun = true;
            }
        }



    #endregion

    #region Collision Detection
        public bool CheckTopCollision()
        {
            // Set the origin of the ray to the top of the player character's collider
            Vector3 rayOrigin = transform.position + Vector3.up * GetComponent<Collider>().bounds.extents.y;
            

            // Set the direction of the ray to be downward
            Vector3 rayDirectionDOWN = Vector3.down;

            // Set the length of the ray to be slightly longer than the height of the player character's collider
            float rayLength = GetComponent<Collider>().bounds.extents.y + 1f;

            // Draw the ray in the Scene view for debugging purposes
            Debug.DrawRay(transform.position, rayDirectionDOWN * rayLength, Color.green, 1f);

            // Perform the raycast and store the result in a RaycastHit variable
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirectionDOWN, out hit, rayLength) && hit.collider.gameObject.tag == "Platform")
            {
                return true;
            }
            return false;
        }

        public bool CheckWallCollision()
        {
            // Set the origin of the ray to the center of the player character's collider
            Vector3 rayOrigin = transform.position;

            // Set the direction of the ray to be forward
            Vector3 rayDirection = new Vector3(transform.forward.x, transform.forward.y, 0);

            // Set the length of the ray to be slightly longer than the width of the player character's collider
            float rayLength = GetComponent<Collider>().bounds.extents.x + 1f;

            // Draw the ray in the Scene view for debugging purposes
            Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red, 1f);

            // Perform the raycast and store the result in a RaycastHit variable
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength) && hit.collider.gameObject.tag == "Wall")
            {
                return true;
            }
            return false;
        }
    #endregion


    #region Grapple 

        private void GrappleToPoint()
        {
            // Calculate the direction from the player to the grapple point
            Vector3 grappleDirection = grapplePoint.position - transform.position;
            // Normalize the direction so that it has a length of 1
            grappleDirection.Normalize();
            // Apply the swing force to the player's rigidbody in the direction of the grapple point
           
            rb.AddForce(grappleDirection * swingForce, ForceMode.Impulse);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint.position);
            
        }
    #endregion
    public void OnEnable()
    {
        playerInput.Enable();
    }

    public void OnDisable()
    {
        playerInput.Disable();
    }
}
