using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool jump, jumpedAlready, lastFrameGrounded, isMoving, lastUpdateMoving, sameJumpAction;
    private float xInput;

    public float acceleration, maxSpeed, startSpeed, jumpSpeed, walkAnimationSlowdown;
    public bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        playerBody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded && !jumpedAlready && !sameJumpAction && Input.GetAxis("Jump") != 0)
        {
            jump = true;
            jumpedAlready = true;
            sameJumpAction = true;
            animator.SetTrigger("jumped");
            animator.ResetTrigger("landed");
        }

        if (Input.GetAxis("Jump") == 0)
        {
            sameJumpAction = false;
        }

        if (!lastFrameGrounded && isGrounded)
        {
            // landed
            jumpedAlready = false;
            animator.SetTrigger("landed");
            animator.ResetTrigger("jumped");
            animator.ResetTrigger("falling");
        }

        lastFrameGrounded = isGrounded;

    }

    private void FixedUpdate()
    {
        xInput = Input.GetAxis("Horizontal");
        if (xInput != 0)
        {
            playerBody.AddForce(new Vector2(acceleration * xInput, 0));
            isMoving = true;
        }

        if (lastUpdateMoving != isMoving)
        {
            playerBody.velocity = new Vector2(startSpeed * Mathf.Sign(xInput), playerBody.velocity.y);
        }

        if (Mathf.Abs(playerBody.velocity.x) > maxSpeed)
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x > 0 ? maxSpeed : (-1) * maxSpeed, playerBody.velocity.y);
        }

        if (jump)
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpSpeed);
            jump = false;
        }


        if (playerBody.velocity.x != 0)
        {
            animator.SetBool("moving", true);
            animator.speed = (Mathf.Abs(playerBody.velocity.x) * walkAnimationSlowdown / maxSpeed) + (1f - walkAnimationSlowdown);
            isMoving = true;
        } 
        else
        {
            animator.SetBool("moving", false);
            animator.speed = 1;
            isMoving = false;
        }

        lastUpdateMoving = isMoving;
        
        if (!isGrounded && playerBody.velocity.y < 0)
        {
            animator.SetTrigger("falling");
        }


        // facing
        spriteRenderer.flipX = playerBody.velocity.x < 0;
    }
}

