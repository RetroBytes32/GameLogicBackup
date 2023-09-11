using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interface : MonoBehaviour {
	
    [Space(10)]
    [Header("Currently selected inventory slot")]
	[Space(5)]
    
	public int selectedSlot = 0;
	public GameObject Selector;
	
	
	public Inventory inventory;
	
	public TickUpdate tickUpdate;
	
	
	public GameObject[] inventory_slots = new GameObject[8];
	
	public GameObject  deathMenu;
	
	public GameObject  world;
	
	
	
	
	void Update () {
	  
	  
	  if (tickUpdate.isPaused == true) return;
	  
	  
	  // Increment inventory selector
	  if (Input.GetAxis("ScrollWheel") < 0.0f) {
	    
	    selectedSlot += 1;
	    if (selectedSlot > 7) selectedSlot = 0;
	    
	    setSelection();
	  }
	  
	  
	  
	  // Decrement inventory selector
	  if (Input.GetAxis("ScrollWheel") > 0.0f) {
	    
	    selectedSlot -= 1;
	    if (selectedSlot < 0) selectedSlot = 7;
	    
	    setSelection();
	  }
	  
	  
	}
	
	
	
	
	public void setSelection() {
	  
	  Selector.transform.position = inventory_slots[selectedSlot].transform.position;
	  
	  updateInHand();
	  
	}
	
	
	
	public void RemoveControllers(GameObject gameObject, string Name) {
	  
	  // Destroy the first sub object
	  // This object is reserved for controller scripts and hit boxes
	  if (gameObject.transform.childCount > 0) 
        Destroy( gameObject.transform.GetChild(0).gameObject );
	  
	  
	  /*
	  if (Name == "arrow") {
	    Destroy( gameObject.GetComponent<ArrowControl>() );
	    gameObject.GetComponent<Rigidbody>().detectCollisions = false;
	  }
	  */
	  
	  // Destroy all sub objects
	  //if (gameObject.transform.childCount > 0) {
	  //  for (int i=0; i < gameObject.transform.childCount; i++) 
	  //    Destroy( gameObject.transform.GetChild(i).gameObject );
      
	  return;
	}
	
	
	public void placeInHand(string name) {
	  
	  GameObject HandObject = world.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
	  
	  if (HandObject.transform.childCount > 1) {
	    Destroy( HandObject.transform.GetChild(1).gameObject );
	  }
	  
	  GameObject newObject = Instantiate( Resources.Load( name )) as GameObject;
	  
	  newObject.name = "InHandRight";
	  
	  newObject.transform.position   = HandObject.transform.position;
	  newObject.transform.rotation   = HandObject.transform.rotation;
	  newObject.transform.localScale = HandObject.transform.localScale;
	  newObject.transform.parent     = HandObject.transform;
	  
	  newObject.layer = LayerMask.NameToLayer("UI");
	  
	  for (int i=0; i < newObject.transform.childCount; i++) 
        newObject.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("UI");
      
	  // Dummy object: Remove object controllers (if any)
	  RemoveControllers(newObject, name);
	  
	}
	
	
	public void updateInHand() {
	  
	  GameObject HandObject = world.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
	  
	  if (HandObject.transform.childCount > 1) {
	    GameObject objectInHand = HandObject.transform.GetChild(1).gameObject;
	    Destroy(objectInHand);
	  }
	  
	  string slotName = inventory.checkSlot();
	  if (slotName != "") {
	    
	    placeInHand(slotName);
	    
	  }
	  
	}
	
	
	
}













