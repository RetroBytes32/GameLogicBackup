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
	
	public int     grabbedCurrentHoverArea = 0;
	public int     grabbedCurrentHoverSlot = -1;
	public int     grabbedLastHoverSlot    = -1;
	
	
	
	
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
        
        
        if (Input.GetButtonDown("Place")) {
            
            grabbedCurrentHoverSlot = -1;
            grabbedLastHoverSlot    = -1;
            
        }
        
        
        if (Input.GetButton("Place")) {
            
            
            // Check inventory system splay
            if (grabbedCurrentHoverArea == 0) {
                
                if (grabbedCurrentHoverSlot != grabbedLastHoverSlot) {
                    grabbedLastHoverSlot = grabbedCurrentHoverSlot;
                    
                    int itemIndex = grabbedLastHoverSlot;
                    
                    if (isHolding) {
                        
                        if (!inventory.State[itemIndex]) {
                            
                            inventory.State[itemIndex] = true;
                            
                            inventory.Name[itemIndex]         = grabbedItemName;
                            inventory.Stack[itemIndex]        = 1;
                            inventory.StackMax[itemIndex]     = grabbedItemStackMax;
                            inventory.Durability[itemIndex]   = grabbedItemDurability;
                            inventory.Data[itemIndex]         = grabbedItemData;
                            
                            
                            inventory.slot_image[itemIndex].texture = grabbedItemImage.texture;
                            
                            inventory.slot_count[itemIndex].text = "";
                            
                            grabbedItemStack--;
                            if (grabbedItemStack < 2) {
                                grabbedItemCount.text = "";
                            } else {
                                grabbedItemCount.text = grabbedItemStack.ToString();
                            }
                            
                            if (grabbedItemStack == 0) {
                                
                                // Clear last item
                                isHolding = false;
                                grabbedItem.SetActive(false);
                                
                                grabbedItemName        = "";
                                grabbedItemStack       = 0;
                                grabbedItemStackMax    = 0;
                                grabbedItemDurability  = 0;
                                grabbedItemData        = "";
                                
                            }
                            
                        // Same item, add one in
                        } else {
                            
                            if ((inventory.Name[itemIndex] == grabbedItemName) & 
                                (inventory.Data[itemIndex] == grabbedItemData)) {
                                
                                inventory.Stack[itemIndex]++;
                                if (inventory.Stack[itemIndex] < 2) {
                                    inventory.slot_count[itemIndex].text = "";
                                } else {
                                    inventory.slot_count[itemIndex].text = inventory.Stack[itemIndex].ToString();
                                }
                                
                                grabbedItemStack--;
                                if (grabbedItemStack < 2) {
                                    grabbedItemCount.text = "";
                                } else {
                                    grabbedItemCount.text = grabbedItemStack.ToString();
                                }
                                
                                if (grabbedItemStack == 0) {
                                    
                                    // Clear last item
                                    isHolding = false;
                                    grabbedItem.SetActive(false);
                                    
                                    grabbedItemName        = "";
                                    grabbedItemStack       = 0;
                                    grabbedItemStackMax    = 0;
                                    grabbedItemDurability  = 0;
                                    grabbedItemData        = "";
                                    
                                }
                                
                            }
                            
                        }
                        
                    }
                    
                    
                }
                
            } 
            
            
            
            
            
            // Check crafting grid splay
            if (grabbedCurrentHoverArea == 1) {
                
                if (grabbedCurrentHoverSlot != grabbedLastHoverSlot) {
                    grabbedLastHoverSlot = grabbedCurrentHoverSlot;
                    
                    int itemIndex = grabbedLastHoverSlot;
                    
                    if (isHolding) {
                        
                        if (!inventory.crafting.State[itemIndex]) {
                            
                            inventory.crafting.State[itemIndex] = true;
                            
                            inventory.crafting.Name[itemIndex]         = grabbedItemName;
                            inventory.crafting.Stack[itemIndex]        = 1;
                            inventory.crafting.StackMax[itemIndex]     = grabbedItemStackMax;
                            inventory.crafting.Durability[itemIndex]   = grabbedItemDurability;
                            inventory.crafting.Data[itemIndex]         = grabbedItemData;
                            
                            
                            inventory.crafting.slot_image[itemIndex].texture = grabbedItemImage.texture;
                            
                            inventory.crafting.slot_count[itemIndex].text = "";
                            
                            grabbedItemStack--;
                            if (grabbedItemStack < 2) {
                                grabbedItemCount.text = "";
                            } else {
                                grabbedItemCount.text = grabbedItemStack.ToString();
                            }
                            
                            if (grabbedItemStack == 0) {
                                
                                // Clear last item
                                isHolding = false;
                                grabbedItem.SetActive(false);
                                
                                grabbedItemName        = "";
                                grabbedItemStack       = 0;
                                grabbedItemStackMax    = 0;
                                grabbedItemDurability  = 0;
                                grabbedItemData        = "";
                                
                            }
                            
                        // Same item, add one in
                        } else {
                            
                            if ((inventory.crafting.Name[itemIndex] == grabbedItemName) & 
                                (inventory.crafting.Data[itemIndex] == grabbedItemData)) {
                                
                                inventory.crafting.Stack[itemIndex]++;
                                if (inventory.crafting.Stack[itemIndex] < 2) {
                                    inventory.crafting.slot_count[itemIndex].text = "";
                                } else {
                                    inventory.crafting.slot_count[itemIndex].text = inventory.crafting.Stack[itemIndex].ToString();
                                }
                                
                                grabbedItemStack--;
                                if (grabbedItemStack < 2) {
                                    grabbedItemCount.text = "";
                                } else {
                                    grabbedItemCount.text = grabbedItemStack.ToString();
                                }
                                
                                if (grabbedItemStack == 0) {
                                    
                                    // Clear last item
                                    isHolding = false;
                                    grabbedItem.SetActive(false);
                                    
                                    grabbedItemName        = "";
                                    grabbedItemStack       = 0;
                                    grabbedItemStackMax    = 0;
                                    grabbedItemDurability  = 0;
                                    grabbedItemData        = "";
                                    
                                }
                                
                            }
                            
                        }
                        
                    }
                    
                    
                }
                
                // Update changes to the crafting grid
                checkCraftingTable();
                
            }
            
	    }
	    
        return;
	}
	
	
	
	
	
	
	
	
	public void clickInventoryItem(int itemIndex) {
	    
	    if (!tickUpdate.isCrafting)
            return;
        
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
                        
                        // Check stack MAX
                        if ((grabbedItemStack + inventory.Stack[itemIndex]) > inventory.StackMax[itemIndex]) {
                            
                            grabbedItemStack = (grabbedItemStack + inventory.Stack[itemIndex]) - inventory.StackMax[itemIndex];
                            if (grabbedItemStack < 2) {
                                grabbedItemCount.text = "";
                            } else {
                                grabbedItemCount.text = grabbedItemStack.ToString();
                            }
                            
                            inventory.Stack[itemIndex] = inventory.StackMax[itemIndex];
                            
                        } else {
                            
                            inventory.Stack[itemIndex] += grabbedItemStack;
                            
                            isHolding = false;
                            grabbedItem.SetActive(false);
                        }
                        
                    // Items are different, swap them
                    } else {
                        
                        string  tempItemName        = grabbedItemName;
                        int     tempItemStack       = grabbedItemStack;
                        int     tempItemStackMax    = grabbedItemStackMax;
                        int     tempItemDurability  = grabbedItemDurability;
                        string  tempItemData        = grabbedItemData;
                        Texture tempItemImage       = grabbedItemImage.texture;
                        
                        grabbedItemName            = inventory.Name[itemIndex];
                        grabbedItemStack           = inventory.Stack[itemIndex];
                        grabbedItemStackMax        = inventory.StackMax[itemIndex];
                        grabbedItemDurability      = inventory.Durability[itemIndex];
                        grabbedItemData            = inventory.Data[itemIndex];
                        grabbedItemImage.texture   = inventory.slot_image[itemIndex].texture;
                        
                        inventory.Name[itemIndex]       = tempItemName;
                        inventory.Stack[itemIndex]      = tempItemStack;
                        inventory.StackMax[itemIndex]   = tempItemStackMax;
                        inventory.Durability[itemIndex] = tempItemDurability;
                        inventory.Data[itemIndex]       = tempItemData;
                        inventory.slot_image[itemIndex].texture = tempItemImage;
                        
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
                
                // Check stack MAX
                if ((inventory.Stack[itemIndex]) >= grabbedItemStackMax) 
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
    
    
    
    
    
	
	
	
	
	
	
	
	public void checkCraftingTable() {
        int recipeIndex = -1;
        
        // Check recipe list
        for (int i=0; i < tickUpdate.recipes.Length; i++) {
            
            for (int a=0; a < 9; a++) {
                
                if (tickUpdate.recipes[i].layout[a] != inventory.crafting.Name[a]) {
                    recipeIndex = -1;
                    break;
                }
                
                recipeIndex = i;
            }
            
            if (recipeIndex > -1) 
                break;
        }
        
        if (recipeIndex == -1) {
            inventory.crafting.crafting_result.SetActive(false);
            inventory.crafting.result_image.texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
            inventory.crafting.result_state = false;
            return;
        }
        
        int resultIndex = findInventoryItem( tickUpdate.recipes[recipeIndex].resultName );
        int resultCount = tickUpdate.recipes[recipeIndex].resultCount;
        
        if (resultIndex == -1) 
            return;
        
        inventory.crafting.result_image.texture = tickUpdate.items[resultIndex].inventoryImage.GetTexture("_MainTex");
        inventory.crafting.result_state = true;
        inventory.crafting.result_durability = tickUpdate.recipes[recipeIndex].resultDurability;
        inventory.crafting.result_stackMax   = tickUpdate.items[resultIndex].stackMax;
        
        if (resultCount < 2) {
            inventory.crafting.result_count.text = "";
            inventory.crafting.result_stack = 1;
        } else {
            inventory.crafting.result_count.text = resultCount.ToString();
            inventory.crafting.result_stack = resultCount;
        }
        
        inventory.crafting.result_name = tickUpdate.items[resultIndex].name;
        
        inventory.crafting.crafting_result.SetActive(true);
        
        return;
    }
	
	
	
	
	
	
	
	
	
	
	
	public void clickCraftingResult() {
        
	    if (!tickUpdate.isCrafting) return;
        if (!inventory.crafting.result_state) return;
        
	    // Left click - Grab item from crafting result
	    if (Input.GetButtonDown("Attack")) {
            
            if (!isHolding) {
                
                if (inventory.crafting.result_stack > inventory.crafting.result_stackMax) 
                    return;
                
                isHolding = true;
                grabbedItem.SetActive(true);
                
                grabbedItemImage.texture = inventory.crafting.result_image.texture;
                grabbedItemCount.text    = inventory.crafting.result_count.text;
                
                grabbedItemName        = inventory.crafting.result_name;
                grabbedItemStack       = inventory.crafting.result_stack;
                grabbedItemStackMax    = inventory.crafting.result_stackMax;
                grabbedItemDurability  = inventory.crafting.result_durability;
                grabbedItemData        = inventory.crafting.result_data;
                
                for (int i=0; i < 9; i++) {
                    
                    if (!inventory.crafting.State[i]) 
                        continue;
                    
                    inventory.crafting.Stack[i]--;
                    inventory.crafting.slot_count[i].text = inventory.crafting.Stack[i].ToString();
                    
                    if (inventory.crafting.Stack[i] < 2) {
                        inventory.crafting.slot_count[i].text = "";
                    } else {
                        inventory.crafting.slot_count[i].text = inventory.crafting.Stack[i].ToString();
                    }
                    
                    
                    if (inventory.crafting.Stack[i] == 0) {
                        
                        inventory.crafting.Name[i]  = "";
                        inventory.crafting.State[i] = false;
                        
                        inventory.crafting.slot_image[i].texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
                        inventory.crafting.slot_count[i].text = "";
                        
                        inventory.crafting.crafting_result.SetActive(false);
                        inventory.crafting.result_image.texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
                        inventory.crafting.result_count.text    = "";
                        
                        inventory.crafting.result_state = false;
                    }
                    
                }
                
            } else {
                
                // Check were holding the same item
                if (grabbedItemName != inventory.crafting.result_name) 
                    return;
                
                if ((inventory.crafting.result_stack + grabbedItemStack) > inventory.crafting.result_stackMax) 
                    return;
                
                isHolding = true;
                grabbedItem.SetActive(true);
                
                grabbedItemName        = inventory.crafting.result_name;
                grabbedItemStack      += inventory.crafting.result_stack;
                grabbedItemStackMax    = inventory.crafting.result_stackMax;
                grabbedItemDurability  = inventory.crafting.result_durability;
                grabbedItemData        = inventory.crafting.result_data;
                
                grabbedItemImage.texture = inventory.crafting.result_image.texture;
                grabbedItemCount.text    = grabbedItemStack.ToString();
                
                for (int i=0; i < 9; i++) {
                    
                    if (!inventory.crafting.State[i]) 
                        continue;
                    
                    inventory.crafting.Stack[i]--;
                    inventory.crafting.slot_count[i].text = inventory.crafting.Stack[i].ToString();
                    
                    if (inventory.crafting.Stack[i] < 2) {
                        inventory.crafting.slot_count[i].text = "";
                    } else {
                        inventory.crafting.slot_count[i].text = inventory.crafting.Stack[i].ToString();
                    }
                    
                    
                    if (inventory.crafting.Stack[i] == 0) {
                        
                        inventory.crafting.Name[i]  = "";
                        inventory.crafting.State[i] = false;
                        
                        inventory.crafting.slot_image[i].texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
                        inventory.crafting.slot_count[i].text = "";
                        
                        inventory.crafting.crafting_result.SetActive(false);
                        inventory.crafting.result_image.texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
                        inventory.crafting.result_count.text    = "";
                        
                        inventory.crafting.result_state = false;
                    }
                    
                }
                
            }
            
	    }
        
        return;
	}
	
	
	
	
	
	
	public void clickCraftingItem(int itemIndex) {
        
	    if (!tickUpdate.isCrafting) return;
        
	    // Left click - Drop items onto crafting grid
	    if (Input.GetButtonDown("Attack")) {
            
            if (isHolding) {
                
                if (!inventory.crafting.State[itemIndex]) {
                    
                    inventory.crafting.Name[itemIndex]       = grabbedItemName;
                    inventory.crafting.Stack[itemIndex]      = grabbedItemStack;
                    inventory.crafting.StackMax[itemIndex]   = grabbedItemStackMax;
                    inventory.crafting.Durability[itemIndex] = grabbedItemDurability;
                    inventory.crafting.Data[itemIndex]       = grabbedItemData;
                    
                    inventory.crafting.State[itemIndex] = true;
                    
                    inventory.crafting.slot_image[itemIndex].texture = grabbedItemImage.texture;
                    if (inventory.crafting.Stack[itemIndex] < 2) {
                        inventory.crafting.slot_count[itemIndex].text = "";
                    } else {
                        inventory.crafting.slot_count[itemIndex].text = inventory.crafting.Stack[itemIndex].ToString();
                    }
                    
                    // Clear last item
                    isHolding = false;
                    grabbedItem.SetActive(false);
                    
                    grabbedItemName        = "";
                    grabbedItemStack       = 0;
                    grabbedItemStackMax    = 0;
                    grabbedItemDurability  = 0;
                    grabbedItemData        = "";
                    
                // Grab item from the crafting grid if were holding the same type
                } else {
                    
                    // Check type and data
                    if ((inventory.crafting.Name[itemIndex] == grabbedItemName) & 
                        (inventory.crafting.Data[itemIndex] == grabbedItemData)) {
                        
                        if ((inventory.crafting.Stack[itemIndex] + grabbedItemStack) > inventory.crafting.StackMax[itemIndex]) {
                            
                            grabbedItemStack = (grabbedItemStack + inventory.crafting.Stack[itemIndex]) - inventory.crafting.StackMax[itemIndex];
                            
                            inventory.crafting.Stack[itemIndex] = inventory.crafting.StackMax[itemIndex];
                            
                            if (inventory.crafting.Stack[itemIndex] < 2) {
                                inventory.crafting.slot_count[itemIndex].text = "";
                            } else {
                                inventory.crafting.slot_count[itemIndex].text = inventory.crafting.Stack[itemIndex].ToString();
                            }
                            
                            if (grabbedItemStack < 2) {
                                grabbedItemCount.text = "";
                            } else {
                                grabbedItemCount.text = grabbedItemStack.ToString();
                            }
                            
                        } else {
                            
                            inventory.crafting.slot_image[itemIndex].texture = grabbedItemImage.texture;
                            
                            inventory.crafting.Stack[itemIndex] += grabbedItemStack;
                            if (inventory.crafting.Stack[itemIndex] < 2) {
                                inventory.crafting.slot_count[itemIndex].text = "";
                            } else {
                                inventory.crafting.slot_count[itemIndex].text = inventory.crafting.Stack[itemIndex].ToString();
                            }
                            
                            // Clear holding item
                            isHolding = false;
                            grabbedItem.SetActive(false);
                            
                            grabbedItemName        = "";
                            grabbedItemStack       = 0;
                            grabbedItemStackMax    = 0;
                            grabbedItemDurability  = 0;
                            grabbedItemData        = "";
                            
                        }
                        
                    // Items are not the same, swap them
                    } else {
                        
                        string   tempItemName        = grabbedItemName;
                        int      tempItemStack       = grabbedItemStack;
                        int      tempItemStackMax    = grabbedItemStackMax;
                        int      tempItemDurability  = grabbedItemDurability;
                        string   tempItemData        = grabbedItemData;
                        Texture  tempItemImage       = grabbedItemImage.texture;
                        
                        grabbedItemName          = inventory.crafting.Name[itemIndex];
                        grabbedItemStack         = inventory.crafting.Stack[itemIndex];
                        grabbedItemStackMax      = inventory.crafting.StackMax[itemIndex];
                        grabbedItemDurability    = inventory.crafting.Durability[itemIndex];
                        grabbedItemData          = inventory.crafting.Data[itemIndex];
                        grabbedItemImage.texture = inventory.crafting.slot_image[itemIndex].texture;
                        
                        inventory.crafting.Name[itemIndex]       = tempItemName;
                        inventory.crafting.Stack[itemIndex]      = tempItemStack;
                        inventory.crafting.StackMax[itemIndex]   = tempItemStackMax;
                        inventory.crafting.Durability[itemIndex] = tempItemDurability;
                        inventory.crafting.Data[itemIndex]       = tempItemData;
                        inventory.crafting.slot_image[itemIndex].texture = tempItemImage;
                        
                        if (grabbedItemStack < 2) {
                            grabbedItemCount.text = "";
                        } else {
                            grabbedItemCount.text = grabbedItemStack.ToString();
                        }
                        
                        if (inventory.crafting.Stack[itemIndex] < 2) {
                            inventory.crafting.slot_count[itemIndex].text = "";
                        } else {
                            inventory.crafting.slot_count[itemIndex].text = inventory.crafting.Stack[itemIndex].ToString();
                        }
                        
                    }
                    
                }
                
            } else {
                
                // Empty hand, grab the item from the crafting grid
                if (inventory.crafting.State[itemIndex]) {
                    
                    isHolding = true;
                    grabbedItem.SetActive(true);
                    
                    grabbedItemName        = inventory.crafting.Name[itemIndex];
                    grabbedItemStack       = inventory.crafting.Stack[itemIndex];
                    grabbedItemStackMax    = inventory.crafting.StackMax[itemIndex];
                    grabbedItemDurability  = inventory.crafting.Durability[itemIndex];
                    grabbedItemData        = inventory.crafting.Data[itemIndex];
                    
                    if (grabbedItemStack < 2) {
                        grabbedItemCount.text = "";
                    } else {
                        grabbedItemCount.text = grabbedItemStack.ToString();
                    }
                    
                    inventory.crafting.Name[itemIndex]  = "";
                    inventory.crafting.State[itemIndex] = false;
                    
                    grabbedItemImage.texture = inventory.crafting.slot_image[itemIndex].texture;
                    
                    inventory.crafting.slot_image[itemIndex].texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
                    inventory.crafting.slot_count[itemIndex].text    = "";
                }
                
            }
            
            // Update changes to the crafting grid
            checkCraftingTable();
            
	    }
        
        
        
	    // Right click - Drop items into the grid
	    if (Input.GetButtonDown("Place")) {
            
            if (isHolding) {
                
                if (inventory.crafting.State[itemIndex]) {
                    
                    if ((inventory.crafting.Name[itemIndex] == grabbedItemName)) {
                        
                        if (inventory.crafting.Stack[itemIndex] >= inventory.crafting.StackMax[itemIndex]) 
                            return;
                        
                        inventory.crafting.Stack[itemIndex]++;
                        
                        if (inventory.crafting.Stack[itemIndex] < 2) {
                            inventory.crafting.slot_count[itemIndex].text = "";
                        } else {
                            inventory.crafting.slot_count[itemIndex].text = inventory.crafting.Stack[itemIndex].ToString();
                        }
                        
                        grabbedItemStack--;
                        
                        if (grabbedItemStack < 2) {
                            grabbedItemCount.text = "";
                        } else {
                            grabbedItemCount.text = grabbedItemStack.ToString();
                        }
                        
                        if (grabbedItemStack == 0) {
                            isHolding = false;
                            grabbedItem.SetActive(false);
                        }
                        
                    }
                    
                // Slot empty, add item
                } else {
                    
                    inventory.crafting.State[itemIndex] = true;
                    
                    inventory.crafting.Name[itemIndex]        = grabbedItemName;
                    inventory.crafting.Stack[itemIndex]       = 1;
                    inventory.crafting.StackMax[itemIndex]    = grabbedItemStackMax;
                    inventory.crafting.Durability[itemIndex]  = grabbedItemDurability;
                    inventory.crafting.Data[itemIndex]        = grabbedItemData;
                    
                    inventory.crafting.slot_image[itemIndex].texture = grabbedItemImage.texture;
                    inventory.crafting.slot_count[itemIndex].text = "";
                    
                    grabbedItemStack--;
                    
                    if (grabbedItemStack < 2) {
                        grabbedItemCount.text = "";
                    } else {
                        grabbedItemCount.text = grabbedItemStack.ToString();
                    }
                    
                    if (grabbedItemStack == 0) {
                        isHolding = false;
                        grabbedItem.SetActive(false);
                    }
                    
                }
                
            // Empty hand, grab half the items in the slot
            } else {
                
                if (inventory.crafting.State[itemIndex]) {
                    
                    int halfStack   = inventory.crafting.Stack[itemIndex] / 2;
                    int remainStack = inventory.crafting.Stack[itemIndex] - halfStack;
                    
                    isHolding = true;
                    grabbedItem.SetActive(true);
                    
                    grabbedItemName        = inventory.crafting.Name[itemIndex];
                    grabbedItemStack       = remainStack;
                    grabbedItemStackMax    = inventory.crafting.StackMax[itemIndex];
                    grabbedItemDurability  = inventory.crafting.Durability[itemIndex];
                    grabbedItemData        = inventory.crafting.Data[itemIndex];
                    
                    grabbedItemImage.texture = inventory.crafting.slot_image[itemIndex].texture;
                    if (grabbedItemStack < 2) {
                        grabbedItemCount.text = "";
                    } else {
                        grabbedItemCount.text = grabbedItemStack.ToString();
                    }
                    
                    inventory.crafting.Stack[itemIndex] = halfStack;
                    
                    if (inventory.crafting.Stack[itemIndex] < 2) {
                        inventory.crafting.slot_count[itemIndex].text = "";
                    } else {
                        inventory.crafting.slot_count[itemIndex].text = inventory.crafting.Stack[itemIndex].ToString();
                    }
                    
                    if (inventory.crafting.Stack[itemIndex] == 0) {
                        
                        inventory.crafting.Name[itemIndex]  = "";
                        inventory.crafting.State[itemIndex] = false;
                        
                        grabbedItemImage.texture = inventory.crafting.slot_image[itemIndex].texture;
                        
                        inventory.crafting.slot_image[itemIndex].texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
                        inventory.crafting.slot_count[itemIndex].text    = "";
                    }
                    
                }
                
            }
            
            // Update changes to the crafting grid
            checkCraftingTable();
	    }
	    
	    return;
	}
	
	
	
	
	
	
	
	
	
	public void flushCraftingGrid() {
        
        for (int i=0; i < 9; i++) {
            
            if (!inventory.crafting.State[i]) 
                continue;
            
            string craftName        = inventory.crafting.Name[i];
            int    craftStack       = inventory.crafting.Stack[i];
            int    craftStackMax    = inventory.crafting.StackMax[i];
            int    craftDurability  = inventory.crafting.Durability[i];
            string craftData        = inventory.crafting.Data[i];
            
            if (inventory.addItem(craftName, craftStack, craftStackMax, craftDurability, craftData) < 0) 
                continue;
            
            inventory.crafting.Name[i]  = "";
            inventory.crafting.State[i] = false;
            
            inventory.crafting.slot_image[i].texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
            inventory.crafting.slot_count[i].text    = "";
        }
        
        inventory.crafting.crafting_result.SetActive(false);
        inventory.crafting.result_image.texture = tickUpdate.items[0].inventoryImage.GetTexture("_MainTex");
        
        return;
	}
	
	
	
	
	
	
	public int findInventoryItem(string name) {
        
        for (int i=0; i < tickUpdate.items.Length; i++) {
            if (tickUpdate.items[i].name != name) 
                continue;
            
            return i;
        }
        
        return -1;
    }
	
	
	
	
	
	public void setLastMouseOverInventorySlot(int index) {
        grabbedCurrentHoverSlot = index;
        grabbedCurrentHoverArea = 0;
    }
	
	
	public void setLastMouseOverCraftingSlot(int index) {
        grabbedCurrentHoverSlot = index;
        grabbedCurrentHoverArea = 1;
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













