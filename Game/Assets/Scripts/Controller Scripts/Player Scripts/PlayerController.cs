using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDataPresistence
{

    public enum PlayerStates {
        idle,
        running,
        walking,
        crouching,
        jumping,
        grappling,
        mantle
    }

    public PlayerStates playerState;
    // The character joint that will connect the player to the grapple point
    public PlayerInput playerInput;
    public Rigidbody rb;
    public Transform height;
    public LineRenderer lineRenderer;
    public Animator anim;

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

    public bool canMantle;
    public float isJumping;
    public float isRunning;
    public float isIdle;
    public float isWalking;
    public float isMantling;
    public bool isGrounded;
    public bool isCollidingWithWall;
    public float isGrappling;

    #endregion

    #region Mantle
    public float mantleHopForce;
    public float mantleHopDuration;
    public string mantleTag;
    #endregion

    public float swingForce = 5;
    public List<Transform> grapplePoints = new List<Transform>();

    public void Awake()
    {
        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody>();
        playerInput.Player.Enable();
        anim = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        horizontalInput = playerInput.Player.Move.ReadValue<float>();
        isJumping = playerInput.Player.Jump.ReadValue<float>();
        isRunning = playerInput.Player.Run.ReadValue<float>();
        isGrappling = playerInput.Player.Grapple.ReadValue<float>();
        isMantling = playerInput.Player.Mantle.ReadValue<float>();


    }

    public void FixedUpdate()
    {
        canMantle = CheckMantleCollision();
        isGrounded = CheckTopCollision();
        anim.SetBool("Grounded", isGrounded);
        isCollidingWithWall = CheckWallCollision();
        UpdatePlayerState();
        switch (playerState)
        {
            case PlayerStates.idle:
                // Perform idle behavior              
                anim.SetBool("Walk", false);
                anim.SetBool("Run", false);
                break;
            case PlayerStates.walking:
                // Perform walking behavior
                if (isRunning < 0.5f) {
                    anim.SetBool("Walk", true);
                    anim.SetBool("Run", false);
                }
                Move();
                break;
            case PlayerStates.running:
                // Perform running behavior
                anim.SetBool("Walk", false);
                anim.SetBool("Run", true);
                Run();
                break;
            case PlayerStates.jumping:
                // Perform jumping behavior
                anim.SetTrigger("Jump");
                Jump();
                break;
            case PlayerStates.grappling:
                GrappleToPoint();
                break;
            case PlayerStates.mantle:
                StartCoroutine(MantleHopCoroutine());
                break;
        }
    }


    public void UpdatePlayerState()
    {
        if (isGrounded)
        {
            lineRenderer.enabled = false;
            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                if (Mathf.Abs(isRunning) > 0.5f && playerState == PlayerStates.walking)
                {
                    playerState = PlayerStates.running;
                }
                else if (Mathf.Abs(isJumping) > 0.5f)
                {
                    playerState = PlayerStates.jumping;
                }
                else
                {
                    playerState = PlayerStates.walking;
                }
            }
            else if (Mathf.Abs(isJumping) > 0.5f)
            {
                playerState = PlayerStates.jumping;
            }

            else
            {
                playerState = PlayerStates.idle;
            }
        }
        else if (!isGrounded)
        {
            lineRenderer.enabled = false;
            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                if (Mathf.Abs(isRunning) > 0.5f)
                {
                    playerState = PlayerStates.running;
                }
                else
                {
                    playerState = PlayerStates.walking;
                }
            }
            else if (Mathf.Abs(isGrappling) > 0.5f || Mathf.Abs(isGrappling) > 0.5f && Mathf.Abs(horizontalInput) > 0.01f)
            {
                playerState = PlayerStates.grappling;
                if (Mathf.Abs(isGrappling) < 0.5f)
                {
                    // Remove the first grapple point from the list
                    grapplePoints.RemoveAt(0);
                }
            }
            else
            {
                playerState = PlayerStates.idle;
            }
        }
        if (canMantle)
        {
            if (Mathf.Abs(isMantling) > 0.5f)
            {
                playerState = PlayerStates.mantle;
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
            rb.velocity = new Vector3(rb.velocity.x + momentum, rb.velocity.y, rb.velocity.z);
        }
    }
    #endregion

    #region Player Actions

    public void Run()
    {
        if (canRun)
        {
            movementSpeed *= 1.5f;
            canRun = false;
            Invoke(nameof(Reset), staminaCooldown);
        }
    }


    public void Jump()
    {
        if (isGrounded && canJump)
            // Calculate the upward jump force based on the jump height
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        canJump = false;
        Invoke(nameof(Reset), jumpCooldown);
    }



    public void Reset()
    {
        if (!canJump)
        {
            canJump = true;
        }
        else if (!canRun)
        {
            movementSpeed = 5.8f;
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
        //Debug.DrawRay(transform.position, rayDirectionDOWN * rayLength, Color.green, 1f);

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
        //Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red, 1f);

        // Perform the raycast and store the result in a RaycastHit variable
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength) && hit.collider.gameObject.tag == "Wall")
        {
            return true;
        }
        return false;
    }

    public bool CheckMantleCollision()
    {
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        // cast a ray forward from the player's position
        Ray ray = new Ray(transform.position, velocity.normalized);
        Vector3 rayDirection = new Vector3(transform.forward.x, transform.forward.y, 0);
        float rayLength = GetComponent<Collider>().bounds.extents.x + 1.5f;
        RaycastHit hit;
        // check if the ray hits an object
        if (Physics.Raycast(ray, out hit, rayLength))
        {
            if (hit.collider.tag == mantleTag)
            {
                Debug.Log(transform.position.y);
                // check if the upper half of the player is higher than the object, excluding the z axis
                if (1 + height.position.y > hit.point.y)
                {
                    // player should mantle

                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        return false;
    }

    IEnumerator MantleHopCoroutine()
    {
        // apply upward force to make the player hop
        GetComponent<Rigidbody>().AddForce(Vector3.up * mantleHopForce, ForceMode.Impulse);

        // wait for the hop duration
        yield return new WaitForSeconds(mantleHopDuration);

        // apply downward force to bring the player back down
        GetComponent<Rigidbody>().AddForce(Vector3.down * mantleHopForce, ForceMode.Impulse);
    }
    #endregion


    #region Grapple 

    public void GrappleToPoint()
    {
        // Set the tag of the objects that the player can grapple to
        string grappleTag = "Grapple Point";

        /// Set the maximum distance that the player can grapple to an object
        float grappleRange = 10f;

        // Set the radius of the sphere that will be cast
        float sphereRadius = 10f;

        // Set the direction in which the player will cast the sphere
        Vector3 grappleDirection = transform.forward;

        // Create a sphere that will be used to detect objects with the grappleTag
        Vector3 sphereOrigin = transform.position;
        RaycastHit[] hits;

        // Perform a sphere cast and store the results in the hits array
        hits = Physics.SphereCastAll(sphereOrigin, sphereRadius, grappleDirection, grappleRange);

        // Find the closest grapple point
        Transform closestGrapplePoint = null;
        float minDistance = float.MaxValue;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == grappleTag)
            {
                lineRenderer.enabled = true;
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestGrapplePoint = hit.transform;
                }
            }
        }

        // If a grapple point was found
        if (closestGrapplePoint != null)
        {
            Transform grapplePoint = closestGrapplePoint;

            // Calculate the direction from the player to the grapple point
            grappleDirection = grapplePoint.position - transform.position;
            // Normalize the direction so that it has a length of 1
            grappleDirection.Normalize();
            // Apply the swing force to the player's rigidbody in the direction of the grapple point
            rb.AddForce(grappleDirection * swingForce, ForceMode.Impulse);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint.position);
        }
    }
    #endregion

    #region Save/Load

    int currentLevelIndex;
    public void LoadData(GameData data)
    {
        this.currentLevelIndex = data.currentLevelIndex;
        this.transform.position = data.playerPosition;
    }

    public void SaveData(ref GameData data)
    {
        data.currentLevelIndex = this.currentLevelIndex;
        data.playerPosition = this.transform.position;
    }

    
    private void GetCurrentLevelIndex()
        //Getting level index
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
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