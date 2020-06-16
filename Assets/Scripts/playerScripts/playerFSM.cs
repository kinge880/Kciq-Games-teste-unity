using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerFSM : MonoBehaviour
{
    #region Variáveis  
    [SerializeField] float walkSpeed = 1.5f; //velocidade de movimento do player
    [SerializeField] float jumpForce = 110f; //força do pulo
    [SerializeField] float dashDuration = 0.625f;
    [SerializeField] float dashSpeed = 2.0f;
    [SerializeField] enum PlayerState { STANDING, JUMPING, DOUBLEJUMP, DASHING } 
    [SerializeField] PlayerState state;
    [SerializeField] Transform groundCheck; //objeto que toca o chão realizar a verificação
    [SerializeField] LayerMask wathIsGround; //identifica o que é o chão
    [SerializeField] float maxAngle =35;
    [SerializeField] float hitSize = 2f;

    private bool is_on_floor; //verifica se o player ta no chão
    private bool isJump; //verifica se o player ta pulando
    private bool isDash;
    private float angle = 0;
    private int dashDirection;
    public float horizontal; //capta o movimento horizontal
    private float dashTimeLeft;
    private Vector2 forceDirection = Vector2.zero;
    private Rigidbody2D playerBody;
    private Animator animations;
    private SpriteRenderer playerSprite;
    #endregion

    #region Start e FixedUpdate
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
            case PlayerState.DOUBLEJUMP:
                Double_Jump();
                break;
            case PlayerState.DASHING:
                Dash();
                break;
        }

    }
    #endregion

    #region Aplicação de movimento e ajustes no sprite, colisão etc
    public void Update_velocity()
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

    public void Move_and_normalize_slope(float directionX, float speed)
    {
        angle = 0;
        forceDirection = Vector2.zero;
        RaycastHit2D hitDirection = Physics2D.Raycast(transform.position, Vector2.down, hitSize, wathIsGround); //cria um raycast apontando para baixo, ele vai detectar o chão e o angulo do chão
        Debug.DrawRay(transform.position, Vector2.down * hitSize, Color.yellow);  //desenha o raycast no editor

        if (hitDirection.collider)
        {
            Debug.DrawRay(transform.position, hitDirection.normal, Color.blue);  //desenha o raycast do vetor oposto ao angulo do chão

            //captura a direção do player
            switch (horizontal)
            {
                case 1:
                    forceDirection = new Vector2(hitDirection.normal.y, -hitDirection.normal.x);
                    break;
                case -1:
                    forceDirection = new Vector2(-hitDirection.normal.y, hitDirection.normal.x);
                    break;
                default:
                    forceDirection = new Vector2(hitDirection.normal.y, hitDirection.normal.x);
                    break;
            }

            RaycastHit2D hitAngle = Physics2D.Raycast(transform.position, forceDirection, 0.5f, wathIsGround); //cria um raycast que detecta o angulo do chão a frente
            Debug.DrawRay(transform.position, hitAngle.normal, Color.white);
            angle = Vector2.Angle(hitAngle.normal, Vector2.up); //captura o angulo do chão

            //verifica se o angulo não é maior que o limite definido no inspector
            if (angle < maxAngle)
            {
                Vector2 moveDirection = forceDirection * Mathf.Abs(directionX) * speed; //captura as variaveis e move o player
                playerBody.velocity = moveDirection; //movimenta o player no angulo correto
                Debug.DrawRay(transform.position, forceDirection * Mathf.Abs(directionX), Color.cyan); //desenha no editor a direção  deve ser seguida
            }
            else
            {
                //Podemos colcoar animações aqui, e outras interações pra visualmente explicar pq ele n sobe, etc
                Debug.DrawRay(transform.position, forceDirection * Mathf.Abs(directionX), Color.red); //mostra o raycast que indica um angulo maior do que o limite
            }
        }
        else
        {
            playerBody.velocity = new Vector2(horizontal * walkSpeed, playerBody.velocity.y);
        }

    }
    #endregion

    #region Maquina de estado
    //Estado para o player parado e andando
    public void Standing()
    {
        Update_velocity();
        Jump_transition();
        Dash_transition();

        if (horizontal == 0){
            animations.SetBool("onWalk", false);
        }
        else
        {
            animations.SetBool("onWalk", true);
        }

        Move_and_normalize_slope(horizontal, walkSpeed);
    }

    //Estado para o player pulando
    public void Jumping()
    {
        //verifica se o player ta no chão
        is_on_floor = Physics2D.Linecast(transform.position, groundCheck.position, wathIsGround);

        if (playerBody.velocity.y > 0)
        {
            animations.SetBool("onJump", true);
        }
        else if(playerBody.velocity.y < 0)
        {
            animations.SetBool("onFall", true);
        }

        if (is_on_floor)
        {
            animations.Rebind();
            state = PlayerState.STANDING;
        }

        Update_velocity();
        Double_jump_transition();
        Move_and_normalize_slope(horizontal, walkSpeed);
    }

    public void Jump_transition()
    {
        //verifica se o player ta no chão
        is_on_floor = Physics2D.Linecast(transform.position, groundCheck.position, wathIsGround);

        if (isJump)
        {
            playerBody.AddForce(new Vector2(0, jumpForce));
            isJump = false;
            state = PlayerState.JUMPING;
        }

        if (!is_on_floor && playerBody.velocity.y < -0.5) //botei -0.1 pq o unity gera valores para y muito pequenos por conta de erro de ponto flutuante como "-2.443569E-08", mas isso impede uma verificação de < 0
        {
            state = PlayerState.JUMPING;
            Debug.Log(playerBody.velocity.y);
        }
    }

    public void Double_Jump()
    {
        Update_velocity();
        animations.SetBool("onDoubleJump", true);
        //verifica se o player ta no chão
        is_on_floor = Physics2D.Linecast(transform.position, groundCheck.position, wathIsGround);

        if (is_on_floor)
        {
            animations.Rebind();
            state = PlayerState.STANDING;
        }
        
        Move_and_normalize_slope(horizontal, walkSpeed);
    }

    public void Double_jump_transition()
    {
        Update_velocity();
        
        if (isJump)
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, 0);
            playerBody.AddForce(new Vector2(0, jumpForce * 1.4f));
            isJump = false;
            state = PlayerState.DOUBLEJUMP;
        }
    }

    public void Dash()
    {
        animations.SetBool("onDash", true);
        dashTimeLeft -= Time.deltaTime;
       
        if (dashTimeLeft <= 0)
        {
            animations.SetBool("onDash", false);
            state = PlayerState.STANDING;
        }

        playerBody.velocity = new Vector2(dashDirection * dashSpeed, playerBody.velocity.y);
    }

    public void Dash_transition()
    {
        if (isDash)
        {
            //seta o tempo de duração do dash em uma váriavel reciclavel, com dashDuration podendo ser definido no inspector
            dashTimeLeft = dashDuration;
            isDash = false;
            state = PlayerState.DASHING;

            //captura as direção do player para jogar o dash nessa direção e impedir que ele altere durante o dash
            if (playerSprite.flipX)
            {
                dashDirection = -1;
            }
            else
            {
                dashDirection = 1;
            }
        }
    }
    #endregion  

    #region Inputs do Player
    public void Set_jump(InputAction.CallbackContext context)
    {
        isJump = context.performed;
    }

    public void Set_movement(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<float>();
    }

    public void Set_dash(InputAction.CallbackContext context)
    {
        isDash = context.performed;
    }
    #endregion  
}
