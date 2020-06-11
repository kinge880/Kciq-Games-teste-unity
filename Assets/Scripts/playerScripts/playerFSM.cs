using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerFSM : MonoBehaviour
{
    [SerializeField] float walkSpeed, jumpForce;
    [SerializeField] bool isJump;
    private float horizontal;
    private Rigidbody2D playerBody;
    private Animator animations;
    private SpriteRenderer playerSprite;
    public enum playerState
    {
        IDLE,
        WALKING,
        JUMPING,
        FALLING
    }
    public playerState states;


    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        animations = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (isJump)
        {
            playerBody.AddForce(new Vector2(0, jumpForce));
            isJump = false;
        }

        playerBody.velocity = new Vector2(horizontal * walkSpeed, playerBody.velocity.y);
    }

    public void setJump(InputAction.CallbackContext context)
    {
        isJump = context.performed;
    }

    public void setMovement(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<float>();

        switch (horizontal)
        {
            case 1:
                animations.SetBool("onWalk", true);
                playerSprite.flipX = false;
                break;
            case -1:
                animations.SetBool("onWalk", true);
                playerSprite.flipX = true;
                break;
            default:
                animations.SetBool("onWalk", false);
                break;
        }
    }

}
