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
	public LayerMask VoxelMask;
	
	[Space(10)]
    [Header("Attack timer")]
	[Space(5)]
    
	public int    attackTimeout = 0;
	public int    attackTimer   = 0;
	public bool   canAttack     = false;
	
	
	[Space(10)]
    [Header("Player base melee damage")]
	[Space(5)]
    
	public int       playerAttackDamage  = 1;
	public int       playerstrikingForce = 1;
	public string    playerTargetItemClass;
	
	public float     Distance            = 7.0f;
	
	
	[Space(10)]
    [Header("Attack animation")]
	[Space(5)]
    
    public float  attackAnimationSwingRate      = 8.0f;
    public float  attackAnimationRetractRate    = 3.0f;
    public float  attackCooloffTimer            = -0.9f;
    
	public float  attackAnimationX              = 1.4f;
	public float  attackAnimationY              = 1.4f;
	public float  attackAnimationZ              = 1.4f;
	
	
	
	[Space(10)]
    [Header("Pickup animation")]
	[Space(5)]
    
	public float  pickupAnimationX              = 1.4f;
	public float  pickupAnimationY              = 1.4f;
	public float  pickupAnimationZ              = 1.4f;
	
	
	
	GameObject targetObject;
	
	TickUpdate tickUpdate;
	
	bool  runAttackAnimCycle = false;
	bool  runPickupAnimCycle = false;
	
	// Animation cycle offsets
	bool  attackOffsetDirection = false;
	float attackOffset;
	
	
	
	
	
	
	
	void Update () {
        
        if (tickUpdate.isPaused) return;
        if (tickUpdate.doShowConsole) return;
        if (tickUpdate.isCrafting) return;
        
        
        // Decrement attack timer
        if (attackTimer > 0) 
            attackTimer--;
        
        
        if (Input.GetButtonDown("Attack")) {
            
            GetComponent<Use>().resetConsumeAnimation();
            
            string inHandItem = tickUpdate.inventory.checkSlot();
            
            playerAttackDamage     = 1;
            playerstrikingForce      = 1;
            playerTargetItemClass  = "";
            
            // Check current weapon melee damage amount
            for (int i=0; i < tickUpdate.weaponItem.Length; i++) {
                if (tickUpdate.weaponItem[i].name != inHandItem) 
                    continue;
                
                playerAttackDamage    = tickUpdate.weaponItem[i].attackDamage;
                playerstrikingForce   = tickUpdate.weaponItem[i].strikingForce;
                playerTargetItemClass = tickUpdate.weaponItem[i].targetItemClass;
                
            }
            
        }
        
        
        if (Input.GetButtonUp("Attack")) {
            
            resetAnimation();
            
            attackTimer = 0;
            
            transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        
        
        if (Input.GetButton("Attack")) {
            
            
            //
            // Ray check pickup objects
            
            RaycastHit hit_obj;
            Ray ray_obj = tickUpdate.cameraObject.ScreenPointToRay(Input.mousePosition);
            
            if ( Physics.Raycast(ray_obj, out hit_obj, Distance, AttackMask) ) {
                
                // Get object name
                string objectName          = hit_obj.transform.parent.transform.gameObject.name;
                GameObject newTargetObject = hit_obj.transform.parent.transform.gameObject;
                GameObject targetChunk     = hit_obj.transform.parent.transform.parent.transform.parent.transform.parent.transform.gameObject;
                ChunkTag chunkTag          = targetChunk.transform.GetChild(0).gameObject.GetComponent<ChunkTag>();
                
                // If we start selecting another object
                if (newTargetObject != targetObject) {
                    GetComponent<Use>().resetConsumeAnimation();
                    
                    targetObject = newTargetObject;
                    
                    // Reset the attack cycle
                    attackTimer = 0;
                }
                
                if (objectName == "hitbox") {
                    objectName      = hit_obj.transform.parent.transform.parent.transform.gameObject.name;
                    newTargetObject = hit_obj.transform.parent.transform.parent.transform.gameObject;
                }
                
                //
                // Attack time out
                
                if (attackTimer == 1) {
                    
                    // Remove some durability
                    if (tickUpdate.inventory.Durability[tickUpdate.hudInterface.selectedSlot] > 0) 
                        tickUpdate.inventory.Durability[tickUpdate.hudInterface.selectedSlot]--;
                    
                    attackTimer = 0;
                    
                    runAttackAnimCycle = false;
                    runPickupAnimCycle = false;
                    attackOffset = 0f;
                    
                    transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f, -1f, 2f);
                    transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    
                    // Check if the item is shatterable
                    for (int i=0; i < tickUpdate.shatterableItem.Length; i++) {
                        if (tickUpdate.shatterableItem[i].name != objectName) 
                            continue;
                        
                        // Chance to drop an item from a shatterable item
                        if (tickUpdate.shatterableItem[i].itemDrop != "") {
                            
                            if (Random.Range(0, tickUpdate.shatterableItem[i].chanceToDrop) == 0) {
                                
                                GameObject dropObject = MonoBehaviour.Instantiate( Resources.Load( tickUpdate.shatterableItem[i].itemDrop )) as GameObject;
                                dropObject.name = tickUpdate.shatterableItem[i].itemDrop;
                                
                                dropObject.transform.position = newTargetObject.transform.position;
                                dropObject.transform.parent   = newTargetObject.transform.parent;
                                
                            }
                            
                        }
                        
                        // Destroy the item
                        Destroy( newTargetObject );
                        
                        // Remove object from the structure
                        if (tickUpdate.chunkGenerator.doSavePlacedObjectsToStructure) 
                            tickUpdate.removeObjectFromStructure( hit_obj.transform.position );
                        
                        return;
                    }
                    
                    // Get max stack count
                    int maxStack=0;
                    for (int i=0; i < tickUpdate.items.Length; i++) {
                        if (tickUpdate.items[i].name != objectName) 
                            continue;
                        maxStack = tickUpdate.items[i].stackMax;
                    }
                    
                    // Pick up the item
                    Destroy( newTargetObject );
                    tickUpdate.inventory.addItem(objectName, 1, maxStack, -1);
                    tickUpdate.hudInterface.updateInHand();
                    
                    // Trigger a chunk update
                    if (chunkTag != null) {
                        chunkTag.shouldUpdateVoxels = true;
                        tickUpdate.staticOptimizeCounter = 0;
                    }
                    
                    // Remove object from the structure
                    if (tickUpdate.chunkGenerator.doSavePlacedObjectsToStructure) 
                        tickUpdate.removeObjectFromStructure( hit_obj.transform.position );
                    
                    return;
                }
                
                
                //
                // Begin attack delay
                
                if (attackTimer < 1) {
                    
                    // Get max stack count
                    int maxStack=0;
                    for (int i=0; i < tickUpdate.items.Length; i++) {
                        if (tickUpdate.items[i].name != objectName) 
                            continue;
                        
                        maxStack = tickUpdate.items[i].stackMax;
                    }
                    
                    // Check if the item requires breaking delay
                    for (int i=0; i < tickUpdate.breakableItem.Length; i++) {
                        if (tickUpdate.breakableItem[i].name != objectName) 
                            continue;
                        
                        attackTimer = tickUpdate.breakableItem[i].hardness;
                        
                        // Check target class
                        if (playerTargetItemClass == tickUpdate.breakableItem[i].itemClass) {
                            
                            attackTimer -= playerstrikingForce;
                            
                        } else {
                            
                            // Default target ALL item classes
                            if (playerTargetItemClass == "") 
                                attackTimer -= playerstrikingForce;
                        }
                        
                        runAttackAnimCycle = true;
                        runPickupAnimCycle = false;
                        
                        if (attackTimer < attackTimeout) 
                            attackTimer = attackTimeout;
                        
                        // Instant mine if debugging
                        if (tickUpdate.doDebugMode) 
                            attackTimer = 5;
                        
                        return;
                    }
                    
                    // Check if the item is shatterable
                    for (int i=0; i < tickUpdate.shatterableItem.Length; i++) {
                        if (tickUpdate.shatterableItem[i].name != objectName) 
                            continue;
                        
                        attackTimer = tickUpdate.shatterableItem[i].hardness;
                        
                        runAttackAnimCycle = true;
                        runPickupAnimCycle = false;
                        
                        if (attackTimer < attackTimeout) 
                            attackTimer = attackTimeout;
                        
                        // Instant mine if debugging
                        if (tickUpdate.doDebugMode) 
                            attackTimer = 5;
                        
                        return;
                    }
                    
                    // No delay on pick up
                    attackTimer = 0;
                    
                    runAttackAnimCycle = false;
                    runPickupAnimCycle = true;
                    
                    // Get item data
                    ItemTag itemTag = newTargetObject.GetComponent<ItemTag>();
                    
                    int durability  = -1;
                    
                    string durabilityTag = itemTag.getTag("durability");
                    if (durabilityTag != "") {
                        
                        durability = int.Parse(durabilityTag);
                    }
                    
                    // Pick up the item
                    Destroy( newTargetObject );
                    tickUpdate.inventory.addItem(objectName, 1, maxStack, durability);
                    tickUpdate.hudInterface.updateInHand();
                    
                    // Remove object from the structure
                    if (tickUpdate.chunkGenerator.doSavePlacedObjectsToStructure) 
                        tickUpdate.removeObjectFromStructure( hit_obj.transform.position );
                    
                } else {
                    
                    runAttackAnimCycle = true;
                }
                
            }
            
            
            
            
            
            //
            // Ray check attack entities
            
            RaycastHit hit_ent;
            Ray ray_entity = tickUpdate.cameraObject.ScreenPointToRay(Input.mousePosition);
            
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
            
            transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(0f, -20f, 0f);
            
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
            
            transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f - (attackOffset * pickupAnimationX),
                                                                                                                     -1f - (attackOffset * pickupAnimationY),
                                                                                                                      2f + (attackOffset * pickupAnimationZ));
            
            transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            
        }
        
        
        
        
        return;
	}
	
	
	
	// Reset animation cycle
    public void resetAnimation() {
        
        runPickupAnimCycle = false;
        
        transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f, -1f, 2f);
    }
	
	
	
	
	void Start () {
        
        tickUpdate = GameObject.Find("GameRules").GetComponent<TickUpdate>();
        
	}
	
	
}
