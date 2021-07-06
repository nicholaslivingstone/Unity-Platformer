using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void ComputeVelocity(){
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if(Input.GetButtonDown("Jump") && grounded){
            velocity.y = jumpTakeOffSpeed;
        }
        else if(Input.GetButtonUp("Jump")){
            if(velocity.y > 0)
                velocity.y = velocity.y * 0.5f; // reduce upwards velocity by half when jump is released
        }

        targetVelocity = move * maxSpeed;
    }
}
