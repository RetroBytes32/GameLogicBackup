using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowControl : MonoBehaviour {
	
	public float LifeSpan = 120.0f;
	public float Damage   = 40.0f;
	
	
	float Timer = 0;
	
	Rigidbody thisBody;
	
	bool isCollided = false;
	
	
	void Update () {
	  
	  // Lifespan
	  Timer += 1.0f * Time.deltaTime;
	  if (Timer > LifeSpan) Destroy(this.gameObject);
	  
	  if (!isCollided) thisBody.rotation = Quaternion.LookRotation( thisBody.velocity );
	  
	}
	
	
	void OnCollisionEnter(Collision collision) {
	  
	  // Avoid hitting other arrows
	  if (collision.collider.name != "arrow") {
	    
	    thisBody.constraints = RigidbodyConstraints.FreezeAll;
	    
	    isCollided = true;
	    
	  }
	  
	  // Entity hit
	  if (collision.gameObject.layer == 10) {
	    
	    EntityTag entityTag = collision.gameObject.GetComponent<EntityTag>();
	    
	    entityTag.Health -= Damage;
	    
	  }
	  
	  
	  
	}
	
	
	void Start () {
	  
	  thisBody = this.GetComponent<Rigidbody>();
	  
	}
	
}
