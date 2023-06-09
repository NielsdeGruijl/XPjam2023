using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float speed;

    [Header("Dash Variables")]
    [SerializeField] private float dashForce;
    [SerializeField] private int dashStaminaCost;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;

    private Rigidbody2D rb;
    private PlayerStaminaManager staminaManager;


    private Vector2 moveDir;
    private Vector2 attackDir;

    public bool isAttacking = false;
    public bool isDashing = false;
    private bool canDash = true;


    private float xInput;
    private float yInput;

    private float elapsedTime;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        staminaManager = GetComponent<PlayerStaminaManager>();

        AudioManager.manager.PlayAudio("Music");
    }

    private void Update()
    {
        if (isDashing || isAttacking) return;
        
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        moveDir = new Vector2(xInput, yInput);
        moveDir = moveDir.normalized;

/*        if(rb.velocity.magnitude >= 0.1f)
        {
            RotatePlayer(moveDir);
        }*/

        if(moveDir.x < 0)
        {
            FlipPlayer(true);
        }
        if(moveDir.x > 0)
        {
            FlipPlayer(false);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && staminaManager.stamina >= dashStaminaCost)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void FlipPlayer(bool flip)
    {
        sprite.flipX = flip;
    }

    public void SetAttacking(bool attacking)
    {
        isAttacking = attacking;
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        if (isAttacking)
        {
            rb.velocity = moveDir * speed * 0.5f;
        }
        else
        {
            rb.velocity = moveDir * speed;
        }
        
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        elapsedTime += Time.deltaTime;
        rb.velocity = moveDir * dashForce;
        staminaManager.UseStamina(dashStaminaCost);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        elapsedTime = 0;
    }
}
