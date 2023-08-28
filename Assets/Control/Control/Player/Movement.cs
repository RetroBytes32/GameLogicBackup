using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
	
	public CharacterController controller;
	public float Speed         = 10f;
	public float Gravity       = 0.5f;
	public float JumpForce     = 3f;
	
    public float flyCamSpeed                = 7f;
	public float flyCamAcceleration         = 4f;
    public float flyCamMimimumSpeedCutoff   = 0.1f;
    public float flyCamDamping              = 0.9f;
    
    public Transform  groundCheck;
	public float      groundDistance = 0.5f;
	public LayerMask  groundMask;
	
	public bool  readyToJump   = false;
	public int   jumpTimer     = 0;
	public int   jumpCoolDown  = 7;
	
	public bool isGrounded    = false;
	
	public Transform PlayerSourceTransform;
	
	public Vector3 Velocity;
	
	public float  camearFov = 60f;
	public float  fovOffset = 0;
	
	public TickUpdate tickUpdate;
	
	public bool isDoubleFast = false;
	
	
	
	void Update () {
        
        //
        // Normal mode
        //
        
        if (!tickUpdate.doDebugMode) {
            
            float doubleFast   = 1f;
            
            Vector3 Movement = new Vector3(0f, 0f, 0f);
            
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            
            // Check turn off double speed
            if (!Input.GetKey(KeyCode.W)) 
                isDoubleFast = false;
            
            if (!tickUpdate.doShowConsole) {
                
                // Check forward double fast
                if (Input.GetKeyDown(KeyCode.LeftControl)) {
                    
                    isDoubleFast = true;
                    doubleFast = 2.8f;
                }
                
                // Camera FOV
                if (isDoubleFast) {
                    if (fovOffset < 6f) 
                        fovOffset += 2;
                } else {
                    if (fovOffset > 0f) 
                        fovOffset -= 2f;
                }
                
                tickUpdate.cameraObject.fieldOfView = camearFov + fovOffset;
                tickUpdate.Player.transform.GetChild(0).transform.GetChild(0).transform.gameObject.GetComponent<Camera>().fieldOfView = camearFov + fovOffset;
                
                
                Movement = (PlayerSourceTransform.transform.right * x) + (PlayerSourceTransform.transform.forward * z);
                // Apply movement force to the player controller
                controller.Move(Movement * Speed * doubleFast * Time.deltaTime);
                
                // Start a jump when the player is on the ground
                if ((Input.GetKey(KeyCode.Space)) & (isGrounded)) {
                    
                    // Jump begin
                    if (readyToJump) {
                        
                        Velocity.y = JumpForce;
                        
                        isGrounded  = false;
                        readyToJump = false;
                        
                        // Start the jump
                        jumpTimer = 1;
                    }
                    
                }
                
            }
            
            
            // Apply gravity
            if (!isGrounded) {
                
                Velocity.y += -Gravity;
                
            } else {
                
                Velocity.y = 0f;
            }
            
            
            // Jump cool down timer
            if (jumpTimer > 0) {
                jumpTimer ++;
                
                if (jumpTimer > jumpCoolDown) {
                    jumpTimer = 0;
                    
                    readyToJump = true;
                }
            }
            
            
            // Apply final velocity
            controller.Move(Velocity * Speed * Time.deltaTime);
            
            
            
        //
        // Fly cam mode
        //
        } else {
            
            float doubleFast = 1f;
            
            Vector3 Axis = new Vector3(0f, 0f, 0f);
            
            if (!tickUpdate.doShowConsole) {
                
                // Check forward double fast
                if (Input.GetKeyDown(KeyCode.LeftControl)) 
                    isDoubleFast = true;
                
                // Check directional velocity
                if (Input.GetKey(KeyCode.A))           Axis.x = -1f;
                if (Input.GetKey(KeyCode.D))           Axis.x =  1f;
                if (Input.GetKey(KeyCode.Space))       Axis.y =  1f;
                if (Input.GetKey(KeyCode.LeftShift))   Axis.y = -1f;
                if (Input.GetKey(KeyCode.S))           Axis.z = -1f;
                if (Input.GetKey(KeyCode.W))          {Axis.z =  1f;} else {
                    
                    // Turn off double speed
                    isDoubleFast = false;
                }
                
                // Check double fast
                if (isDoubleFast) 
                    doubleFast = 3f;
            }
            
            // Apply movement force to the player controller
            if (Axis != new Vector3(0f, 0f, 0f)) {
                Vector3 newVelocity = ((PlayerSourceTransform.transform.right * Axis.x) + (PlayerSourceTransform.transform.forward * Axis.z)) * 
                                       flyCamAcceleration * 
                                       doubleFast;
                
                Velocity   += newVelocity;
                Velocity.y += (Axis.y * flyCamAcceleration) * (doubleFast * 0.8f);
            }
            
            // Camera FOV
            if (isDoubleFast) {
                if (fovOffset < 9f) 
                    fovOffset += 3;
            } else {
                if (fovOffset > 0f) 
                    fovOffset -= 3f;
            }
            
            tickUpdate.cameraObject.fieldOfView = camearFov + fovOffset;
            tickUpdate.Player.transform.GetChild(0).transform.GetChild(0).transform.gameObject.GetComponent<Camera>().fieldOfView = camearFov + fovOffset;
            
            
            // Maximum speed cutoff
            if (Velocity.x >  flyCamSpeed) Velocity.x =  flyCamSpeed;
            if (Velocity.x < -flyCamSpeed) Velocity.x = -flyCamSpeed;
            if (Velocity.y >  flyCamSpeed) Velocity.y =  flyCamSpeed;
            if (Velocity.y < -flyCamSpeed) Velocity.y = -flyCamSpeed;
            if (Velocity.z >  flyCamSpeed) Velocity.z =  flyCamSpeed;
            if (Velocity.z < -flyCamSpeed) Velocity.z = -flyCamSpeed;
            
            // Damping
            if (Velocity.x > 0f) Velocity.x *= flyCamDamping;
            if (Velocity.x < 0f) Velocity.x *= flyCamDamping;
            if (Velocity.y > 0f) Velocity.y *=(flyCamDamping * 0.95f);
            if (Velocity.y < 0f) Velocity.y *=(flyCamDamping * 0.95f);
            if (Velocity.z > 0f) Velocity.z *= flyCamDamping;
            if (Velocity.z < 0f) Velocity.z *= flyCamDamping;
            
            
            controller.Move(Velocity * Time.deltaTime);
            
            // Minimal speed cutoff
            if ((Velocity.x > -flyCamMimimumSpeedCutoff) & (Velocity.x < flyCamMimimumSpeedCutoff)) Velocity.x = 0f;
            if ((Velocity.y > -flyCamMimimumSpeedCutoff) & (Velocity.y < flyCamMimimumSpeedCutoff)) Velocity.y = 0f;
            if ((Velocity.z > -flyCamMimimumSpeedCutoff) & (Velocity.z < flyCamMimimumSpeedCutoff)) Velocity.z = 0f;
            
        }
        
        return;
	}
	
	
	
	
	
	void Start() {
        
        tickUpdate = GameObject.Find("GameRules").GetComponent<TickUpdate>();
        
	}
	
	
}







