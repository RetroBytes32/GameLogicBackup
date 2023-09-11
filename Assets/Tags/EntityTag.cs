using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTag : MonoBehaviour {
	
	public GameObject currentChunk;
	
	public string Name = "Sheep";
	
	
	[Space(3)]
	[Header("Vitality")]
	[Space(10)]
	
	public float  Health       = 4f;
	public float  Armor        = 0f;
	
	public float  AttackDamage = 3f;
	
	public int    Age          = 0;
    public float  Speed        = 2f;
	
	
	[Space(3)]
	[Header("Orientation")]
	[Space(10)]
	
	public Vector3 Position;
	public Vector3 Rotation;
	public Vector3 Facing;
	
	public Vector3 RotateTo = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 LookAt   = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 Focus    = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 Velocity = new Vector3(0.0f, 0.0f, 0.0f);
	
	
	[Space(3)]
	[Header("Movement")]
	[Space(10)]
	
	public float  HeadSnapSpeed     = 5.0f;
	public float  BodySnapSpeed     = 3.0f;
	public float  SpeedMultiplier   = 2.5f;
	
	
	[Space(3)]
	[Header("States")]
	[Space(10)]
	
	public bool   isWalking    = false;
	public bool   isRunning    = false;
	public bool   isRotating   = false;
	public bool   isFacing     = true;
	public bool   isGrounded   = false;
	
	[Space(3)]
	
	public bool   isGenetic    = false;
	public bool   useAI        = false;
	
	
	[Space(3)]
	[Header("Attack cooloff timer")]
	[Space(10)]
	
	public float  AttackTimout  = 80f;
	
	
	[Space(3)]
	[Header("Body transforms")]
	[Space(10)]
	
	public Transform Head;
	public Transform Body;
	public Transform LegLeft;
	public Transform LegRight;
	public Transform ArmLeft;
	public Transform ArmRight;
	
	
	[Space(3)]
	[Header("Physics")]
	[Space(10)]
	
	public float Gravity       = 70.0f;
	public float JumpHeight    = 2.5f;
	
	
	[Space(3)]
	[Header("Controller")]
	[Space(10)]
	
	public CharacterController  controller;
	
	public float      groundDistance = 0.1f;
	public Transform  groundCheck;
	public LayerMask  groundMask;
	
	
	[Space(30)]
	
	public float  AttackCounter = 0f;
	
	
	
	
	
	
	
	
	
	
	public void physicsUpdate() {
	  
	}
	
	
	
	
	void Update() {
        
        if (!gameObject.activeInHierarchy) 
            return;
        
        if (AttackCounter > 0) AttackCounter--;
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        
        //
        // Gravity check
        
        if (!isGrounded) {
            
            controller.Move( -Vector3.up * Gravity * Time.deltaTime);
            
        }
        
        //controller.velocity = Vector3.ClampMagnitude(controller.velocity, 0.9f);
        
        
        
        //
        // Entity movement
        
        if (isWalking) {
            
            if (isRunning) {
                
                controller.Move((transform.forward * Speed * Time.deltaTime * SpeedMultiplier));
                
            } else {
                
                controller.Move((transform.forward * Speed * Time.deltaTime));
                
            }
            
            
        }
        
        
        // Turn body
        if (Rotation.y != RotateTo.y) {
            
            if (Rotation.y < RotateTo.y) {
                Rotation.y += BodySnapSpeed * Time.deltaTime * 50f;
                if (Rotation.y > RotateTo.y) {Rotation.y = RotateTo.y;}
            } else {
                Rotation.y -=  BodySnapSpeed * Time.deltaTime * 50f;
                if (Rotation.y < RotateTo.y) {Rotation.y = RotateTo.y;}
            }
            
            this.transform.localRotation = Quaternion.Euler(Rotation);
            
            
            //Vector3 targetPoint = Focus - this.transform.position;
            //Vector3 newRotation = Vector3.RotateTowards(Rotation, targetPoint, BodySnapSpeed * Time.deltaTime, 1f);
            //newRotation.y = 0f;
            //this.transform.rotation = Quaternion.LookRotation( newRotation );
            
        }
        
        
        // Turn head
        if (Facing.y != LookAt.y) {
            
            if (Facing.y < LookAt.y) {
                Facing.y += HeadSnapSpeed;
                if (Facing.y > LookAt.y) {Facing.y = LookAt.y;}
            } else {
                Facing.y -=  HeadSnapSpeed;
                if (Facing.y < LookAt.y) {Facing.y = LookAt.y;}
            }
            
            Head.transform.localRotation = Quaternion.Euler(Facing);
            
        }
        
        
        if (isRotating) {
            
            isRotating = false;
            
            float xx = this.transform.position.x - Focus.x;
            float zz = this.transform.position.z - Focus.z;
            
            if (isFacing) {
                RotateTo.y = (float)((System.Math.Atan2(xx, zz) / System.Math.PI) * 180f) - 180f;
            } else {
                RotateTo.y = (float)((System.Math.Atan2(xx, zz) / System.Math.PI) * 180f);
            }
            
            
        }
        
        
	}
	
	
	
	
	
	
}








