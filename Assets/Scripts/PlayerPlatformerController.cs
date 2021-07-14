using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7; 

    public float pushDecelerationRate = 0.25f;

    private SpriteRenderer spriteRenderer;
    private MultiAnimator multiAnimator;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        multiAnimator = new MultiAnimator(gameObject);
    }

    protected override void ComputeVelocity(){
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        multiAnimator.SetFloat("moveX", Mathf.Abs(move.x));
        

        if(Input.GetButtonDown("Jump") && grounded){
            velocity.y = jumpTakeOffSpeed;
            multiAnimator.SetTrigger("jumped");
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
        multiAnimator.SetFloat("moveY", velocity.y);
        multiAnimator.SetBool("grounded", grounded); 
    }

    protected override void HandleCollision(RaycastHit2D collision, Vector2 currentNormal)
    {
        GameObject collisionGameObject = collision.transform.gameObject;
        PhysicsObject collisionPhysicsObject = collisionGameObject.GetComponent<PhysicsObject>();

        // * Pushable object
        bool pushable = collisionPhysicsObject ? collisionPhysicsObject.IsPushable() : false; // determine if physics object and pushable

        if(pushable && currentNormal.x != 0){           // Object is pushble and player is not on top of it
            velocity.x *= pushDecelerationRate;         // slow down player when pushing objects
            collisionPhysicsObject.Movement(CalcMoveX(velocity * Time.deltaTime), false);   // push the object
        }

        base.HandleCollision(collision, currentNormal); // Handle remainder of collision normally;
    }
}
