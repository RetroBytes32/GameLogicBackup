﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public struct LootTable {
    
	[Space(10)]
	[Header("Item to drop")]
	[Space(3)]
    
	public string name;
    
    
	[Space(10)]
	[Header("Numbers of items to drop")]
	[Space(3)]
    
	public int min;
	public int max;
    
	[Space(10)]
	[Header("Item lifetime")]
	[Space(3)]
    
    public int lifeTime;
    
}




public class ActorTag : MonoBehaviour {
    
	public EntityTag  entityTag;
	public bool       isActive;
    
    
    // AI parameters
    
	[Space(10)]
	[Header("AI personality parameters")]
	[Space(3)]
    
	public float chanceToChangeDirection  = 0.1f;
	public float chanceToFocusOnPlayer    = 0.08f;
	public float chanceToFocusOnEntity    = 0.05f;
	public float chanceToAttackPlayer     = 0f;
    public float chanceToWalk             = 0.2f;
    
	public float distanceToFocusOnPlayer  = 10f;
	public float distanceToFocusOnEntity  = 10f;
	public float distanceToAttackPlayer   = 0f;
	public float distanceToWalk           = 20f;
    
	public float heightPreferenceMin      = 0f;
	public float heightPreferenceMax      = 40f;
    
	public float distanceToAttack         = 14f;
	public float distanceToFlee           = 14f;
    
    
    //
    // Timers
    //
    
    public int directionChangeCoolDown  = 10;
    public int observationCoolDown      = 10;
    
    
    
    [Space(10)]
	[Header("AI emotion states")]
	[Space(3)]
    
	public int love    = 0;
	public int stress  = 0;
	public int hunger  = 0;
	
	
	
	[Space(10)]
	[Header("AI memories")]
	[Space(3)]
    
    public string memories;
    
    
    
    
	[Space(10)]
	[Header("Pray")]
	[Space(3)]
    
	public string[]  AttackEntities;
    
    
    
	[Space(10)]
	[Header("Predator")]
	[Space(3)]
    
	public string[]  FleeFromEntities;
    
    
    
	[Space(10)]
	[Header("Item drops")]
	[Space(3)]
    
	public LootTable[]  DropOnDeath;
    
    
    
	// Animation
    
	[Space(10)]
	[Header("Animation")]
	[Space(3)]
    
    public int   limbAxis          = 0;
	public bool  limbAxisInvert    = false;
	public float limbCycleRate     = 50.0f;
	public float limbCycleRange    = 20.0f;
	
	float walkCycleAnim = 0f;
	bool  walkDirectionAnim = false;
    
    
    
	// State
    
	[Space(10)]
	[Header("AI state")]
	[Space(3)]
    
	public bool  isDying        = false;
	public bool  isInPain       = false;
	
	public bool  isAttacking    = false;
	public bool  isFleeing      = false;
    public bool  isAvoiding     = false;
    
    
    
	[Space(10)]
	[Header("Consumption")]
	[Space(3)]
    
	public bool  isConsuming     = false;
	public int   consumptionTimer = 40;
    
    
    
	[Space(10)]
	[Header("Entity in focus")]
	[Space(3)]
    
	public GameObject targetEntity = null;
    
    
    
	[Space(10)]
	[Header("Body transforms")]
	[Space(3)]
    
	public Transform Head;
	public Transform Body;
	public Transform ArmLeft;
	public Transform ArmRight;
	public Transform LegLeft;
	public Transform LegRight;
    
    
    [Space(10)]
	[Header("Navigation")]
	[Space(3)]
    
    public float avoidanceDistance   = 5f;
    public float avoidanceHeight     = 0.8f;
    public bool  avoidanceDirection  = false;
    
    public LayerMask groundLayerMask;
    
    
    
    
    
    [Space(10)]
	[Header("Counters")]
	[Space(3)]
    
	public float deathCounter=0f;
	public int   changeDirectionCounter=0;
    public int   consumeTimer=0;
	
	int        directionChangeCoolDownTimer=0;
    int        observationCoolDownTimer=0;
    
	
    
    
    
    
    
    
	public void updateAI() {
        
        //
        // Consumption timer
        
        if (isConsuming) {
            
            consumeTimer++;
            if (consumeTimer > consumptionTimer) {
                
                consumeTimer = 0;
                
                if (transform.GetChild(1).transform.GetChild(1).transform.childCount > 0)
                    Destroy(transform.GetChild(1).transform.GetChild(1).transform.GetChild(0).transform.gameObject);
                
                isConsuming = false;
            }
            
        }
        
        
        //
        // Equalize emotional states
        if (Random.Range(0, 10) > 7) {
            if (love >  0)   love --;
            if (love < -0)   love ++;
            
            if (stress >  0) stress --;
            if (stress < -0) stress ++;
            
            if (hunger > -100) hunger++;
        }
        
        
        //
        // Update AI memories
        
        int intelligenceQuota = 200;
        
        processMemories(intelligenceQuota);
        
        
        //
        // Check object avoidance while moving
        //
        
        if (entityTag.isWalking) 
            AI_check_avoidance();
        
        
        
        //
        // Attack profile - high level
        //
        
        if ((isAttacking) & (targetEntity != null)) {
            
            float distanceToEntity = Vector3.Distance(this.transform.position, targetEntity.transform.position);
            
            // Check if the target has unloaded
            if ((!targetEntity.activeInHierarchy) | 
                (distanceToEntity > distanceToFocusOnEntity)) {
                
                targetEntity = null;
                
                isAttacking = false;
                isFleeing   = false;
                
                entityTag.isRotating = false;
                
                AI_stop_moving();
                
                resetAnimation();
                
                return;
            }
            
            entityTag.Focus = targetEntity.transform.position;
            entityTag.isRotating = true;
            
            AI_start_running();
            
            return;
        }
        
        
        
        
        //
        // Chance to attack player
        
        if ((chanceToAttackPlayer > 0f) & (hunger > 0)) {
            
            if (Random.Range(0f, 1f) < chanceToAttackPlayer) {
                
                AI_attack_player(distanceToAttackPlayer);
                
                return;
            }
            
        }
        
        
        
        
        //
        // Check attackable entities
        
        if ((AttackEntities.Length > 0) & (hunger > 0)) {
            
            for (int i=0; i < AttackEntities.Length; i++) {
                
                if (AI_attack_nearest_entity_type( AttackEntities[i] ))
                    break;
            }
            
        }
        
        
        
        
        //
        // Check entities to flee from
        
        if (FleeFromEntities.Length > 0) {
            
            for (int i=0; i < FleeFromEntities.Length; i++) {
                
                if (AI_run_from_nearest_entity_type( FleeFromEntities[i] )) {
                    
                    if (stress < 100) stress = 100;
                    
                    break;
                }
                
            }
            
        }
        
        
        
        
        //
        // Do not make low level decisions 
        // during attack or flee state
        
        if ((isAttacking) | (isFleeing)) 
            return;
        
        
        
        //
        // Check internal decision making process
        
        // Chance to change direction
        directionChangeCoolDownTimer++;
        if (directionChangeCoolDownTimer > directionChangeCoolDown) {
            
            if (entityTag.isWalking)
                if (Random.Range(0, 10) > 3) 
                    return;
            
            if (Random.Range(0f, 1f) < chanceToChangeDirection) {
                directionChangeCoolDownTimer=0;
                
                if (entityTag.doWalkSideways) {
                    if ((Random.Range(0, 10)) > 4) {
                        entityTag.WalkSidewaysRight = true;
                    } else {
                        entityTag.WalkSidewaysRight = false;
                    }
                }
                
                if (entityTag.isWalking) {
                    
                    if (Random.Range(0, 10) > 8) 
                        AI_change_direction();
                    
                } else {
                    AI_change_direction();
                }
                
            }
            
            // Chance to start walking
            if (Random.Range(0f, 1f) < chanceToWalk) {
                directionChangeCoolDownTimer=0;
                if (AI_change_direction()) 
                    AI_start_walking();
            }
            
        }
        
        
        observationCoolDownTimer++;
        if (observationCoolDownTimer > observationCoolDown) {
            
            // Chance to focus on the nearest entity
            if (Random.Range(0f, 1f) < chanceToFocusOnEntity) 
                AI_focus_nearest_entity();
            
            // Chance to focus on the player
            if (Random.Range(0f, 1f) < chanceToFocusOnPlayer) 
                AI_focus_player(distanceToFocusOnPlayer);
            
            observationCoolDownTimer=0;
        }
        
        
        
        
        
        
        
        return;
	}
    
    
    
    
    
    
    
    
    //
    // AI actions
    
	public void AI_start_walking() {
        entityTag.isWalking  = true;
        entityTag.isRunning  = false;
	}
    
	public void AI_start_running() {
        entityTag.isWalking  = true;
        entityTag.isRunning  = true;
	}
    
	public void AI_stop_moving() {
        entityTag.isWalking  = false;
        entityTag.isRunning  = false;
	}
    
    
    
    
    
    
    public bool AI_check_avoidance() {
        
        float xx = this.transform.position.x;
        float yy = this.transform.position.y + avoidanceHeight + 1f;
        float zz = this.transform.position.z;
        Vector3 from = new Vector3(xx, yy, zz);
        
        RaycastHit hit_obj;
        Ray ray_obj = new Ray(from, this.transform.forward);
        
        if (!Physics.Raycast(ray_obj, out hit_obj, avoidanceDistance, groundLayerMask) ) {
            isAvoiding = false;
            Debug.DrawRay(from, this.transform.forward * avoidanceDistance, Color.white, 0.1f);
            return false;
        }
        
        if (!isAvoiding) {
            
            avoidanceDirection = false;
            
            if (Random.Range(0, 10) > 4) 
                avoidanceDirection = true;
            
        }
        
        for (int i=0; i < 5; i++) {
            Vector3 debugRayFrom = this.transform.position + (this.transform.forward * (avoidanceDistance / i));
            Vector3 debugRayTo   = this.transform.position + (this.transform.forward * (avoidanceDistance / i));
            
            debugRayTo.y += 3;
            
            Debug.DrawLine(debugRayFrom, debugRayTo, Color.red, 3f);
            Debug.DrawLine(debugRayFrom, debugRayTo, Color.red, 3f);
        }
        
        
        if (AI_change_direction()) 
            isAvoiding = true;
        
        return true;
    }
    
    
    
    
	public bool AI_change_direction() {
        
        Vector3 chunkPos  = transform.parent.transform.parent.transform.position;
        
        float xx = this.transform.position.x + (Random.Range(0f, distanceToWalk) - Random.Range(0f, distanceToWalk));
        float zz = this.transform.position.z + (Random.Range(0f, distanceToWalk) - Random.Range(0f, distanceToWalk));
        Vector3 randomPos = new Vector3(xx, 500f, zz);
        
        RaycastHit hit_obj;
        Ray ray_obj = new Ray(randomPos, -Vector3.up);
        
        // Pick a new point toward which we should walk
        if ( Physics.Raycast(ray_obj, out hit_obj, 1000f, groundLayerMask) ) {
            
            // Check height preference
            if ((hit_obj.point.y > heightPreferenceMin) & (hit_obj.point.y < heightPreferenceMax)) {
                
                entityTag.Focus = new Vector3(hit_obj.point.x, hit_obj.point.y, hit_obj.point.z);
                
                entityTag.isRotating = true;
                entityTag.isFacing   = true;
                
                return true;
                
            // Chance to move anyway if we are outside the
            // prefered height range
            } else {
                
                if (Random.Range(0, 2) == 1)
                    return false;
                
                entityTag.Focus = new Vector3(hit_obj.point.x, hit_obj.point.y, hit_obj.point.z);
                
                entityTag.isRotating = true;
                entityTag.isFacing   = true;
                
                return true;
                
            }
            
        }
        
        return false;
	}
    
    
    
    
	public void AI_focus_nearest_entity() {
        
        GameObject entityList = this.transform.parent.transform.parent.transform.GetChild(1).gameObject;
        
        float minDistance = distanceToFocusOnEntity;
        
        for (int i=0; i < entityList.transform.childCount; i++) {
            
            GameObject entityObject = entityList.transform.GetChild(i).gameObject;
            
            if (!entityObject.activeInHierarchy) continue;
            if (entityObject.transform.position == this.transform.position) continue;
            
            float currentDistance = Vector3.Distance(this.transform.position, entityObject.transform.position);
            if (currentDistance < minDistance) {
                
                minDistance = currentDistance;
                
                entityTag.Focus = entityObject.transform.position;
                entityTag.isRotating = true;
                entityTag.isFacing   = true;
                
                targetEntity = entityObject;
                
            }
            
        }
        
        
        return;
	}
    
    
    
    
    
	public bool AI_focus_nearest_entity_type(string entity_name) {
        
        GameObject entityList = this.transform.parent.transform.parent.transform.GetChild(1).gameObject;
        
        float minDistance = distanceToFocusOnEntity;
        
        for (int i=0; i < entityList.transform.childCount; i++) {
            
            GameObject entityObject = entityList.transform.GetChild(i).gameObject;
            
            if (!entityObject.activeInHierarchy) continue;
            if (entityObject.transform.position == this.transform.position) continue;
            
            float currentDistance = Vector3.Distance(this.transform.position, entityObject.transform.position);
            if (currentDistance < minDistance) {
                
                if (entity_name == entityObject.name) {
                    
                    minDistance = currentDistance;
                    
                    entityTag.Focus = entityObject.transform.position;
                    entityTag.isRotating = true;
                    entityTag.isFacing   = true;
                    
                    targetEntity = entityObject;
                    
                }
                
            }
            
        }
        
        if (minDistance == distanceToFocusOnEntity) 
            return false;
        
        return true;
	}
    
    
    
    
    
	public void AI_focus_player(float distance) {
        
        if (Vector3.Distance(this.transform.position, this.transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(2).position) < distance) {
            
            entityTag.Focus      = this.transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(2).position;
            entityTag.isRotating = true;
            entityTag.isFacing   = true;
            
        }
        
        if (Random.Range(0, 10) > 1) {
            AI_stop_moving();
            resetAnimation();
        }
        
        return;
	}
    
    
    
    
	public void AI_attack_player(float distance) {
        
        if (isConsuming) return;
        
        if (Vector3.Distance(this.transform.position, this.transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(2).position) < distance) {
            
            targetEntity = this.transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(2).gameObject;
            
            entityTag.Focus      = this.transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(2).position;
            entityTag.isRotating = true;
            entityTag.isFacing   = true;
            
            isAttacking = true;
            isFleeing   = false;
            
        }
        
        return;
	}
    
    
    
    
    public bool AI_run_from_nearest_entity_type(string entity_name) {
        
        GameObject entityList = this.transform.parent.transform.parent.transform.GetChild(1).gameObject;
        
        float minDistance = distanceToFlee;
        
        for (int i=0; i < entityList.transform.childCount; i++) {
            
            GameObject entityObject = entityList.transform.GetChild(i).gameObject;
            
            if (!entityObject.activeInHierarchy) continue;
            if (entityObject.transform.position == this.transform.position) continue;
            
            float currentDistance = Vector3.Distance(this.transform.position, entityObject.transform.position);
            if (currentDistance < minDistance) {
                
                if (entity_name == entityObject.name) {
                    
                    minDistance = currentDistance;
                    
                    //entityTag.Focus      = entityObject.transform.position;
                    //entityTag.isRotating = true;
                    //entityTag.isFacing   = false;
                    
                    AI_change_direction();
                    AI_start_running();
                    
                    targetEntity = entityObject;
                    
                    isAttacking = false;
                    isFleeing   = true;
                    
                }
                
            }
            
        }
        
        if (minDistance == distanceToFlee) return false;
        
        return true;
	}
    
    
    
    
	public bool AI_attack_nearest_entity_type(string entity_name) {
        
        if (isConsuming) return false;
        
        GameObject entityList = this.transform.parent.transform.parent.transform.GetChild(1).gameObject;
        
        float minDistance = distanceToAttack;
        
        for (int i=0; i < entityList.transform.childCount; i++) {
            
            GameObject entityObject = entityList.transform.GetChild(i).gameObject;
            
            if (!entityObject.activeInHierarchy) continue;
            if (entityObject.transform.position == this.transform.position) continue;
            
            float currentDistance = Vector3.Distance(this.transform.position, entityObject.transform.position);
            if (currentDistance < minDistance) {
                
                if (entity_name == entityObject.name) {
                    
                    minDistance = currentDistance;
                    
                    entityTag.Focus      = entityObject.transform.position;
                    entityTag.isRotating = true;
                    entityTag.isFacing   = true;
                    
                    AI_start_running();
                    
                    targetEntity = entityObject;
                    
                    isAttacking = true;
                    isFleeing   = false;
                    
                }
                
            }
            
        }
        
        if (minDistance == distanceToAttack) return false;
        
        return true;
	}
    
    
    
    
    
    
    
    
    
    
    
	void Update() {
        
        if (!isActive) {return;}
        
        if (Time.timeScale == 0f) return;
        
        if (targetEntity != null) {
            
            if (isAttacking) Debug.DrawRay(this.transform.position, targetEntity.transform.position - this.transform.position, Color.red);
            if (isFleeing)   Debug.DrawRay(this.transform.position, entityTag.Focus - this.transform.position, Color.yellow);
            
        }
        
        
        if (entityTag.isWalking) {
            Debug.DrawRay(this.transform.position, entityTag.Focus - this.transform.position, Color.blue);
            Debug.DrawRay(this.transform.position, Vector3.up, Color.green);
            Debug.DrawRay(entityTag.Focus, Vector3.up, Color.red);
        }
        
        
        
        
        //
        // Attack profile - low level
        //
        
        if (entityTag.AttackCounter > 0) 
            entityTag.AttackCounter--;
        
        
        if ((isAttacking) & (targetEntity != null)) {
            
            // Check close enough to register an attack
            if (Vector3.Distance(this.transform.position, targetEntity.transform.position) < 3.2f) {
                
                AI_stop_moving();
                
                resetAnimation();
                
                if ((entityTag.AttackCounter == 0) & (isDying == false)) {
                    
                    entityTag.AttackCounter = entityTag.AttackTimout;
                    
                    if (targetEntity.name == "Player") {
                        
                        //
                        // Damage player
                        
                        TickUpdate tickUpdate = transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(0).gameObject.GetComponent<TickUpdate>();
                        tickUpdate.inventory.removeHealth( (int)entityTag.AttackDamage );
                        
                        tickUpdate.runCameraDamageAnimation = true;
                        
                        Vector3 playerPos = this.transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(2).position;
                        
                    } else {
                        
                        //
                        // Damage entity
                        
                        ActorTag  targetActorTag  = targetEntity.GetComponent<ActorTag>();
                        EntityTag targetEntityTag = targetEntity.GetComponent<EntityTag>();
                        
                        targetEntityTag.Health -= entityTag.AttackDamage;
                        
                        if (targetActorTag.stress < 100) targetActorTag.stress = 100;
                        
                        //
                        // Score a kill
                        
                        if (targetEntityTag.Health <= 0) {
                            
                            //
                            // Consume target entity item drop
                            if (targetActorTag.DropOnDeath.Length > 0) {
                                
                                int randomDrop = Random.Range(0, targetActorTag.DropOnDeath.Length);
                                string itemName = targetActorTag.DropOnDeath[randomDrop].name;
                                
                                GameObject consumable = Instantiate( Resources.Load( itemName )) as GameObject;
                                consumable.name = itemName;
                                
                                LayerMask defaultLayer = LayerMask.GetMask("Default");
                                consumable.layer = defaultLayer;
                                consumable.transform.GetChild(0).gameObject.layer = defaultLayer;
                                
                                // Place the item in the actors mouth
                                consumable.transform.parent = transform.GetChild(1).transform.GetChild(1).transform;
                                
                                // Set item lifetime
                                consumable.GetComponent<ItemTag>().lifeTime = targetActorTag.DropOnDeath[randomDrop].lifeTime;
                                
                                consumable.transform.localPosition = new Vector3(0f, 0f, 0.65f);
                                consumable.transform.localScale    = new Vector3(1f, 1f, 1f);
                                consumable.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                                
                                entityTag.AttackCounter = entityTag.AttackTimout * 2;
                                
                                isAttacking = false;
                                isFleeing   = false;
                                
                                entityTag.isFacing    = false;
                                
                                isConsuming = true;
                                
                                targetActorTag.isDying = true;
                                
                                hunger -= 100;
                            }
                            
                        }
                        
                        
                    }
                    
                }
                
                
            }
            
            
            //
            // Track target entity
            entityTag.Focus = targetEntity.transform.position;
            
            entityTag.isRotating = true;
            entityTag.isFacing   = true;
            
            
        } else {
            
            //
            // Entity pain reaction
            
            if (isInPain) {
                isInPain = false;
                
                if (stress < 40) stress = 40;
                
                AI_change_direction();
                
                AI_start_running();
            }
            
        }
        
        
        
        
        //
        // Check to cancel flee or attack
        
        if ((isFleeing) | (isAttacking)) {
            
            if (targetEntity == null)  {
                
                //entityTag.isRotating = false;
                //entityTag.isFacing   = false;
                
                isAttacking  = false;
                isFleeing    = false;
                
                AI_stop_moving();
                
                resetAnimation();
                
            } else {
                
                if (!targetEntity.activeInHierarchy) {
                    
                    //entityTag.isRotating = false;
                    //entityTag.isFacing   = false;
                    
                    isAttacking  = false;
                    isFleeing    = false;
                    
                    AI_stop_moving();
                    
                    resetAnimation();
                    
                }
                
            }
            
        }
        
        
        
        //
        // Check outside flee distance
        
        if ((isFleeing) & (targetEntity != null)) {
            
            float currentDistance = Vector3.Distance(this.transform.position, targetEntity.transform.position);
            
            if (currentDistance > distanceToFlee) {
                
                isFleeing = false;
                
                AI_start_walking();
                
            } else {
                
                if (changeDirectionCounter > 100) {
                    
                    AI_change_direction();
                    
                    changeDirectionCounter = 0;
                    
                } else {
                    
                    changeDirectionCounter++;
                    
                }
                
            }
            
        }
        
        
        
        
        //
        // Health state
        
        if (entityTag.Health <= 0) 
            isDying = true;
        
        
        //
        // Death animation cycle
        
        stepDeathAnimationCycle();
        
        
        
        if (!entityTag.isWalking) 
            return;
        
        
        
        
        //
        // Check if we have reached our destination
        
        
        if (Vector3.Distance(this.transform.position, entityTag.Focus) < 2f) {
            
            AI_stop_moving();
            resetAnimation();
            
            if (isFleeing) {
                
                if (AI_change_direction())
                    AI_start_walking();
                
            }
            
            return;
        }
        
        
        
        //
        // Animation cycle
        
        stepAnimationCycle();
        
        
        
	}
    
    
    
    
    
    
    
    
	public void stepAnimationCycle() {
        
        float current_walk_rate;
        
        if (entityTag.isRunning) {
            
            current_walk_rate = limbCycleRate * 2.5f * Time.deltaTime;
            
        } else {
            
            current_walk_rate = limbCycleRate * Time.deltaTime;
            
        }
        
        
        if (walkDirectionAnim) {
            
            walkCycleAnim += current_walk_rate;
            
            if (walkCycleAnim > limbCycleRange) {
                walkDirectionAnim = false;
            }
            
        } else {
            
            walkCycleAnim -= current_walk_rate;
            
            if (walkCycleAnim < -limbCycleRange) {
                walkDirectionAnim = true;
            }
            
        }
        
        if (!limbAxisInvert) {
            switch (limbAxis) {
                default:
                case 0:
                    ArmLeft.localRotation   = Quaternion.Euler(new Vector3( walkCycleAnim, 0.0f, 0.0f));
                    ArmRight.localRotation  = Quaternion.Euler(new Vector3(-walkCycleAnim, 0.0f, 0.0f));
                    LegLeft.localRotation   = Quaternion.Euler(new Vector3(-walkCycleAnim, 0.0f, 0.0f));
                    LegRight.localRotation  = Quaternion.Euler(new Vector3( walkCycleAnim, 0.0f, 0.0f));
                    break;
                case 1:
                    ArmLeft.localRotation   = Quaternion.Euler(new Vector3(0.0f,  walkCycleAnim, 0.0f));
                    ArmRight.localRotation  = Quaternion.Euler(new Vector3(0.0f, -walkCycleAnim, 0.0f));
                    LegLeft.localRotation   = Quaternion.Euler(new Vector3(0.0f, -walkCycleAnim, 0.0f));
                    LegRight.localRotation  = Quaternion.Euler(new Vector3(0.0f,  walkCycleAnim, 0.0f));
                    break;
                case 2:
                    ArmLeft.localRotation   = Quaternion.Euler(new Vector3(0.0f, 0.0f,  walkCycleAnim));
                    ArmRight.localRotation  = Quaternion.Euler(new Vector3(0.0f, 0.0f, -walkCycleAnim));
                    LegLeft.localRotation   = Quaternion.Euler(new Vector3(0.0f, 0.0f, -walkCycleAnim));
                    LegRight.localRotation  = Quaternion.Euler(new Vector3(0.0f, 0.0f,  walkCycleAnim));
                    break;
            }
        } else {
            switch (limbAxis) {
                default:
                case 0:
                    ArmLeft.localRotation   = Quaternion.Euler(new Vector3(-walkCycleAnim, 0.0f, 0.0f));
                    ArmRight.localRotation  = Quaternion.Euler(new Vector3(-walkCycleAnim, 0.0f, 0.0f));
                    LegLeft.localRotation   = Quaternion.Euler(new Vector3( walkCycleAnim, 0.0f, 0.0f));
                    LegRight.localRotation  = Quaternion.Euler(new Vector3( walkCycleAnim, 0.0f, 0.0f));
                    break;
                case 1:
                    ArmLeft.localRotation   = Quaternion.Euler(new Vector3(0.0f, -walkCycleAnim, 0.0f));
                    ArmRight.localRotation  = Quaternion.Euler(new Vector3(0.0f, -walkCycleAnim, 0.0f));
                    LegLeft.localRotation   = Quaternion.Euler(new Vector3(0.0f,  walkCycleAnim, 0.0f));
                    LegRight.localRotation  = Quaternion.Euler(new Vector3(0.0f,  walkCycleAnim, 0.0f));
                    break;
                case 2:
                    ArmLeft.localRotation   = Quaternion.Euler(new Vector3(0.0f, 0.0f, -walkCycleAnim));
                    ArmRight.localRotation  = Quaternion.Euler(new Vector3(0.0f, 0.0f, -walkCycleAnim));
                    LegLeft.localRotation   = Quaternion.Euler(new Vector3(0.0f, 0.0f,  walkCycleAnim));
                    LegRight.localRotation  = Quaternion.Euler(new Vector3(0.0f, 0.0f,  walkCycleAnim));
                    break;
            }
        }
        return;
	}
    
    
    public void dropItemsOnDeath() {
        
        LayerMask GroundLayerMask = LayerMask.GetMask("Ground");
        
        for (int i=0; i < DropOnDeath.Length; i++) {
            
            int summonCount = Random.Range(DropOnDeath[i].min, DropOnDeath[i].max);
            
            for (int c=0; c <= summonCount; c++) {
                
                GameObject staticList = this.transform.parent.transform.parent.transform.GetChild(2).gameObject;
                
                Vector3 thisPos = this.transform.position;
                
                thisPos.x += Random.Range(0f, 2f) - Random.Range(0f, 2f);
                thisPos.z += Random.Range(0f, 2f) - Random.Range(0f, 2f);
                
                GameObject newObject = Instantiate( Resources.Load( DropOnDeath[i].name )) as GameObject;
                newObject.name = DropOnDeath[i].name;
                newObject.transform.parent = staticList.transform;
                newObject.transform.localRotation = Quaternion.Euler(new Vector3( 0f, Random.Range(0f, 360f), 0f));
                
                // Set item lifetime
                newObject.GetComponent<ItemTag>().lifeTime = DropOnDeath[i].lifeTime;
                
                thisPos.y = 500f;
                
                RaycastHit hit_obj;
                Ray ray_obj = new Ray(thisPos, -Vector3.up);
                
                if ( Physics.Raycast(ray_obj, out hit_obj, 1000f, GroundLayerMask) ) {
                    newObject.transform.position = new Vector3(hit_obj.point.x, hit_obj.point.y + 0.18f, hit_obj.point.z);
                } else {
                    newObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
                }
                
            }
            
        }
        
    }
    
    
    
    
	public void stepDeathAnimationCycle() {
        
        if (isDying) {
            
            transform.localRotation = Quaternion.Euler(new Vector3( 0f, 0f, deathCounter));
            
            deathCounter += 5f;
            if (deathCounter > 90.0f) {
                
                Destroy(transform.gameObject);
                
                dropItemsOnDeath();
                
                return;
            }
            
        }
        
        return;
	}
    
	public void resetAnimation() {
        
        ArmLeft.localRotation   = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        ArmRight.localRotation  = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        LegLeft.localRotation   = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        LegRight.localRotation  = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        
	}
    
    
    public void processMemories(int intelligenceFactor) {
        
        if (Random.Range(0, intelligenceFactor) == 0 ) return;
        
        // Encourage to stay within the home chunk
        string memory = getMemory("home");
        if (memory != "") {
            
            if (entityTag.currentChunk != null) {
                
                string[] posArray = memory.Split('.');
                float posX = float.Parse(posArray[0]);
                float posZ = float.Parse(posArray[1]);
                
                Vector3 currentPosition = new Vector3(posX, 0f, posZ);
                
                if (Vector3.Distance(transform.position, currentPosition) > 45f) {
                    
                    entityTag.Focus = currentPosition;
                    
                    entityTag.isRotating = true;
                    entityTag.isFacing   = true;
                }
            }
        }
        
        
        return;
    }
    
    
    
    
    //
    // AI memory
    //
    
    public void addMemory(string name, string data) {
        memories += name +"="+ data+";";
    }
    
    public string getMemory(string name) {
        
        string[] pairList = memories.Split(';');
        
        for (int i=0; i < pairList.Length; i++) {
            
            string[] nameValueSet = pairList[i].Split('=');
            
            if (nameValueSet[0] != name) 
                continue;
            
            return nameValueSet[1];
        }
        
        return "";
    }
    
    public bool removeMemory(string name) {
        
        string[] pairList = memories.Split(';');
        clearMemory();
        
        for (int i=0; i < pairList.Length; i++) {
            
            string[] nameValueSet = pairList[i].Split('=');
            
            if (nameValueSet[0] == name) 
                continue;
            
            addMemory(nameValueSet[0], nameValueSet[1]);
        }
        
        return true;
    }
    
    public void clearMemory() {
        memories = "";
    }
    
}














