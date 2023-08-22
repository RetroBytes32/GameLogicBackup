using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour {
	
	public LayerMask DropMask;
	public LayerMask GroundMask;
	
	public float     Distance = 7.0f;
	
	Interface interfaceScript;
	
	Inventory inventory;
	
	Camera cameraObject;
	
	TickUpdate tickUpdate;
	
	public GameObject HUD;
	
	
	
	
	
	void Update () {
        
        if (tickUpdate.isPaused) return;
        if (tickUpdate.doShowConsole) return;
        
        
        if (Input.GetButtonDown("Place")) {
            
            string slotName = interfaceScript.inventory.checkSlot();
            
            if (slotName == "") 
                return;
            
            // Check if the item is placeable / breakable
            bool isPlacable = false;
            for (int i=0; i < tickUpdate.breakableItem.Length; i++) {
                if (tickUpdate.breakableItem[i].name != slotName) 
                    continue;
                
                isPlacable = true;
                break;
            }
            
            RaycastHit hit_gnd;
            
            Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
            
            
            
            //
            // Drop object onto an object
            //
            
            RaycastHit hit_obj;
            if ( Physics.Raycast(ray, out hit_obj, Distance, DropMask) ) {
                
                if (!isPlacable) 
                    return;
                
                float chunk_x = Mathf.Round(hit_obj.point.x / 100) * 100;
                float chunk_z = Mathf.Round(hit_obj.point.z / 100) * 100;
                
                GameObject chunkObject = tickUpdate.chunkGenerator.getChunk(chunk_x, chunk_z);
                if (chunkObject == null) 
                    return;
                
                string objectName = hit_obj.transform.parent.transform.gameObject.name;
                
                // If stack is empty, remove the object from players hand
                if (interfaceScript.inventory.removeItem() == 0) {
                    
                    GameObject objectInHand = GameObject.Find("InHandRight");
                    if (objectInHand != null) Destroy(objectInHand);
                    
                    interfaceScript.updateInHand();
                }
                
                // Create drop object
                GameObject newObject = Instantiate( Resources.Load( slotName )) as GameObject;
                newObject.name = slotName;
                newObject.transform.parent = chunkObject.transform.GetChild(2).transform;
                
                // Calculate target position
                
                float offsetX = 0f;
                float offsetY = 0f;
                float offsetZ = 0f;
                
                // Check player position is larger than hit point
                
                // Calculate X offset
                /*
                if (transform.position.x > hit_obj.transform.position.x) {
                    offsetX += hit_obj.transform.localScale.x + 0.05f;
                } else {
                    offsetX -= hit_obj.transform.localScale.x + 0.05f;
                }
                */
                
                // Calculate Y offset
                offsetY = hit_obj.transform.localScale.y;
                
                // Calculate Z offset
                /*
                if (transform.position.z > hit_obj.transform.position.z) {
                    offsetZ += hit_obj.transform.localScale.z + 0.05f;
                } else {
                    offsetZ -= hit_obj.transform.localScale.z + 0.05f;
                }
                */
                
                newObject.transform.position = new Vector3(hit_obj.transform.position.x + offsetX,
                                                           hit_obj.transform.position.y + offsetY,
                                                           hit_obj.transform.position.z + offsetZ);
                
                return;
            }
            
            
            
            //
            // Check to drop directly on the ground
            //
            
            if ( Physics.Raycast(ray, out hit_gnd, Distance, GroundMask) ) {
                
                //
                // Empty hand modify terrain
                /*
                if (slotName == "") {
                    
                    ChunkTag chunkTag = hit_gnd.transform.gameObject.GetComponent<ChunkTag>();
                    if (chunkTag == null) return;
                    
                    float terrain_damage = 1f;
                    
                    // Update joining chunk edges
                    tickUpdate.update_nearby_chunks(hit_gnd.point.x, hit_gnd.point.z, chunkTag, terrain_damage);
                    
                    return;
                }
                */
                
                // Dont place on ground if its not breakable
                if (!isPlacable) 
                    return;
                
                
                //
                // Drop item on ground
                
                float chunk_x = Mathf.Round(hit_gnd.point.x / 100) * 100;
                float chunk_z = Mathf.Round(hit_gnd.point.z / 100) * 100;
                
                GameObject chunkObject = tickUpdate.chunkGenerator.getChunk(chunk_x, chunk_z);
                if (chunkObject == null) 
                    return;
                
                GameObject newObject = Instantiate( Resources.Load( slotName )) as GameObject;
                newObject.name = slotName;
                Vector3 roundedPosition;
                roundedPosition.x = Mathf.Round(hit_gnd.point.x);
                roundedPosition.z = Mathf.Round(hit_gnd.point.z);
                
                newObject.transform.Translate( new Vector3(roundedPosition.x, 
                                                           hit_gnd.point.y + (newObject.transform.localScale.y / 2), 
                                                           roundedPosition.z) );
                
                newObject.transform.parent = chunkObject.transform.GetChild(2).transform;
                
                // If stack is empty, remove the object from players hand
                if (interfaceScript.inventory.removeItem() == 0) 
                    interfaceScript.updateInHand();
                
                return;
            }
            
            
            
            
            
        }
        
        
        
        if (Input.GetButtonDown("Drop")) {
            
            string slotName = interfaceScript.inventory.checkSlot();
            
            if (slotName == "") 
                return;
            
            RaycastHit hit_gnd;
            
            Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
            
            // Check to drop directly on the ground
            if ( Physics.Raycast(ray, out hit_gnd, Distance, GroundMask) ) {
                
                //
                // Drop item on ground
                
                float chunk_x = Mathf.Round(hit_gnd.point.x / 100) * 100;
                float chunk_z = Mathf.Round(hit_gnd.point.z / 100) * 100;
                
                GameObject chunkObject = tickUpdate.chunkGenerator.getChunk(chunk_x, chunk_z);
                if (chunkObject == null) 
                    return;
                
                GameObject newObject = Instantiate( Resources.Load( slotName )) as GameObject;
                newObject.name = slotName;
                Vector3 roundedPosition;
                roundedPosition.x = Mathf.Round(hit_gnd.point.x);
                roundedPosition.z = Mathf.Round(hit_gnd.point.z);
                
                newObject.transform.Translate( new Vector3(roundedPosition.x, 
                                                           hit_gnd.point.y + (newObject.transform.localScale.y / 2), 
                                                           roundedPosition.z) );
                
                newObject.transform.parent = chunkObject.transform.GetChild(2).transform;
                
                // If stack is empty, remove the object from players hand
                if (interfaceScript.inventory.removeItem() == 0) 
                    interfaceScript.updateInHand();
                
                return;
            }
            
        }
        
	}
	
	
	
	
	void Start () {
        
        tickUpdate = GameObject.Find("GameRules").GetComponent<TickUpdate>();
        
        interfaceScript = GameObject.Find("HUD").GetComponent<Interface>();
        
        cameraObject = GameObject.Find("Main Camera").GetComponent<Camera>();
        
	}
	
	
}




