using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    Rigidbody rb;

    [SerializeField]
    public float jumpHeight;
    public float movementSpeed;
    bool canJump;

    public float jumpCooldown;


    public float playerHeight;
    public LayerMask isOnGround;

    bool grounded;
    bool isGrounded;

    void Awake()
    {
        playerInput = new PlayerInput();
        rb = GetComponent<Rigidbody>();

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isOnGround);
        
        playerInput.Player.Jump.performed += ctx => Jump();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Jump()
    {

        if(canJump && grounded)
        {

            rb.velocity = Vector3.up * jumpHeight;
       
            print("FUCKING JUMP");

            canJump = false;

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        canJump = true;
    }

    void Move()
    {
        
        if (playerInput.Player.Movement.ReadValue<float>() > 0.01f)
        {
            print("We just moved right");
            rb.velocity = Vector3.right * movementSpeed;
        }
        else if (playerInput.Player.Movement.ReadValue<float>() < -0.01f)
        {
            print("We just moved left");
            rb.velocity = Vector3.left * movementSpeed;
        }

    }

    void OnEnable()
    {
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }
}