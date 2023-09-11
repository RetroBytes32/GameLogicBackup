using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour {
	
	public LayerMask DropMask;
	public LayerMask GroundMask;
	
	public float   HitDistance = 7.0f;
	
	int coolDownTimer = 1;
	public int coolDownTimeOut = 18;
	
	
	TickUpdate tickUpdate;
	
	
	
	
	
	void Update () {
        
        if (tickUpdate.isPaused) return;
        if (tickUpdate.doShowConsole) return;
        
        if ((Input.GetButtonUp("Place")) | (Input.GetButtonUp("Drop"))) 
            coolDownTimer = 1;
        
        
        coolDownTimer--;
        if (coolDownTimer > 0)
            return;
        
        //
        // Place objects
        
        if (Input.GetButton("Place")) {
            
            coolDownTimer = coolDownTimeOut;
            
            string slotName = tickUpdate.inventory.checkSlot();
            
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
            
            
            
            
            //
            // Drop object onto an object
            //
            
            RaycastHit hit_obj;
            Ray ray = tickUpdate.cameraObject.ScreenPointToRay(Input.mousePosition);
            
            if ( Physics.Raycast(ray, out hit_obj, HitDistance, DropMask) ) {
                
                if (!isPlacable) 
                    return;
                
                float chunk_x = Mathf.Round(hit_obj.point.x / 100) * 100;
                float chunk_z = Mathf.Round(hit_obj.point.z / 100) * 100;
                
                GameObject chunkObject = tickUpdate.chunkGenerator.getChunk(chunk_x, chunk_z);
                if (chunkObject == null) 
                    return;
                
                string objectName = hit_obj.transform.parent.transform.gameObject.name;
                
                // Check if the object has a precise hit box
                if (objectName == "hitbox") {
                    objectName = hit_obj.transform.parent.transform.parent.transform.gameObject.name;
                    
                    // Create item drop
                    GameObject newObject = Instantiate( Resources.Load( slotName )) as GameObject;
                    newObject.name = slotName;
                    newObject.transform.parent = chunkObject.transform.GetChild(2).transform;
                    
                    // Calculate the direction offset
                    float posX=0f;
                    float posY=0f;
                    float posZ=0f;
                    
                    // Offset rotation
                    float rotX=0f;
                    float rotY=0f;
                    float rotZ=0f;
                    
                    // Check hit box edges
                    if (hit_obj.transform.gameObject.name == "front")   {posX -= hit_obj.transform.localScale.x; rotZ  = 90f;}
                    if (hit_obj.transform.gameObject.name == "back")    {posX  = hit_obj.transform.localScale.x; rotZ  = 90f;}
                    if (hit_obj.transform.gameObject.name == "top")     {posY  = hit_obj.transform.localScale.y; ;}
                    if (hit_obj.transform.gameObject.name == "bottom")  {posY -= hit_obj.transform.localScale.y; ;}
                    if (hit_obj.transform.gameObject.name == "left")    {posZ  = hit_obj.transform.localScale.z; rotX  = 90f;}
                    if (hit_obj.transform.gameObject.name == "right")   {posZ -= hit_obj.transform.localScale.z; rotX  = 90f;}
                    
                    // Translate item
                    Vector3 newPosition = new Vector3(hit_obj.transform.position.x + posX,
                                                      hit_obj.transform.position.y + posY,
                                                      hit_obj.transform.position.z + posZ);
                    newObject.transform.position = newPosition;
                    
                    // Rotate item
                    Vector3 newRotation = new Vector3(rotX, rotY, rotZ);
                    newObject.transform.localRotation = Quaternion.Euler(newRotation);
                    
                    Vector3 hitboxRotation = new Vector3(rotX, rotY, rotZ);
                    newObject.transform.GetChild(0).transform.localRotation = Quaternion.Euler(-hitboxRotation);
                    
                    // Check to add placed blocks into a structure for saving
                    if (tickUpdate.chunkGenerator.doSavePlacedObjectsToStructure) 
                        tickUpdate.addObjectToStructure(slotName, "", newPosition, newRotation, new Vector3(1f, 1f, 1f));
                    
                } else {
                    
                    // Create item drop
                    GameObject newObject = Instantiate( Resources.Load( slotName )) as GameObject;
                    newObject.name = slotName;
                    newObject.transform.parent = chunkObject.transform.GetChild(2).transform;
                    
                    // Stack the objects up
                    newObject.transform.position = new Vector3(hit_obj.transform.position.x,
                                                               hit_obj.transform.position.y + hit_obj.transform.localScale.y,
                                                               hit_obj.transform.position.z);
                    
                    // Check to add placed blocks into a structure for saving
                    if (tickUpdate.chunkGenerator.doSavePlacedObjectsToStructure) 
                        tickUpdate.addObjectToStructure(slotName, "", newObject.transform.position, new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
                    
                }
                
                // If stack is empty, remove the object from players hand
                if (tickUpdate.inventory.removeItem(tickUpdate.hudInterface.selectedSlot, 1) == 0) {
                    
                    GameObject objectInHand = GameObject.Find("InHandRight");
                    if (objectInHand != null) Destroy(objectInHand);
                    
                    tickUpdate.hudInterface.updateInHand();
                }
                
                return;
            }
            
            
            
            //
            // Check to drop directly on the ground
            //
            
            RaycastHit hit_gnd;
            
            if ( Physics.Raycast(ray, out hit_gnd, HitDistance, GroundMask) ) {
                
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
                
                // Check to add placed blocks into a structure for saving
                if (tickUpdate.chunkGenerator.doSavePlacedObjectsToStructure) 
                    tickUpdate.addObjectToStructure(slotName, "", newObject.transform.position, new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
                
                // If stack is empty, remove the object from players hand
                if (tickUpdate.inventory.removeItem(tickUpdate.hudInterface.selectedSlot, 1) == 0) 
                    tickUpdate.hudInterface.updateInHand();
                
                return;
            }
            
            
            
            
            
        }
        
        
        
        //
        // Drop items
        
        if (Input.GetButton("Drop")) {
            
            coolDownTimer = coolDownTimeOut;
            
            string slotName = tickUpdate.inventory.checkSlot();
            
            if (slotName == "") 
                return;
            
            RaycastHit hit_gnd;
            
            Ray ray = tickUpdate.cameraObject.ScreenPointToRay(Input.mousePosition);
            
            if ( Physics.Raycast(ray, out hit_gnd, HitDistance, GroundMask) ) {
                
                //
                // Drop item on ground
                
                float chunk_x = Mathf.Round(hit_gnd.point.x / 100) * 100;
                float chunk_z = Mathf.Round(hit_gnd.point.z / 100) * 100;
                
                GameObject chunkObject = tickUpdate.chunkGenerator.getChunk(chunk_x, chunk_z);
                if (chunkObject == null) 
                    return;
                
                GameObject newObject = Instantiate( Resources.Load( slotName )) as GameObject;
                newObject.name = slotName;
                
                // Round the object position
                Vector3 roundedPosition;
                roundedPosition.x = Mathf.Round(hit_gnd.point.x);
                roundedPosition.y = Mathf.Round(hit_gnd.point.y);
                roundedPosition.z = Mathf.Round(hit_gnd.point.z);
                
                // Set item data
                ItemTag itemTag = newObject.GetComponent<ItemTag>();
                
                // Save item durability
                if (tickUpdate.inventory.Durability[tickUpdate.hudInterface.selectedSlot] != -1) 
                    itemTag.addTag("durability", tickUpdate.inventory.Durability[tickUpdate.hudInterface.selectedSlot].ToString());
                
                newObject.transform.position = new Vector3(roundedPosition.x, 
                                                           roundedPosition.y + 0.5f + (newObject.transform.localScale.y / 2), 
                                                           roundedPosition.z);
                
                newObject.transform.parent = chunkObject.transform.GetChild(2).transform;
                
                // Check to add an item pickup into a structure for saving
                if (tickUpdate.chunkGenerator.doSavePlacedObjectsToStructure) 
                    tickUpdate.addObjectToStructure(slotName, "", newObject.transform.position, new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
                
                // If stack is empty, remove the object from players hand
                if (tickUpdate.inventory.removeItem(tickUpdate.hudInterface.selectedSlot, 1) == 0) 
                    tickUpdate.hudInterface.updateInHand();
                
                return;
            }
            
        }
        
	}
	
	
	
	public void saveItemToStructure(string name, string data, Vector3 position, Vector3 rotation, Vector3 scale) {
        
        // Check to add an item pickup into a structure for saving
        if (!tickUpdate.chunkGenerator.doSavePlacedObjectsToStructure) 
            return;
        
        StructureItem newStructureItem = new StructureItem();
        newStructureItem.name = name;
        newStructureItem.data = data;
        
        newStructureItem.position = position;
        newStructureItem.rotation = rotation;
        newStructureItem.scale    = scale;
        
        tickUpdate.chunkGenerator.items.Add(newStructureItem);
        
        return;
    }
	
	
	
	
	
	
	void Start () {
        
        tickUpdate = GameObject.Find("GameRules").GetComponent<TickUpdate>();
        
	}
	
	
}




