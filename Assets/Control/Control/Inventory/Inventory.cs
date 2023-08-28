using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct InventoryItemImage {
	
	public string      name;
    
	public GameObject  image;
}





public class Inventory : MonoBehaviour {
	
	[Space(10)]
	[Header("Inventory item images")]
	[Space(5)]
    
    public InventoryItemImage[]  intentoryItems;
	
	[Space(10)]
	[Header("Slots")]
	[Space(5)]
    
	public bool[]         State           = new bool[8];
	public string[]       Name            = new string[8];
	public int[]          Stack           = new int[8];
	public int[]          StackMax        = new int[8];
	public int[]          Durability      = new int[8];
	
	public GameObject[]   inventory_slots = new GameObject[8];
	public RawImage[]     slot_image      = new RawImage[8];
	public Text[]         slot_count      = new Text[8];
	
	
	[Space(10)]
	[Header("Health / hunger")]
	[Space(5)]
    
	public GameObject[]   health_image  = new GameObject[10];
	public GameObject[]   hunger_image  = new GameObject[10];
	
	
	[Space(10)]
	[Header("Internal state")]
	[Space(5)]
    
	public int     health        = 0;
	public int     hunger        = 0;
	public int     saturation    = 0;
	
	public bool    hungerShake   = false;
	public int     ImageCounter  = 0;
	public int     ShakeCounter  = 0;
	
	public TickUpdate tickUpdate;
	
	
	public void initiate() {
        
        // Initiate inventory item images
        for (int i=0; i < intentoryItems.Length; i++) {
            
            GameObject invItem = Instantiate( Resources.Load( "inv/"+intentoryItems[i].name )) as GameObject;
            invItem.SetActive(false);
            invItem.name = intentoryItems[i].name;
            invItem.transform.SetParent(transform.GetChild(3).transform);
            
        }
        
    }
	
	
	// Clear out associated inventory system objects
	public void shutdown() {
        
        for (int i=0; i < transform.childCount; i++) 
            Destroy( transform.GetChild(3).transform.GetChild(i).transform.gameObject );
        
    }
	
	
	public void setHealth(int value) {
        if (value > 10) value = 10;
        for (int i=0; i < 10; i++) {
            if ( i >= value) {
                health_image[i].GetComponent<RawImage>().color = Color.black;
            } else {
                health_image[i].GetComponent<RawImage>().color = Color.white;
            }
        }
        health = value;
	}
	
	
	
	public void setHunger(int value) {
        if (value > 10) value = 10;
        for (int i=0; i < 10; i++) {
            if ( i >= value) {
                hunger_image[i].GetComponent<RawImage>().color = Color.black;
            } else {
                hunger_image[i].GetComponent<RawImage>().color = Color.white;
            }
        }
        hunger = value;
	}
	
	
	public void setSaturation(int value) {
	    saturation = value;
        if (saturation < 0) saturation = 0;
	}
	
	public void addSaturation(int ammount) {
        saturation += ammount;
	}
	
	public void addHunger(int ammount)    {hunger += ammount; setHunger(hunger);}
	
	public void addHealth(int ammount)    {health += ammount; setHealth(health);}
	
	public void removeSaturation(int ammount) {
        saturation -= ammount;
        if (saturation < 0) saturation = 0;
	}
	
	public void removeHunger(int ammount) {hunger -= ammount; setHunger(hunger); ShakeCounter = 0; hungerShake = true;}
	
	public void removeHealth(int ammount) {health -= ammount; setHealth(health);}
	
	
	
	
	void Update() {
        
        if (!hungerShake) return;
        
        ShakeCounter++;
        if (ShakeCounter >= 2) {
            
            ShakeCounter=0;
            
            hunger_image[ImageCounter].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            
            ImageCounter++;
            if (ImageCounter >= 10) {
                
                ImageCounter=0;
                
                hungerShake = false;
                
            }
            
        }
        
        hunger_image[ImageCounter].transform.localScale = new Vector3(0.2f + ((float)ShakeCounter * 0.1f), 0.2f + ((float)ShakeCounter * 0.1f), 0.2f);
        
	}
	
	
	
	
	
	
	
	public int addItem(string name, int amount, int stackMax, int durability) {
        
        int  ReturnValue = -1;
        
        // Add amount of entities
        for (int c=0; c < amount; c++) {
            
            int slot = 0;
            
            bool isFound     = false;
            
            // Check if this item is in our inventory
            for (int i=0; i < 8; i++) {
                if (Name[i] != name) 
                    continue;
                
                // Ignore if stack is full
                if (Stack[i] >= StackMax[i]) {
                    Stack[i] = StackMax[i];
                    continue;
                }
                
                slot = i;
                
                isFound = true;
                
                break;
            }
            
            // Item found, add to an existing slot
            if (isFound) {
                
                if (Stack[slot] < 1) 
                    Stack[slot] = 1;
                
                // Add one item
                Stack[slot] ++;
                Durability[slot] = durability;
                
                ReturnValue = slot;
                
            // Item not found, check for an empty slot
            } else {
                
                // Get current slot
                int slotTest = GetComponent<Interface>().selectedSlot;
                
                if (State[slotTest]) slotTest = 0;
                
                for (slot=slotTest; slot < 8; slot++) {
                    
                    // Check slot free
                    if (!State[slot]) {
                        
                        State[slot]      = true;
                        Name[slot]       = name;
                        Stack[slot]     += 1;
                        StackMax[slot]   = stackMax;
                        Durability[slot] = durability;
                        
                        ReturnValue = slot;
                        
                        break;
                    }
                    
                }
                
            }
        }
        
        updateInventory();
        
        return ReturnValue;
	}
	
	
	
	
	
	
	public int removeItem() {
        
        int slot = GetComponent<Interface>().selectedSlot;
        
        if (!State[slot]) return 0;
        
        Stack[slot]--;
        
        if (Stack[slot] < 1) {
            
            clearItem();
            return 0;
        }
        
        string FinalCount = Stack[slot].ToString();
        if (Stack[slot] < 2) FinalCount = "";
        
        
        slot_count[slot].text = FinalCount;
        
        
        return Stack[slot];
	}
	
	
	
	
	
	
	public void clear() {
        
        for (int i=0; i < 8; i++) {
            State[i]       = false;
            Name[i]        = "";
            Stack[i]       = 0;
            Durability[i]  = -1;
            
            slot_image[i].texture = tickUpdate.items[0].inventoryImage.texture;
            slot_count[i].text    = "";
            
        }
        
	}
	
	
	
	
	public bool clearItem() {
        
        int slot = GetComponent<Interface>().selectedSlot;
        
        // Check if a slot is taken
        if (State[slot]) {
            
            State[slot]       = false;
            Name[slot]        = "";
            Durability[slot]  = -1;
            slot_image[slot].texture = tickUpdate.items[0].inventoryImage.texture;
            slot_count[slot].text    = "";
            
            return true;
        }
        
        return false;
	}
	
	
	
	public string checkSlot() {
        
        int slot = GetComponent<Interface>().selectedSlot;
        
        if (State[slot]) 
            return Name[slot];
        
        return "";
	}
	
	
	
	public void updateInventory() {
        
        GameObject invObject=null;
        
        for (int i=0; i < 8; i++) {
            
            invObject = null;
            
            for (int c=0; c < this.transform.GetChild(3).childCount; c++) {
                
                if ("inv_"+Name[i] == this.transform.GetChild(3).transform.GetChild(c).gameObject.name) {
                    invObject = this.transform.GetChild(3).transform.GetChild(c).gameObject;
                    break;
                }
            }
            
            if (invObject == null) continue;
            
            State[i] = true;
            slot_image[i].texture = invObject.GetComponent<RawImage>().texture;
            if (Stack[i] > 1) slot_count[i].text = Stack[i].ToString(); else slot_count[i].text = "";
            
        }
        
	}
	
	
	
}




