using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;






public class TickUpdate : MonoBehaviour {
    
	public int clientVersion   = 100;
    
    [Space(10)]
    [Header("World information")]
	[Space(5)]
    
	public string   worldName;
	public Vector3  worldSpawn;
	public int      worldSeed;
    
    
	[Space(10)]
    [Header("Game state")]
	[Space(5)]
    
    public bool doDebugMode             = false;
    public bool doSaveChunks            = true;
    public bool doDayNightCycle         = true;
	public bool doWeatherCycle          = true;
    
    
	[Space(10)]
    [Header("Internal state")]
	[Space(5)]
    
    public bool isPaused                = false;
    public bool isCrafting              = false;
    public bool doShowConsole           = false;
	public bool doMouseLook             = true;
	public bool isCameraUnderWater      = false;
	
	
	[Space(10)]
    [Header("Day/night cycle")]
	[Space(5)]
    
	public float InitialSunAngle        = 0.0f;
	public float dayNightCycleRate      = 0.25f;
	public float dayNightCycleCurrent   = 0.0f;
	public float dayNightCycleAngle     = -90.0f;
    
    
	[Space(10)]
    [Header("Sun and sky")]
	[Space(5)]
    
	public Light    sun;
	public float    ambientDay   = 0.87f;
	public float    ambientNight = 0.45f;
	public Material worldSkybox;
    
    
	[Space(10)]
	[Header("Weather cycle")]
	[Space(5)]
    
	public Vector2 fogNear;
	public Vector2 fogFar;
    
	public Color   fogDay               = Color.white;
	public Color   fogNight             = Color.black;
    
    
	[Space(10)]
	[Header("Draw distances")]
	[Space(5)]
    
	public int   RenderDistance      = 4;
	public float StaticDistance      = 0.9f;
	public float EntityDistance      = 0.2f;
    
    
	[Space(10)]
	[Header("Tick rate counter")]
	[Space(5)]
    
	public int   TickRate    = 3;
	public int   TickCounter = 0;
    public float BaseCounter = 0f;
    
    
    
    
    
	[Space(50)]
	[Header("Entity breeding")]
	[Space(5)]
    
	public bool  allowCrossBreeding = false;
	public bool  allowMutations     = false;
	
	public float mutationFactor   = 1f;
	public int   numberOfChildren = 1;
	
    public GameObject gene_m;
	public GameObject gene_p;
    
    public int breedingCoolDownTimer = 0;
    public int breedingCoolDownReset = 30;
    
    public int   maxOffspringPerChunk  = 40;
    public float maxBreedDistance      = 10f;
    
    
    
    [Space(10)]
	[Header("Entity youth")]
	[Space(5)]
    
    public float entityYouthSpawnArea = 10f;
    
    
    [Space(10)]
	[Header("Entity expiration")]
	[Space(5)]
    
    public bool allowEntityExpiration = false;
    
    
    
    
    
    
    [Space(50)]
	[Header("Collectable items")]
	[Space(5)]
    
    public InventoryItem[] items;
    
    
    
    [Space(10)]
	[Header("Summonable entities")]
	[Space(5)]
    
    public SummonableEntity[] entities;
    
    
    
    [Space(10)]
	[Header("Consumable items")]
	[Space(5)]
    
    public ConsumableItem[] consumableItem;
    
    
    
    [Space(10)]
	[Header("Breakable items")]
	[Space(5)]
    
    public BreakableItem[] breakableItem;
    
    
    
    [Space(10)]
	[Header("Shatterable items")]
	[Space(5)]
    
    public ShatterableItem[] shatterableItem;
    
    
    
    [Space(10)]
	[Header("Weapon items")]
	[Space(5)]
    
    public WeaponItem[] weaponItem;
    
    
    
    [Space(40)]
	[Header("Chunk processing")]
	[Space(5)]
    
	public int chunkUpdateRate = 30;
	public int ChunkCounterX=0;
	public int ChunkCounterZ=0;
    public int chunksPerTick=0;
	public int UpdateCounter=0;
    
	[Space(40)]
	[Header("Static object processing")]
	[Space(5)]
    
    public int staticUpdateRate = 50;
    public int staticChunkCounter=0;
	public int staticObjectsPerTick=0;
    public int staticCounter=0;
    
	
	[Space(40)]
	[Header("Static object optimizer")]
	[Space(5)]
    
    public int staticOptimizeUpdateRate = 1;
    public int staticOptimizeChunkCounter=0;
	public int staticOptimizeObjectsPerTick=0;
    public int staticOptimizeCounter=0;
    
	
	[Space(40)]
	[Header("Entity object processing")]
	[Space(5)]
    
    public int entityUpdateRate = 10;
    public int entityChunkCounter=0;
	public int entityObjectsPerTick=0;
	public int entityCounter=0;
    
    
	public float currentLoadingX;
	public float currentLoadingZ;
    public int   currentChunkRing=2;
    
	public bool reset_player = false;
    
    
    
    
    [Space(40)]
	[Header("Internal")]
	[Space(5)]
    
	public LayerMask            VoxelLayerMask;
	
	public bool                 runCameraDamageAnimation = false;
	public float                damageIndicationOffset  = 0f;
    
	public GameObject           ChunkList;
    
	public GameObject           CommandConsole;
    public int                  CommandConsoleTimer;
    public float                CommandConsoleFade;
    
    public GameObject           HUD;
	public PauseMenuController  pauseMenu;
    
    public ChunkGeneration chunkGenerator;
    public ChunkSerializer chunkSerializer;
    
	public GameObject  playerObject;
	public Camera      cameraObject;
    public Inventory   inventory;
	public Interface   hudInterface;
	
	public float playerChunkX;
	public float playerChunkZ;
    
    
    
	//
	// Internal
    
    public       GameObject   deathMenu;
    
	int          HungerTickCounter=0;
	int          HungerShakeCounter=0;
    
	public int   HealthRecharge=0;
    
	int          loadTimeOut=0;
    int          loadCounter=0;
    
    
    
    
    
    
    
    
	//
	// Tick update core
	//
	
	void Update() {
        
        if (!processGUI()) 
            return;
        
        // Load / save chunks quicker when paused
        if (isPaused) {
            
            unloadChunks();
            
            loadChunks();
            
            updateChunks();
            
            updateOptimizeStaticObjects();
            
            return;
        }
        
        
        //
        // Cycle the day and night
        if (doDayNightCycle)
            updateDayNightCycle();
        
        // Correct the fog color for under water effect
        if (isCameraUnderWater) {
            RenderSettings.fogStartDistance = -100;
            RenderSettings.fogEndDistance   =  50;
            RenderSettings.fogColor = chunkGenerator.WaterColor;
        }
        
        
        //
        // Generate or load chunks
        loadTimeOut--;
        if (loadTimeOut < 0) {
            
            if (loadChunks())
                loadTimeOut = Random.Range(2, 10);
            
            unloadChunks();
            
        }
        
        if (cameraObject.transform.position.y < -0.3f) {
            if (!reset_player) 
                isCameraUnderWater = true;
        } else {
            isCameraUnderWater = false;
        }
        
        
        //
        // Tick base counter
        //
        
        updateStaticObjects();
        
        updateOptimizeStaticObjects();
        
        updateChunks();
        
        BaseCounter += Time.deltaTime;
        
        float baseTickRate = 1f + (TickRate * 0.3f);
        
        while (BaseCounter > 1f / baseTickRate) {
            BaseCounter   -= 1f / baseTickRate;
            
            updateEntities();
            
            // Advance the world tick by one
            TickCounter++;
            
            
            //
            // Slow burn saturation
            
            if (Random.Range(0, 100) == 1) 
                inventory.removeSaturation(1);
            
            
            
            //
            // Check to join entity genetics
            //
            
            if (breedingCoolDownTimer < 0) {
                if ((gene_m != null) & (gene_p != null)) {
                    
                    for (int i=0; i < numberOfChildren; i++) 
                        joinGeneticPair();
                    
                    gene_m = null;
                    gene_p = null;
                }
                breedingCoolDownTimer = breedingCoolDownReset;
            } else {
                breedingCoolDownTimer--;
            }
            
            
            
            
            //
            // Check player fall reset
            //
            
            if ((reset_player) | (playerObject.transform.position.y < -100f)) {
                
                reset_player = false;
                
                Vector3 playerPos = new Vector3(playerObject.transform.position.x, 200f, playerObject.transform.position.z);
                Ray ray_obj = new Ray(playerPos, -Vector3.up);
                
                Vector3 hitPos = Vector3.zero;
                
                RaycastHit hit_obj;
                LayerMask GroundLayerMask = LayerMask.GetMask("Ground");
                
                if ( Physics.Raycast(ray_obj, out hit_obj, 500f, GroundLayerMask) ) {
                    hitPos = new Vector3( hit_obj.point.x, hit_obj.point.y + 1f, hit_obj.point.z);
                    playerObject.transform.position = hitPos;
                }
                
            }
            
            
            
            // Update camera FOG distance
            cameraObject.farClipPlane = ((((float)RenderDistance * (float)100))) * 3.0f;
            
            
            // No health or hunger in debug mode
            if (doDebugMode) 
                return;
            
            
            //
            // Hunger indicator animation
            //
            
            HungerShakeCounter++;
            
            if ((inventory.hunger > 5) & (inventory.hunger < 11))
                                    {if (HungerShakeCounter > 80) {HungerShakeCounter=0; inventory.hungerShake = true;}}
            if (inventory.hunger == 5) {if (HungerShakeCounter > 40) {HungerShakeCounter=0; inventory.hungerShake = true;}}
            if (inventory.hunger == 4) {if (HungerShakeCounter > 20) {HungerShakeCounter=0; inventory.hungerShake = true;}}
            if (inventory.hunger == 3) {if (HungerShakeCounter > 6)  {HungerShakeCounter=0; inventory.hungerShake = true;}}
            if (inventory.hunger == 2) {if (HungerShakeCounter > 4)  {HungerShakeCounter=0; inventory.hungerShake = true;}}
            if (inventory.hunger == 1) {if (HungerShakeCounter > 2)  {HungerShakeCounter=0; inventory.hungerShake = true;}}
            if (inventory.hunger == 0) {inventory.hungerShake = true;}
            
            
            
            
            //
            // Health recharge timer
            //
            
            HealthRecharge++;
            if (HealthRecharge > (inventory.health * 8)) {
                
                HealthRecharge = 0;
                
                if ((inventory.health < 10) & (inventory.hunger > 4)) {
                    
                    inventory.removeHunger(1);
                    inventory.addHealth(1);
                    
                    HealthRecharge = (inventory.health * (inventory.hunger / 10));
                }
            }
            
            
            
            //
            // Hunger timer
            //
            
            int hungerBaseCounter = 350;
            
            HungerTickCounter++;
            if (HungerTickCounter > (hungerBaseCounter + (inventory.saturation * inventory.saturation))) {
                HungerTickCounter = 0;
                
                inventory.removeSaturation(10);
                
                // Remove hunger until starving
                if (inventory.hunger > 0) {
                    
                    inventory.removeHunger( 1 );
                } else {
                    
                    inventory.removeHealth( 1 );
                }
                
            }
            
            
            
            
        }
        
        
        updateCommandConsoleFadeoutTimer();
        
        
        if (runCameraDamageAnimation) 
            animationDamageIndicator();
        
        
        checkPlayerDeath();
        
        return;
	}
	
	
	
	
	
	
	
	
	
	//
	// Fade out the command console log
	
	void updateCommandConsoleFadeoutTimer() {
        
        if (CommandConsoleTimer > -80) {
            
            CommandConsoleTimer--;
            
            // Fade out start
            if (CommandConsoleTimer < 0) {
                
                CommandConsoleTimer = 0;
                
                CommandConsoleFade -= 0.008f;
                
                Text       consoleLine  = CommandConsole.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>();
                Image      consoleBack  = CommandConsole.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
                
                consoleBack.color = new Color(consoleBack.color.r, consoleBack.color.g, consoleBack.color.b, CommandConsoleFade);
                consoleLine.color = new Color(consoleLine.color.r, consoleLine.color.g, consoleLine.color.b, consoleLine.color.a * (CommandConsoleFade + 0.7f));
            }
            
            // Fade out end
            if (CommandConsoleFade < 0) {
                
                CommandConsoleFade = 0.3f;
                
                CommandConsoleTimer = -80;
                
                Text       consoleLine  = CommandConsole.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>();
                Image      consoleBack  = CommandConsole.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
                
                consoleBack.color = new Color(consoleBack.color.r, consoleBack.color.g, consoleBack.color.b, CommandConsoleFade);
                consoleLine.color = new Color(consoleLine.color.r, consoleLine.color.g, consoleLine.color.b, CommandConsoleFade);
                
                GameObject consoleTextObject = CommandConsole.transform.GetChild(0).GetChild(1).gameObject;
                consoleTextObject.SetActive(false);
            }
            
        }
        
    }
	
	
	
	
	
	
	
	
	
	
	
	//
	// Process in game GUI
	//
	
	public bool processGUI() {
        
        //
        // Command console toggle
        
        if (Input.GetKeyDown(KeyCode.Slash)) {
            
            if ((isPaused) | (isCrafting)) 
                return false;
            
            if (doShowConsole) {
                
                // Show the console
                GameObject consoleTextFieldObject = CommandConsole.transform.GetChild(0).GetChild(0).gameObject;
                consoleTextFieldObject.SetActive(false);
                
                doShowConsole = false;
                doMouseLook = true;
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
            } else {
                
                // Hide the console
                doShowConsole = true;
                doMouseLook = false;
                
                GameObject consoleTextFieldObject = CommandConsole.transform.GetChild(0).GetChild(0).gameObject;
                consoleTextFieldObject.SetActive(true);
                
                InputField consoleField = CommandConsole.transform.GetChild(0).GetChild(0).GetComponent<InputField>();
                
                consoleField.Select();
                consoleField.ActivateInputField();
                consoleField.text = "";
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
        }
        
        
        //
        // "Run" the text from the console command
        
        if (doShowConsole) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                
                GameObject consoleTextFieldObject = CommandConsole.transform.GetChild(0).GetChild(0).gameObject;
                GameObject consoleTextObject      = CommandConsole.transform.GetChild(0).GetChild(1).gameObject;
                
                consoleTextFieldObject.SetActive(false);
                consoleTextObject.SetActive(true);
                
                Text       consoleLine  = CommandConsole.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>();
                Image      consoleBack  = CommandConsole.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
                InputField consoleField = CommandConsole.transform.GetChild(0).GetChild(0).GetComponent<InputField>();
                
                consoleLine.color = new Color(consoleLine.color.r, consoleLine.color.g, consoleLine.color.b, 1.0f);
                
                doShowConsole = false;
                doMouseLook = true;
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                string commandText = consoleField.text;
                string result = RunConsoleCommand(commandText);
                
                consoleLine.text = result;
                
                // Reset fade timer
                CommandConsoleTimer = 200;
                CommandConsoleFade -= 0.008f;
                
                consoleBack.color = new Color(consoleBack.color.r, consoleBack.color.g, consoleBack.color.b, CommandConsoleFade);
                consoleLine.color = new Color(consoleLine.color.r, consoleLine.color.g, consoleLine.color.b, consoleLine.color.a * (CommandConsoleFade + 0.7f));
                
            }
        }
        
        
        //
        // Pause menu toggle
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            
            if (isCrafting) {
                
                // Check if were holding an item
                if (hudInterface.isHolding) {
                    hudInterface.isHolding = false;
                    
                    // Put it back
                    inventory.addItem(hudInterface.grabbedItemName, 
                                      hudInterface.grabbedItemStack, 
                                      hudInterface.grabbedItemStackMax, 
                                      hudInterface.grabbedItemDurability, 
                                      hudInterface.grabbedItemData);
                    
                    inventory.updateInventory();
                    hudInterface.updateInHand();
                }
                
                isCrafting = false;
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible   = false;
                doMouseLook      = true;
                
                HUD.transform.GetChild(7).gameObject.SetActive(false);
                HUD.transform.GetChild(8).gameObject.SetActive(false);
                HUD.transform.GetChild(9).gameObject.SetActive(false);
                
                // Return items to the inventory
                hudInterface.flushCraftingGrid();
                
                return true;
            }
            
            isPaused = !isPaused;
            
            if (!isPaused) {
                
                // Deactivate pause menu
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                doMouseLook = true;
                
                pauseMenu.Deactivate();
                
            } else {
                
                // Disable the console if active
                if (doShowConsole) {
                    doShowConsole = false;
                    
                    GameObject consoleTextFieldObject = CommandConsole.transform.GetChild(0).GetChild(0).gameObject;
                    consoleTextFieldObject.SetActive(false);
                    
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    doMouseLook = true;
                    
                    pauseMenu.Deactivate();
                    return true;
                }
                
                // Activate pause menu
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                doMouseLook = false;
                
                pauseMenu.Activate();
                
            }
            
        }
        
        //
        // Toggle crafting grid
        
        if ((!isPaused) & (!doShowConsole)) {
            
            if (Input.GetKeyDown(KeyCode.E)) {
                
                isCrafting = !isCrafting;
                
                // Fade out the console
                CommandConsoleTimer=0;
                CommandConsole.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                
                if (isCrafting) {
                    // Enable the crafting grid
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible   = true;
                    doMouseLook      = false;
                    
                    HUD.transform.GetChild(7).gameObject.SetActive(true);
                    HUD.transform.GetChild(9).gameObject.SetActive(true);
                    
                } else {
                    // Disable the crafting grid
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible   = false;
                    doMouseLook      = true;
                    
                    HUD.transform.GetChild(7).gameObject.SetActive(false);
                    HUD.transform.GetChild(8).gameObject.SetActive(false);
                    HUD.transform.GetChild(9).gameObject.SetActive(false);
                    
                    // Return items to the inventory
                    hudInterface.flushCraftingGrid();
                }
                
            }
            
        }
        
        return true;
    }
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	//
	// Player damage indicator camera shift
	
	void animationDamageIndicator() {
        
        if (damageIndicationOffset == 0f) {
            if (Random.Range(0, 10) > 4) {
                damageIndicationOffset =  40f;
            } else {
                damageIndicationOffset = -40f;
            }
        }
        
        if (damageIndicationOffset > 0f) {
            damageIndicationOffset -= 10f;
        } else {
            damageIndicationOffset += 10f;
        }
        
        if ((damageIndicationOffset > -10f) & (damageIndicationOffset < 10f)) {
            damageIndicationOffset = 0f;
            runCameraDamageAnimation = false;
        }
        
    }
	
	
	
	
	//
	// Check if the player is dead
	
	void checkPlayerDeath() {
        
        if ((inventory.health <= 0)) {
            if (!doDebugMode) {
                
                inventory.health = 0;
                
                deathMenu.SetActive(true);
                
                pauseGame();
                doMouseLook = false;
            }
            
        }
        
        return;
    }
	
	
	
	
	
	
	//
	// Update world chunks
	
	void updateChunks() {
        
        if (isPaused) {
            chunksPerTick = chunkUpdateRate * 10;
        } else {
            chunksPerTick = chunkUpdateRate * (int) (1f / Time.deltaTime);
        }
        
        for (int i=0; i < chunksPerTick; i++) {
            
            if (UpdateCounter >= ChunkList.transform.childCount) 
                UpdateCounter = 0;
            
            GameObject chunkObject = ChunkList.transform.GetChild(UpdateCounter).gameObject;
            GameObject staticList  = chunkObject.transform.GetChild(2).gameObject;
            
            
            UpdateCounter++;
            
            if (UpdateCounter >= ChunkList.transform.childCount) {
                UpdateCounter = 0;
                return;
            }
            
            
            //
            // Process static objects
            //
            
            Vector3 chunkPos  = chunkObject.transform.position;
            Vector3 playerPos = playerObject.transform.position;
            
            chunkPos.y  = 0f;
            playerPos.y = 0f;
            
            float totalEntityDistance = ((float)RenderDistance * StaticDistance) * 100;
            
            // Check chunk distance
            if (Vector3.Distance(chunkPos, playerPos) > totalEntityDistance) {
                
                if (staticList.activeInHierarchy) {
                    staticList.SetActive(false);
                    return;
                }
                
            } else {
                
                if (!staticList.activeInHierarchy) {
                    staticList.SetActive(true);
                    return;
                }
                
            }
            
        }
        
        return;
	}
	
	
	
	
	
	
	
	//
	// Optimize static objects
	//
	
	public void updateOptimizeStaticObjects() {
        
        if (staticOptimizeChunkCounter >= ChunkList.transform.childCount) {
            staticOptimizeChunkCounter = 0;
            staticOptimizeCounter=0;
        }
        
        GameObject chunkObject = ChunkList.transform.GetChild(staticOptimizeChunkCounter).gameObject;
        GameObject staticList  = chunkObject.transform.GetChild(2).gameObject;
        ChunkTag   chunkTag    = chunkObject.transform.GetChild(0).gameObject.GetComponent<ChunkTag>();
        
        staticOptimizeObjectsPerTick = staticOptimizeUpdateRate * (int) (1f / Time.deltaTime);
        
        for (int i=0; i < staticOptimizeObjectsPerTick; i++) {
            
            staticOptimizeCounter++;
            
            // Check end of static object list
            if (staticOptimizeCounter >= staticList.transform.childCount) {
                
                staticOptimizeChunkCounter++;
                
                // Check end of chunk list
                if (staticOptimizeChunkCounter >= ChunkList.transform.childCount)
                    staticOptimizeChunkCounter = 0;
                
                chunkTag.shouldUpdateVoxels = false;
                
                // Get next chunk
                chunkObject = ChunkList.transform.GetChild(staticOptimizeChunkCounter).gameObject;
                staticList  = chunkObject.transform.GetChild(2).gameObject;
                chunkTag    = chunkObject.transform.GetChild(0).gameObject.GetComponent<ChunkTag>();
                
                staticOptimizeCounter=-1;
                continue;
            }
            
            
            // Check should chunk be updated
            if (!chunkTag.shouldUpdateVoxels) {
                
                staticOptimizeChunkCounter++;
                
                // Check end of chunk list
                if (staticOptimizeChunkCounter >= ChunkList.transform.childCount)
                    staticOptimizeChunkCounter = 0;
                
                // Get next chunk
                chunkObject = ChunkList.transform.GetChild(staticOptimizeChunkCounter).gameObject;
                staticList  = chunkObject.transform.GetChild(2).gameObject;
                chunkTag    = chunkObject.transform.GetChild(0).gameObject.GetComponent<ChunkTag>();
                
                staticOptimizeCounter=-1;
                continue;
            }
            
            
            //
            // Optimize out adjacent voxel edges
            //
            
            GameObject staticObject = staticList.transform.GetChild(staticOptimizeCounter).transform.gameObject;
            
            //
            // A cylinder voxel will have five sub objects
            if (staticObject.transform.childCount == 5) {
                GameObject hitbox = staticObject.transform.GetChild(0).gameObject;
                
                MeshRenderer bottomRenderer = staticObject.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>();
                MeshRenderer topRenderer    = staticObject.transform.GetChild(2).gameObject.GetComponent<MeshRenderer>();
                
                hitbox.SetActive(false);
                
                Vector3 voxelDirection = staticObject.transform.rotation.eulerAngles;
                
                if (hitbox.transform.childCount == 6) {
                    
                    // Voxel is laying sideways on its X axis
                    if (voxelDirection.x > 0f) {
                        RaycastHit hit;
                        Ray rayForward  = new Ray(staticObject.transform.position, Vector3.forward);
                        Ray rayBack     = new Ray(staticObject.transform.position, Vector3.back);
                        
                        if (Physics.Raycast(rayForward, out hit, 1, VoxelLayerMask)) {topRenderer   .enabled = false;} else {topRenderer   .enabled = true;}
                        if (Physics.Raycast(rayBack,    out hit, 1, VoxelLayerMask)) {bottomRenderer.enabled = false;} else {bottomRenderer.enabled = true;}
                    }
                    
                    // Voxel is laying sideways on its Z axis
                    if (voxelDirection.z > 0f) {
                        RaycastHit hit;
                        Ray rayLeft   = new Ray(staticObject.transform.position, Vector3.left);
                        Ray rayRight  = new Ray(staticObject.transform.position, Vector3.right);
                        
                        if (Physics.Raycast(rayLeft,  out hit, 1, VoxelLayerMask)) {topRenderer   .enabled = false;} else {topRenderer   .enabled = true;}
                        if (Physics.Raycast(rayRight, out hit, 1, VoxelLayerMask)) {bottomRenderer.enabled = false;} else {bottomRenderer.enabled = true;}
                    }
                    
                    // Voxel is pointing upward
                    if ((voxelDirection.x == 0f) & (voxelDirection.z == 0f)) {
                        RaycastHit hit;
                        Ray rayTop     = new Ray(staticObject.transform.position, Vector3.up);
                        Ray rayBottom  = new Ray(staticObject.transform.position, Vector3.down);
                        
                        if (Physics.Raycast(rayTop,    out hit, 1, VoxelLayerMask)) {topRenderer   .enabled = false;} else {topRenderer   .enabled = true;}
                        if (Physics.Raycast(rayBottom, out hit, 1, VoxelLayerMask)) {bottomRenderer.enabled = false;} else {bottomRenderer.enabled = true;}
                    }
                    
                }
                
                
                hitbox.SetActive(true);
            }
            
            
            //
            // A box voxel will have two sub objects
            //
            if (staticObject.transform.childCount == 2) {
                GameObject hitbox = staticObject.transform.GetChild(0).gameObject;
                hitbox.SetActive(false);
                
                if (hitbox.transform.childCount == 6) {
                    
                    RaycastHit hit;
                    Ray rayFront   = new Ray(staticObject.transform.position, Vector3.forward);
                    Ray rayBack    = new Ray(staticObject.transform.position, Vector3.back);
                    Ray rayLeft    = new Ray(staticObject.transform.position, Vector3.left);
                    Ray rayRight   = new Ray(staticObject.transform.position, Vector3.right);
                    Ray rayTop     = new Ray(staticObject.transform.position, Vector3.up);
                    Ray rayBottom  = new Ray(staticObject.transform.position, Vector3.down);
                    
                    MeshRenderer backRenderer   = hitbox.transform.GetChild(0).GetComponent<MeshRenderer>();
                    MeshRenderer frontRenderer  = hitbox.transform.GetChild(1).GetComponent<MeshRenderer>();
                    MeshRenderer rightRenderer  = hitbox.transform.GetChild(2).GetComponent<MeshRenderer>();
                    MeshRenderer leftRenderer   = hitbox.transform.GetChild(3).GetComponent<MeshRenderer>();
                    MeshRenderer topRenderer    = hitbox.transform.GetChild(4).GetComponent<MeshRenderer>();
                    MeshRenderer bottomRenderer = hitbox.transform.GetChild(5).GetComponent<MeshRenderer>();
                    
                    if (Physics.Raycast(rayBack,   out hit, 1, VoxelLayerMask)) {if (hit.transform.parent.transform.parent.transform.childCount == 2) backRenderer  .enabled = false;} else {backRenderer  .enabled = true;}
                    if (Physics.Raycast(rayFront,  out hit, 1, VoxelLayerMask)) {if (hit.transform.parent.transform.parent.transform.childCount == 2) frontRenderer .enabled = false;} else {frontRenderer .enabled = true;}
                    if (Physics.Raycast(rayRight,  out hit, 1, VoxelLayerMask)) {if (hit.transform.parent.transform.parent.transform.childCount == 2) rightRenderer .enabled = false;} else {rightRenderer .enabled = true;}
                    if (Physics.Raycast(rayLeft,   out hit, 1, VoxelLayerMask)) {if (hit.transform.parent.transform.parent.transform.childCount == 2) leftRenderer  .enabled = false;} else {leftRenderer  .enabled = true;}
                    if (Physics.Raycast(rayTop,    out hit, 1, VoxelLayerMask)) {if (hit.transform.parent.transform.parent.transform.childCount == 2) topRenderer   .enabled = false;} else {topRenderer   .enabled = true;}
                    if (Physics.Raycast(rayBottom, out hit, 1, VoxelLayerMask)) {if (hit.transform.parent.transform.parent.transform.childCount == 2) bottomRenderer.enabled = false;} else {bottomRenderer.enabled = true;}
                    
                }
                
                hitbox.SetActive(true);
            }
            
            continue;
        }
        
        return;
	}
	
	
	
	
	
	
	
	
	
	
	
	//
	// Update static objects
	//
	
	public void updateStaticObjects() {
        
        if (staticChunkCounter >= ChunkList.transform.childCount) {
            staticChunkCounter = 0;
            staticCounter=0;
        }
        
        GameObject chunkObject = ChunkList.transform.GetChild(staticChunkCounter).gameObject;
        GameObject staticList  = chunkObject.transform.GetChild(2).gameObject;
        //ChunkTag   chunkTag    = chunkObject.transform.GetChild(0).gameObject.GetComponent<ChunkTag>();
        
        staticObjectsPerTick = staticUpdateRate * (int) (1f / Time.deltaTime);
        
        for (int i=0; i < staticObjectsPerTick; i++) {
            
            staticCounter++;
            
            // Check end of static object list
            if (staticCounter >= staticList.transform.childCount) {
                
                staticChunkCounter++;
                
                // Check end of chunk list
                if (staticChunkCounter >= ChunkList.transform.childCount)
                    staticChunkCounter = 0;
                
                // Get next chunk
                chunkObject = ChunkList.transform.GetChild(staticChunkCounter).gameObject;
                staticList  = chunkObject.transform.GetChild(2).gameObject;
                //chunkTag    = chunkObject.transform.GetChild(0).gameObject.GetComponent<ChunkTag>();
                
                staticCounter=-1;
                continue;
            }
            
            // Process the static object
            GameObject staticObject = staticList.transform.GetChild(staticCounter).transform.gameObject;
            
            
            // Item life-time counter
            if (staticObject.GetComponent<ItemTag>().lifeTime > 0) {
                staticObject.GetComponent<ItemTag>().lifeTime--;
                
                if (staticObject.GetComponent<ItemTag>().lifeTime == 0) {
                    
                    Destroy(staticObject);
                    continue;
                }
            }
            
            continue;
        }
        
        
        return;
	}
	
	
	
	
	
	
	
	
	// Update nearby entities
	public void updateEntities() {
        
        if (entityChunkCounter >= ChunkList.transform.childCount)
            entityChunkCounter = 0;
        
        GameObject chunkObject = ChunkList.transform.GetChild(entityChunkCounter).gameObject;
        GameObject entityList  = chunkObject.transform.GetChild(1).gameObject;
        
        entityObjectsPerTick = entityUpdateRate * (int) (1f / Time.deltaTime);
        
        // Entity auto breeding
        entityAutoBreeding(entityList);
        
        // Update each entity in the chunk
        for (int i=0; i < entityObjectsPerTick; i++) {
            
            entityCounter++;
            
            // End of chunk.. next
            if (entityCounter >= entityList.transform.childCount) {
                
                entityCounter = -1;
                
                entityChunkCounter++;
                
                // Check last chunk
                if (entityChunkCounter >= ChunkList.transform.childCount)
                    entityChunkCounter = 0;
                
                chunkObject = ChunkList.transform.GetChild(entityChunkCounter).gameObject;
                entityList  = chunkObject.transform.GetChild(1).gameObject;
                
                continue;
            }
            
            // Process the entity
            GameObject entityObject = entityList.transform.GetChild(entityCounter).gameObject;
            if (entityObject == null)
                break;
            
            EntityTag entityTag = entityObject.GetComponent<EntityTag>();
            ActorTag actorTag = entityObject.GetComponent<ActorTag>();
            GeneTag geneTag = entityObject.GetComponent<GeneTag>();
            
            Vector3 entityPos = entityObject.transform.position;
            Vector3 playerPos = playerObject.transform.position;
            
            // Respawn entities who fall through the world
            //  
            //  Respect (-_-) to the few brave entities who fell through the
            //  void before this was implemented...
            
            if (entityPos.y < -100f) {
                
                Vector3 newEntityPos = new Vector3(entityObject.transform.position.x, 500f, entityObject.transform.position.z);
                Ray ray_obj = new Ray(newEntityPos, -Vector3.up);
                
                Vector3 hitPos = Vector3.zero;
                
                RaycastHit hit_obj;
                LayerMask GroundLayerMask = LayerMask.GetMask("Ground");
                
                if ( Physics.Raycast(ray_obj, out hit_obj, 1000f, GroundLayerMask) ) {
                    hitPos = new Vector3( hit_obj.point.x, hit_obj.point.y + 1f, hit_obj.point.z);
                    entityObject.transform.position = hitPos;
                }
            }
            
            //
            // Check entity update distance
            float totalEntityDistance = ((float)RenderDistance * EntityDistance)  * 100;
            
            entityPos.y = 0;
            playerPos.y = 0;
            if (Vector3.Distance(entityPos, playerPos) > totalEntityDistance) {
                
                // Deactivate the entity
                entityObject.SetActive(false);
                
                entityTag.isWalking  = false;
                entityTag.isRunning  = false;
                entityTag.isRotating = false;
                
                if (actorTag != null) {
                    actorTag.isActive = false;
                    actorTag.resetAnimation();
                }
                
                continue;
            }
            
            
            // Check NO AI
            if ((actorTag == null) | (geneTag == null)) 
                continue;
            
            // Advance the age
            entityTag.Age++;
            
            // Stop growth when adult
            if (entityTag.Age < geneTag.ageWhenAdult) {
                
                float growthStep = geneTag.ageSizeInc * 0.001f;
                
                entityObject.transform.localScale += new Vector3(growthStep, growthStep, growthStep);
            }
            
            // Entity death
            if (allowEntityExpiration) {
                
                if (entityTag.Age > geneTag.ageSpan) {
                    
                    if (Random.Range(0, 10) > 8) 
                        entityTag.Health--;
                    
                    if (entityTag.Health < 0) 
                        entityTag.Health = 0;
                }
            }
            
            
            
            
            //
            // Activate and update entity
            
            actorTag.isActive = true;
            
            if (!entityObject.activeInHierarchy) 
                entityObject.SetActive(true);
            
            // Call update on the brain
            if (entityTag.useAI) 
                actorTag.updateAI();
            
            
            // Update entity physics
            entityTag.physicsUpdate();
            
            
            // Update entity genetics tag
            if (geneTag.doUpdateGenetics) {
                geneTag.updateGenetics();
                geneTag.extractGeneticSequence();
                
                //geneTag.doUpdateGenetics = false;
            }
            
            // Check to inject the genetic tag
            if (geneTag.doInjectGenetics) {
                geneTag.injectGeneticSequence( geneTag.gene_string );
                geneTag.updateGenetics();
                
                geneTag.doInjectGenetics = false;
            }
            
            
            
            // Calibrate the entity with the chunk on which it stands
            float chunkX = Mathf.Round(entityObject.transform.position.x / 100) * 100;
            float chunkZ = Mathf.Round(entityObject.transform.position.z / 100) * 100;
            
            GameObject checkChunk = getChunk(chunkX, chunkZ);
            
            if (checkChunk != null) {
                entityTag.currentChunk = checkChunk;
                entityObject.transform.parent = checkChunk.transform.GetChild(1).gameObject.transform;
            }
            
        }
        
        return;
	}
	
	
	
	
	
	
	
	
	
	
	
	
	//
	// Check to load nearby chunks
	
	public bool loadChunks() {
        
        if (chunkSerializer.isLoading) return false;
        
        //
        // Check to finalize a loaded chunk
        
        if (chunkSerializer.isChunkFinalized) {
            
            if (loadCounter > 1) {
                chunkSerializer.chunkFinalize();
                loadCounter = 0;
            } else {
                loadCounter++;
                return false;
            }
            
        }
        
        
        // Start loading a chunk
        if (chunkSerializer.isChunkLoaded) {
            
            if (loadCounter > 1) {
                loadCounter = 0;
                
                chunkSerializer.chunkBuild();
            } else {
                loadCounter++;
                return false;
            }
            
        }
        
        if (chunkSerializer.isChunkFinalized) 
            return false;
        
        
        // Chunk load counter
        ChunkCounterX++;
        if (ChunkCounterX > (currentChunkRing * 2)) {
            
            ChunkCounterX = 0;
            
            ChunkCounterZ++;
            if (ChunkCounterZ > (currentChunkRing * 2)) {
                
                ChunkCounterZ = 0;
            }
        }
        
        
        float newPlayerChunkX = Mathf.Round(playerObject.transform.position.x / 100) * 100;
        float newPlayerChunkZ = Mathf.Round(playerObject.transform.position.z / 100) * 100;
        
        if ((newPlayerChunkX != playerChunkX) | (newPlayerChunkZ != playerChunkZ)) {
            playerChunkX = Mathf.Round(playerObject.transform.position.x / 100) * 100;
            playerChunkZ = Mathf.Round(playerObject.transform.position.z / 100) * 100;
            currentChunkRing = 2;
        }
        
        bool canLoadChunk = false;
        
        float currentPlayerChunkCheckX = (((ChunkCounterX * 100) + playerChunkX)) - (currentChunkRing * 100);
        float currentPlayerChunkCheckZ = (((ChunkCounterZ * 100) + playerChunkZ)) - (currentChunkRing * 100);
        
        if (checkChunkFree(currentPlayerChunkCheckX, currentPlayerChunkCheckZ)) {
            currentLoadingX = currentPlayerChunkCheckX;
            currentLoadingZ = currentPlayerChunkCheckZ;
            canLoadChunk = true;
        }
        
        
        // Check if the chunk can be loaded
        if (canLoadChunk == false) {
            
            // Advance to the next chunk layer outward
            if ((ChunkCounterX == 0) & (ChunkCounterZ == 0)) 
                currentChunkRing++;
            
            if (currentChunkRing > RenderDistance) 
                currentChunkRing = 2;
            
            return false;
        }
        
        if (chunkSerializer.chunkExists(currentLoadingX, currentLoadingZ)) {
            
            // Generate a dummy chunk for loading
            GameObject dummyChunk = chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, false,   true);
            
            if (dummyChunk != null) {
                
                if (!chunkSerializer.chunkLoadStart(dummyChunk, currentLoadingX, currentLoadingZ)) {
                    
                    Destroy(dummyChunk);
                    
                    return false;
                }
            }
            
            return true;
        } else {
            
            //
            // Generate a new chunk
            
            chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, chunkGenerator.addWorldDecorations, false);
            
        }
        
        return true;
	}
	
	
	
	
	
	
	
	
	
	
	
	//
	// Check to unload far away chunks
	//
	
	public void unloadChunks() {
        
        // Finalize if chunk is finished saving
        if ((!chunkSerializer.isSaving) & (chunkSerializer.isChunkSaved)) {
            Destroy(chunkSerializer.saving_chunk);
            chunkSerializer.isChunkSaved = false;
            chunkSerializer.isSaving     = false;
            chunkSerializer.saving_chunk = null;
        }
        
        if ((!chunkSerializer.isSaving) & (!chunkSerializer.isChunkSaved)) {
            
            float farRenderDistance = 0;
            
            if (RenderDistance == 2) farRenderDistance = RenderDistance * 100f * 2.0f;
            if (RenderDistance == 3) farRenderDistance = RenderDistance * 100f * 1.75f;
            if (RenderDistance > 3)  farRenderDistance = RenderDistance * 100f * 1.5f;
            
            for(int i = 0; i < ChunkList.transform.childCount; i++) {
                
                GameObject chunkObject  = ChunkList.transform.GetChild(i).gameObject;
                
                Vector3 chunk_pos  = new Vector3(chunkObject.transform.position.x, 0f, chunkObject.transform.position.z);
                Vector3 player_pos = new Vector3(playerObject.transform.position.x, 0f, playerObject.transform.position.z);
                
                if (Vector3.Distance(chunk_pos, player_pos) > farRenderDistance) {
                    
                    // Save bypass for debugging
                    if (!doSaveChunks) {
                        Destroy(chunkObject);
                        continue;
                    }
                    
                    // Begin saving chunk
                    chunkSerializer.chunkSaveStart(chunkObject, chunkObject.transform.position.x, chunkObject.transform.position.z);
                    
                    break;
                }
            
            }
            
        }
        
	}
	
	
	
	
	
	
	
	
	
	//
	// Day / night cycle
	//
	
	void updateDayNightCycle() {
        
        dayNightCycleCurrent += dayNightCycleRate * Time.deltaTime;
        
        if (dayNightCycleCurrent > 360.0f) dayNightCycleCurrent -= 360.0f;
        
        sun.transform.localRotation = Quaternion.Euler(new Vector3(dayNightCycleCurrent - 10.0f, dayNightCycleAngle, 0f));
        
        // Day cycle
        if ((dayNightCycleCurrent > 0.0f) | (dayNightCycleCurrent < 180.0f)) {
            
            float lerp_intensity = 0.1f * dayNightCycleCurrent;
            float lerp_fog       = 0.05f * dayNightCycleCurrent;
            
            Color sunBrightColor = new Color(1.0f,1.0f,1.0f,1.0f);
            Color sunDarkColor   = new Color(0.0f,0.0f,0.0f,1.0f);
            
            Vector2 lerpdistance  = Vector2.Lerp(fogNear,    fogFar,         lerp_fog);
            Color   lerpintensity = Color.Lerp(sunDarkColor, sunBrightColor, lerp_intensity);
            
            if (!isCameraUnderWater) {
                RenderSettings.fogStartDistance = lerpdistance.x;
                RenderSettings.fogEndDistance   = lerpdistance.y;
                RenderSettings.fogColor = Color.Lerp(fogNight, fogDay, lerp_fog);
            }
            
            Color ambientDayColor   = new Color(ambientDay,   ambientDay,   ambientDay,   1.0f);
            Color ambientNightColor = new Color(ambientNight, ambientNight, ambientNight, 1.0f);
            
            RenderSettings.ambientLight = Color.Lerp(ambientNightColor, ambientDayColor, lerp_fog);
            
            sun.intensity = lerpintensity.r;
            if (sun.intensity > 0.87f) sun.intensity = 0.87f;
            if (sun.intensity < 0f)    sun.intensity = 0f;
            
        }
        
        // Night cycle
        if ((dayNightCycleCurrent > 180.0f) & (dayNightCycleCurrent < 360.0f)) {
            
            float lerp_intensity = 0.1f * (dayNightCycleCurrent - 180.0f);
            float lerp_fog       = 0.04f * (dayNightCycleCurrent - 180.0f);
            
            Color fogNearColor = Color.white;
            Color fogFarColor = Color.white;
            
            Color sunBrightColor = new Color(1.0f,1.0f,1.0f,1.0f);
            Color sunDarkColor   = new Color(0.0f,0.0f,0.0f,1.0f);
            
            fogNearColor.r = fogNear.x;
            fogNearColor.g = fogNear.y;
            fogFarColor.r  = fogFar.x;
            fogFarColor.g  = fogFar.y;
            
            Color lerpdistance  = Color.Lerp(fogFarColor,    fogNearColor, lerp_fog);
            Color lerpintensity = Color.Lerp(sunBrightColor, sunDarkColor, lerp_intensity);
            
            if (!isCameraUnderWater) {
                RenderSettings.fogStartDistance = lerpdistance.r;
                RenderSettings.fogEndDistance   = lerpdistance.g;
                RenderSettings.fogColor = Color.Lerp(fogDay, fogNight, lerp_fog);
            }
            
            Color ambientDayColor   = new Color(ambientDay,   ambientDay,   ambientDay,   1.0f);
            Color ambientNightColor = new Color(ambientNight, ambientNight, ambientNight, 1.0f);
            
            RenderSettings.ambientLight = Color.Lerp(ambientDayColor, ambientNightColor, lerp_fog);
            
            sun.intensity = lerpintensity.r;
            if (sun.intensity > 0.87f) sun.intensity = 0.87f;
            if (sun.intensity < 0f)    sun.intensity = 0f;
            
        }
        
        return;
	}
	
	
	
	
	
	
	
	
	
	
	public void entityAutoBreeding(GameObject entityList) {
        
        if ((gene_m != null) & (gene_p != null)) 
            return;
        if (entityList.transform.childCount == 0) 
            return;
        if (entityList.transform.childCount > maxOffspringPerChunk) 
            return;
        
        // Locate some entities in the same chunk
        GameObject geneSrcP = entityList.transform.GetChild( Random.Range(0, entityList.transform.childCount) ).transform.gameObject;
        GameObject geneSrcM = entityList.transform.GetChild( Random.Range(0, entityList.transform.childCount) ).transform.gameObject;
        
        if ((!geneSrcM.activeSelf) | (!geneSrcP.activeSelf)) 
            return;
        
        // Entity breeding distance
        if (Vector3.Distance(geneSrcM.transform.position, geneSrcP.transform.position) > maxBreedDistance) 
            return;
        
        EntityTag entityTagP = geneSrcP.GetComponent<EntityTag>();
        EntityTag entityTagM = geneSrcM.GetComponent<EntityTag>();
        
        //ActorTag actorTagP = geneSrcP.GetComponent<ActorTag>();
        //ActorTag actorTagM = geneSrcM.GetComponent<ActorTag>();
        
        GeneTag geneTagP = geneSrcP.GetComponent<GeneTag>();
        GeneTag geneTagM = geneSrcM.GetComponent<GeneTag>();
        
        // Must be of age
        if ((entityTagM.Age < geneTagM.ageWhenAdult) | (entityTagP.Age < geneTagP.ageWhenAdult)) 
            return;
        
        //if ((actorTagP.love   < 0f) | (actorTagM.love   < 0f)) return;
        //if ((actorTagP.stress > 0f) | (actorTagM.stress > 0f)) return;
        //if ((actorTagP.hunger > 0f) | (actorTagM.hunger > 0f)) return;
        
        // Must be the same base species
        if (!allowCrossBreeding) {
            if (entityTagP.name != entityTagM.name) 
                return;
        }
        
        // Parents cannot be the same entity
        if (geneSrcM == geneSrcP)
            return;
        
        // Check cannot reproduce with the same sex...
        if (geneTagP.masculine != geneTagM.masculine) {
            
            // If the maternal gene is masculine its backwards.. flip them
            if (geneTagM.masculine == true) {
                
                GameObject maternalGene = geneSrcP;
                GameObject paternalGene = geneSrcM;
                
                geneSrcM = maternalGene;
                geneSrcP = paternalGene;
            }
            
        } else {
            return;
        }
        
        // Passed.. select the pair for breeding
        if (geneSrcM != null) gene_m = geneSrcM;
        if (geneSrcP != null) gene_p = geneSrcP;
        
        return;
    }
	
	
	
	
	
	
	
	
	
	//
	// Entity genetic breeding
	//
	
	public void joinGeneticPair() {
        
        if ((gene_m == null) | (gene_p == null)) 
            return;
        
        Vector3 entityPosition = Vector3.Lerp(gene_m.transform.position, gene_p.transform.position, 0.1f);
        entityPosition.x += Random.Range(entityYouthSpawnArea*0.5f, entityYouthSpawnArea) - Random.Range(entityYouthSpawnArea*0.5f, entityYouthSpawnArea);
        entityPosition.z += Random.Range(entityYouthSpawnArea*0.5f, entityYouthSpawnArea) - Random.Range(entityYouthSpawnArea*0.5f, entityYouthSpawnArea);
        
        // Paternal name dictates entity type
        string entityName = gene_p.GetComponent<EntityTag>().name;
        
        // Create the child entity
        GameObject newEntity = Instantiate( Resources.Load( entityName )) as GameObject;
        newEntity.name = entityName;
        
        // Place offspring on terrain surface
        entityPosition.y = 500f;
        RaycastHit hit_obj;
        Ray ray_obj = new Ray(entityPosition, -Vector3.up);
        if ( Physics.Raycast(ray_obj, out hit_obj, 1000f, LayerMask.GetMask("Ground")) ) {
            newEntity.transform.position = new Vector3(hit_obj.point.x, hit_obj.point.y + 1.5f, hit_obj.point.z);
        } else {
            newEntity.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        }
        
        newEntity.transform.parent = gene_m.transform.parent;
        
        GeneTag newGene = newEntity.GetComponent<GeneTag>();
        
        GeneTag geneTagM = gene_m.GetComponent<GeneTag>();
        GeneTag geneTagP = gene_p.GetComponent<GeneTag>();
        
        // Initial genetic merge bias
        newGene.mergeGenetics(geneTagM, geneTagP);
        
        // Mutations
        if (allowMutations) {
            int randomGene = Random.Range(0, 4);
            newGene.addRandomMutation(randomGene, mutationFactor);
        }
        
        newGene.updateGenetics();
        
        
        // Initiate actor AI genes
        ActorTag actorTagM  = gene_m.GetComponent<ActorTag>();
        ActorTag actorTagP  = gene_p.GetComponent<ActorTag>();
        ActorTag offspring  = newEntity.GetComponent<ActorTag>();
        EntityTag youthEntityTag  = newEntity.GetComponent<EntityTag>();
        
        // Transfer important memories
        offspring.addMemory("home", actorTagP.getMemory("home"));
        
        
        // Initiate as a young entity
        newEntity.transform.localScale = new Vector3(newGene.ageYouthScale, newGene.ageYouthScale, newGene.ageYouthScale);
        youthEntityTag.Age = geneTagM.ageWhenBorn;
        
        
        // Merge the AI parameters
        float bias = Random.Range(0f, 1f);
        offspring.chanceToChangeDirection  = Mathf.Lerp(actorTagM.chanceToChangeDirection,  actorTagP.chanceToChangeDirection, bias);
        bias = Random.Range(0f, 1f);
        offspring.chanceToWalk             = Mathf.Lerp(actorTagM.chanceToWalk,             actorTagP.chanceToWalk,          bias);
        bias = Random.Range(0f, 1f);
        offspring.chanceToFocusOnPlayer    = Mathf.Lerp(actorTagM.chanceToFocusOnPlayer,    actorTagP.chanceToFocusOnPlayer, bias);
        bias = Random.Range(0f, 1f);
        offspring.chanceToFocusOnEntity    = Mathf.Lerp(actorTagM.chanceToFocusOnEntity,    actorTagP.chanceToFocusOnEntity, bias);
        bias = Random.Range(0f, 1f);
        offspring.chanceToAttackPlayer     = Mathf.Lerp(actorTagM.chanceToAttackPlayer,     actorTagP.chanceToAttackPlayer,  bias);
        
        bias = Random.Range(0f, 1f);
        offspring.distanceToFocusOnPlayer  = Mathf.Lerp(actorTagM.distanceToFocusOnPlayer,  actorTagP.distanceToFocusOnPlayer, bias);
        bias = Random.Range(0f, 1f);
        offspring.distanceToFocusOnEntity  = Mathf.Lerp(actorTagM.distanceToFocusOnEntity,  actorTagP.distanceToFocusOnEntity, bias);
        bias = Random.Range(0f, 1f);
        offspring.distanceToAttackPlayer   = Mathf.Lerp(actorTagM.distanceToAttackPlayer,   actorTagP.distanceToAttackPlayer,  bias);
        bias = Random.Range(0f, 1f);
        offspring.distanceToWalk           = Mathf.Lerp(actorTagM.distanceToWalk,           actorTagP.distanceToWalk,          bias);
        
        bias = Random.Range(0f, 1f);
        offspring.heightPreferenceMin      = Mathf.Lerp(actorTagM.heightPreferenceMin,      actorTagP.heightPreferenceMin, bias);
        bias = Random.Range(0f, 1f);
        offspring.heightPreferenceMax      = Mathf.Lerp(actorTagM.heightPreferenceMax,      actorTagP.heightPreferenceMax, bias);
        
        bias = Random.Range(0f, 1f);
        offspring.distanceToAttack         = Mathf.Lerp(actorTagM.distanceToAttack,         actorTagP.distanceToAttack,    bias);
        bias = Random.Range(0f, 1f);
        offspring.distanceToFlee           = Mathf.Lerp(actorTagM.distanceToFlee,           actorTagP.distanceToFlee,      bias);
        
        offspring.limbAxis                 = Random.Range(actorTagM.limbAxis, actorTagP.limbAxis);
        
        if (Random.Range(0, 10) > 4) {offspring.limbAxisInvert = actorTagP.limbAxisInvert;} else {offspring.limbAxisInvert = actorTagM.limbAxisInvert;}
        
        bias = Random.Range(0f, 1f);
        offspring.limbCycleRate            = Mathf.Lerp(actorTagM.limbCycleRate,     actorTagP.limbCycleRate,       bias);
        bias = Random.Range(0f, 1f);
        offspring.limbCycleRange           = Mathf.Lerp(actorTagM.limbCycleRange,    actorTagP.limbCycleRange,      bias);
        
        offspring.consumptionTimer          = Random.Range(actorTagM.consumptionTimer, actorTagP.consumptionTimer);
        
        return;
	}
	
	
	
	
	
	
	
	
	
	//
	// Add object to the current structure
	
	public void addObjectToStructure(string name, string data, Vector3 position, Vector3 rotation, Vector3 scale) {
        
        StructureItem newStructureItem = new StructureItem();
        newStructureItem.name = name;
        newStructureItem.data = data;
        
        newStructureItem.position = position;
        newStructureItem.rotation = rotation;
        newStructureItem.scale    = scale;
        
        chunkGenerator.items.Add(newStructureItem);
        
        return;
    }
	
	
	//
	// Remove an object from the structure
	
	public void removeObjectFromStructure(Vector3 position) {
        int numberOfItems = chunkGenerator.items.Count;
        
        for (int i=0; i < numberOfItems; i++) {
            
            if (chunkGenerator.items[i].position != position) 
                continue;
            
            chunkGenerator.items.RemoveAt(i);
            
            break;
        }
        return;
    }
	
	
	//
	// Save current structure to a file
	
	public void saveCurrentStructureToFile(string filename) {
        
        int numberOfItems = chunkGenerator.items.Count;
        
        StructureData structureData = new StructureData(numberOfItems);
        int structureHeight = 0;
        
        for (int i=0; i < numberOfItems; i++) {
            
            structureData.name[i]     = chunkGenerator.items[i].name;
            structureData.data[i]     = chunkGenerator.items[i].data;
            
            structureData.position[i] = new Vec3(chunkGenerator.items[i].position);
            structureData.rotation[i] = new Vec3(chunkGenerator.items[i].rotation);
            structureData.scale[i]    = new Vec3(chunkGenerator.items[i].scale);
            
            // Find structure height
            if ((int)structureData.rotation[i].y > structureHeight) 
                structureHeight = (int)structureData.rotation[i].y;
        }
        
        // Check structures directory
        string structuresPath = "structures/";
        if (!System.IO.Directory.Exists(structuresPath)) 
            System.IO.Directory.CreateDirectory(structuresPath);
        
        chunkGenerator.structureName = filename;
        
        // Check if the file already exists
        string filePath = structuresPath + chunkGenerator.structureName + ".structure";
        if (System.IO.File.Exists(filePath)) 
            System.IO.File.Delete(filePath);
        
        // Binary stream
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter;
        formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        
        System.IO.FileStream fileStream_save;
        
        // Stream data to the file
        fileStream_save = System.IO.File.Open(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        formatter.Serialize(fileStream_save, structureData);
        
        // Close out
        fileStream_save.Close();
        
        return;
    }
	
	
	
	//
	// Load a structure from a file
	
	public int loadStructureFileToCurrentStructure(string filename) {
        
        // Check structures directory
        string structuresPath = "structures/";
        if (!System.IO.Directory.Exists(structuresPath)) 
            System.IO.Directory.CreateDirectory(structuresPath);
        
        chunkGenerator.structureName = filename;
        
        // Check if the file does not exist
        string filePath = structuresPath + chunkGenerator.structureName + ".structure";
        if (!System.IO.File.Exists(filePath)) 
            return 0;
        
        // Binary stream
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter;
        formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        
        System.IO.FileStream fileStream_load;
        fileStream_load = System.IO.File.Open(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        
        StructureData structureData = new StructureData(2048);
        structureData = formatter.Deserialize(fileStream_load) as StructureData;
        
        // Close out
        fileStream_load.Close();
        
        
        // Load the data
        int numberOfItems = structureData.name.Length;
        int structureHeight=0;
        
        for (int i=0; i < numberOfItems; i++) {
            
            StructureItem newStructureItem = new StructureItem();
            
            newStructureItem.name = structureData.name[i];
            newStructureItem.data = structureData.data[i];
            newStructureItem.position = new Vector3(structureData.position[i].x, structureData.position[i].y, structureData.position[i].z);
            newStructureItem.rotation = new Vector3(structureData.rotation[i].x, structureData.rotation[i].y, structureData.rotation[i].z);
            newStructureItem.scale    = new Vector3(structureData.scale[i].x, structureData.scale[i].y, structureData.scale[i].z);
            
            chunkGenerator.items.Add(newStructureItem);
            
            if ((int)newStructureItem.position.y > structureHeight) 
                structureHeight = (int)newStructureItem.position.y;
            
        }
        chunkGenerator.structureHeight = structureHeight;
        
        return 1;
    }
	
	
	
	
	
	
	
	//
	// Place a structure into the world at the player position
	
	public void placeStructureInWorld(string structureName, Vector3 position) {
        //LayerMask objectLayerMask = LayerMask.GetMask("Object");
        
        StructureBuild build = null;
        
        for (int i=0; i < chunkGenerator.worldStructures.Count; i++) {
            
            if (chunkGenerator.worldStructures[i].structureName != structureName) 
                continue;
            
            build = chunkGenerator.worldStructures[i];
            break;
        }
        
        if (build == null) 
            return;
        
        for (int i=0; i < build.items.Count; i++) {
            
            // Get target chunk
            float chunkX = Mathf.Round(position.x / 100) * 100;
            float chunkZ = Mathf.Round(position.z / 100) * 100;
            
            GameObject targetChunk = getChunk(chunkX, chunkZ);
            if (targetChunk == null)
                return;
            
            GameObject newStatic = MonoBehaviour.Instantiate( Resources.Load( build.items[i].name )) as GameObject;
            newStatic.name             = build.items[i].name;
            newStatic.transform.parent = targetChunk.transform.GetChild(2).gameObject.transform;
            
            newStatic.transform.position = build.items[i].position;
            
            
            // Rotate the whole structure
            Vector3 targetPosition = new Vector3(0f, 0f, 0f);
            newStatic.transform.RotateAround(targetPosition, Vector3.up, chunkGenerator.structureRotation);
            // Counter rotate the object
            newStatic.transform.RotateAround(newStatic.transform.position, Vector3.up, -chunkGenerator.structureRotation);
            
            // Calculate stupid rotation offset pattern
            // It would have helped if i had payed better attention in math class...
            Vector3 newRotation;
            if ((chunkGenerator.structureRotation == 0f) | (chunkGenerator.structureRotation == 180f)) {
                newRotation = new Vector3(build.items[i].rotation.x, 
                                          build.items[i].rotation.y, 
                                          build.items[i].rotation.z);
            } else {
                newRotation = new Vector3(build.items[i].rotation.z, 
                                          build.items[i].rotation.y, 
                                          build.items[i].rotation.x);
            }
            
            
            // Rotate local object
            newStatic.transform.localRotation *= Quaternion.Euler(newRotation);
            // Counter rotate the hit box
            newStatic.transform.GetChild(0).transform.localRotation *= Quaternion.Euler(-newRotation);
            
            
            
            // Scale local
            newStatic.transform.localScale = build.items[i].scale;
            
            // Translate to correct position
            newStatic.transform.position += position;
            
            // Check to record this structure
            if (chunkGenerator.doSavePlacedObjectsToStructure) {
                StructureItem structureItem = new StructureItem();
                
                structureItem.name          = build.items[i].name;
                structureItem.data          = build.items[i].data;
                structureItem.position      = newStatic.transform.position;
                structureItem.rotation      = newStatic.transform.eulerAngles;
                structureItem.scale         = newStatic.transform.localScale;
                
                chunkGenerator.items.Add(structureItem);
            }
            
            // Setup item state
            ItemTag itemTag = newStatic.GetComponent<ItemTag>();
            itemTag.data       = build.items[i].data;
            itemTag.lifeTime   = -1;
            
        }
        
    }
	
	
	
	//
	// Clear the current structure
	
	public void clearCurrentStructure() {
        chunkGenerator.structureName = "";
        chunkGenerator.items.Clear();
        chunkGenerator.entities.Clear();
    }
	
	
	
    
    
    
    
    public void loadWorldStructures() {
        
        DirectoryInfo dir = new DirectoryInfo("structures");
        FileInfo[] info = dir.GetFiles("*.*");
        
        chunkGenerator.worldStructures.Clear();
        
        for (int i=0; i < info.Length; i++) {
            string structureName = info[i].Name.Split('.')[0];
            
            if (loadStructureFileToCurrentStructure(structureName) == 0) 
                continue;
            
            // Add the structure to the list
            StructureBuild structBuild = new StructureBuild();
            
            structBuild.structureName = chunkGenerator.structureName;
            
            structBuild.structureHeight = chunkGenerator.structureHeight;
            
            structBuild.items    = new List<StructureItem>(chunkGenerator.items);
            structBuild.entities = new List<StructureEntity>(chunkGenerator.entities);
            
            chunkGenerator.worldStructures.Add(structBuild);
            
            clearCurrentStructure();
        }
        
        return;
    }
    
    
    
    
    
    
	
	
	
	public float chunk_get_point_height(float point_x, float point_z) {
        
        int chunk_x = (int) (Mathf.Round(point_x / 100) * 100) - (100 / 2);
        int chunk_z = (int) (Mathf.Round(point_z / 100) * 100) - (100 / 2);
        
        int target_x = (int)point_x - chunk_x + (100 / 2);
        int target_z = (int)point_z - chunk_z + (100 / 2);
        
        GameObject chunk = getChunk( target_x, target_z );
        if (chunk == null) return 0f;
        
        ChunkTag chunkTag = chunk.transform.GetChild(0).GetComponent<ChunkTag>();
        
        return chunkTag.vertex_grid[(int)target_x, (int)target_z];
	}
    
    
    
    
    
    
	
	//
	// Initiate world before loading
	//
	
	public void initiateWorld() {
        
        //
        // Spawn player
        
        playerObject = Instantiate( Resources.Load("Player")) as GameObject;
        
        playerObject.name = "Player";
        
        playerObject.transform.parent = GameObject.Find("World").transform;
        GameObject playerCameraObject = GameObject.Find("Main Camera");
        
        cameraObject = playerCameraObject.GetComponent<Camera>();
        
        cameraObject.nearClipPlane  = 0.05f;
        cameraObject.farClipPlane   = (((float)RenderDistance * (float)100)) * 0.5f;
        
        damageIndicationOffset = 0f;
        
        
        // Disable health and hunger in debug mode
        if (doDebugMode) {
            HUD.transform.GetChild(5).gameObject.SetActive(false);
            HUD.transform.GetChild(6).gameObject.SetActive(false);
        } else {
            // Activate hunger and health bars
            HUD.transform.GetChild(5).gameObject.SetActive(true);
            HUD.transform.GetChild(6).gameObject.SetActive(true);
        }
        
        
        
        //
        // Initiate the chunk serialization system
        
        chunkSerializer = new ChunkSerializer();
        chunkSerializer.clientVersion  = clientVersion;
        chunkSerializer.chunkList = ChunkList;
        
        // Check world directory structure
        if (!chunkSerializer.openWorld(worldName)) {
            chunkSerializer.createWorld(worldName);
            
            // Initiate player position in new world
            playerObject.transform.Translate(worldSpawn.x, -1000f, worldSpawn.z);
            reset_player = true;
        }
        
        // Clear the inventory
        inventory.clear();
        
        // Default health and hunger
        inventory.setHealth( 10 );
        inventory.setHunger( 8 );
        inventory.setSaturation( 100 );
        
        
        // Load world data and check client version
        bool success = chunkSerializer.loadWorldData();
        
        // Set the world seed and name
        chunkSerializer.worldName = worldName;
        chunkGenerator.worldSeed = worldSeed;
        
        chunkGenerator.initiate();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        isPaused           = false;
        doShowConsole      = false;
        doMouseLook        = true;
        
        Time.timeScale = 1f;
        
        // Load world structures
        loadWorldStructures();
        
        // Reset animations
        runCameraDamageAnimation = false;
        
        // Set the time of day
        if (!success) 
            dayNightCycleCurrent = 30f;
        if (dayNightCycleCurrent > 360.0f) dayNightCycleCurrent -= 360.0f;
        
        // Update directional light
        sun.transform.localRotation = Quaternion.Euler(new Vector3(dayNightCycleCurrent, dayNightCycleAngle, 0.0f));
        
        // Initiate global fog
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        
        RenderSettings.fogStartDistance = 100;
        RenderSettings.fogEndDistance   = 200;
        RenderSettings.fogColor         = fogNight;
        
        RenderSettings.skybox = worldSkybox;
        
        updateDayNightCycle();
        
        // Find the current chunk the player is standing on
        playerChunkX = Mathf.Round(playerObject.transform.position.x / 100) * 100;
        playerChunkZ = Mathf.Round(playerObject.transform.position.z / 100) * 100;
        
        chunkSerializer.gameRules = this.gameObject;
        
        
        //
        // Load first few surrounding chunks
        
        int numberOfChunksHorz = 3;
        int numberOfChunksVert = 3;
        
        for (int z=0; z < numberOfChunksVert; z++) {
            
            for (int x=0; x < numberOfChunksHorz; x++) {
                
                currentLoadingX = (((x * 100) + playerChunkX)) - 100;
                currentLoadingZ = (((z * 100) + playerChunkZ)) - 100;
                
                // Check if the chunk exists on disk
                if (chunkSerializer.chunkExists(currentLoadingX, currentLoadingZ)) {
                    
                    GameObject generated_chunk = chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, false, true);
                    //ChunkTag generated_chunk_tag = generated_chunk.gameObject.transform.GetChild(0).GetComponent<ChunkTag>();
                    
                    chunkSerializer.chunkLoadStart(generated_chunk, currentLoadingX, currentLoadingZ);
                    
                    while (chunkSerializer.isLoading == true)
                    continue;
                    
                    chunkSerializer.chunkBuild();
                    chunkSerializer.chunkFinalize();
                    
                    if (Vector3.Distance(playerObject.transform.position, generated_chunk.transform.position) < (RenderDistance * StaticDistance * 100f))
                    generated_chunk.transform.GetChild(2).gameObject.SetActive(true);
                    
                // Generate a new chunk
                } else {
                    
                    GameObject generated_chunk = chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, chunkGenerator.addWorldDecorations, false);
                    //ChunkTag generated_chunk_tag = generated_chunk.gameObject.transform.GetChild(0).GetComponent<ChunkTag>();
                    
                    if (Vector3.Distance(playerObject.transform.position, generated_chunk.transform.position) < (RenderDistance * StaticDistance * 100f))
                    generated_chunk.transform.GetChild(2).gameObject.SetActive(true);
                    
                }
                
            }
            
        }
        
        return;
	}
    
    
    
    
    
    
    
    //
    // Clear out and close the currently loaded world
    //
    
    public void purgeWorld() {
        
        // Reset command console
        GameObject consoleTextFieldObject = CommandConsole.transform.GetChild(0).GetChild(0).gameObject;
        GameObject consoleTextObject      = CommandConsole.transform.GetChild(0).GetChild(1).gameObject;
        
        consoleTextFieldObject.SetActive(false);
        consoleTextObject.SetActive(false);
        
        InputField consoleField = CommandConsole.transform.GetChild(0).GetChild(0).GetComponent<InputField>();
        consoleField.text = "";
        
        
        chunkSerializer.closeWorld();
        
        chunkGenerator.shutdown();
        
        
        // Reset world settings
        doDebugMode        = false;
        doSaveChunks       = true;
        doDayNightCycle    = true;
        doWeatherCycle     = true;
        
        isPaused           = false;
        doShowConsole      = false;
        doMouseLook        = true;
        
        InitialSunAngle          = 0.0f;
        dayNightCycleRate        = 0.25f;
        
        dayNightCycleCurrent     = 0.0f;
        
        dayNightCycleAngle       = -90.0f;
        
        TickRate          = 3;
        TickCounter       = 0;
        BaseCounter       = 0;
        
    }
    
    
    
    
    
    
	
	
	
	
	//
	// Save currently loaded chunks
	//
	
	public void saveWorld() {
        
        for(int i = 0; i < ChunkList.transform.childCount; i++) {
            
            GameObject chunkObject = ChunkList.transform.GetChild(i).gameObject;
            ChunkTag chunkTag = chunkObject.transform.GetChild(0).GetComponent<ChunkTag>();
            
            // Do not save the chunk if it has not loaded
            if (!chunkTag.isLoaded) 
                continue;
            
            chunkSerializer.saving_chunk = chunkObject;
            chunkSerializer.savingX = chunkObject.transform.position.x;
            chunkSerializer.savingZ = chunkObject.transform.position.z;
            
            chunkSerializer.chunkSave();
            
            string chunkName = chunkSerializer.savingX + "_" + chunkSerializer.savingZ;
            
            chunkSerializer.fileStream_save = System.IO.File.Open(chunkSerializer.worldsPath + chunkSerializer.worldName + chunkSerializer.chunksPath + chunkName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            chunkSerializer.formatter.Serialize(chunkSerializer.fileStream_save, chunkSerializer.saveChunk);
            chunkSerializer.fileStream_save.Close();
            
        }
        
        // Save world data
        chunkSerializer.saveWorldData();
        
	}
    
    
    
    
	bool checkChunkFree(float chunk_x, float chunk_z) {
        
        string chunkName = chunk_x + "_" + chunk_z;
        
        for (int i=0; i < ChunkList.transform.childCount; i++) {
            
            string chunk_name = ChunkList.transform.GetChild(i).gameObject.name;
            
            if (chunk_name == chunkName) 
                return false;
            
        }
        
        return true;
	}
    
    
    
	GameObject getChunk(float chunk_x, float chunk_z) {
        
        string chunkName = chunk_x + "_" + chunk_z;
        
        for (int i=0; i < ChunkList.transform.childCount; i++) {
            
            string chunk_name = ChunkList.transform.GetChild(i).gameObject.name;
            
            if (chunk_name == chunkName) 
                return ChunkList.transform.GetChild(i).gameObject;
            
        }
        
        return null;
	}
    
    
	public void pauseGame() {
        
        Time.timeScale = 0f;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        isPaused = true;
        return;
	}
    
	public void unpauseGame() {
        
        Time.timeScale = 1f;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        
        isPaused = false;
        return;
	}
    
	void Start() {
        
        inventory = HUD.GetComponent<Inventory>();
        
        chunkSerializer.gameRules = GameObject.Find("GameRules");
        
	}
	
	
	
	
	
	
	
	//
	// Command console
	//
    
    string RunConsoleCommand(string inputString) {
        
        if (inputString == "") 
            return "";
        
        string[] paramaters = inputString.Split(' ');
        
        if (paramaters.Length < 2) 
            return "";
        
        //
        // List of console commands
        string result="";
        switch (paramaters[0]) {
            
            case "structure":   result = consoleStructure(paramaters); break;
            case "summon":      result = consoleSummon(paramaters); break;
            case "give":        result = consoleGive(paramaters); break;
            case "rule":        result = consoleRule(paramaters); break;
            case "time":        result = consoleTime(paramaters); break;
            case "kill":        result = consoleKill(paramaters); break;
            case "tp":          result = consoleTP(paramaters); break;
            
            default: break;
        }
        
        return result;
    }
    
    
    
    //
    // Summon an entity
    //
    
    string consoleSummon(string[] paramaters) {
        string entityName="";
        if (paramaters.Length > 1) 
            entityName = paramaters[1];
        
        // Check entity exists
        for (int i=0; i < entities.Length; i++) {
            if (entities[i].name == entityName) {
                
                GameObject newEntity = MonoBehaviour.Instantiate( Resources.Load( entityName )) as GameObject;
                GameObject currentChunk = getChunk(playerChunkX, playerChunkZ);
                
                Vector3 newPosition;
                newPosition.x = playerObject.transform.position.x + (playerObject.transform.forward.x * 3.0f);
                newPosition.y = playerObject.transform.position.y;
                newPosition.z = playerObject.transform.position.z + (playerObject.transform.forward.z * 3.0f);
                newEntity.transform.position = newPosition;
                
                newEntity.name = entityName;
                newEntity.transform.parent = currentChunk.transform.GetChild(1).gameObject.transform;
                
                // Update the genetics
                GeneTag entityGene = newEntity.GetComponent<GeneTag>();
                
                entityGene.masculine = false;
                if (Random.Range(0, 10) > 4) 
                    entityGene.masculine = true;
                
                entityGene.updateGenetics();
                
                return "Summoned " + entityName;
            }
        }
        
        return "Entity not found";
    }
    
    
    
    //
    // Give item
    //
    
    string consoleGive(string[] paramaters) {
        string itemName="";
        if (paramaters.Length > 1) 
            itemName = paramaters[1];
        
        int itemCount = 1;
        if (paramaters.Length > 2) 
            if (paramaters[2] != "") 
                itemCount = int.Parse(paramaters[2]);
        
        // Check item exists
        for (int i=0; i < items.Length; i++) {
            if (items[i].name == itemName) {
                
                // Get weapon durability
                int durability=-1;
                for (int a=0; a < weaponItem.Length; a++) {
                    if (weaponItem[a].name != itemName) 
                        continue;
                    durability = weaponItem[a].durability;
                }
                
                inventory.addItem(itemName, itemCount, items[i].stackMax, durability);
                
                hudInterface.updateInHand();
                
                return "Given " + itemName;
            }
        }
        
        return "Item not found";
    }
    
    
    
    //
    // Set world rule state
    //
    
    string consoleRule(string[] paramaters) {
        
        string ruleName="";
        if (paramaters.Length > 1) 
            ruleName = paramaters[1];
        
        string status="true";
        if (paramaters.Length > 2) 
            status = paramaters[2];
        
        if (ruleName == "weather") {
            if (status=="true")  {doWeatherCycle = true;  return "Weather cycle enabled";}
            if (status=="false") {doWeatherCycle = false; return "Weather cycle disabled";}
        }
        
        if (ruleName == "daynight") {
            if (status=="true")  {doDayNightCycle = true;  return "Day night cycle enabled";}
            if (status=="false") {doDayNightCycle = false; return "Day night cycle disabled";}
        }
        
        if (ruleName == "oldage") {
            if (status=="true")  {allowEntityExpiration = true;  return "Entity expiration enabled";}
            if (status=="false") {allowEntityExpiration = false; return "Entity expiration disabled";}
        }
        
        // DEBUG mode
        if (ruleName == "debug") {
            if ((status=="true") | (status=="1"))  {
                doDebugMode = true;
                HUD.transform.GetChild(5).gameObject.SetActive(false);
                HUD.transform.GetChild(6).gameObject.SetActive(false);
                return "Debug enabled";}
            if ((status=="false") | (status=="0")) {
                doDebugMode = false;
                HUD.transform.GetChild(5).gameObject.SetActive(true);
                HUD.transform.GetChild(6).gameObject.SetActive(true);
                return "Debug disabled";}
        }
        
        return "Unknown rule";
    }
    
    
    
    //
    // Teleport
    //
    
    string consoleTP(string[] paramaters) {
        
        if (paramaters.Length < 4) 
            return "Invalid location";
        
        float xPos = float.Parse(paramaters[1]);
        float yPos = float.Parse(paramaters[2]);
        float zPos = float.Parse(paramaters[3]);
        
        playerObject.transform.position = new Vector3(xPos, yPos, zPos);
        
        return "Teleported to " + xPos.ToString() + " " + yPos.ToString() + " " + zPos.ToString();
    }
    
    
    
    //
    // Structure
    //
    
    string consoleStructure(string[] paramaters) {
        
        if (paramaters.Length < 2) 
            return "";
        
        // Clear the current structure
        if (paramaters[1] == "clear") {
            clearCurrentStructure();
            return "Structure data cleared";
        }
        
        if (paramaters[1] == "enable") {
            chunkGenerator.doSavePlacedObjectsToStructure = true; clearCurrentStructure();
            return "Structure saving enabled";
        }
        
        if (paramaters[1] == "disable") {
            chunkGenerator.doSavePlacedObjectsToStructure = false; clearCurrentStructure();
            return "Structure saving disabled";
        }
        
        if (paramaters.Length < 3) 
            return "";
        
        // Place a structure
        if (paramaters[1] == "place") {
            Vector3 playerPos;
            playerPos.x = Mathf.Round(playerObject.transform.position.x);
            playerPos.y = Mathf.Round(playerObject.transform.position.y) - 1f;
            playerPos.z = Mathf.Round(playerObject.transform.position.z);
            
            placeStructureInWorld(paramaters[2], playerPos);
            return "Structure placed '"+paramaters[2]+"'";
        }
        
        // Save the current structure
        if (paramaters[1] == "save") {
            saveCurrentStructureToFile(paramaters[2]);
            return "Structure saved to file '"+paramaters[2]+"'";
        }
        
        // Load structure to world structures list
        if (paramaters[1] == "reload") {
            chunkGenerator.worldStructures.Clear();
            clearCurrentStructure();
            loadWorldStructures();
            return "Structure files loaded.";
        }
        
        // Check structure size
        if (paramaters[1] == "size") {
            return "Current structure size '"+ (chunkGenerator.items.Count + chunkGenerator.entities.Count) +"'";
        }
        
        return "Unknown structure";
    }
    
    
    
    //
    // Kill
    //
    
    string consoleKill(string[] paramaters) {
        
        string target="";
        if (paramaters.Length > 1) 
            target = paramaters[1];
        
        if (target == "all") {
            
            for (int i=0; i < ChunkList.transform.childCount; i++) {
                
                // Get chunk
                GameObject chunkObject = ChunkList.transform.GetChild(i).gameObject;
                
                // Run entity list
                for (int a=0; a < chunkObject.transform.GetChild(1).transform.childCount; a++) {
                    GameObject entityObject = chunkObject.transform.GetChild(1).transform.GetChild(a).gameObject;
                    
                    // Kill the entity
                    entityObject.GetComponent<EntityTag>().Health = 0;
                    entityObject.GetComponent<ActorTag>().isDying = true;
                    
                }
                
                
                
            }
            
            return "Killed all entities";
        }
        
        return "Entity not found";
    }
    
    
    
    //
    // Time
    //
    
    string consoleTime(string[] paramaters) {
        
        string func="";
        if (paramaters.Length > 1) 
            func = paramaters[1];
        
        string status="";
        if (paramaters.Length > 2) 
            status = paramaters[2];
        
        if (paramaters.Length < 2) 
            return "Cannot generate structure";
        
        if (func == "set") {
            
            switch (status) {
                    
                case "day":
                    dayNightCycleCurrent = 30f;
                    updateDayNightCycle();
                    return "Time set to day";
                    
                case "night":
                    dayNightCycleCurrent = 185f;
                    updateDayNightCycle();
                    return "Time set to night";
                    
                case "midnight":
                    dayNightCycleCurrent = 280f;
                    updateDayNightCycle();
                    return "Time set to midnight";
                    
                case "noon":
                    dayNightCycleCurrent = 100f;
                    updateDayNightCycle();
                    return "Time set to noon";
                    
                default:
                    break;
            }
            
        }
        
        return "Unknown time command";
    }
    
    
    
    /*
    
    //
    // Change a point in the terrain and update the edges
    // Can be useful for stitching the chunk edges
    public GameObject modifyChunk(float point_x, float point_z, ChunkTag chunkTag, float terrain_damage) {
        
        float hit_x = point_x - (100 / 2);
        float hit_z = point_z - (100 / 2);
        
        float chunk_x = (Mathf.Round(point_x / 100) * 100) - (100 / 2);
        float chunk_z = (Mathf.Round(point_z / 100) * 100) - (100 / 2);
        
        float target_x = hit_x - chunk_x + (100 / 2);
        float target_z = hit_z - chunk_z + (100 / 2);
        
        if ((target_x < 1) & (target_z < 1)) {
            
            float join_x = ((chunk_x+100)-50-100);
            float join_z = ((chunk_z+100)-50-100);
            
            GameObject edge_check;
            
            edge_check = getChunk( join_x+100, join_z );
            if (edge_check == null) return null;
            edge_check = getChunk( join_x, join_z+100 );
            if (edge_check == null) return null;
            
            
            GameObject corner_chunk = getChunk( join_x, join_z );
            if (corner_chunk == null) return null;
            
            ChunkTag cornerChunkTag = corner_chunk.transform.GetChild(0).GetComponent<ChunkTag>();
            
            cornerChunkTag.vertex_grid[100, 100] += terrain_damage;
            if (cornerChunkTag.vertex_grid[100, 100] < 0f) cornerChunkTag.vertex_grid[100, 100] = 0f;
            
            cornerChunkTag.update_mesh();
        }
        
        
        if ((target_x > 99) & (target_z > 99)) {
            
            float join_x = ((chunk_x+100)-50+100);
            float join_z = ((chunk_z+100)-50+100);
            
            GameObject edge_check;
            
            edge_check = getChunk( join_x-100, join_z );
            if (edge_check == null) return null;
            edge_check = getChunk( join_x, join_z-100 );
            if (edge_check == null) return null;
            
            GameObject corner_chunk = getChunk( join_x, join_z );
            if (corner_chunk == null) return null;
            
            ChunkTag cornerChunkTag = corner_chunk.transform.GetChild(0).GetComponent<ChunkTag>();
            
            cornerChunkTag.vertex_grid[0, 0] += terrain_damage;
            if (cornerChunkTag.vertex_grid[0, 0] < 0f) cornerChunkTag.vertex_grid[0, 0] = 0f;
            
            cornerChunkTag.update_mesh();
        }
        
        
        if ((target_x < 1) & (target_z > 99)) {
            
            float join_x = ((chunk_x+100)-50-100);
            float join_z = ((chunk_z+100)-50+100);
            
            GameObject edge_check;
            
            edge_check = getChunk( join_x+100, join_z );
            if (edge_check == null) return null;
            edge_check = getChunk( join_x, join_z-100 );
            if (edge_check == null) return null;
            
            GameObject corner_chunk = getChunk( join_x, join_z );
            if (corner_chunk == null) return null;
            
            ChunkTag cornerChunkTag = corner_chunk.transform.GetChild(0).GetComponent<ChunkTag>();
            
            cornerChunkTag.vertex_grid[100, 0] += terrain_damage;
            if (cornerChunkTag.vertex_grid[100, 0] < 0f) cornerChunkTag.vertex_grid[100, 0] = 0f;
            
            cornerChunkTag.update_mesh();
        }
        
        
        if ((target_x > 99) & (target_z < 1)) {
            
            float join_x = ((chunk_x+100)-50+100);
            float join_z = ((chunk_z+100)-50-100);
            
            GameObject edge_check;
            
            edge_check = getChunk( join_x-100, join_z );
            if (edge_check == null) return null;
            edge_check = getChunk( join_x, join_z+100 );
            if (edge_check == null) return null;
            
            GameObject corner_chunk = getChunk( join_x, join_z );
            if (corner_chunk == null) return null;
            
            ChunkTag cornerChunkTag = corner_chunk.transform.GetChild(0).GetComponent<ChunkTag>();
            
            cornerChunkTag.vertex_grid[0, 100] += terrain_damage;
            if (cornerChunkTag.vertex_grid[0, 100] < 0f) cornerChunkTag.vertex_grid[0, 100] = 0f;
            
            cornerChunkTag.update_mesh();
        }
        
        
        
        if (target_x < 1) {
            
            float join_x = ((chunk_x+100)-50-100);
            float join_z = ((chunk_z+100)-50);
            
            GameObject join_chunk = getChunk( join_x, join_z );
            if (join_chunk == null) return null;
            
            ChunkTag joinChunkTag = join_chunk.transform.GetChild(0).GetComponent<ChunkTag>();
            
            joinChunkTag.vertex_grid[100,   (int)target_z]   += terrain_damage;
            joinChunkTag.vertex_grid[100,   (int)target_z+1] += terrain_damage;
            if (joinChunkTag.vertex_grid[100, (int)target_z]   < 0f) joinChunkTag.vertex_grid[100, (int)target_z]   = 0f;
            if (joinChunkTag.vertex_grid[100, (int)target_z+1] < 0f) joinChunkTag.vertex_grid[100, (int)target_z+1] = 0f;
            
            joinChunkTag.update_mesh();
        }
        
        
        if (target_x > 99) {
            
            float join_x = ((chunk_x+100)-50+100);
            float join_z = ((chunk_z+100)-50);
            
            GameObject join_chunk = getChunk( join_x, join_z );
            if (join_chunk == null) return null;
            
            ChunkTag joinChunkTag = join_chunk.transform.GetChild(0).GetComponent<ChunkTag>();
            
            joinChunkTag.vertex_grid[0,   (int)target_z]   += terrain_damage;
            joinChunkTag.vertex_grid[0,   (int)target_z+1] += terrain_damage;
            if (joinChunkTag.vertex_grid[0, (int)target_z]   < 0f) joinChunkTag.vertex_grid[0, (int)target_z] = 0f;
            if (joinChunkTag.vertex_grid[0, (int)target_z+1] < 0f) joinChunkTag.vertex_grid[0, (int)target_z+1] = 0f;
            
            joinChunkTag.update_mesh();
        }
        
        
        if (target_z < 1) {
            
            float join_x = ((chunk_x+100)-50);
            float join_z = ((chunk_z+100)-50-100);
            
            GameObject join_chunk = getChunk( join_x, join_z );
            if (join_chunk == null) return null;
            
            ChunkTag joinChunkTag = join_chunk.transform.GetChild(0).GetComponent<ChunkTag>();
            
            joinChunkTag.vertex_grid[(int)target_x,   100] += terrain_damage;
            joinChunkTag.vertex_grid[(int)target_x+1, 100] += terrain_damage;
            if (joinChunkTag.vertex_grid[(int)target_x,   100] < 0f) joinChunkTag.vertex_grid[(int)target_x,   100] = 0f;
            if (joinChunkTag.vertex_grid[(int)target_x+1, 100] < 0f) joinChunkTag.vertex_grid[(int)target_x+1, 100] = 0f;
            
            joinChunkTag.update_mesh();
        }
        
        
        if (target_z > 99) {
            
            float join_x = ((chunk_x+100)-50);
            float join_z = ((chunk_z+100)-50+100);
            
            GameObject join_chunk = getChunk( join_x, join_z );
            if (join_chunk == null) return null;
            
            ChunkTag joinChunkTag = join_chunk.transform.GetChild(0).GetComponent<ChunkTag>();
            
            joinChunkTag.vertex_grid[(int)target_x,   0] += terrain_damage;
            joinChunkTag.vertex_grid[(int)target_x+1, 0] += terrain_damage;
            if (joinChunkTag.vertex_grid[(int)target_x,   0] < 0f) joinChunkTag.vertex_grid[(int)target_x,   0] = 0f;
            if (joinChunkTag.vertex_grid[(int)target_x+1, 0] < 0f) joinChunkTag.vertex_grid[(int)target_x+1, 0] = 0f;
            
            joinChunkTag.update_mesh();
        }
        
        chunkTag.vertex_grid[(int)target_x,   (int)target_z]   += terrain_damage;
        chunkTag.vertex_grid[(int)target_x+1, (int)target_z]   += terrain_damage;
        chunkTag.vertex_grid[(int)target_x,   (int)target_z+1] += terrain_damage;
        chunkTag.vertex_grid[(int)target_x+1, (int)target_z+1] += terrain_damage;
        
        if (chunkTag.vertex_grid[(int)target_x,   (int)target_z]   < 0f) chunkTag.vertex_grid[(int)target_x,   (int)target_z]   = 0f;
        if (chunkTag.vertex_grid[(int)target_x+1, (int)target_z]   < 0f) chunkTag.vertex_grid[(int)target_x+1, (int)target_z]   = 0f;
        if (chunkTag.vertex_grid[(int)target_x,   (int)target_z+1] < 0f) chunkTag.vertex_grid[(int)target_x,   (int)target_z+1] = 0f;
        if (chunkTag.vertex_grid[(int)target_x+1, (int)target_z+1] < 0f) chunkTag.vertex_grid[(int)target_x+1, (int)target_z+1] = 0f;
        
        chunkTag.update_mesh();
        
        return null;
	}
    */
    
    
    
}







//
// Container base types for items, entities, weapons, breakable items etc...
//

[System.Serializable]
public struct InventoryItem {
	public string     name;
	public int        stackMax;
	public Material   inventoryImage;
}


[System.Serializable]
public struct SummonableEntity {
	public string     name;
}


[System.Serializable]
public struct ConsumableItem {
    public string     name;
	public int        hunger;
	public int        saturation;
}


[System.Serializable]
public struct BreakableItem {
    public string     name;
	public string     itemClass;
	public int        hardness;
}


[System.Serializable]
public struct ShatterableItem {
    public string     name;
    public int        hardness;
    public string     itemDrop;
    public int        chanceToDrop;
}


[System.Serializable]
public struct WeaponItem {
    public string     name;
	public int        attackDamage;
	public int        strikingForce;
	public int        durability;
	public string     targetItemClass;
}









