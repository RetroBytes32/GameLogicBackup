using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

}


public class ActorTag : MonoBehaviour {

	public EntityTag  entityTag;
	public bool       isActive;

	// AI paramaters

	[Space(10)]
	[Header("AI parameters")]
	[Space(3)]

	public float chanceChangeDirection    = 0.3f;
	public float chanceToWalk             = 0.2f;
	public float chanceToFocusOnPlayer    = 0.01f;
	public float chanceToFocusOnEntity    = 0.03f;
	public float chanceToAttackPlayer     = 0f;

	public float distanceToFocusOnPlayer  = 10f;
	public float distanceToFocusOnEntity  = 10f;
	public float distanceToAttackPlayer   = 8f;
	public float distanceToWalk           = 20.0f;

	public float heightPreferenceMin      = 0f;
	public float heightPreferenceMax      = 40f;


	public float distanceToAttack         = 20f;
	public float distanceToFlee           = 20f;



	[Space(10)]
	[Header("Pray entities")]
	[Space(3)]

	public string[]  EntitiesToAttack;



	[Space(10)]
	[Header("Predator entities")]
	[Space(3)]

	public string[]  EntitiesToFlee;



	[Space(10)]
	[Header("Item drops")]
	[Space(3)]

	public LootTable[]  DropOnDeath;



	// Animation

	[Space(10)]
	[Header("Amimation")]
	[Space(3)]
    
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
	public bool  isRedirecting  = false;

	public bool  isAttacking    = false;
	public bool  isFleeing      = false;



	[Space(10)]
	[Header("Consumtion")]
	[Space(3)]

	public bool  isConsuming     = false;
	public int   consumtionTimer = 40;



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




	[Space(20)]

	public int   consumeTimer   = 0;
	public float deathCounter   = 0f;
	public float redirectTimer  = 0f;

	public int   changeDirectionCounter  = 0;









	public void updateAI() {


	  // Health state

	  if (entityTag.Health <= 0) {

	    isDying = true;

	  }




	  //
	  // Consumption timer

	  if (isConsuming) {

	    consumeTimer++;
	    if (consumeTimer > consumtionTimer) {

	      consumeTimer = 0;

	      if (transform.GetChild(1).transform.GetChild(1).transform.childCount > 0)
	        Destroy(transform.GetChild(1).transform.GetChild(1).transform.GetChild(0).transform.gameObject);

	      isConsuming = false;
	    }


	  }



	  //
	  // Attack profile - high level
	  //

	  if ((isAttacking) & (targetEntity != null)) {

	    if (!targetEntity.activeInHierarchy) {
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





	  // Chance to attack player
	  if (chanceToAttackPlayer > 0f) {

	    if (Random.Range(0f, 1f) < chanceToAttackPlayer) {

	      AI_attack_player(distanceToAttackPlayer);

	      return;
	    }

	  }









	  //
	  // Check entity relationships
	  //



	  // Attack entities

	  if (EntitiesToAttack.Length > 0) {


	    // Check attackable entities

	    for (int i=0; i < EntitiesToAttack.Length; i++) {
	      if (AI_attack_nearest_entity_type( EntitiesToAttack[i] ))
	        break;
	    }

	  }



	  // Run from entities

	  if (EntitiesToFlee.Length > 0) {


	    // Check fleeable entities

	    for (int i=0; i < EntitiesToFlee.Length; i++) {
	      if (AI_run_from_nearest_entity_type( EntitiesToFlee[i] ))
	        break;
	    }

	  }






	  if ((isAttacking) | (isFleeing)) return;


	  //
	  // Check internal action

	  // Chance to change direction
	  if (Random.Range(0f, 1f) < chanceChangeDirection)  AI_change_direction();


	  // Chance to start walking
	  if (Random.Range(0f, 1f) < chanceToWalk)           if (AI_change_direction()) AI_start_walking();


	  // Chance to focus on the nearest entity
	  if (Random.Range(0f, 1f) < chanceToFocusOnEntity)  AI_focus_nearest_entity();


	  // Chance to focus on the player
	  if (Random.Range(0f, 1f) < chanceToFocusOnPlayer)  AI_focus_player(distanceToFocusOnPlayer);




	  return;
	}












	public void AI_initiate() {



	}



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



	public void AI_redirect_if_stuck() {

	  float velocity_redirection_minimum = 0.4f;

	  if ((entityTag.controller.velocity.x < velocity_redirection_minimum) & (entityTag.controller.velocity.y < velocity_redirection_minimum) & (entityTag.controller.velocity.z < velocity_redirection_minimum)) {
	    isRedirecting = true;
	  }


	  if (isRedirecting) {

	    if (redirectTimer == 0) AI_change_direction();

	    redirectTimer++;
	    if (redirectTimer > 20) {

	      redirectTimer = 0;

	      isRedirecting = false;
	    }

	  }

	  return;
	}






	public bool AI_change_direction() {

	  //TickUpdate tickUpdate = transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(0).gameObject.GetComponent<TickUpdate>();

	  Vector3 chunkPos  = transform.parent.transform.parent.transform.position;


	  float xx = this.transform.position.x + (Random.Range(0f, distanceToWalk) - Random.Range(0f, distanceToWalk));
	  float zz = this.transform.position.z + (Random.Range(0f, distanceToWalk) - Random.Range(0f, distanceToWalk));

	  Vector3 randomPos = new Vector3(xx, 500f, zz);

	  RaycastHit hit_obj;
	  Ray ray_obj = new Ray(randomPos, -Vector3.up);

	  LayerMask GroundLayerMask = LayerMask.GetMask("Ground");

	  if ( Physics.Raycast(ray_obj, out hit_obj, 1000f, GroundLayerMask) ) {

	    if ((hit_obj.point.y > heightPreferenceMin) & (hit_obj.point.y < heightPreferenceMax)) {

	      entityTag.Focus = new Vector3(hit_obj.point.x, hit_obj.point.y, hit_obj.point.z);

	      entityTag.isRotating = true;
	      entityTag.isFacing   = true;

	      return true;
	    }



	    if (hit_obj.point.y > 0f) {

	      if (Random.Range(0, 10) > 8) {

	        entityTag.Focus = new Vector3(hit_obj.point.x, hit_obj.point.y, hit_obj.point.z);

	        entityTag.isRotating = true;
	        entityTag.isFacing   = true;

	        return true;
	      }
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

	  if (minDistance == distanceToFocusOnEntity) return false;

	  return true;
	}





	public void AI_focus_player(float distance) {

	  if (Vector3.Distance(this.transform.position, this.transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(2).position) < distance) {

	    entityTag.Focus      = this.transform.parent.transform.parent.transform.parent.transform.parent.transform.GetChild(2).position;
	    entityTag.isRotating = true;
	    entityTag.isFacing   = true;

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






	//
	// Flee


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





	//
	// Attack


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

	          //
	          // Score a kill

	          if (targetEntityTag.Health <= 0) {

	            //
	            // Consume target entity
	            if (targetActorTag.DropOnDeath.Length > 0) {

	              int randomDrop = Random.Range(0, targetActorTag.DropOnDeath.Length);
	              string itemName = targetActorTag.DropOnDeath[randomDrop].name;

	              GameObject consumable = Instantiate( Resources.Load( itemName )) as GameObject;
	              consumable.name = itemName;
	              consumable.transform.parent = transform.GetChild(1).transform.GetChild(1).transform;

	              consumable.transform.localPosition = new Vector3(0f, 0f, 0.65f);
	              consumable.transform.localScale    = new Vector3(1f, 1f, 1f);
	              consumable.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

	              entityTag.AttackCounter = entityTag.AttackTimout * 2;

	              isAttacking = false;
	              isFleeing   = false;

	              entityTag.isFacing    = false;

	              isConsuming = true;

	              targetActorTag.isDying = true;

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

	      if (AI_change_direction())
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
	  // Death animation cycle

	  stepDeathAnimationCycle();






	  if (!entityTag.isWalking) return;





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
	  // Redirect if the entity is stuck

	  if (!isAttacking)
	    AI_redirect_if_stuck();



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

	  ArmLeft.localRotation   = Quaternion.Euler(new Vector3( walkCycleAnim, 0.0f, 0.0f));
	  ArmRight.localRotation  = Quaternion.Euler(new Vector3(-walkCycleAnim, 0.0f, 0.0f));
	  LegLeft.localRotation   = Quaternion.Euler(new Vector3(-walkCycleAnim, 0.0f, 0.0f));
	  LegRight.localRotation  = Quaternion.Euler(new Vector3( walkCycleAnim, 0.0f, 0.0f));

	  return;
	}





	public void stepDeathAnimationCycle() {

	  if (isDying) {

	    transform.localRotation = Quaternion.Euler(new Vector3( 0f, 0f, deathCounter));

	    deathCounter += 5f;
	    if (deathCounter > 90.0f) {

	      Destroy(transform.gameObject);

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


}














