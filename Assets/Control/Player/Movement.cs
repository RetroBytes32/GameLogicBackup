using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
	
	public CharacterController controller;
	public float Speed       = 7.0f;
	public float Gravity     = 70.0f;
	public float JumpHeight  = 2.5f;
	
	public Transform  groundCheck;
	public float      groundDistance = 0.1f;
	public LayerMask  groundMask;
	
	public bool  readyToJump   = false;
	public float jumpTimer     = 0;
	public float jumpCoolDown  = 2;
	
	public bool isGrounded = false;
	
	public Transform PlayerSourceTransform;
	
	public Vector3 Velocity;
	
	
	public TickUpdate tickUpdate;
	
	
	
	
	
	void Update () {
        
        // Calculate forward movement
        Vector3 movement = new Vector3(0f, 0f, 0f);
        
        if (tickUpdate.doShowConsole) 
            return;
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        movement = (PlayerSourceTransform.transform.right * x) + (PlayerSourceTransform.transform.forward * z);
        
        // DEBUG "creative" mode
        if (tickUpdate.doDebugMode) {
            if (Input.GetButton("Jump")) 
                movement.y = 0.87f;
            
            if (Input.GetButton("Crouch")) 
                movement.y -= 0.87f;
            
            controller.Move(movement * (Speed * 3.5f) * Time.deltaTime);
            
            return;
        }
        
        // Directional movement
        controller.Move(movement * Speed * Time.deltaTime);
        
        
        // Remove hunger saturation when moving
        if (controller.velocity != Vector3.zero) {
            if (Random.Range(0, 20) == 1) 
                tickUpdate.inventory.removeSaturation(1);
        }
        
        
        // Check if we are on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        // Apply gravity when the player is off ground
        if ((readyToJump) & (isGrounded)) {
            
            if ((readyToJump) & (Input.GetButton("Jump"))) {
                
                Velocity.y = JumpHeight;
                
                isGrounded = false;
                readyToJump=false;
                
                tickUpdate.inventory.removeSaturation(3);
                
            } else {
                
                Velocity.y = 0f;
            }
            
        } else {
            
            // Apply gravity
            Velocity.y += -(Gravity * Time.deltaTime);
            
        }
        
        // Jump cool down timer
        if (!readyToJump) {
            
            jumpTimer += 1f;
            
            if (jumpTimer > jumpCoolDown) {
                jumpTimer=0;
                
                readyToJump=true;
            }
        }
        
        controller.Move(Velocity * Time.deltaTime);
        
        return;
	}
	
	
	
	
	
	void Start() {
        
        tickUpdate = GameObject.Find("GameRules").GetComponent<TickUpdate>();
        
	}
	
	
}







