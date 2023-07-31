﻿using System.Collections;
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

	public bool isPaused = false;

	public bool saveChunksOnDestroy;

	public bool doDayNightCycle         = true;
	public bool doWeatherCycle          = true;

	[Space(10)]
    [Header("Day/night cycle")]
	[Space(5)]

	public float InitialSunAngle        = 0.0f;
	public float dayNightCycleRate      = 0.25f;

	public float dayNightCycle          = 0.0f;

	public float dayNightCycleDirection = -90.0f;
    
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

	public bool  breedPairState   = false;
	public int   numberOfChildren = 1;
	public GameObject gene_m;
	public GameObject gene_p;




	[Space(20)]

	public bool  runCameraDamageAnimation = false;
	public float damageIndicationOffset  = -1f;

	public GameObject           ChunkList;

	public GameObject           HUD;
	public PauseMenuController  pauseMenu;

	public ChunkGeneration chunkGenerator;

	ChunkSerializer chunkSerializer;

	public Inventory   inventory;
	public GameObject  Player;
	public Camera      cameraObject;

	public float playerChunkX;
	public float playerChunkZ;





	//
	// Internal counters


	float        BaseCounter         = 0f;
	float        TickLowCounter      = 0f;

	int          HungerTickCounter   = 0;
	int          HungerShakeCounter  = 0;
	
	public int   HealthRecharge      = 0;

	int          loadTimeOut         = 0;

	int          loadCounter         = 0;




	//
	// Loading counters
	//

	float currentLoadingX;
	float currentLoadingZ;


	int ChunkCounterX=0;
	int ChunkCounterZ=0;

	int ChunkLoadDirection   = 0;


	//
	// Counters
	//

	// Chunk processing
	int ChunkCounter         = 0;
    
	// Static object processing
	int staticChunkCounter   = 0;
	int staticCounter        = 0;
    
	// Entity processing
	int EntityChunkCounter   = 0;
	int EntityCounter        = 0;
    
	bool reset_player = false;
    





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
	
	
	
	void Update() {
      
	  // ESCAPE key
	  if (Input.GetKeyDown(KeyCode.Escape)) {
        
	    isPaused = !isPaused;
        
	    if (isPaused == false) {
          
	      Cursor.lockState = CursorLockMode.Locked;
	      Cursor.visible = false;
	      
	      pauseMenu.Deactivate();
	      
	    } else {
          
	      Cursor.lockState = CursorLockMode.None;
	      Cursor.visible = true;
	      
	      pauseMenu.Activate();
	    }
	    
	  }
	  
      
      // Update chunks while paused
	  if (isPaused) {
        
	    unloadChunks();
	    loadChunks();
        
	    updateChunks();
        
	    return;
	  }
      
      
	  if (doDayNightCycle)
	    updateDayNightCycle();
      
      
	  // Check to generate new chunks
	  loadTimeOut++;
	  if (loadTimeOut > 4) {
        
	    if (loadChunks())
	      loadTimeOut=0;
        
	  }
	  
	  
	  
	  
	  //
	  // Base tick counter
	  //
	  
	  BaseCounter += Time.deltaTime / ((float)TickRate);
	  
	  if (BaseCounter > 0.045f) {
	    BaseCounter  -= 0.045f;
        
        unloadChunks();
        
        //updateStaticObjects();
        
        // Check to join entity genetics
        if (breedPairState == true) {
            breedPairState = false;
            
            for (int i=0; i < numberOfChildren; i++) {
                joinGeneticPair();
            }
        }
        
        
        
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
        
	  }
	  
	  
	  
	  
	  
	  
	  //
	  // Camera damage indication animation cycle

	  if (runCameraDamageAnimation) {
            
            if (damageIndicationOffset == -1f) 
                damageIndicationOffset = 5f;
            
            if (damageIndicationOffset > 0) {
                damageIndicationOffset -= 1f;
            } else {
                
                damageIndicationOffset = -1f;
                runCameraDamageAnimation = false;
                
            }
            
	  }
	  
	  return;
	}
	
	
	
	
	
	
	
	
	//
	// Update world chunks
	void updateChunks() {

	  for (int i=0; i < 10; i++) {

	  if (ChunkCounter >= ChunkList.transform.childCount) {
	    ChunkCounter = 0;
	  }

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














	public void updateStaticObjects() {

	  if (staticChunkCounter >= ChunkList.transform.childCount)
	    staticChunkCounter = 0;

	  GameObject chunkObject = ChunkList.transform.GetChild(staticChunkCounter).gameObject;
	  GameObject staticList  = chunkObject.transform.GetChild(2).gameObject;

	  for (int i=0; i < 80; i++) {

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





	    //
	    // Check chunk distance

	    //float totalDistance = ((float)RenderDistance) * 100f;
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


	    /*
	    
	    //
	    // Check static object distance

	    GameObject staticObject = staticList.transform.GetChild(staticCounter).gameObject;
	    if (staticObject == null) continue;

	    Vector3 staticPos = staticObject.transform.position;

	    staticPos.y = 0f;


	    if (Vector3.Distance(staticPos, playerPos) > totalDistance) {

	      if (staticObject.activeInHierarchy)
	        staticObject.SetActive(false);

	    } else {

	      if (!staticObject.activeInHierarchy)
	        staticObject.SetActive(true);

	    }
	    */
	    
	    continue;
	  }



	  return;
	}
















	// Update nearby entities
	public void updateEntities() {

	  if (EntityChunkCounter >= ChunkList.transform.childCount)
	    EntityChunkCounter = 0;

	  GameObject chunkObject = ChunkList.transform.GetChild(EntityChunkCounter).gameObject;
	  GameObject entityList  = chunkObject.transform.GetChild(1).gameObject;

	  for (int i=0; i < 100; i++) {

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

	    if (!entityObject.activeInHierarchy)
	      entityObject.SetActive(true);

	    //
	    // Update entity physics
	    entityTag.physicsUpdate();


	    // Update entity genetics tag (too slow for production)
	    GeneTag geneTag = entityObject.GetComponent<GeneTag>();
	    geneTag.updateGenetics();


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
        
        if (chunkSerializer.isChunkFinalized == true) {
            
            if (loadCounter > 1) {
                chunkSerializer.chunkFinalize();
                loadCounter = 0;
            } else {
                loadCounter++;
                return false;
            }
            
        }
        
        if (chunkSerializer.isChunkLoaded) {
            
            if (loadCounter > 1) {
                chunkSerializer.chunkBuild();
                loadCounter = 0;
            } else {
                loadCounter++;
                return false;
            }
            
        }
        
        if (chunkSerializer.isChunkFinalized == true) 
            return false;
        
        playerChunkX = Mathf.Round(Player.transform.position.x / 100) * 100;
        playerChunkZ = Mathf.Round(Player.transform.position.z / 100) * 100;
        
        // Check chunks nearby and outward
        ChunkCounterX++;
        if (ChunkCounterX >= RenderDistance) {
            
            ChunkCounterX = 0;
            
            ChunkCounterZ++;
            if (ChunkCounterZ >= RenderDistance) {
                
                ChunkLoadDirection++;
                if (ChunkLoadDirection > 3) 
                    ChunkLoadDirection = 0;
                
                ChunkCounterZ = 0;
            }
        }
        
        
        float nextChunkX = 0;
        float nextChunkZ = 0;
        bool canLoadChunk = false;
        
        if (ChunkLoadDirection == 0) {
            nextChunkX = playerChunkX + (ChunkCounterX * 100);
            nextChunkZ = playerChunkZ + (ChunkCounterZ * 100);
        }
        if (ChunkLoadDirection == 1) {
            nextChunkX = playerChunkX - (ChunkCounterX * 100);
            nextChunkZ = playerChunkZ - (ChunkCounterZ * 100);
        }
        if (ChunkLoadDirection == 2) {
            nextChunkX = playerChunkX - (ChunkCounterX * 100);
            nextChunkZ = playerChunkZ + (ChunkCounterZ * 100);
        }
        if (ChunkLoadDirection == 3) {
            nextChunkX = playerChunkX + (ChunkCounterX * 100);
            nextChunkZ = playerChunkZ - (ChunkCounterZ * 100);
        }
        
        // Check nearby chunks first
        for (int x=0; x < 3; x++) {
            for (int z=0; z < 3; z++) {
                
                float currentPlayerChunkCheckX = ((int)playerChunkX - 100) + (100 * x);
                float currentPlayerChunkCheckZ = ((int)playerChunkZ - 100) + (100 * z);
                
                if (checkChunkFree(currentPlayerChunkCheckX, currentPlayerChunkCheckZ)) {
                    nextChunkX = currentPlayerChunkCheckX;
                    nextChunkZ = currentPlayerChunkCheckZ;
                    break;
                }
            }
        }
        
        // Check chunk for processing
        if (checkChunkFree(nextChunkX, nextChunkZ)) {
            currentLoadingX = nextChunkX;
            currentLoadingZ = nextChunkZ;
            
            canLoadChunk = true;
        }
        
        // Check if the chunk can be loaded from disk
        if (canLoadChunk == false) return false;
        if (chunkSerializer.chunkExists(currentLoadingX, currentLoadingZ)) {
            
            // Generate a dummy chunk
            GameObject generated_chunk = chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, 100, false, false, false);
            
            if (generated_chunk != null) {
                
                if (!chunkSerializer.chunkLoadStart(generated_chunk, currentLoadingX, currentLoadingZ)) {
                    
                    Destroy(generated_chunk);
                    
                    return false;
                }
            }
            
            return true;
        } else {
            
            // Generate a new chunk
            chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, 100, chunkGenerator.addWorldDecorations, false, false);
            
        }
        
        return true;
	}















	//
	// Check to unload nearby chunks
	public void unloadChunks() {

	  //
	  // Check old chunks for unloading

	  if ((!chunkSerializer.isSaving) & (chunkSerializer.isChunkSaved)) {
	    Destroy(chunkSerializer.saving_chunk);
	    chunkSerializer.isChunkSaved = false;
	    chunkSerializer.isSaving     = false;
	    chunkSerializer.saving_chunk = null;
	  }

	  if ((!chunkSerializer.isSaving) & (!chunkSerializer.isChunkSaved)) {

	    float totalRenderDistance  = RenderDistance * 100 * 2f;

	    for(int i = 0; i < ChunkList.transform.childCount; i++) {

	      GameObject chunkObject  = ChunkList.transform.GetChild(i).gameObject;

	      Vector3 chunk_pos  = new Vector3(chunkObject.transform.position.x, 0f, chunkObject.transform.position.z);
	      Vector3 player_pos = new Vector3(Player.transform.position.x, 0f, Player.transform.position.z);

	      // Destroy old chunks
	      if (Vector3.Distance(chunk_pos, player_pos) > totalRenderDistance) {

	        if (!saveChunksOnDestroy) {
	          Destroy(chunkObject);
	          continue;
	        }

	        chunkSerializer.chunkSaveStart(chunkObject, chunkObject.transform.position.x, chunkObject.transform.position.z);

	        break;
	      }

	    }

	  }

	}















	//
	// Initiate world loading
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
        
        damageIndicationOffset = -1f;
        
        
        
        //
        // Initiate the chunk serializaton system
        
        chunkSerializer = new ChunkSerializer();
        chunkSerializer.clientVersion  = clientVersion;
        chunkSerializer.ChunkSize = 100;
        chunkSerializer.chunkList = ChunkList;
        chunkSerializer.generation = chunkGenerator;
        
        // Check world directory structure
        if (!chunkSerializer.openWorld(worldName))
            chunkSerializer.createWorld(worldName);
        
        
        
        // Clear the inventory
        Inventory inventoryScript = HUD.GetComponent<Inventory>();
        inventoryScript.clear();
        
        
        // Default health and hunger
        inventoryScript.setHealth( 10 );
        inventoryScript.setHunger( 8 );
        inventoryScript.setSaturation( 100 );
        
        
        // Reset animations
        runCameraDamageAnimation = false;
        
        
        // Load world data and check client version
        chunkSerializer.loadWorldData();
        
        // Set the world seed and name
        chunkSerializer.worldName = worldName;
        chunkGenerator.worldSeed = worldSeed;
        
        // Set loaded player position
        Player.transform.Translate(worldSpawn.x, -100f, worldSpawn.z);
        reset_player = true;
        
        
        
        
        
        // Initiate generation cache
        chunkGenerator.Initiate();
        
        // Setup cursor state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        isPaused = false;
        Time.timeScale = 1f;
        
        // Set the time of day
        dayNightCycle = 30f;
        if (dayNightCycle > 360.0f) dayNightCycle -= 360.0f;
        
        // Update directional light
        sun.transform.localRotation = Quaternion.Euler(new Vector3(dayNightCycle, dayNightCycleDirection, 0.0f));
        
        
        
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
        // Load first chunks
        for (int z=0; z < 4; z++) {
            
            for (int x=0; x < 4; x++) {
                
                currentLoadingX = (((x * 100) + playerChunkX)) - (2 * 100);
                currentLoadingZ = (((z * 100) + playerChunkZ)) - (2 * 100);
                
                // Check if the chunk exists on disk
                if (chunkSerializer.chunkExists(currentLoadingX, currentLoadingZ)) {
                    
                    GameObject generated_chunk = chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, 100, false, true, false);
                    //ChunkTag generated_chunk_tag = generated_chunk.gameObject.transform.GetChild(0).GetComponent<ChunkTag>();
                    
                    chunkSerializer.chunkLoadStart(generated_chunk, currentLoadingX, currentLoadingZ);
                    
                    while (chunkSerializer.isLoading == true)
                    continue;
                    
                    chunkSerializer.chunkBuild();
                    chunkSerializer.chunkFinalize();
                    
                    if (Vector3.Distance(Player.transform.position, generated_chunk.transform.position) < (RenderDistance * StaticDistance * 100f))
                    generated_chunk.transform.GetChild(2).gameObject.SetActive(true);
                    
                    
                } else {
                    
                    GameObject generated_chunk = chunkGenerator.generateChunk(currentLoadingX, currentLoadingZ, 100, chunkGenerator.addWorldDecorations, true, false);
                    //ChunkTag generated_chunk_tag = generated_chunk.gameObject.transform.GetChild(0).GetComponent<ChunkTag>();
                    
                    if (Vector3.Distance(Player.transform.position, generated_chunk.transform.position) < (RenderDistance * StaticDistance * 100f))
                    generated_chunk.transform.GetChild(2).gameObject.SetActive(true);
                    
                }
                
            }
            
        }
        
        return;
	}











	void updateDayNightCycle() {

	  dayNightCycle += dayNightCycleRate * Time.deltaTime;

	  if (dayNightCycle > 360.0f) dayNightCycle -= 360.0f;

	  sun.transform.localRotation = Quaternion.Euler(new Vector3(dayNightCycle - 10.0f, dayNightCycleDirection, 0f));


	  // Day fog cycle
	  if ((dayNightCycle > 0.0f) | (dayNightCycle < 180.0f)) {

	    float lerp_intensity = 0.1f * dayNightCycle;
	    float lerp_fog       = 0.05f * dayNightCycle;

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

	  // Night fog cycle
	  if ((dayNightCycle > 180.0f) & (dayNightCycle < 360.0f)) {

	    float lerp_intensity = 0.1f * (dayNightCycle - 180.0f);
	    float lerp_fog       = 0.04f * (dayNightCycle - 180.0f);

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








	// Summon entity offspring
	public void joinGeneticPair() {
        
        if ((gene_m == null) | (gene_p == null)) 
            return;
        
        Vector3 entityPosition = Vector3.Lerp(gene_m.transform.position, gene_p.transform.position, 0.1f);
        entityPosition.x += Random.Range(1, 3);
        entityPosition.z += Random.Range(1, 3);
        
        // Chance for parent family influence on entity type
        EntityTag entityTagM = gene_m.GetComponent<EntityTag>();
        EntityTag entityTagP = gene_p.GetComponent<EntityTag>();
        
        string entityName = entityTagP.name;
        
        if (Random.Range(1, 2) == 1) 
            entityName = entityTagM.name;
        
        // Update physical characteristic genes
        GameObject newEntity = Instantiate( Resources.Load( entityName )) as GameObject;
        newEntity.name = entityName;
        newEntity.transform.Translate(entityPosition);
        
        newEntity.transform.parent = gene_m.transform.parent;
        
        GeneTag newGene = newEntity.GetComponent<GeneTag>();
        
        // Initial genetic merge bias
        float bias = Random.Range(0.0f, 1.0f);
        newGene.mergeGenetics(gene_m.GetComponent<GeneTag>(), gene_p.GetComponent<GeneTag>(), bias);
        
        newGene.updateGenetics();
        
        
        // Update actor AI genes
        ActorTag actorTagM  = gene_m.GetComponent<ActorTag>();
        ActorTag actorTagP  = gene_p.GetComponent<ActorTag>();
        ActorTag offspring  = newEntity.GetComponent<ActorTag>();
        
        offspring.chanceChangeDirection    = Mathf.Lerp(actorTagM.chanceChangeDirection, actorTagP.chanceChangeDirection, bias);
        offspring.chanceToWalk             = Mathf.Lerp(actorTagM.chanceToWalk,          actorTagP.chanceToWalk,          bias);
        offspring.chanceToFocusOnPlayer    = Mathf.Lerp(actorTagM.chanceToFocusOnPlayer, actorTagP.chanceToFocusOnPlayer, bias);
        offspring.chanceToFocusOnEntity    = Mathf.Lerp(actorTagM.chanceToFocusOnEntity, actorTagP.chanceToFocusOnEntity, bias);
        offspring.chanceToAttackPlayer     = Mathf.Lerp(actorTagM.chanceToAttackPlayer,  actorTagP.chanceToAttackPlayer,  bias);
        
        offspring.distanceToFocusOnPlayer  = Mathf.Lerp(actorTagM.distanceToFocusOnPlayer,  actorTagP.distanceToFocusOnPlayer, bias);
        offspring.distanceToFocusOnEntity  = Mathf.Lerp(actorTagM.distanceToFocusOnEntity,  actorTagP.distanceToFocusOnEntity, bias);
        offspring.distanceToAttackPlayer   = Mathf.Lerp(actorTagM.distanceToAttackPlayer,   actorTagP.distanceToAttackPlayer,  bias);
        offspring.distanceToWalk           = Mathf.Lerp(actorTagM.distanceToWalk,           actorTagP.distanceToWalk,          bias);
        
        offspring.heightPreferenceMin      = Mathf.Lerp(actorTagM.heightPreferenceMin,  actorTagP.heightPreferenceMin, bias);
        offspring.heightPreferenceMax      = Mathf.Lerp(actorTagM.heightPreferenceMax,  actorTagP.heightPreferenceMax, bias);
        
        offspring.distanceToAttack         = Mathf.Lerp(actorTagM.distanceToAttack,  actorTagP.distanceToAttack,    bias);
        offspring.distanceToFlee           = Mathf.Lerp(actorTagM.distanceToFlee,    actorTagP.distanceToFlee,      bias);
        
        offspring.limbCycleRate            = Mathf.Lerp(actorTagM.limbCycleRate,     actorTagP.limbCycleRate,       bias);
        offspring.limbCycleRange           = Mathf.Lerp(actorTagM.limbCycleRange,    actorTagP.limbCycleRange,      bias);
        
        //offspring.consumtionTimer          = Mathf.Lerp(actorTagM.consumtionTimer,   actorTagP.consumtionTimer,     bias);
        
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





	public void purgeWorld() {

	  // Destroy world chunks
	  chunkSerializer.closeWorld();

	  //
	  // Reset world settings
	  doDayNightCycle          = true;

	  InitialSunAngle          = 0.0f;
	  dayNightCycleRate        = 0.25f;

	  dayNightCycle            = 0.0f;

	  dayNightCycleDirection   = -90.0f;

	  TickRate          = 3;
	  TickCounter       = 0;
	  BaseCounter       = 0;

	  isPaused = false;

	}






	public void saveWorld() {

	  // Save currently loaded chunks
	  for(int i = 0; i < ChunkList.transform.childCount; i++) {

	    GameObject chunkObject = ChunkList.transform.GetChild(i).gameObject;
	    //ChunkTag chunkTag = chunkObject.transform.GetChild(0).GetComponent<ChunkTag>();

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

	    if (chunk_name == chunkName) {

	      return false;
	    }

	  }

	  return true;
	}










	GameObject getChunk(float chunk_x, float chunk_z) {

	  string chunkName = chunk_x + "_" + chunk_z;

	  for (int i=0; i < ChunkList.transform.childCount; i++) {

	    string chunk_name = ChunkList.transform.GetChild(i).gameObject.name;

	    if (chunk_name == chunkName) {

	      return ChunkList.transform.GetChild(i).gameObject;
	    }

	  }

	  return null;
	}











}













