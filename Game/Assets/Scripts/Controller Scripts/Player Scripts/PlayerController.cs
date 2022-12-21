using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour  
{   
    // The character joint that will connect the player to the grapple point
    public PlayerInput playerInput;
    public Rigidbody rb;
    private LineRenderer lineRend;
    private SpringJoint springJnt;

    #region Movement

    [Header("Movement")]
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
    public bool isGrounded;
    public bool isCollidingWithWall;
    public bool movingClockwise;
    public bool isGrappling;
    #endregion
    
    public float moveSpeed;
    public float leftAngle;
    public float rightAngle;
    
    public void Start()
    {
        
        
        movingClockwise = true;
    }
    public void Awake()
    {
        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody>(); 
        lineRend = GetComponent<LineRenderer>();
        springJnt = GetComponent<SpringJoint>();

        playerInput.Player.Run.performed += ctx => Run();
        playerInput.Player.Jump.performed += ctx => Jump();
        playerInput.Player.Crouch.performed += ctx => Crouch();
        playerInput.Player.Grapple.performed += ctx => Grapple();

        lineRend.enabled = false;
        springJnt.enableCollision = false;
    }

    public void Update()
    {
        if(isGrappling)
        {
            Swing();
        }
    }

    public void FixedUpdate()
    {
        isGrounded = CheckTopCollision();
        isCollidingWithWall = CheckWallCollision();
        Move();
    }

    #region Player Movement        
        public void Move()
        {
            // Get the horizontal input value
            float horizontalInput = playerInput.Player.Move.ReadValue<float>();

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
            movementSpeed = canCrouch ? movementSpeed / 2 : 8;
            canCrouch = !canCrouch;
        }

        public void Run()
        {
            if(canRun && canCrouch)
                movementSpeed *= 1.5f;
                canRun = false;
                Invoke(nameof(Reset), staminaCooldown);
        }


        public void Jump()
        {
            if(canJump && isGrounded)
                // Calculate the upward jump force based on the jump height
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                canJump = false;
                Invoke(nameof(Reset), jumpCooldown); 
        }

        public void Grapple()
        {
            if(isGrappling)
            {
                // SelectNode();
            }
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
                staminaCooldown = 5;
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


    #region Grapple Angle
    // private void SelectNode(Node node)
    // {
    //     selectedNode = node;
    // }

    public void ChangeMoveDir()
    {
        if (transform.rotation.z > rightAngle)
        {
            movingClockwise = false;
        }
        if (transform.rotation.z < leftAngle)
        {
            movingClockwise = true;
        }
    }

    public void Swing()
    {
        ChangeMoveDir();

        if (movingClockwise)
        {
            rb.angularVelocity = Vector3.forward * moveSpeed;
        }

        if (!movingClockwise)
        {
            rb.angularVelocity = Vector3.forward * moveSpeed * -1;
        }
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
