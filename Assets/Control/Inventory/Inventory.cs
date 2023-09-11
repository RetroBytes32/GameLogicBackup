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
	[Header("Inventory slots")]
	[Space(5)]
    
	public string[]       Name            = new string[8];
	public bool[]         State           = new bool[8];
	
	public int[]          Stack           = new int[8];
	public int[]          StackMax        = new int[8];
	public int[]          Durability      = new int[8];
	public string[]       Data            = new string[8];
	
	public GameObject[]   inventory_slots = new GameObject[8];
	public RawImage[]     slot_image      = new RawImage[8];
	public Text[]         slot_count      = new Text[8];
	
	
	[Space(10)]
	[Header("Health / hunger")]
	[Space(5)]
    
	public GameObject[]   health_image  = new GameObject[10];
	public GameObject[]   hunger_image  = new GameObject[10];
	
	
	[Space(10)]
	[Header("Player vitality")]
	[Space(5)]
    
	public int     health        = 0;
	public int     hunger        = 0;
	public int     saturation    = 0;
	
	public bool    hungerShake   = false;
	public int     ImageCounter  = 0;
	public int     ShakeCounter  = 0;
	
	public TickUpdate tickUpdate;
	
	
	
	
	public void setHunger(int ammount) {if (ammount > 10) {ammount = 10;} hunger = ammount; updateVitalityBars();}
	public void addHunger(int ammount) {if (ammount > 10) {ammount = 10;} hunger += ammount; updateVitalityBars();}
	
	public void setHealth(int ammount) {if (ammount > 10) {ammount = 10;} health = ammount; updateVitalityBars();}
	public void addHealth(int ammount) {if (ammount > 10) {ammount = 10;} health += ammount; updateVitalityBars();}
	
	public void setSaturation(int ammount) {saturation = ammount; if (saturation < 0) saturation = 0;}
	public void addSaturation(int ammount) {saturation += ammount;}
	public void removeSaturation(int ammount) {saturation -= ammount; if (saturation < 0) saturation = 0;}
	
	public void removeHunger(int ammount) {hunger -= ammount; setHunger(hunger); ShakeCounter = 0; hungerShake = true;}
	public void removeHealth(int ammount) {health -= ammount; setHealth(health);}
    
    void updateVitalityBars() {
        for (int i=0; i < 10; i++) {
            if (i >= hunger) {
                hunger_image[i].GetComponent<RawImage>().color = Color.black;
            } else {
                hunger_image[i].GetComponent<RawImage>().color = Color.white;
            }
            if (i >= health) {
                health_image[i].GetComponent<RawImage>().color = Color.black;
            } else {
                health_image[i].GetComponent<RawImage>().color = Color.white;
            }
        }
    }
	
	
	
	
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
	
	
	
	
	// Add an item to the inventory
	public int addItem(string name, int amount, int stackMax, int durability, string data="") {
        
        int  ReturnValue = -1;
        
        // Add item with custom data
        if (data != "") {
            
            int slot = 0;
            
            // Check is current slot empty
            int selectedSlot = GetComponent<Interface>().selectedSlot;
            if (State[selectedSlot]) 
                selectedSlot = 0;
            
            // Check selected slot or start from the beginning
            for (slot=selectedSlot; slot < 8; slot++) {
                
                // Check slot free
                if (State[slot]) 
                    continue;
                
                // Add the item
                State[slot]      = true;
                Name[slot]       = name;
                Stack[slot]     += 1;
                StackMax[slot]   = stackMax;
                Durability[slot] = durability;
                Data[slot]       = data;
                
                ReturnValue = slot;
                
                break;
            }
            
            return ReturnValue;
        }
        
        // Add amount of items
        for (int c=0; c < amount; c++) {
            
            int slot = 0;
            
            bool isFound = false;
            
            // Check if this item is in our inventory
            for (int i=0; i < 8; i++) {
                if (Name[i] != name) 
                    continue;
                
                // Ignore this slot if stack is full
                if (Stack[i] >= StackMax[i]) 
                    continue;
                
                // Select this slot
                slot = i;
                
                isFound = true;
                
                break;
            }
            
            // Item found, add to an existing slot
            if (isFound) {
                
                // Add one item
                Stack[slot]++;
                Durability[slot] = durability;
                Data[slot]       = "";
                
                ReturnValue = slot;
                
            // Item not found, check for an empty slot
            } else {
                
                // Check is current slot empty
                int selectedSlot = GetComponent<Interface>().selectedSlot;
                if (State[selectedSlot]) 
                    selectedSlot = 0;
                
                // Check selected slot or start from the beginning
                for (slot=selectedSlot; slot < 8; slot++) {
                    
                    // Check slot free
                    if (State[slot]) 
                        continue;
                    
                    // Add the item
                    State[slot]      = true;
                    Name[slot]       = name;
                    Stack[slot]     += 1;
                    StackMax[slot]   = stackMax;
                    Durability[slot] = durability;
                    Data[slot]       = "";
                    
                    ReturnValue = slot;
                    
                    break;
                }
                
            }
        }
        
        updateInventory();
        
        return ReturnValue;
	}
	
	
	
	
	
	
	
	
	
	// Remove one item from the selected slot
	public int removeItem(int slot, int amount) {
        
        for (int i=0; i < amount; i++) {
            
            if (!State[slot]) 
                return 0;
            
            Stack[slot]--;
            
            if (Stack[slot] < 1) {
                
                clearItem(slot);
                return 0;
            }
            
            // No text when 1 or less
            if (Stack[slot] < 2) {
                slot_count[slot].text = "";
            } else {
                slot_count[slot].text = Stack[slot].ToString();
            }
            
        }
        
        return Stack[slot];
	}
	
	
	
	
	// Reset the system, clearing the slots
	public void clear() {
        for (int i=0; i < 8; i++) 
            clearItem(i);
	}
	
	
	
	// Zero out and clear the slot item
	public bool clearItem(int slot) {
        
        // Check if a slot is taken
        if (State[slot]) {
            
            State[slot]       = false;
            Name[slot]        = "";
            Stack[slot]       = 0;
            StackMax[slot]    = 0;
            Durability[slot]  = -1;
            Data[slot]        = "";
            
            slot_image[slot].texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
            slot_count[slot].text    = "";
            
            return true;
        }
        
        return false;
	}
	
	
	
	// Return the name of the item in the currently selected slot
	public string checkSlot() {
        
        int slot = GetComponent<Interface>().selectedSlot;
        
        if (State[slot]) 
            return Name[slot];
        
        return "";
	}
	
	
	
	
	// Update inventory item images
	public void updateInventory() {
        
        for (int i=0; i < 8; i++) {
            
            Material invItem = null;
            
            for (int c=0; c < tickUpdate.items.Length; c++) {
                if (Name[i] != tickUpdate.items[c].name)
                    continue;
                
                invItem = tickUpdate.items[c].inventoryImage;
                break;
            }
            
            if (invItem == null) continue;
            
            State[i] = true;
            
            // Place the image in the slot
            slot_image[i].texture = invItem.GetTexture("_MainTex");
            
            // Add the inventory count text if needed
            if (Stack[i] > 1) {
                slot_count[i].text = Stack[i].ToString();
            } else {
                slot_count[i].text = "";
            }
            
        }
        
	}
	
	
	
}




