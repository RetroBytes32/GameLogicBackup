using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Use : MonoBehaviour {
	
	public LayerMask UseMask;
	public float  Distance  = 5.0f;
	
	public Transform handRight;
	
	public float shootForce = 24.0f;
	
	public int useTimeout   = 40;
	public int useCounter;
	
	
	
	Interface interfaceScript;
	TickUpdate tickUpdate;
	
	bool  runConsumeAnimation = false;
	float consumeOffset;
	float consumeOffsetWave;
	bool  consumeOffsetWaveDir = false;
	
	
	
	
	public void resetConsumeAnimation() {
	  
	  // Reset animation cycle
	  consumeOffset=0;
	  consumeOffsetWave=0f;
	  consumeOffsetWaveDir = true;
	  runConsumeAnimation  = false;
	  useCounter = useTimeout;
	  transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f, -1f, 2f);
	  
	}
	
	void Update () {
	  
	  if (tickUpdate.isPaused) return;
	  
	  
	  if (useCounter > 0) useCounter--;
	  
	  
	  
	  if (Input.GetButton("Use")) {
	    
	    string slotName = interfaceScript.inventory.checkSlot();
	    
	    if (slotName == "meat") {
	      
	      if (interfaceScript.inventory.hunger == 10) {
	        
	        resetConsumeAnimation();
	        
	        return;
	      }
	      
	      runConsumeAnimation = true;
	      
	      
	      if (useCounter == 1) {
	        
	        tickUpdate.HealthRecharge = (tickUpdate.inventory.health * 7);
	        
	        tickUpdate.inventory.addHunger(2);
	        tickUpdate.inventory.addSaturation(100);
	        
	        tickUpdate.inventory.removeItem();
	        interfaceScript.updateInHand();
	      }
	      
	    }
	    
	    
	    
	    
	    
	    
	    
	    
	    //
	    // Use animation cycle
	    
	    if (runConsumeAnimation == true) {
	      
	      if (useCounter <= 0) {
	        
	        // Reset animation cycle
	        consumeOffset=0;
	        consumeOffsetWave=0f;
	        consumeOffsetWaveDir = true;
	        runConsumeAnimation  = false;
	        useCounter = useTimeout;
	        transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f, -1f, 2f);
	        
	        return;
	      }
	      
	      
	      if (useCounter > 0) {
	        
	        consumeOffset += 7f / useCounter;
	        
	        if (consumeOffset > 1.5f) {
	          
	          consumeOffset = 1.5f;
	          if (consumeOffsetWaveDir) {
	            
	            consumeOffsetWave += 0.1f;
	            
	            if (consumeOffsetWave > 0f) {
	              consumeOffsetWaveDir = false;
	            }
	            
	            
	          } else {
	            
	            consumeOffsetWave -= 0.1f;
	            
	            if (consumeOffsetWave < 0f) {
	              consumeOffsetWaveDir = true;
	            }
	            
	          }
	          
	          
	        }
	        
	      }
	      
	      
	      transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f - consumeOffset, -1f + (consumeOffset / 10f) + (consumeOffsetWave * 0.9f), 2f);
	      
	    }
	    
	    
	    
	    
	  }
	  
	  
	  if (Input.GetButtonUp("Use")) {
	    
	    string slotName = interfaceScript.inventory.checkSlot();
	    
	    // Reset animation cycle
	    consumeOffset=0;
	    consumeOffsetWave=0f;
	    consumeOffsetWaveDir = true;
	    runConsumeAnimation  = false;
	    useCounter = useTimeout;
	    transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f, -1f, 2f);
	    
	    
	    
	    // Check the current slot for a usable object
	    slotName = interfaceScript.inventory.checkSlot();
	    
	    
	    // Use the bow to shoot an arrow
	    if (slotName == "bow") {
	      
	      GameObject newObject = Instantiate( Resources.Load( "arrow" )) as GameObject;
	      newObject.name = "arrow";
	      
	      newObject.transform.position = GameObject.Find("HandRight").transform.position;
	      newObject.transform.rotation = GameObject.Find("HandRight").transform.rotation;
	      newObject.transform.localScale = GameObject.Find("HandRight").transform.localScale;
	      
	      newObject.GetComponent<Rigidbody>().velocity = GameObject.Find("Main Camera").transform.forward * shootForce;
	      
	    }
	    
	    
	  }
	  
	  
	  
	  
	}
	
	
	
	
	
	void Start () {
	  
	  tickUpdate = GameObject.Find("GameRules").GetComponent<TickUpdate>();
	  
	  interfaceScript = GameObject.Find("HUD").GetComponent<Interface>();
	  
	}
	
	
}




