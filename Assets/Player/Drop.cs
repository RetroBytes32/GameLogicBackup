﻿using System.Collections;
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
        if (tickUpdate.isCrafting) return;
        
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
            
            // Check if the item is placeable
            bool isPlaceable = false;
            for (int i=0; i < tickUpdate.breakableItem.Length; i++) {
                if (tickUpdate.breakableItem[i].name != slotName) 
                    continue;
                
                isPlaceable = true;
                break;
            }
            
            
            
            
            //
            // Drop a voxel
            //
            
            RaycastHit hit_obj;
            Ray ray = tickUpdate.cameraObject.ScreenPointToRay(Input.mousePosition);
            
            if ( Physics.Raycast(ray, out hit_obj, HitDistance, DropMask) ) {
                
                if (!isPlaceable) 
                    return;
                
                float chunk_x = Mathf.Round(hit_obj.point.x / 100) * 100;
                float chunk_z = Mathf.Round(hit_obj.point.z / 100) * 100;
                
                GameObject chunkObject = tickUpdate.chunkGenerator.getChunk(chunk_x, chunk_z);
                if (chunkObject == null) 
                    return;
                
                ChunkTag chunkTag = chunkObject.GetComponent<ChunkTag>();
                
                // Trigger a chunk update
                if (chunkTag != null) {
                    chunkTag.shouldUpdateVoxels = true;
                    tickUpdate.staticOptimizeCounter = 0;
                }
                
                string objectName = hit_obj.transform.parent.transform.gameObject.name;
                
                // Check if the object has a precise hit box
                if (objectName == "hitbox") {
                    objectName = hit_obj.transform.parent.transform.parent.transform.gameObject.name;
                    
                    // Create item drop
                    GameObject newObject = Instantiate( Resources.Load( slotName )) as GameObject;
                    newObject.name = slotName;
                    newObject.transform.parent = chunkObject.transform.GetChild(2).transform;
                    
                    // Direction offset
                    float posX=0f;
                    float posY=0f;
                    float posZ=0f;
                    
                    // Rotation offset
                    float rotX=0f;
                    float rotY=0f;
                    float rotZ=0f;
                    
                    // Check hit box edges
                    if (hit_obj.normal == Vector3.forward) {posZ  = hit_obj.transform.localScale.z; rotX  = 90f;}
                    if (hit_obj.normal == Vector3.back)    {posZ -= hit_obj.transform.localScale.z; rotX  = 90f;}
                    if (hit_obj.normal == Vector3.left)    {posX -= hit_obj.transform.localScale.x; rotZ  = 90f;}
                    if (hit_obj.normal == Vector3.right)   {posX  = hit_obj.transform.localScale.x; rotZ  = 90f;}
                    if (hit_obj.normal == Vector3.up)      {posY  = hit_obj.transform.localScale.y;}
                    if (hit_obj.normal == Vector3.down)    {posY -= hit_obj.transform.localScale.y;}
                    
                    
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
                    
                    Vector3 randomRotation = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                    newObject.transform.localRotation = Quaternion.Euler(randomRotation);
                    
                    // Stack the objects up
                    newObject.transform.position = new Vector3(hit_obj.transform.position.x,
                                                               hit_obj.transform.position.y + hit_obj.transform.localScale.y,
                                                               hit_obj.transform.position.z);
                    
                    // Check to add placed blocks into a structure for saving
                    if (tickUpdate.chunkGenerator.doSavePlacedObjectsToStructure) 
                        tickUpdate.addObjectToStructure(slotName, "", newObject.transform.position, new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
                    
                }
                
                if (tickUpdate.doDebugMode) 
                    return;
                
                // If stack is empty, remove the object from players hand
                if (tickUpdate.inventory.removeItem(tickUpdate.hudInterface.selectedSlot, 1) == 0) {
                    
                    GameObject objectInHand = GameObject.Find("InHandRight");
                    if (objectInHand != null) Destroy(objectInHand);
                    
                    tickUpdate.hudInterface.updateInHand();
                }
                
                return;
            }
            
            
            
            
            
            
            //
            // Drop a voxel on the ground
            //
            
            RaycastHit hit_gnd;
            
            if ( Physics.Raycast(ray, out hit_gnd, HitDistance, GroundMask) ) {
                
                //
                // Empty hand modify terrain
                /*
                if (slotName == "") {
                    
                    ChunkTag chunkTag = hit_gnd.GetComponent<ChunkTag>();
                    if (chunkTag == null) return;
                    
                    float terrain_damage = 1f;
                    
                    // Update joining chunk edges
                    tickUpdate.update_nearby_chunks(hit_gnd.point.x, hit_gnd.point.z, chunkTag, terrain_damage);
                    
                    return;
                }
                */
                
                // Dont place on ground if its not placeable
                if (!isPlaceable) 
                    return;
                
                float chunk_x = Mathf.Round(hit_gnd.point.x / 100) * 100;
                float chunk_z = Mathf.Round(hit_gnd.point.z / 100) * 100;
                
                GameObject chunkObject = tickUpdate.chunkGenerator.getChunk(chunk_x, chunk_z);
                if (chunkObject == null) 
                    return;
                
                ChunkTag chunkTag = chunkObject.GetComponent<ChunkTag>();
                
                // Trigger a chunk update
                if (chunkTag != null) {
                    chunkTag.shouldUpdateVoxels = true;
                    tickUpdate.staticOptimizeCounter = 0;
                }
                
                // Create voxel drop
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
                
                if (tickUpdate.doDebugMode) 
                    return;
                
                // If stack is empty, remove the object from players hand
                if (tickUpdate.inventory.removeItem(tickUpdate.hudInterface.selectedSlot, 1) == 0) 
                    tickUpdate.hudInterface.updateInHand();
                
                return;
            }
            
            
            
            
            
        }
        
        
        
        //
        // Drop an item
        
        if (Input.GetButton("Drop")) {
            
            string slotName = tickUpdate.inventory.checkSlot();
            
            if (slotName == "") 
                return;
            
            // Check if the item is placeable
            for (int i=0; i < tickUpdate.breakableItem.Length; i++) {
                if (tickUpdate.breakableItem[i].name != slotName) 
                    continue;
                
                return;
            }
            
            coolDownTimer = coolDownTimeOut;
            
            
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
                roundedPosition.y = hit_gnd.point.y;
                roundedPosition.z = Mathf.Round(hit_gnd.point.z);
                
                Vector3 randomRotation = new Vector3(0f, Random.Range(0, 360), 0f);
                newObject.transform.localRotation = Quaternion.Euler(randomRotation);
                
                // Set item data
                ItemTag itemTag  = newObject.GetComponent<ItemTag>();
                itemTag.lifeTime = 100;
                
                // Save item durability
                if (tickUpdate.inventory.Durability[tickUpdate.hudInterface.selectedSlot] != -1) 
                    itemTag.addTag("durability", tickUpdate.inventory.Durability[tickUpdate.hudInterface.selectedSlot].ToString());
                
                newObject.transform.position = new Vector3(roundedPosition.x, 
                                                           roundedPosition.y + (newObject.transform.localScale.y / 2), 
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




