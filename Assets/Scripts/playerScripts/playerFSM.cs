using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerFSM : MonoBehaviour
{
    [SerializeField] float walkSpeed = 1.5f; //velocidade de movimento do player
    [SerializeField] float jumpForce = 110f; //força do pulo
    [SerializeField] enum PlayerState { STANDING, JUMPING, FALLING } 
    [SerializeField] PlayerState state;
    [SerializeField] Transform groundCheck; //objeto que toca o chão realizar a verificação
    [SerializeField] LayerMask wathIsGround; //identifica o que é o chão
    private bool is_on_floor; //verifica se o player ta no chão
    private bool isJump; //verifica se o player ta pulando
    private float horizontal; //capta o movimento horizontal
    private Rigidbody2D playerBody;
    private Animator animations;
    private SpriteRenderer playerSprite;

    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>(); 
        animations = GetComponent<Animator>(); 
        playerSprite = GetComponent<SpriteRenderer>(); 
        state = PlayerState.STANDING; 
    }

    void FixedUpdate()
    {
        //switch entre os estados
        switch (state)
        {
            case PlayerState.STANDING:
               Standing();
               break;
            case PlayerState.JUMPING:
                Jumping();
                break;
        }

    }

    public void update_velocity()
    {
        //define a direção do sprite, direção das áreas de colisão e outras coisas que devem virar junto do player
        switch (horizontal)
        {
            case 1:
                playerSprite.flipX = false;
                break;
            case -1:
                playerSprite.flipX = true;
                break;
        }
    }
    
    //Estado para o player parado e andando
    public void Standing()
    {
        update_velocity();
        jump_transition();

        if (horizontal == 0){
            animations.SetBool("onWalk", false);
        }
        else
        {
            animations.SetBool("onWalk", true);
        }

        playerBody.velocity = new Vector2(horizontal * walkSpeed, playerBody.velocity.y);
    }

    //Estado para o player pulando
    public void Jumping()
    {
        update_velocity();
        //verifica se o player ta no chão
        is_on_floor = Physics2D.Linecast(transform.position, groundCheck.position, wathIsGround);

        if (playerBody.velocity.y > 0)
        {
            animations.SetBool("onJump", true);
        }else if(playerBody.velocity.y < 0)
        {
            animations.SetBool("onJump", false);
            animations.SetBool("onFall", true);
        }

        if (is_on_floor)
        {
            animations.SetBool("onJump", false);
            animations.SetBool("onFall", false);
            state = PlayerState.STANDING;
        }

        playerBody.velocity = new Vector2(horizontal * walkSpeed, playerBody.velocity.y);
    }

    public void jump_transition()
    {
        if (isJump)
        {
            animations.SetBool("onWalk", false);
            playerBody.AddForce(new Vector2(0, jumpForce));
            isJump = false;
            state = PlayerState.JUMPING;
        }

        if (playerBody.velocity.y < 0)
        {
            animations.SetBool("onWalk", false);
            state = PlayerState.JUMPING;
        }
    }

    //captura a entrada da tecla de pulo
    public void SetJump(InputAction.CallbackContext context)
    {
        isJump = context.performed;
    }

    //captura a entrada da tecla de movimento horizontal
    public void SetMovement(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<float>();

    }

}
