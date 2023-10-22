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
	
	
    [Space(10)]
    [Header("Mouse hold inventory item")]
	[Space(5)]
    
	public bool isHolding = false;
	public GameObject grabbedItem;
	public RawImage   grabbedItemImage;
	public Text       grabbedItemCount;
	
	public string  grabbedItemName;
	public int     grabbedItemStack;
	public int     grabbedItemStackMax;
	public int     grabbedItemDurability;
	public string  grabbedItemData;
	
	
	
	
	void Update () {
        
        if (tickUpdate.isPaused) 
            return;
        
        
        // Increment inventory selector
        if (Input.GetAxis("ScrollWheel") < 0.0f) {
            
            selectedSlot += 1;
            if (selectedSlot > 7) 
                selectedSlot = 0;
            
            setSelection();
        }
        
        // Decrement inventory selector
        if (Input.GetAxis("ScrollWheel") > 0.0f) {
            
            selectedSlot -= 1;
            if (selectedSlot < 0) 
                selectedSlot = 7;
            
            setSelection();
        }
        
        
        if (isHolding) {
            
            Vector3 mousePosition = new Vector3(Input.mousePosition.x + 5f, Input.mousePosition.y - 7f, 0f);
            
            grabbedItem.transform.position = mousePosition;
            
            
            
        }
        
        
        
	}
	
	
	
	
	public void checkCraftingTable() {
        
        if (inventory.crafting.Name[4] == "") {
            inventory.crafting.crafting_result.SetActive(false);
            return;
        }
        
        inventory.crafting.result_image.texture = inventory.crafting.slot_image[4].texture;
        inventory.crafting.result_count.text    = inventory.crafting.slot_count[4].text;
        
        inventory.crafting.crafting_result.SetActive(true);
        
        return;
    }
	
	
	
	public void flushCraftingGrid() {
        
        for (int i=0; i < 9; i++) {
            
            if (!inventory.crafting.State[i]) 
                continue;
            
            string craftName        = inventory.crafting.Name[i];
            //int    craftStack       = inventory.crafting.Stack[i];
            int    craftStackMax    = inventory.crafting.StackMax[i];
            int    craftDurability  = inventory.crafting.Durability[i];
            string craftData        = inventory.crafting.Data[i];
            
            inventory.addItem(craftName, 1, craftStackMax, craftDurability, craftData);
            
            inventory.crafting.State[i] = false;
            
            inventory.crafting.slot_image[i].texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
        }
        
        return;
	}
	
	
	
	
	
	
	public void clickCraftingResult() {
        
        
        
        return;
	}
	
	
	
	
	
	
	public void clickCraftingItem(int itemIndex) {
        
	    if (!tickUpdate.isCrafting) return;
        
	    // Left click - Drop item onto crafting grid
	    if (Input.GetButtonDown("Attack")) {
            
            if (isHolding) {
                
                if (!inventory.crafting.State[itemIndex]) {
                    
                    inventory.crafting.slot_image[itemIndex].texture = grabbedItemImage.texture;
                    grabbedItemStack--;
                    
                    if (grabbedItemStack < 2) {
                        grabbedItemCount.text = "";
                    } else {
                        grabbedItemCount.text = grabbedItemStack.ToString();
                    }
                    
                    // Last item
                    if (grabbedItemStack < 1) {
                        isHolding = false;
                        grabbedItem.SetActive(false);
                    }
                    
                    inventory.crafting.Name[itemIndex]       = grabbedItemName;
                    inventory.crafting.Stack[itemIndex]      = grabbedItemStack;
                    inventory.crafting.StackMax[itemIndex]   = grabbedItemStackMax;
                    inventory.crafting.Durability[itemIndex] = grabbedItemDurability;
                    inventory.crafting.Data[itemIndex]       = grabbedItemData;
                    
                    inventory.crafting.State[itemIndex] = true;
                    
                // Grab item from the crafting grid if were holding the same type
                } else {
                    
                    // Check type and data
                    if ((inventory.crafting.Name[itemIndex] == grabbedItemName) & 
                        (inventory.crafting.Data[itemIndex] == grabbedItemData)) {
                        
                        grabbedItemStack++;
                        
                        if (grabbedItemStack < 2) {
                            grabbedItemCount.text = "";
                        } else {
                            grabbedItemCount.text = grabbedItemStack.ToString();
                        }
                        
                        inventory.crafting.State[itemIndex] = false;
                        
                        inventory.crafting.slot_image[itemIndex].texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
                    }
                }
                
            } else {
                
                // Empty hand, grab the item from the crafting grid
                if (inventory.crafting.State[itemIndex]) {
                    
                    isHolding = true;
                    grabbedItem.SetActive(true);
                    
                    grabbedItemName        = inventory.crafting.Name[itemIndex];
                    grabbedItemStack       = 1;
                    grabbedItemStackMax    = inventory.crafting.StackMax[itemIndex];
                    grabbedItemDurability  = inventory.crafting.Durability[itemIndex];
                    grabbedItemData        = inventory.crafting.Data[itemIndex];
                    
                    grabbedItemCount.text = "";
                    
                    inventory.crafting.State[itemIndex] = false;
                    
                    grabbedItemImage.texture = inventory.crafting.slot_image[itemIndex].texture;
                    
                    inventory.crafting.slot_image[itemIndex].texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
                }
                
            }
            
	    }
        
        checkCraftingTable();
        
        return;
	}
	
	
	
	
	
	
	public void clickInventoryItem(int itemIndex) {
	    
	    if (!tickUpdate.isCrafting) return;
        
	    // Left click - pick up or swap items
	    if (Input.GetButtonDown("Attack")) {
            
            // Item is in the slot
            if (inventory.State[itemIndex]) {
                
                // Pick up the inventory item from the slot
                if (!isHolding) {
                    isHolding = true;
                    grabbedItem.SetActive(true);
                    
                    grabbedItemName        = inventory.Name[itemIndex];
                    grabbedItemStack       = inventory.Stack[itemIndex];
                    grabbedItemStackMax    = inventory.StackMax[itemIndex];
                    grabbedItemDurability  = inventory.Durability[itemIndex];
                    grabbedItemData        = inventory.Data[itemIndex];
                    
                    grabbedItemImage.texture = inventory.slot_image[itemIndex].texture;
                    if (grabbedItemStack < 2) {
                        grabbedItemCount.text = "";
                    } else {
                        grabbedItemCount.text = grabbedItemStack.ToString();
                    }
                    
                    inventory.clearItem(itemIndex);
                    
                // Swap both items
                } else {
                    
                    // Items are the same, add them to the same slot
                    if (inventory.Name[itemIndex] == grabbedItemName) {
                        inventory.Stack[itemIndex] += grabbedItemStack;
                        
                        isHolding = false;
                        grabbedItem.SetActive(false);
                        
                    // Items are different, swap them
                    } else {
                        
                        string tempItemName        = grabbedItemName;
                        int    tempItemStack       = grabbedItemStack;
                        int    tempItemStackMax    = grabbedItemStackMax;
                        int    tempItemDurability  = grabbedItemDurability;
                        string tempItemData        = grabbedItemData;
                        
                        grabbedItemName        = inventory.Name[itemIndex];
                        grabbedItemStack       = inventory.Stack[itemIndex];
                        grabbedItemStackMax    = inventory.StackMax[itemIndex];
                        grabbedItemDurability  = inventory.Durability[itemIndex];
                        grabbedItemData        = inventory.Data[itemIndex];
                        
                        inventory.Name[itemIndex]       = tempItemName;
                        inventory.Stack[itemIndex]      = tempItemStack;
                        inventory.StackMax[itemIndex]   = tempItemStackMax;
                        inventory.Durability[itemIndex] = tempItemDurability;
                        inventory.Data[itemIndex]       = tempItemData;
                        
                        grabbedItemImage.texture = inventory.slot_image[itemIndex].texture;
                        if (grabbedItemStack < 2) {
                            grabbedItemCount.text = "";
                        } else {
                            grabbedItemCount.text = grabbedItemStack.ToString();
                        }
                        
                    }
                    
                }
                
            // No item in the slot
            } else {
                
                // Place the item into an empty slot
                if (isHolding) {
                    isHolding = false;
                    grabbedItem.SetActive(false);
                    
                    inventory.Name[itemIndex]       = grabbedItemName;
                    inventory.Stack[itemIndex]      = grabbedItemStack;
                    inventory.StackMax[itemIndex]   = grabbedItemStackMax;
                    inventory.Durability[itemIndex] = grabbedItemDurability;
                    inventory.Data[itemIndex]       = grabbedItemData;
                    
                }
                
            }
            
            inventory.updateInventory();
            updateInHand();
	    }
	    
	    
	    // Right click - Drop one item from the stack into the slot
	    if (Input.GetButtonDown("Place")) {
            
            if (isHolding) {
                
                // Slot item must be the same as what were holding
                if (inventory.State[itemIndex]) 
                    if (inventory.Name[itemIndex] != grabbedItemName) 
                        return;
                
                // Must have the same item data
                if (inventory.Data[itemIndex] != grabbedItemData) 
                    return;
                
                inventory.Name[itemIndex] = grabbedItemName;
                
                grabbedItemStack--;
                inventory.Stack[itemIndex]++;
                
                inventory.StackMax[itemIndex]   = grabbedItemStackMax;
                inventory.Durability[itemIndex] = grabbedItemDurability;
                inventory.Data[itemIndex]       = grabbedItemData;
                
                if (grabbedItemStack < 2) {
                    grabbedItemCount.text = "";
                } else {
                    grabbedItemCount.text = grabbedItemStack.ToString();
                }
                
                // Last item, clear out the item were holding
                if (grabbedItemStack == 0) {
                    isHolding = false;
                    grabbedItem.SetActive(false);
                }
                
            // Take half the items in the slot
            } else {
                
                if (!inventory.State[itemIndex]) 
                    return;
                
                int halfStack   = inventory.Stack[itemIndex] / 2;
                int remainStack = inventory.Stack[itemIndex] - halfStack;
                
                isHolding = true;
                grabbedItem.SetActive(true);
                
                grabbedItemName        = inventory.Name[itemIndex];
                grabbedItemStack       = remainStack;
                grabbedItemStackMax    = inventory.StackMax[itemIndex];
                grabbedItemDurability  = inventory.Durability[itemIndex];
                grabbedItemData        = inventory.Data[itemIndex];
                
                grabbedItemImage.texture = inventory.slot_image[itemIndex].texture;
                if (grabbedItemStack < 2) {
                    grabbedItemCount.text = "";
                } else {
                    grabbedItemCount.text = grabbedItemStack.ToString();
                }
                
                inventory.Stack[itemIndex] = halfStack;
                
                if (inventory.Stack[itemIndex] == 0) {
                    inventory.clearItem(itemIndex);
                }
                
            }
            
            inventory.updateInventory();
            updateInHand();
            
	    }
	    
	    return;
    }
    
	
	
	
	
	
	
	
	
	public void setSelection() {
        
        Selector.transform.position = inventory_slots[selectedSlot].transform.position;
        
        updateInHand();
        
	}
	
	
	
	public void RemoveControllers(GameObject gameObject, string Name) {
        
        // Destroy the first sub object
        // This object is reserved for controller scripts and hit boxes
        if (gameObject.transform.childCount > 0) 
            Destroy( gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider>() );
        
        
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
        
        if (HandObject.transform.childCount > 1) 
            Destroy( HandObject.transform.GetChild(1).gameObject );
        
        GameObject newObject = Instantiate( Resources.Load( name )) as GameObject;
        
        newObject.name = "InHandRight";
        
        newObject.transform.position   = HandObject.transform.position;
        newObject.transform.rotation   = HandObject.transform.rotation;
        newObject.transform.localScale = HandObject.transform.localScale;
        newObject.transform.parent     = HandObject.transform;
        
        LayerMask uiLayer = LayerMask.NameToLayer("UI");
        newObject.layer = uiLayer;
        
        for (int i=0; i < newObject.transform.childCount; i++) 
            newObject.transform.GetChild(i).gameObject.layer = uiLayer;
        
        // Set voxel UI layer
        if (newObject.transform.GetChild(0).transform.childCount == 6) {
            
            for (int i=0; i < newObject.transform.GetChild(0).transform.childCount; i++) 
                newObject.transform.GetChild(0).transform.GetChild(i).gameObject.layer = uiLayer;
            
        }
        
        
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













