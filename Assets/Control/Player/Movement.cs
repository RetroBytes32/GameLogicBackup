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
	  float x = Input.GetAxis("Horizontal");
	  float z = Input.GetAxis("Vertical");
	  
	  Vector3 movement = (PlayerSourceTransform.transform.right * x) + (PlayerSourceTransform.transform.forward * z);
	  
	  controller.Move(movement * Speed * Time.deltaTime);
	  
	  // Remove hunger saturation when moving
	  if (controller.velocity != Vector3.zero) {
	    if (Random.Range(0, 20) == 1) 
	      tickUpdate.inventory.removeSaturation(1);
	  }
	  
	  
	  // Apply gravity when the player is off ground
	  isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
	  
	  if (!readyToJump) {
	    
	    jumpTimer+=1f;
	    if (jumpTimer > jumpCoolDown) {
	      jumpTimer=0;
	      readyToJump=true;
	    }
	  }
	  
	  
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
	    
	    Velocity.y += -(Gravity * Time.deltaTime);
	    
	  }
	  
	  
	  // Apply gravity
	  controller.Move(Velocity * Time.deltaTime);
	  
	  return;
	}
	
	
	
	
	
	void Start() {
	  
	  tickUpdate = GameObject.Find("GameRules").GetComponent<TickUpdate>();
	  
	}
	
	
}







