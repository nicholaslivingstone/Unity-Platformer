using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float minGroundNormalY = 0.65f;
    public float gravityModifier = 1f;
    protected Vector2 targetVelocity; 
    protected bool grounded; 
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f; 
    protected const float shellRadius = 0.01f; // bit of padding to prevent object from passing into another collider

    void OnEnable() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start() {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask (gameObject.layer)); // collide with what's on the objects layer
                                                                                        // Uses physics2d settings 
        contactFilter.useLayerMask = true;                                                                                 
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity(){

    }

    protected virtual void UpdateAnimationParams(){

    }
    
    // Since we're doing physics...
    private void FixedUpdate() {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime; // Apply gravity to velocity
        velocity.x = targetVelocity.x;

        grounded = false; // register as false until we've collided this frame

        Vector2 deltaPosition = velocity * Time.deltaTime; // Change in position

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x); // perpendicular to the ground normal

        // X Movement
        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement(move, false);

        // Y movement
        move = Vector2.up * deltaPosition.y; 
        Movement(move, true);

        UpdateAnimationParams();
    }

    /// <summary>
    /// Apply movement to rigid body
    /// </summary>
    /// <param name="move"> Change in movment </param>
    void Movement(Vector2 move, bool yMovement){
        
        float distance = move.magnitude;

        // Collisions
        if(distance > minMoveDistance){// Only check for collisions when we're attempting to move a min Distance
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for(int i = 0; i < count; i++){
                hitBufferList.Add(hitBuffer[i]); // list of objects that are overlapping with the objects collider
            }

            foreach(RaycastHit2D collision in hitBufferList)
            {
                Vector2 currentNormal = collision.normal; 

                // Check if grounded, based on minimum y angle
                if(currentNormal.y > minGroundNormalY)
                {
                    grounded = true; 
                    if(yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0; 
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal); // get the difference between the velocity and current normal
                if(projection < 0)
                {
                    velocity = velocity - projection * currentNormal; // cancel out the part of our velocity that would be stopped by the current collision 
                }

                // this bit will prevent us from accidentally entering another collider
                float modifiedDistance = collision.distance - shellRadius; // distance to the raycast object 
                distance = modifiedDistance < distance ? modifiedDistance : distance; // modify the distance only if the distance would cause us to stick into a wall or something

                
            }
        }

        rb2d.position = rb2d.position + move.normalized * distance;
        
    }
}