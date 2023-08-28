using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct InventoryItem {
	public string     name;
	public int        stackMax;
	public RawImage   inventoryImage;
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
}

[System.Serializable]
public struct WeaponItem {
    public string     name;
	public int        attackDamage;
	public int        strikingForce;
	public int        durability;
	public string     targetItemClass;
}






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
    public bool doShowConsole           = false;
	public bool doMouseLook             = true;
	
	
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
	public float StaticDistance      = 0.8f;
	public float EntityDistance      = 0.2f;
    
    
	[Space(10)]
	[Header("Tick rate counter")]
	[Space(5)]
    
	public int   TickRate    = 3;
	public int   TickCounter = 0;
    
    
	[Space(10)]
	[Header("Entity breeding")]
	[Space(5)]
    
	public int   numberOfChildren       = 1;
	
    public GameObject gene_m;
	public GameObject gene_p;
    
    
    
	[Space(10)]
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
	[Header("Internal")]
	[Space(5)]
    
	public bool  runCameraDamageAnimation = false;
	public float damageIndicationOffset  = 0f;
    
	public GameObject           ChunkList;
    
	
	public GameObject           CommandConsole;
    public int                  CommandConsoleTimer;
    public float                CommandConsoleFade;
    
	public GameObject           HUD;
	public PauseMenuController  pauseMenu;
    
    public ChunkGeneration chunkGenerator;
    
	public Inventory   inventory;
	public Interface   hudInterface;
	
	ChunkSerializer chunkSerializer;
    public GameObject  Player;
	public Camera      cameraObject;
    
	public float playerChunkX;
	public float playerChunkZ;
    
    
    
    
    
	//
	// Internal
    
    public       GameObject   deathMenu;
    
	float        BaseCounter=0f;
	float        TickLowCounter=0f;
    
	int          HungerTickCounter=0;
	int          HungerShakeCounter=0;
    
	public int   HealthRecharge=0;
    
	int          loadTimeOut=0;
    int          loadCounter=0;
    
    
    
    
	//
	// Chunk loading / saving counters
	//
    
	float currentLoadingX;
	float currentLoadingZ;
    int   currentChunkRing=2;
    
	int ChunkCounterX=0;
	int ChunkCounterZ=0;
    
	int ChunkCounter=0;
    
	// Static object processing
	int staticChunkCounter=0;
	int staticCounter=0;
    
	// Entity processing
	int EntityChunkCounter=0;
	int EntityCounter=0;
    
	bool reset_player = false;
    
    
    
    
    
    
    
    
    
	//
	// Tick update core
	//
	
	void Update() {
        
        //
        // Command console
        
        if (Input.GetKeyDown(KeyCode.Slash)) {
            if (isPaused) 
                return;
            
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
        // Process console commands
        
        if (doShowConsole) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                
                CommandConsoleTimer = 200;
                
                GameObject consoleTextFieldObject = CommandConsole.transform.GetChild(0).GetChild(0).gameObject;
                GameObject consoleTextObject      = CommandConsole.transform.GetChild(0).GetChild(1).gameObject;
                
                consoleTextFieldObject.SetActive(false);
                consoleTextObject.SetActive(true);
                
                Text       consoleLine  = CommandConsole.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>();
                InputField consoleField = CommandConsole.transform.GetChild(0).GetChild(0).GetComponent<InputField>();
                
                consoleLine.color = new Color(consoleLine.color.r, consoleLine.color.g, consoleLine.color.b, 1.0f);
                
                doShowConsole = false;
                doMouseLook = true;
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                string commandText = consoleField.text;
                string result = RunConsoleCommand(commandText);
                
                consoleLine.text = result;
            }
        }
        
        //
        // Pause menu
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            
            isPaused = !isPaused;
            
            if (isPaused == false) {
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                doMouseLook = true;
                
                pauseMenu.Deactivate();
                
            } else {
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                doMouseLook = false;
                
                pauseMenu.Activate();
                
                // Disable the console if active
                if (doShowConsole) {
                    
                    GameObject consoleTextFieldObject = CommandConsole.transform.GetChild(0).GetChild(0).gameObject;
                    consoleTextFieldObject.SetActive(false);
                    
                    doShowConsole = false;
                }
                
            }
            
        }
        
        
        // Load / save chunks quicker when paused
        if (isPaused) {
            
            unloadChunks();
            
            loadChunks();
            
            updateChunks();
            
            return;
        }
        
        
        //
        // Cycle the day and night
        if (doDayNightCycle)
            updateDayNightCycle();
        
        
        //
        // Generate or load chunks
        loadTimeOut--;
        if (loadTimeOut < 0) {
            
            if (loadChunks())
                loadTimeOut = Random.Range(3, 12);
            
        }
        
        
        
        //
        // Console count down timer fade
        
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
        
        
        //
        // Global tick counter
        //
        
        BaseCounter += Time.deltaTime / ((float)TickRate);
        
        if (BaseCounter > 0.045f) {
            BaseCounter  -= 0.045f;
            
            unloadChunks();
            
            updateStaticObjects();
            
            
            //
            // Check to join entity genetics
            //
            if ((gene_m != null) & (gene_p != null)) {
                
                for (int i=0; i < numberOfChildren; i++) 
                    joinGeneticPair();
                
                gene_m = null;
                gene_p = null;
            }
            
            
            
            //
            // Internal tick update timer
            //
            
            TickLowCounter += (float)TickRate;
            if (TickLowCounter > 10f) {
                TickLowCounter -= 10f;
                
                TickCounter++;
                
                updateEntities();
                
                updateChunks();
                
                
                // Update camera FOG distance
                cameraObject.farClipPlane = ((((float)RenderDistance * (float)100))) * 3.0f;
                
                
                //
                // Check player fall reset
                //
                if (reset_player == true) {
                    
                    reset_player = false;
                    
                    Vector3 playerPos = new Vector3(Player.transform.position.x, 200f, Player.transform.position.z);
                    Ray ray_obj = new Ray(playerPos, -Vector3.up);
                    
                    Vector3 hitPos = Vector3.zero;
                    
                    RaycastHit hit_obj;
                    LayerMask GroundLayerMask = LayerMask.GetMask("Ground");
                    
                    if ( Physics.Raycast(ray_obj, out hit_obj, 500f, GroundLayerMask) ) {
                        hitPos = new Vector3( hit_obj.point.x, hit_obj.point.y + 1f, hit_obj.point.z);
                        Player.transform.position = hitPos;
                    }
                    
                }
                
            }
            
            
            
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
                
                if (inventory.health < 10) {
                    if (inventory.hunger > 4) {
                        HealthRecharge = (inventory.health * (inventory.hunger / 10));
                        inventory.removeHunger(1);
                        inventory.addHealth(1);
                    }
                }
            }
            
            
            
            //
            // Hunger timer
            //
            
            HungerTickCounter++;
            
            if (HungerTickCounter > (350 + (inventory.saturation * inventory.saturation))) {
                HungerTickCounter = 0;
                
                inventory.removeSaturation(10);
                
                // Remove hunger until starving
                if (inventory.hunger > 0) {
                    inventory.removeHunger( 1 );
                } else {
                    inventory.removeHealth( 1 );
                }
                
            }
            
            return;
        }
        
        
        
        
        
        
        //
        // Camera damage indication animation cycle
        
        if (runCameraDamageAnimation) {
            
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
        // Check player death
        
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
	//
	
	void updateChunks() {
        
        int numberOfChunksPerTick = 40;
        
        for (int i=0; i < numberOfChunksPerTick; i++) {
            
            if (ChunkCounter >= ChunkList.transform.childCount) 
                ChunkCounter = 0;
            
            GameObject chunkObject = ChunkList.transform.GetChild(ChunkCounter).gameObject;
            GameObject staticList  = chunkObject.transform.GetChild(2).gameObject;
            
            
            ChunkCounter++;
            
            if (ChunkCounter >= ChunkList.transform.childCount) {
                ChunkCounter = 0;
                return;
            }
            
            
            //
            // Process static objects
            //
            
            Vector3 chunkPos  = chunkObject.transform.position;
            Vector3 playerPos = Player.transform.position;
            
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
	// Save current structure to an external file
	public void saveCurrentStructureToFile() {
        
        int numberOfItems = chunkGenerator.currentStructure.items.Count;
        
        StructureData structureData = new StructureData(numberOfItems);
        
        for (int i=0; i < numberOfItems; i++) {
            
            structureData.name[i]     = chunkGenerator.currentStructure.items[i].name;
            structureData.data[i]     = chunkGenerator.currentStructure.items[i].data;
            
            structureData.position[i] = new Vec3(chunkGenerator.currentStructure.items[i].position);
            structureData.rotation[i] = new Vec3(chunkGenerator.currentStructure.items[i].rotation);
            structureData.scale[i]    = new Vec3(chunkGenerator.currentStructure.items[i].scale);
            
        }
        
        // Check structures directory
        string structuresPath = "structures/";
        if (!System.IO.Directory.Exists(structuresPath)) 
            System.IO.Directory.CreateDirectory(structuresPath);
        
        // Check if the file already exists
        string filePath = structuresPath + chunkGenerator.currentStructure.name + ".structure";
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
	// Load a structure from an external file
	public int loadStructureFileToCurrentStructure() {
        
        // Check structures directory
        string structuresPath = "structures/";
        if (!System.IO.Directory.Exists(structuresPath)) 
            System.IO.Directory.CreateDirectory(structuresPath);
        
        // Check if the file does not exist
        string filePath = structuresPath + chunkGenerator.currentStructure.name + ".structure";
        if (!System.IO.File.Exists(filePath)) 
            return 0;
        
        // Binary stream
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter;
        formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        
        System.IO.FileStream fileStream_load;
        fileStream_load = System.IO.File.Open(structuresPath + filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        
        StructureData structureData = new StructureData(2048);
        structureData = formatter.Deserialize(fileStream_load) as StructureData;
        
        // Close out
        fileStream_load.Close();
        
        
        // Load the data
        int numberOfItems = structureData.name.Length;
        
        for (int i=0; i < numberOfItems; i++) {
            
            StructureItem newStructureItem = new StructureItem();
            
            newStructureItem.name = structureData.name[i];
            newStructureItem.data = structureData.data[i];
            newStructureItem.position = new Vector3(structureData.position[i].x, structureData.position[i].y, structureData.position[i].z);
            newStructureItem.rotation = new Vector3(structureData.rotation[i].x, structureData.rotation[i].y, structureData.rotation[i].z);
            newStructureItem.scale    = new Vector3(structureData.scale[i].x, structureData.scale[i].y, structureData.scale[i].z);
            
            chunkGenerator.currentStructure.items.Add(newStructureItem);
            
        }
        
        return 1;
    }
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	//
	// Update static objects
	//
	
	public void updateStaticObjects() {
        
        if (staticChunkCounter >= ChunkList.transform.childCount)
            staticChunkCounter = 0;
        
        GameObject chunkObject = ChunkList.transform.GetChild(staticChunkCounter).gameObject;
        GameObject staticList  = chunkObject.transform.GetChild(2).gameObject;
        
        int numberOfObjectsPerTick = 80;
        
        for (int i=0; i < numberOfObjectsPerTick; i++) {
            
            staticCounter++;
            
            if (staticCounter >= staticList.transform.childCount) {
                
                staticCounter=-1;
                
                staticChunkCounter++;
                
                // Check last chunk
                if (staticChunkCounter >= ChunkList.transform.childCount)
                staticChunkCounter = 0;
                
                chunkObject = ChunkList.transform.GetChild(staticChunkCounter).gameObject;
                staticList  = chunkObject.transform.GetChild(2).gameObject;
                
                continue;
            }
            
            GameObject staticObject = staticList.transform.GetChild(staticCounter).transform.gameObject;
            
            // Item end of life counter
            if (staticObject.GetComponent<ItemTag>().lifeTime > 0) {
                staticObject.GetComponent<ItemTag>().lifeTime--;
                
                if (staticObject.GetComponent<ItemTag>().lifeTime == 0) {
                    
                    Destroy(staticObject);
                    continue;
                }
            }
            
            
            //
            // Check chunk distance
            
            float totalDistance = ((float)RenderDistance * StaticDistance) * 100f;
            
            Vector3 playerPos = Player.transform.position;
            Vector3 chunkPos  = chunkObject.transform.position;
            
            playerPos.y = 0f;
            chunkPos.y  = 0f;
            
            if (Vector3.Distance(chunkPos, playerPos) > totalDistance) {
                
                staticCounter = -1;
                staticChunkCounter++;
                staticList.SetActive(false);
                
                continue;
            } else {
                
                staticList.SetActive(true);
            }
            
            continue;
        }
        
        
        
        return;
	}
	
	
	
	
	public void entityAutoBreeding(GameObject entityList) {
        
        int   maxCountPerChunk  = 50;
        float maxBreedDistance  = 10f;
        
        GameObject geneSrcM = null;
        GameObject geneSrcP = null;
        
        if (entityList.transform.childCount < maxCountPerChunk) 
            return;
        
        // Locate some entities in the same chunk
        for (int i=0; i < 20; i++) {
            geneSrcM = entityList.transform.GetChild( Random.Range(0, entityList.transform.childCount) ).transform.gameObject;
            geneSrcP = entityList.transform.GetChild( Random.Range(0, entityList.transform.childCount) ).transform.gameObject;
            
            // Entity breeding distance
            if (Vector3.Distance(geneSrcM.transform.position, geneSrcP.transform.position) < maxBreedDistance) 
                continue;
            
            // Must be active
            if (!geneSrcM.activeSelf) 
                continue;
            if (!geneSrcP.activeSelf) 
                continue;
            
            // Parents cannot be the same
            if (geneSrcM == geneSrcP)
                continue;
            
            // Passed.. select pair for breeding
            if (geneSrcM != null) gene_m = geneSrcM;
            if (geneSrcP != null) gene_p = geneSrcP;
            
            break;
        }
        
        
        numberOfChildren = 1;
        
        return;
    }
	
	
	
	
	
	
	
	
	// Update nearby entities
	public void updateEntities() {
        
        if (EntityChunkCounter >= ChunkList.transform.childCount)
            EntityChunkCounter = 0;
        
        GameObject chunkObject = ChunkList.transform.GetChild(EntityChunkCounter).gameObject;
        GameObject entityList  = chunkObject.transform.GetChild(1).gameObject;
        
        int numberOfEntitiesPerTick = 100;
        
        for (int i=0; i < numberOfEntitiesPerTick; i++) {
            
            EntityCounter++;
            
            if (EntityCounter >= entityList.transform.childCount) {
                
                EntityCounter=-1;
                
                EntityChunkCounter++;
                
                // Check last chunk
                if (EntityChunkCounter >= ChunkList.transform.childCount)
                EntityChunkCounter = 0;
                
                chunkObject = ChunkList.transform.GetChild(EntityChunkCounter).gameObject;
                entityList  = chunkObject.transform.GetChild(1).gameObject;
                
                continue;
            }
            
            GameObject entityObject = entityList.transform.GetChild(EntityCounter).gameObject;
            if (entityObject == null) break;
            
            // Update entity tag
            EntityTag entityTag = entityObject.GetComponent<EntityTag>();
            entityTag.Age++;
            
            // Update actor AI
            ActorTag actorTag = entityObject.GetComponent<ActorTag>();
            
            Vector3 entityPos = entityObject.transform.position;
            Vector3 playerPos = Player.transform.position;
            
            entityPos.y = 0;
            playerPos.y = 0;
            
            
            // Entity auto breeding
            
            entityAutoBreeding(entityList);
            
            
            
            
            //
            // Check entity update distance
            float totalEntityDistance = ((float)RenderDistance * EntityDistance)  * 100;
            if (Vector3.Distance(entityPos, playerPos) > totalEntityDistance) {
                
                // Deactivate the entity
                entityObject.SetActive(false);
                
                actorTag.isActive = false;
                actorTag.resetAnimation();
                
                entityTag.isWalking  = false;
                entityTag.isRunning  = false;
                entityTag.isRotating = false;
            
                continue;
            }
            
            
            
            
            //
            // Activate and update entity
            
            actorTag.isActive = true;
            
            if (entityTag.useAI)
                actorTag.updateAI();
            
            // No AI attacks in debug mode
            if (doDebugMode) {
                if (actorTag.targetEntity == Player) {
                    actorTag.targetEntity = null;
                    actorTag.isAttacking = false;
                }
            }
            
            if (!entityObject.activeInHierarchy)
                entityObject.SetActive(true);
            
            //
            // Update entity physics
            entityTag.physicsUpdate();
            
            
            // Update entity genetics tag
            GeneTag geneTag = entityObject.GetComponent<GeneTag>();
            if (geneTag.doUpdateGenetics) {
                geneTag.updateGenetics();
                geneTag.doUpdateGenetics = false;
            }
            
            //
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
        
        // Check to finalize a loaded chunk
        if (chunkSerializer.isChunkFinalized == true) {
            
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
        
        if (chunkSerializer.isChunkFinalized == true) 
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
        
        
        playerChunkX = Mathf.Round(Player.transform.position.x / 100) * 100;
        playerChunkZ = Mathf.Round(Player.transform.position.z / 100) * 100;
        
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
            if ((ChunkCounterX == 0) & (ChunkCounterZ == 0)) {
                currentChunkRing++;
                
                if (currentChunkRing >= RenderDistance) 
                    currentChunkRing = 2;
            }
            
            return false;
        }
        
        if (chunkSerializer.chunkExists(currentLoadingX, currentLoadingZ)) {
            
            // Generate a dummy chunk
            GameObject generated_chunk = chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, 100, false,   true);
            
            if (generated_chunk != null) {
                
                if (!chunkSerializer.chunkLoadStart(generated_chunk, currentLoadingX, currentLoadingZ)) {
                    
                    Destroy(generated_chunk);
                    
                    return false;
                }
            }
            
            return true;
        } else {
            
            // Generate a new chunk
            chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, 100, chunkGenerator.addWorldDecorations,   false);
            
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
            
            float farRenderDistance  = RenderDistance * 100f * 2f;
            
            for(int i = 0; i < ChunkList.transform.childCount; i++) {
                
                GameObject chunkObject  = ChunkList.transform.GetChild(i).gameObject;
                
                Vector3 chunk_pos  = new Vector3(chunkObject.transform.position.x, 0f, chunkObject.transform.position.z);
                Vector3 player_pos = new Vector3(Player.transform.position.x, 0f, Player.transform.position.z);
                
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
            
            RenderSettings.fogStartDistance = lerpdistance.x;
            RenderSettings.fogEndDistance   = lerpdistance.y;
            RenderSettings.fogColor = Color.Lerp(fogNight, fogDay, lerp_fog);
            
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
            
            RenderSettings.fogStartDistance = lerpdistance.r;
            RenderSettings.fogEndDistance   = lerpdistance.g;
            RenderSettings.fogColor = Color.Lerp(fogDay, fogNight, lerp_fog);
            
            Color ambientDayColor   = new Color(ambientDay,   ambientDay,   ambientDay,   1.0f);
            Color ambientNightColor = new Color(ambientNight, ambientNight, ambientNight, 1.0f);
            
            RenderSettings.ambientLight = Color.Lerp(ambientDayColor, ambientNightColor, lerp_fog);
            
            sun.intensity = lerpintensity.r;
            if (sun.intensity > 0.87f) sun.intensity = 0.87f;
            if (sun.intensity < 0f)    sun.intensity = 0f;
            
        }
        
        return;
	}
	
	
	
	
	
	
	
	
	//
	// Entity genetic breeding
	//
	
	public void joinGeneticPair() {
        
        if ((gene_m == null) | (gene_p == null)) 
            return;
        
        Vector3 entityPosition = Vector3.Lerp(gene_m.transform.position, gene_p.transform.position, 0.1f);
        entityPosition.x += Random.Range(1, 2) - Random.Range(1, 2);
        entityPosition.z += Random.Range(1, 2) - Random.Range(1, 2);
        
        // Chance for parent family influence on entity type
        EntityTag entityTagM = gene_m.GetComponent<EntityTag>();
        EntityTag entityTagP = gene_p.GetComponent<EntityTag>();
        
        string entityName = entityTagP.name;
        
        if (Random.Range(0, 10) >= 5) 
            entityName = entityTagM.name;
        
        GameObject newEntity = Instantiate( Resources.Load( entityName )) as GameObject;
        newEntity.name = entityName;
        
        // Place offspring on terrain surface
        entityPosition.y = 500f;
        
        RaycastHit hit_obj;
        Ray ray_obj = new Ray(entityPosition, -Vector3.up);
        
        if ( Physics.Raycast(ray_obj, out hit_obj, 1000f, LayerMask.GetMask("Ground")) ) {
            newEntity.transform.position = new Vector3(hit_obj.point.x, hit_obj.point.y + 0.5f, hit_obj.point.z);
        } else {
            newEntity.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        }
        
        
        newEntity.transform.parent = gene_m.transform.parent;
        
        GeneTag newGene = newEntity.GetComponent<GeneTag>();
        
        // Initial genetic merge bias
        newGene.mergeGenetics(gene_m.GetComponent<GeneTag>(), gene_p.GetComponent<GeneTag>());
        
        newGene.updateGenetics();
        
        // Update actor AI genes
        
        ActorTag actorTagM  = gene_m.GetComponent<ActorTag>();
        ActorTag actorTagP  = gene_p.GetComponent<ActorTag>();
        ActorTag offspring  = newEntity.GetComponent<ActorTag>();
        
        float bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.chanceToChangeDirection  = Mathf.Lerp(actorTagM.chanceToChangeDirection,  actorTagP.chanceToChangeDirection, bias);
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.chanceToWalk             = Mathf.Lerp(actorTagM.chanceToWalk,             actorTagP.chanceToWalk,          bias);
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.chanceToFocusOnPlayer    = Mathf.Lerp(actorTagM.chanceToFocusOnPlayer,    actorTagP.chanceToFocusOnPlayer, bias);
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.chanceToFocusOnEntity    = Mathf.Lerp(actorTagM.chanceToFocusOnEntity,    actorTagP.chanceToFocusOnEntity, bias);
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.chanceToAttackPlayer     = Mathf.Lerp(actorTagM.chanceToAttackPlayer,     actorTagP.chanceToAttackPlayer,  bias);
        
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.distanceToFocusOnPlayer  = Mathf.Lerp(actorTagM.distanceToFocusOnPlayer,  actorTagP.distanceToFocusOnPlayer, bias);
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.distanceToFocusOnEntity  = Mathf.Lerp(actorTagM.distanceToFocusOnEntity,  actorTagP.distanceToFocusOnEntity, bias);
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.distanceToAttackPlayer   = Mathf.Lerp(actorTagM.distanceToAttackPlayer,   actorTagP.distanceToAttackPlayer,  bias);
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.distanceToWalk           = Mathf.Lerp(actorTagM.distanceToWalk,           actorTagP.distanceToWalk,          bias);
        
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.heightPreferenceMin      = Mathf.Lerp(actorTagM.heightPreferenceMin,      actorTagP.heightPreferenceMin, bias);
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.heightPreferenceMax      = Mathf.Lerp(actorTagM.heightPreferenceMax,      actorTagP.heightPreferenceMax, bias);
        
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.distanceToAttack         = Mathf.Lerp(actorTagM.distanceToAttack,         actorTagP.distanceToAttack,    bias);
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.distanceToFlee           = Mathf.Lerp(actorTagM.distanceToFlee,           actorTagP.distanceToFlee,      bias);
        
        offspring.limbAxis                 = Random.Range(actorTagM.limbAxis, actorTagP.limbAxis);
        if (Random.Range(0, 1) == 1) {offspring.limbAxisInvert = true;} else {offspring.limbAxisInvert = false;}
        
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.limbCycleRate            = Mathf.Lerp(actorTagM.limbCycleRate,     actorTagP.limbCycleRate,       bias);
        bias = (Random.Range(0f, 1f) - Random.Range(0f, 1f));
        offspring.limbCycleRange           = Mathf.Lerp(actorTagM.limbCycleRange,    actorTagP.limbCycleRange,      bias);
        
        offspring.consumptionTimer          = Random.Range(actorTagM.consumptionTimer, actorTagP.consumptionTimer);
        
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
    // Change a point in the terrain and update the edges
    // Can be useful for stitching the chunk edges
    
	public GameObject update_nearby_chunks(float point_x, float point_z, ChunkTag chunkTag, float terrain_damage) {
        
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
    
    
    
    
    
    
    
    
    
    
	
	//
	// Initiate world before loading
	//
	
	public void initiateWorld() {
        
        //
        // Spawn player
        
        Player = Instantiate( Resources.Load("Player")) as GameObject;
        
        Player.name = "Player";
        
        Player.transform.parent = GameObject.Find("World").transform;
        GameObject playerCameraObject = GameObject.Find("Main Camera");
        
        cameraObject = playerCameraObject.GetComponent<Camera>();
        
        cameraObject.nearClipPlane  = 0.05f;
        cameraObject.farClipPlane   = (((float)RenderDistance * (float)100)) * 0.5f;
        
        damageIndicationOffset = 0f;
        
        
        
        //
        // Initiate the chunk serialization system
        
        chunkSerializer = new ChunkSerializer();
        chunkSerializer.clientVersion  = clientVersion;
        chunkSerializer.ChunkSize = 100;
        chunkSerializer.chunkList = ChunkList;
        
        // Check world directory structure
        if (!chunkSerializer.openWorld(worldName))
            chunkSerializer.createWorld(worldName);
        
        // Clear the inventory
        inventory.initiate();
        inventory.clear();
        
        // Default health and hunger
        inventory.setHealth( 10 );
        inventory.setHunger( 8 );
        inventory.setSaturation( 100 );
        
        // Reactivate hunger and health bars
        HUD.transform.GetChild(5).gameObject.SetActive(true);
        HUD.transform.GetChild(6).gameObject.SetActive(true);
        
        
        // Load world data and check client version
        bool success = chunkSerializer.loadWorldData();
        
        // Set the world seed and name
        chunkSerializer.worldName = worldName;
        chunkGenerator.worldSeed = worldSeed;
        
        // Set loaded player position
        Player.transform.Translate(worldSpawn.x, -100f, worldSpawn.z);
        reset_player = true;
        
        chunkGenerator.initiate();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        isPaused           = false;
        doShowConsole      = false;
        doMouseLook        = true;
        
        Time.timeScale = 1f;
        
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
        playerChunkX = Mathf.Round(Player.transform.position.x / 100) * 100;
        playerChunkZ = Mathf.Round(Player.transform.position.z / 100) * 100;
        
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
                    
                    GameObject generated_chunk = chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, 100, false, true);
                    //ChunkTag generated_chunk_tag = generated_chunk.gameObject.transform.GetChild(0).GetComponent<ChunkTag>();
                    
                    chunkSerializer.chunkLoadStart(generated_chunk, currentLoadingX, currentLoadingZ);
                    
                    while (chunkSerializer.isLoading == true)
                    continue;
                    
                    chunkSerializer.chunkBuild();
                    chunkSerializer.chunkFinalize();
                    
                    if (Vector3.Distance(Player.transform.position, generated_chunk.transform.position) < (RenderDistance * StaticDistance * 100f))
                    generated_chunk.transform.GetChild(2).gameObject.SetActive(true);
                    
                // Else generate a new chunk
                } else {
                    
                    GameObject generated_chunk = chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, 100, chunkGenerator.addWorldDecorations, false);
                    //ChunkTag generated_chunk_tag = generated_chunk.gameObject.transform.GetChild(0).GetComponent<ChunkTag>();
                    
                    if (Vector3.Distance(Player.transform.position, generated_chunk.transform.position) < (RenderDistance * StaticDistance * 100f))
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
        
        
        inventory.shutdown();
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
        
        string commandNotFound = "Command not identified";
        if (inputString == "") 
            return commandNotFound;
        
        string[] paramaters = inputString.Split(' ');
        if (paramaters.Length < 2) 
            return commandNotFound;
        
        switch (paramaters[0]) {
            
            case "structure":   return consoleStructure(paramaters);
            case "summon":      return consoleSummon(paramaters);
            case "give":        return consoleGive(paramaters);
            case "rule":        return consoleRule(paramaters);
            case "time":        return consoleTime(paramaters);
            case "tp":          return consoleTP(paramaters);
            
        }
        
        return commandNotFound;
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
                newPosition.x = Player.transform.position.x + (Player.transform.forward.x * 3.0f);
                newPosition.y = Player.transform.position.y;
                newPosition.z = Player.transform.position.z + (Player.transform.forward.z * 3.0f);
                newEntity.transform.position = newPosition;
                
                newEntity.name = entityName;
                newEntity.transform.parent = currentChunk.transform.GetChild(1).gameObject.transform;
                
                GeneTag geneticTag = newEntity.GetComponent<GeneTag>();
                geneticTag.updateGenetics();
                
                return "Summoned " + entityName;
            }
        }
        
        return "Entity not found " + '"' + entityName + '"';
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
        
        return "Item not found " + '"' + itemName + '"';
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
        
        if (ruleName == "weathercycle") {
            if (status=="true")  {doWeatherCycle = true;  return "Weather cycle enabled";}
            if (status=="false") {doWeatherCycle = false; return "Weather cycle disabled";}
        }
        
        if (ruleName == "suncycle") {
            if (status=="true")  {doDayNightCycle = true;  return "Day night cycle enabled";}
            if (status=="false") {doDayNightCycle = false; return "Day night cycle disabled";}
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
            return "Cannot teleport to location";
        
        float xPos = float.Parse(paramaters[1]);
        float yPos = float.Parse(paramaters[2]);
        float zPos = float.Parse(paramaters[3]);
        
        Player.transform.position = new Vector3(xPos, yPos, zPos);
        
        return "Teleported player to "+paramaters[1]+", "+paramaters[2]+", "+paramaters[3];
    }
    
    
    
    //
    // Structure
    //
    
    string consoleStructure(string[] paramaters) {
        
        if (paramaters.Length < 1) 
            return "Cannot generate structure";
        
        
        if (paramaters[1] == "save") {
            saveCurrentStructureToFile();
            
            return "Structure saved to file '"+chunkGenerator.currentStructure.name+"'";
        }
        
        
        if (paramaters[1] == "load") {
            
            chunkGenerator.currentStructure.name = paramaters[2];
            
            if (loadStructureFileToCurrentStructure() == 0) 
                return "Structure file not found '"+chunkGenerator.currentStructure.name+"'";
            
            return "Structure loaded from file '"+chunkGenerator.currentStructure.name+"'";
        }
        
        
        if (paramaters[1] == "place") {
            
            Vector3 playerPos;
            playerPos.x = Mathf.Round(Player.transform.position.x);
            playerPos.y = Mathf.Round(Player.transform.position.y) + 0.5f;
            playerPos.z = Mathf.Round(Player.transform.position.z);
            
            
            return "Structure placed ??????  '"+chunkGenerator.currentStructure.name+"'";
        }
        
        return "Unknown structure function '"+paramaters[1]+"'";
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
                    return "Time set to night";
                    
                case "noon":
                    dayNightCycleCurrent = 100f;
                    updateDayNightCycle();
                    return "Time set to night";
                    
                default:
                    break;
            }
            
        }
        
        return "Time function not recognized";
    }
    
    
    
}










