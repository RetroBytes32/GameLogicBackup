using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour {
	
	public LayerMask AttackMask;
	public LayerMask EntityMask;
	public LayerMask GroundMask;
	
	public float  AttackTimeout = 0;
	public float  attackTimer   = 0;
	public bool   canAttack     = false;
	
	public float  playerAttackDamage = 1;
	
	public float  Distance  = 7.0f;
	
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
	  
	  
	  if (attackTimer > 0) attackTimer--;
	  
	  
	  if (Input.GetButtonDown("Attack")) {
	    
	    GetComponent<Use>().resetConsumeAnimation();
	    
	  }
	  
	  
	  if (Input.GetButtonUp("Attack")) {
	    
	    GetComponent<Use>().resetConsumeAnimation();
	    
	    //
	    // Raycheck entities
	    
	    RaycastHit hit_ent;
	    Ray ray_entity = cameraObject.ScreenPointToRay(Input.mousePosition);
	    
	    if ( Physics.Raycast(ray_entity, out hit_ent, Distance, EntityMask) ) {
	      
	      attackTimer = 0;
	      
	    }
	    
	  }
	  
	  
	  if (Input.GetButton("Attack")) {
	    
	    
	    //
	    // Raycheck pickup objects
	    
	    RaycastHit hit_obj;
	    Ray ray_obj = cameraObject.ScreenPointToRay(Input.mousePosition);
	    
	    if ( Physics.Raycast(ray_obj, out hit_obj, Distance, AttackMask) ) {
	      
	      // Get object name
	      string objectName = hit_obj.transform.parent.transform.gameObject.name;
	      
	      //
	      // Delay then attack
	      
	      if (attackTimer <= 0) {
	        
	        attackTimer = AttackTimeout;
	        
	        if (objectName == "log") {
	          
	          runAttackAnimCycle = true;
	          runPickupAnimCycle = false;
	          
	          return;
	        }
	        
	        attackTimer = 0;
	        
	        runAttackAnimCycle = false;
	        runPickupAnimCycle = true;
	        
	      }
	      
	      if (attackTimer == 0) {
	        
	        Destroy( hit_obj.transform.parent.transform.gameObject );
	        
	        inventory.addItem(objectName, 1);
	        
	        interfaceScript.updateInHand();
	        
	      }
	      
	      
	      
	    }
	    
	    
	    
	    
	    
	    //
	    // Raycheck entities
	    
	    RaycastHit hit_ent;
	    Ray ray_entity = cameraObject.ScreenPointToRay(Input.mousePosition);
	    
	    if ( Physics.Raycast(ray_entity, out hit_ent, Distance, EntityMask) ) {
	      
	      
	      //
	      // Attack entity then delay
	      
	      if (attackTimer <= 0) 
	        attackTimer = AttackTimeout;
	      
	      if (attackTimer == AttackTimeout) {
	        
	        runAttackAnimCycle = true;
	        
	        // Get object name
	        string objectName = hit_ent.transform.gameObject.name;
	        
	        EntityTag entityTag = hit_ent.transform.gameObject.GetComponent<EntityTag>();
	        ActorTag  actorTag  = hit_ent.transform.gameObject.GetComponent<ActorTag>();
	        
	        if (entityTag == null) return;
	        if (actorTag == null) return;
	        
	        //
	        // Calculate and apply attack damage
	        
	        entityTag.Health -= playerAttackDamage;
	        actorTag.isInPain = true;
	        
	        if (actorTag.chanceToAttackPlayer > 0f) 
	          actorTag.AI_attack_player(20);
	        
	        if (entityTag.Health < 0) {
	          
	          if (actorTag.isDying == false) {
	            
	            actorTag.isDying = true;
	            
	          }
	          
	        }
	        
	      }
	      
	      
	    }
	    
	    
	    
	    
	    
	  }
	  
	  
	  
	  
	  
	  
	  
	  //
	  // Attack animation cycle
	  
	  if (runAttackAnimCycle == true) {
	    
	    if (attackOffsetDirection == false) {
	      attackOffset += 0.8f;
	    } else {
	      attackOffset -= 0.8f;
	    }
	    
	    
	    if (attackOffset <= 0f) {
	      attackOffset = 0f;
	      attackOffsetDirection = false;
	      runAttackAnimCycle = false;
	      
	      transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f, -1f, 2f);
	      
	      return;
	    }
	    
	    
	    if (attackOffset > 1.1f) {
	      attackOffset = 1.1f;
	      attackOffsetDirection = true;
	      
	    }
	    
	    
	    transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f - attackOffset, -1f - (0.1f / attackOffset), 2f + (0.4f / attackOffset));
	    
	    
	  }
	  
	  
	  
	  //
	  // Item pickup animation cycle
	  
	  if (runPickupAnimCycle == true) {
	    
	    attackOffset += 0.5f;
	    
	    
	    if (attackOffset > 4f) {
	      attackOffset = 4f;
	      attackOffsetDirection = false;
	      
	      transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f, -1f, 2f);
	      
	      runPickupAnimCycle = false;
	      attackOffset = 0f;
	      return;
	    }
	    
	    
	    transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f - (0.1f / attackOffset), -1f / (1.0f / attackOffset), 2f + (0.7f / attackOffset));
	    
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
