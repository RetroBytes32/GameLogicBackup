using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour {
	
	[Space(10)]
    [Header("Masks")]
	[Space(5)]
    
	public LayerMask AttackMask;
	public LayerMask EntityMask;
	public LayerMask GroundMask;
	
	
	[Space(10)]
    [Header("Attack timer")]
	[Space(5)]
    
	public int    attackTimeout = 0;
	public int    attackTimer   = 0;
	public bool   canAttack     = false;
	
	
	[Space(10)]
    [Header("Player base melee damage")]
	[Space(5)]
    
	public int    playerAttackDamage = 1;
	
	public float  Distance  = 7.0f;
	
	
	[Space(10)]
    [Header("Attack animation")]
	[Space(5)]
    
    public float  attackAnimationSwingRate      = 8.0f;
    public float  attackAnimationRetractRate    = 3.0f;
    public float  attackCooloffTimer            = -0.9f;
    
	public float  attackAnimationX              = 1.4f;
	public float  attackAnimationY              = 1.4f;
	public float  attackAnimationZ              = 1.4f;
	
	
	
	
	
	Interface interfaceScript;
	
	Inventory inventory;
	
	Camera cameraObject;
	
	TickUpdate tickUpdate;
	
	bool  runAttackAnimCycle = false;
	bool  runPickupAnimCycle = false;
	
	// Animation cycle offsets
	bool  attackOffsetDirection = false;
	float attackOffset;
	
	
	
	
	
	
	
	void Update () {
        
        if (tickUpdate.isPaused) return;
        if (tickUpdate.doShowConsole) return;
        
        
        // Decrement attack timer
        if (attackTimer > 0) 
            attackTimer--;
        
        
        if (Input.GetButtonDown("Attack")) {
            
            GetComponent<Use>().resetConsumeAnimation();
            
            string inHandItem = inventory.checkSlot();
            
            playerAttackDamage = 1;
            
            // Check current weapon melee damage amount
            for (int i=0; i < tickUpdate.weaponItem.Length; i++) {
                if (tickUpdate.weaponItem[i].name != inHandItem) 
                    continue;
                
                playerAttackDamage = tickUpdate.weaponItem[i].damage;
                
            }
            
        }
        
        
        if (Input.GetButtonUp("Attack")) {
            
            GetComponent<Use>().resetConsumeAnimation();
            
            attackTimer = 0;
            
            transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        
        
        if (Input.GetButton("Attack")) {
            
            
            //
            // Ray check pickup objects
            
            RaycastHit hit_obj;
            Ray ray_obj = cameraObject.ScreenPointToRay(Input.mousePosition);
            
            if ( Physics.Raycast(ray_obj, out hit_obj, Distance, AttackMask) ) {
                
                // Get object name
                string objectName = hit_obj.transform.parent.transform.gameObject.name;
                
                //
                // Attack time out
                
                if (attackTimer == 1) {
                    
                    attackTimer = 0;
                    
                    runAttackAnimCycle = false;
                    runPickupAnimCycle = true;
                    
                    // Pick up the item
                    Destroy( hit_obj.transform.parent.transform.gameObject );
                    inventory.addItem(objectName, 1);
                    interfaceScript.updateInHand();
                    
                    return;
                }
                
                
                //
                // Begin attack delay
                
                if (attackTimer < 1) {
                    
                    // Check if the item requires breaking delay
                    for (int i=0; i < tickUpdate.breakableItem.Length; i++) {
                        if (tickUpdate.breakableItem[i].name != objectName) 
                            continue;
                        
                        runAttackAnimCycle = true;
                        runPickupAnimCycle = false;
                        
                        // Apply attack damage timer
                        attackTimer = tickUpdate.breakableItem[i].hardness - playerAttackDamage;
                        
                        if (attackTimer < attackTimeout) 
                            attackTimer = attackTimeout;
                        
                        return;
                    }
                    
                    // No delay on pick up
                    attackTimer = 0;
                    
                    runAttackAnimCycle = false;
                    runPickupAnimCycle = true;
                    
                    // Pick up the item
                    Destroy( hit_obj.transform.parent.transform.gameObject );
                    inventory.addItem(objectName, 1);
                    interfaceScript.updateInHand();
                    
                } else {
                    
                    runAttackAnimCycle = true;
                }
                
            }
            
            
            
            
            
            //
            // Ray check attack entities
            
            RaycastHit hit_ent;
            Ray ray_entity = cameraObject.ScreenPointToRay(Input.mousePosition);
            
            if ( Physics.Raycast(ray_entity, out hit_ent, Distance, EntityMask) ) {
                
                string objectName = hit_ent.transform.gameObject.name;
                
                
                //
                // Attack entity then delay
                
                if (attackTimer <= 0) {
                    attackTimer = attackTimeout;
                }
                
                if (attackTimer == attackTimeout) {
                    
                    runAttackAnimCycle = true;
                    
                    EntityTag entityTag = hit_ent.transform.gameObject.GetComponent<EntityTag>();
                    ActorTag  actorTag  = hit_ent.transform.gameObject.GetComponent<ActorTag>();
                    
                    if (entityTag == null) return;
                    if (actorTag == null) return;
                    
                    //
                    // Calculate and apply attack damage
                    
                    entityTag.Health -= playerAttackDamage;
                    
                    actorTag.isInPain = true;
                    actorTag.targetEntity = gameObject;
                    
                    actorTag.fear   = 40;
                    actorTag.stress = 100;
                    
                    if (actorTag.chanceToAttackPlayer > 0f) 
                        actorTag.AI_attack_player(20);
                    
                    if (entityTag.Health < 0) {
                        
                        if (!actorTag.isDying) 
                            actorTag.isDying = true;
                        
                    }
                    
                }
                
            }
            
        }
        
        
        
        
        
        
        
        //
        // Attack animation cycle
        
        if (runAttackAnimCycle == true) {
            
            // Switch from swing to retract
            if (attackOffsetDirection == false) {
                // Swing rate
                attackOffset += attackAnimationSwingRate;
            } else {
                // Retract rate
                attackOffset -= attackAnimationRetractRate;
            }
            
            // Retract back
            if (attackOffset < 0f) {
                
                if (attackOffset < attackCooloffTimer) {
                    attackOffset = 0f;
                    attackOffsetDirection = false;
                    runAttackAnimCycle    = false;
                }
                
                transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f, -1f, 2f);
                transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                
                return;
            }
            
            // Swing out
            if (attackOffset > 1.1f) {
                attackOffset = 1.1f;
                attackOffsetDirection = true;
            }
            
            transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(0f, -40f, 0f);
            
            transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f - (attackOffset * attackAnimationX ),
                                                                                                                     -1f - (attackOffset * attackAnimationY ), 
                                                                                                                      2f + (attackOffset * attackAnimationZ ));
            
            
        }
        
        
        
        //
        // Item pickup animation cycle
        
        if (runPickupAnimCycle == true) {
            
            attackOffset += 0.8f;
            
            
            if (attackOffset > 3f) {
                attackOffset = 3f;
                attackOffsetDirection = false;
                
                transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f, -1f, 2f);
                
                runPickupAnimCycle = false;
                attackOffset = 0f;
                return;
            }
            
            transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f - (attackOffset * 0.5f), -1f - (attackOffset * -0.2f), 2f + (attackOffset * 1.4f));
            
            transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            
        }
        
        
        
        
        return;
	}
	
	
	
	
	void Start () {
        
        tickUpdate = GameObject.Find("GameRules").GetComponent<TickUpdate>();
        
        inventory = GameObject.Find("HUD").GetComponent<Inventory>();
        interfaceScript = GameObject.Find("HUD").GetComponent<Interface>();
        
        cameraObject = GameObject.Find("Main Camera").GetComponent<Camera>();
        
        
	}
	
	
}
