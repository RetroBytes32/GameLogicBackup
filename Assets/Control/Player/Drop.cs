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
	  
	  if (Input.GetButtonDown("Drop")) {
	    
	    RaycastHit hit_gnd;
	    
	    Ray ray = cameraObject.ScreenPointToRay(Input.mousePosition);
	    
	    /*
	    // Drop onto an object
	    RaycastHit hit_obj;
        if ( Physics.Raycast(ray, out hit_obj, Distance, DropMask) ) {
	      
	      float chunk_x = Mathf.Round(hit_obj.point.x / 100) * 100;
	      float chunk_z = Mathf.Round(hit_obj.point.z / 100) * 100;
	      
	      GameObject chunkObject = tickUpdate.chunkGenerator.getChunk(chunk_x, chunk_z);
	      if (chunkObject == null) return;
	      
	      string objectName = hit_obj.transform.parent.transform.gameObject.name;
	      
	      string SlotName = interfaceScript.inventory.checkSlot();
	      if (SlotName == "") return;
	      
	      
	      // If stack is empty, remove the object from players hand
	      if (interfaceScript.inventory.removeItem() == 0) {
	        
	        GameObject objectInHand = GameObject.Find("InHandRight");
	        if (objectInHand != null) Destroy(objectInHand);
	        
	        interfaceScript.updateInHand();
	        
	      }
	      
	      
	      
	      //
	      // Create drop object
	      GameObject newObject = Instantiate( Resources.Load( SlotName )) as GameObject;
	      newObject.name = SlotName;
	      newObject.transform.parent = chunkObject.transform.GetChild(2).transform;
	      
	      
	      
	      
	      //
	      // Check target object
	      
	      if (objectName == "log") {
	        
	        Vector3 hit_pos    = new Vector3(hit_obj.transform.position.x, hit_obj.transform.position.y, hit_obj.transform.position.z);
	        Vector3 target_pos = new Vector3(1f, 1f, 1f);
	        
	        if (hit_obj.transform.gameObject.name == "top")    target_pos = new Vector3(hit_pos.x, hit_pos.y+1f, hit_pos.z);
	        if (hit_obj.transform.gameObject.name == "bottom") target_pos = new Vector3(hit_pos.x, hit_pos.y-1f, hit_pos.z);
	        if (hit_obj.transform.gameObject.name == "left")   target_pos = new Vector3(hit_pos.x, hit_pos.y, hit_pos.z+1f);
	        if (hit_obj.transform.gameObject.name == "right")  target_pos = new Vector3(hit_pos.x, hit_pos.y, hit_pos.z-1f);
	        if (hit_obj.transform.gameObject.name == "front")  target_pos = new Vector3(hit_pos.x-1f, hit_pos.y, hit_pos.z);
	        if (hit_obj.transform.gameObject.name == "back")   target_pos = new Vector3(hit_pos.x+1f, hit_pos.y, hit_pos.z);
	        
	        newObject.transform.position = new Vector3(target_pos.x, target_pos.y, target_pos.z);
	        
	      }
	      
	      
	      
	      //
	      // Check object to be placed
	      
	      if (SlotName == "log") {
	        
	        ChunkTag chunkTag = chunkObject.transform.GetChild(0).gameObject.GetComponent<ChunkTag>();
	        if (chunkTag == null) return;
	        
	        Renderer staticMeshRenderer = newObject.GetComponent<Renderer>();
	        if (staticMeshRenderer == null) return;
	        
	        //staticMeshRenderer.material.color = tickUpdate.chunkGenerator.biomes[chunkTag.biome].treeWoodColor;
	        
	      }
	      
	      return;
	    }
	    
	    */
	    
	    
	    
	    
	    
	    
	    // Check to drop directly on the ground
	    if ( Physics.Raycast(ray, out hit_gnd, Distance, GroundMask) ) {
	      
	      string SlotName = interfaceScript.inventory.checkSlot();
	      
	      if (SlotName == "") 
	        return;
	      
	      
	      
	      
	      //
	      // Empty hand modify terrain
	      /*
	      if (SlotName == "") {
	        
	        ChunkTag chunkTag = hit_gnd.transform.gameObject.GetComponent<ChunkTag>();
	        if (chunkTag == null) return;
	        
	        float terrain_damage = 1f;
	        
	        // Update joining chunk edges
	        tickUpdate.update_nearby_chunks(hit_gnd.point.x, hit_gnd.point.z, chunkTag, terrain_damage);
	        
	        return;
	      }
	      */
	      
	      
	      
	      
	      
	      
	      //
	      // Drop item on ground
	      
	      float chunk_x = Mathf.Round(hit_gnd.point.x / 100) * 100;
	      float chunk_z = Mathf.Round(hit_gnd.point.z / 100) * 100;
	      
	      GameObject chunkObject = tickUpdate.chunkGenerator.getChunk(chunk_x, chunk_z);
	      if (chunkObject == null) return;
	      
	      GameObject newObject = Instantiate( Resources.Load( SlotName )) as GameObject;
	      newObject.name = SlotName;
	      newObject.transform.Translate( new Vector3(hit_gnd.point.x, hit_gnd.point.y + (newObject.transform.localScale.y / 2), hit_gnd.point.z) );
	      
	      newObject.transform.parent = chunkObject.transform.GetChild(2).transform;
	      
	      
	      if (SlotName == "log") {
	        
	        ChunkTag chunkTag = chunkObject.transform.GetChild(0).gameObject.GetComponent<ChunkTag>();
	        if (chunkTag == null) return;
	        
	        Renderer staticMeshRenderer = newObject.GetComponent<Renderer>();
	        if (staticMeshRenderer == null) return;
	        
	        //staticMeshRenderer.material.color = tickUpdate.chunkGenerator.biomes[chunkTag.biome].treeWoodColor;
	        
	      }
	      
	      
	      
	      // If stack is empty, remove the object from players hand
	      if (interfaceScript.inventory.removeItem() == 0) {
	        
	        interfaceScript.updateInHand();
	        
	      }
	      
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




