using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7; 

    [SerializeField]
    private Animator effectAnimator; 

    private SpriteRenderer spriteRenderer;
    private Animator animator; 

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity(){
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        animator.SetFloat("moveX", Mathf.Abs(move.x));
        

        if(Input.GetButtonDown("Jump") && grounded){
            velocity.y = jumpTakeOffSpeed;
            animator.SetTrigger("jumped");
            effectAnimator.SetTrigger("jumped");
        }
        else if(Input.GetButtonUp("Jump")){
            if(velocity.y > 0)
                velocity.y = velocity.y * 0.5f; // reduce upwards velocity by half when jump is released
        }

        
        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));

        if(flipSprite)
            spriteRenderer.flipX = !spriteRenderer.flipX;
        
        

        targetVelocity = move * maxSpeed;

        
    }

    protected override void UpdateAnimationParams()
    {
        animator.SetFloat("moveY", velocity.y);
        animator.SetBool("grounded", grounded); 
    }
}
