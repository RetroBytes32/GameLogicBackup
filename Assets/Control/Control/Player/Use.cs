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
	bool  isConsuming         = false;
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
	  if (tickUpdate.doShowConsole) return;
	  
	  if (useCounter > 0) useCounter--;
	  
	  string slotName = interfaceScript.inventory.checkSlot();
      
      // Check if the item is consumable
      if (Input.GetButtonDown("Use")) {
        
	    isConsuming = false;
	    
	    for (int i=0; i < tickUpdate.consumableItem.Length; i++) {
            if (tickUpdate.consumableItem[i].name != slotName) 
                continue;
            
            isConsuming = true;
            break;
	    }
        
	  }
	  
	  
	  if (Input.GetButton("Use")) {
	    
	    // Check is currently consuming
        if (isConsuming) {
            
            // Check are we full
            if (interfaceScript.inventory.hunger == 10) {
                resetConsumeAnimation();
                return;
            }
            
            runConsumeAnimation = true;
            
            if (useCounter == 1) {
                
                for (int i=0; i < tickUpdate.consumableItem.Length; i++) {
                    if (tickUpdate.consumableItem[i].name != slotName) 
                        continue;
                    
                    tickUpdate.HealthRecharge = (tickUpdate.inventory.health * 7);
                    
                    tickUpdate.inventory.addHunger(tickUpdate.consumableItem[i].hunger);
                    tickUpdate.inventory.addSaturation(tickUpdate.consumableItem[i].saturation);
                    
                    tickUpdate.inventory.removeItem();
                    interfaceScript.updateInHand();
                    
                    resetConsumeAnimation();
                    
                    if (interfaceScript.inventory.checkSlot() == "")
                        isConsuming = false;
                    
                    break;
                }
                
            }
            
        } else {
            
            resetConsumeAnimation();
        }
	    
	    
	    
	    
	    
	    //
	    // Use animation cycle
	    
	    if (runConsumeAnimation) {
	      
	      if (useCounter < 0) {
	        
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
	    resetConsumeAnimation();
	    
	    // Reset animation cycle
	    consumeOffset=0;
	    consumeOffsetWave=0f;
	    consumeOffsetWaveDir = true;
	    runConsumeAnimation  = false;
	    useCounter = useTimeout;
	    transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.localPosition = new Vector3(1.5f, -1f, 2f);
	    
	    
	    
	    // Check the current slot for a usable object
	    slotName = interfaceScript.inventory.checkSlot();
	    
	    /*
	    // Use the bow to shoot an arrow
	    if (slotName == "bow") {
	      
	      GameObject newObject = Instantiate( Resources.Load( "arrow" )) as GameObject;
	      newObject.name = "arrow";
	      
	      newObject.transform.position = GameObject.Find("HandRight").transform.position;
	      newObject.transform.rotation = GameObject.Find("HandRight").transform.rotation;
	      newObject.transform.localScale = GameObject.Find("HandRight").transform.localScale;
	      
	      newObject.GetComponent<Rigidbody>().velocity = GameObject.Find("Main Camera").transform.forward * shootForce;
	      
	    }
	    */
	    
	  }
	  
	  
	  
	  
	}
	
	
	
	
	
	void Start () {
	  
	  tickUpdate = GameObject.Find("GameRules").GetComponent<TickUpdate>();
	  
	  interfaceScript = GameObject.Find("HUD").GetComponent<Interface>();
	  
	}
	
	
}




