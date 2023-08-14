﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;








public class ChunkSerializer {
	
	public int clientVersion = 100;
	
	public string worldName;
	
	public int ChunkSize = 100;
	
	public string worldsPath;
	public string chunksPath;
	
	public System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter;
	public System.IO.FileStream fileStream_save;
	public System.IO.FileStream fileStream_load;
	
	public GameObject chunkList;
	public GameObject gameRules;
	
	
	//
	// Thread loading
	
	public float loadingX;
	public float loadingZ;
	
	public GameObject loading_chunk;
	
	public bool       isChunkLoaded;
	public bool       isLoading;
	public bool       isChunkFinalized;
	
	public ChunkData  loadChunk;
	
	
	//
	// Thread saving
	
	public float savingX;
	public float savingZ;
	
	public GameObject saving_chunk;
	
	public bool       isChunkSaved;
	public bool       isSaving;
	
	public ChunkData  saveChunk;
	
	
	
	
	public WorldData  worldData;
	
	
	public ChunkGeneration generation;
	
	public ChunkSerializer() {
        
        isChunkLoaded = false;
        isLoading     = false;
        isChunkSaved  = false;
        isSaving      = false;
        
        worldName       = "";
        worldsPath      = "worlds/";
        chunksPath      = "/chunks/";
        
        formatter  = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        
        loadChunk  = new ChunkData(10000, 10000);
        
	}
	
	
	public bool chunkSaveStart(GameObject chunk, float chunk_x, float chunk_z) {
        
        //
        // Do not save if we are loading
        
        if (loading_chunk != null) 
            return false;
        
        
        if ((!isChunkSaved) & (isSaving)) return false;
        
        savingX = chunk_x;
        savingZ = chunk_z;
        saving_chunk = chunk;
        
        if (chunkSave()) {
            
            isChunkSaved = false;
            isSaving     = true;
            
            Thread threadSave = new Thread( chunkThreadSave );
            threadSave.Start();
            
            return true;
        }
        
        return false;
	}
	
	
	
	public void chunkThreadSave() {
        
        string chunkName = savingX + "_" + savingZ;
        
        fileStream_save = System.IO.File.Open(worldsPath + worldName + chunksPath + chunkName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        formatter.Serialize(fileStream_save, saveChunk);
        fileStream_save.Close();
        
        isChunkSaved = true;
        isSaving     = false;
        
        return;
	}
	
	
	public bool chunkSave() {
        
        GameObject chunk = saving_chunk;
        
        //float chunk_x    = savingX;
        //float chunk_z    = savingZ;
        //string chunkName = chunk_x + "_" + chunk_z;
        
        GameObject entities   = chunk.gameObject.transform.GetChild(1).gameObject;
        GameObject statics    = chunk.gameObject.transform.GetChild(2).gameObject;
        saveChunk = new ChunkData(statics.gameObject.transform.childCount, entities.gameObject.transform.childCount);
        
        // Save chunk biome type
        ChunkTag chunkTag = chunk.gameObject.transform.GetChild(0).GetComponent<ChunkTag>();
        saveChunk.biome = chunkTag.biome;
        
        //
        // Save vertex buffer data
        
        for (int z=0; z <= 100; z++) {
            for (int x=0; x <= 100; x++) {
                
                float vertex  = chunkTag.vertex_grid[x, z];
                Vector3 Color = chunkTag.color_grid[x, z];
                
                saveChunk.vertexGrid[x, z] = vertex;
                saveChunk.colorGrid[x, z]  = new Vec3(Color.x, Color.y, Color.z);
                
            }
        }
        
        
        
        // 
        // Save static objects
        for (int i = 0; i < statics.gameObject.transform.childCount; i++) {
            
            // Get entity object
            GameObject staticObject     = statics.gameObject.transform.GetChild(i).gameObject;
            
            saveChunk.staticName[i]     = staticObject.name;
            saveChunk.staticPosition[i] = new Vec3(staticObject.transform.position.x, staticObject.transform.position.y, staticObject.transform.position.z);
            saveChunk.staticRotation[i] = new Vec3(staticObject.transform.rotation.x, staticObject.transform.rotation.y, staticObject.transform.rotation.z);
            saveChunk.staticScale[i]    = new Vec3(staticObject.transform.localScale.x, staticObject.transform.localScale.y, staticObject.transform.localScale.z);
            
        }
        
        // Save dynamic entities
        for (int i = 0; i < entities.gameObject.transform.childCount; i++) {
            
            // Get entity object
            GameObject entityObject = entities.gameObject.transform.GetChild(i).gameObject;
            
            saveChunk.entityPosition[i] = new Vec3(entityObject.transform.position.x, entityObject.transform.position.y, entityObject.transform.position.z);
            saveChunk.entityRotation[i] = new Vec3(entityObject.transform.rotation.x, entityObject.transform.rotation.y, entityObject.transform.rotation.z);
            
            // Tag data
            EntityTag entityTag     = entityObject.gameObject.GetComponent<EntityTag>();
            
            saveChunk.entityName[i] = entityTag.Name;
            saveChunk.Health[i]     = entityTag.Health;
            saveChunk.Armor[i]      = entityTag.Armor;
            saveChunk.Age[i]        = entityTag.Age;
            saveChunk.Team[i]       = entityTag.Team;
            saveChunk.Speed[i]      = entityTag.Speed;
            saveChunk.isGenetic[i]  = entityTag.isGenetic;
            saveChunk.useAI[i]      = entityTag.useAI;
            
            // AI
            ActorTag actorTag = entityObject.GetComponent<ActorTag>();
            
            saveChunk.chanceToChangeDirection[i] = actorTag.chanceToChangeDirection;
            saveChunk.chanceToWalk[i]            = actorTag.chanceToWalk;
            saveChunk.chanceToFocusOnPlayer[i]   = actorTag.chanceToFocusOnPlayer;
            saveChunk.chanceToFocusOnEntity[i]   = actorTag.chanceToFocusOnEntity;
            saveChunk.chanceToAttackPlayer[i]    = actorTag.chanceToAttackPlayer;
            
            saveChunk.distanceToFocusOnPlayer[i] = actorTag.distanceToFocusOnPlayer;
            saveChunk.distanceToFocusOnEntity[i] = actorTag.distanceToFocusOnEntity;
            saveChunk.distanceToAttackPlayer[i]  = actorTag.distanceToAttackPlayer;
            saveChunk.distanceToWalk[i]          = actorTag.distanceToWalk;
            
            saveChunk.heightPreferenceMin[i]     = actorTag.heightPreferenceMin;
            saveChunk.heightPreferenceMax[i]     = actorTag.heightPreferenceMax;
            
            saveChunk.distanceToAttack[i]        = actorTag.distanceToAttack;
            saveChunk.distanceToFlee[i]          = actorTag.distanceToFlee;
            
            // Emotional state
            saveChunk.love[i]                    = actorTag.love;
            saveChunk.fear[i]                    = actorTag.fear;
            saveChunk.stress[i]                  = actorTag.stress;
            saveChunk.hunger[i]                  = actorTag.hunger;
            
            // Animation
            saveChunk.limbAxis[i]       = actorTag.limbAxis;
            saveChunk.limbAxisInvert[i] = actorTag.limbAxisInvert;
            saveChunk.limbCycleRate[i]  = actorTag.limbCycleRate;
            saveChunk.limbCycleRange[i] = actorTag.limbCycleRange;
            
            saveChunk.consumptionTimer[i] = actorTag.consumptionTimer;
            
            
            
            
            //
            // Genetic state
            if (entityTag.isGenetic) {
            
            GeneTag entityGeneticTag  = entityObject.GetComponent<GeneTag>();
            
            saveChunk.AdultSizeMul[i] = entityGeneticTag.AdultSizeMul;
            
            saveChunk.BodyO[i]       = new Vec3(entityGeneticTag.BodyO.x,   entityGeneticTag.BodyO.y,   entityGeneticTag.BodyO.z);
            saveChunk.HeadO[i]       = new Vec3(entityGeneticTag.HeadO.x,   entityGeneticTag.HeadO.y,   entityGeneticTag.HeadO.z);
            saveChunk.LimbFLO[i]     = new Vec3(entityGeneticTag.LimbFLO.x, entityGeneticTag.LimbFLO.y, entityGeneticTag.LimbFLO.z);
            saveChunk.LimbFRO[i]     = new Vec3(entityGeneticTag.LimbFRO.x, entityGeneticTag.LimbFRO.y, entityGeneticTag.LimbFRO.z);
            saveChunk.LimbRLO[i]     = new Vec3(entityGeneticTag.LimbRLO.x, entityGeneticTag.LimbRLO.y, entityGeneticTag.LimbRLO.z);
            saveChunk.LimbRRO[i]     = new Vec3(entityGeneticTag.LimbRRO.x, entityGeneticTag.LimbRRO.y, entityGeneticTag.LimbRRO.z);
            
            saveChunk.BodyP[i]       = new Vec3(entityGeneticTag.BodyP.x,   entityGeneticTag.BodyP.y,   entityGeneticTag.BodyP.z);
            saveChunk.HeadP[i]       = new Vec3(entityGeneticTag.HeadP.x,   entityGeneticTag.HeadP.y,   entityGeneticTag.HeadP.z);
            saveChunk.LimbFLP[i]     = new Vec3(entityGeneticTag.LimbFLP.x, entityGeneticTag.LimbFLP.y, entityGeneticTag.LimbFLP.z);
            saveChunk.LimbFRP[i]     = new Vec3(entityGeneticTag.LimbFRP.x, entityGeneticTag.LimbFRP.y, entityGeneticTag.LimbFRP.z);
            saveChunk.LimbRLP[i]     = new Vec3(entityGeneticTag.LimbRLP.x, entityGeneticTag.LimbRLP.y, entityGeneticTag.LimbRLP.z);
            saveChunk.LimbRRP[i]     = new Vec3(entityGeneticTag.LimbRRP.x, entityGeneticTag.LimbRRP.y, entityGeneticTag.LimbRRP.z);
            
            saveChunk.BodyR[i]       = new Vec3(entityGeneticTag.BodyR.x,   entityGeneticTag.BodyR.y,   entityGeneticTag.BodyR.z);
            saveChunk.HeadR[i]       = new Vec3(entityGeneticTag.HeadR.x,   entityGeneticTag.HeadR.y,   entityGeneticTag.HeadR.z);
            saveChunk.LimbFLR[i]     = new Vec3(entityGeneticTag.LimbFLR.x, entityGeneticTag.LimbFLR.y, entityGeneticTag.LimbFLR.z);
            saveChunk.LimbFRR[i]     = new Vec3(entityGeneticTag.LimbFRR.x, entityGeneticTag.LimbFRR.y, entityGeneticTag.LimbFRR.z);
            saveChunk.LimbRLR[i]     = new Vec3(entityGeneticTag.LimbRLR.x, entityGeneticTag.LimbRLR.y, entityGeneticTag.LimbRLR.z);
            saveChunk.LimbRRR[i]     = new Vec3(entityGeneticTag.LimbRRR.x, entityGeneticTag.LimbRRR.y, entityGeneticTag.LimbRRR.z);
            
            saveChunk.BodyS[i]       = new Vec3(entityGeneticTag.BodyS.x,   entityGeneticTag.BodyS.y,   entityGeneticTag.BodyS.z);
            saveChunk.HeadS[i]       = new Vec3(entityGeneticTag.HeadS.x,   entityGeneticTag.HeadS.y,   entityGeneticTag.HeadS.z);
            saveChunk.LimbFLS[i]     = new Vec3(entityGeneticTag.LimbFLS.x, entityGeneticTag.LimbFLS.y, entityGeneticTag.LimbFLS.z);
            saveChunk.LimbFRS[i]     = new Vec3(entityGeneticTag.LimbFRS.x, entityGeneticTag.LimbFRS.y, entityGeneticTag.LimbFRS.z);
            saveChunk.LimbRLS[i]     = new Vec3(entityGeneticTag.LimbRLS.x, entityGeneticTag.LimbRLS.y, entityGeneticTag.LimbRLS.z);
            saveChunk.LimbRRS[i]     = new Vec3(entityGeneticTag.LimbRRS.x, entityGeneticTag.LimbRRS.y, entityGeneticTag.LimbRRS.z);
            
            saveChunk.BodyC[i]       = new Vec3(entityGeneticTag.BodyC.x,   entityGeneticTag.BodyC.y,   entityGeneticTag.BodyC.z);
            saveChunk.HeadC[i]       = new Vec3(entityGeneticTag.HeadC.x,   entityGeneticTag.HeadC.y,   entityGeneticTag.HeadC.z);
            saveChunk.LimbFLC[i]     = new Vec3(entityGeneticTag.LimbFLC.x, entityGeneticTag.LimbFLC.y, entityGeneticTag.LimbFLC.z);
            saveChunk.LimbFRC[i]     = new Vec3(entityGeneticTag.LimbFRC.x, entityGeneticTag.LimbFRC.y, entityGeneticTag.LimbFRC.z);
            saveChunk.LimbRLC[i]     = new Vec3(entityGeneticTag.LimbRLC.x, entityGeneticTag.LimbRLC.y, entityGeneticTag.LimbRLC.z);
            saveChunk.LimbRRC[i]     = new Vec3(entityGeneticTag.LimbRRC.x, entityGeneticTag.LimbRRC.y, entityGeneticTag.LimbRRC.z);
            
            }
            
        }
        
        return true;
	}
	
	
	
	
	
	public bool chunkLoadStart(GameObject chunk, float chunk_x, float chunk_z) {
        
        if ((!isChunkLoaded) & (!isLoading)) {
            
            loadingX = chunk_x;
            loadingZ = chunk_z;
            loading_chunk = chunk;
            
            isChunkLoaded = false;
            isLoading     = true;
            isChunkFinalized = false;
            
            Thread threadLoad = new Thread( chunkThreadLoad );
            threadLoad.Start();
            
            return true;
        }
        
        return false;
	}
	
	
	
	public void chunkThreadLoad() {
        
        string chunkName = loadingX + "_" + loadingZ;
        
        fileStream_load = System.IO.File.Open(worldsPath + worldName + chunksPath + chunkName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        loadChunk = formatter.Deserialize(fileStream_load) as ChunkData;
        fileStream_load.Close();
        
        isChunkLoaded = true;
        isLoading     = false;
        
        return;
	}
	
	
	
	
	
	
	
	
	
	public void chunkBuild() {
        
        isChunkLoaded = false;
        isLoading     = false;
        isChunkFinalized = true;
        
        //float chunk_x    = loadingX;
        //float chunk_z    = loadingZ;
        //GameObject chunk = loading_chunk;
        
        // Setup chunk biome type
        ChunkTag chunkTag = loading_chunk.transform.GetChild(0).GetComponent<ChunkTag>();
        chunkTag.biome = loadChunk.biome;
        chunkTag.isLoaded = true;
        
        // Generate mesh
        //GameObject chunkMeshObject = chunk.transform.GetChild(0).gameObject;
        //Mesh       chunkMesh       = chunk.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh;
        //Renderer   chunkRenderer   = chunkMeshObject.GetComponent<Renderer>();
        
        //MeshCollider chunkMeshCollider = chunkMeshObject.GetComponent<MeshCollider>();
        
        
        // Hard generation values
        
        //int[]  index_array  = new int[ (101) * (101) * 6 ];
        for (int z=0; z <= 100; z++) {
            
            for (int x=0; x <= 100; x++) {
                
                float   vertex = loadChunk.vertexGrid[x, z];
                Vec3    color  = loadChunk.colorGrid[x, z];
                
                chunkTag.vertex_grid[x, z] = vertex;
                chunkTag.color_grid[x, z]  = new Vector3(color.x, color.y, color.z);
                
            }
            
        }
        
        chunkTag.update_mesh();
        chunkTag.update_color();
        
        return;
	}
	
	
	
	
	
	
	
	
	public void chunkFinalize() {
        
        isChunkFinalized = false;
        
        ChunkTag chunkTag = loading_chunk.transform.GetChild(0).GetComponent<ChunkTag>();
        chunkTag.isLoaded = true;
        
        GameObject newEntitiesList = loading_chunk.transform.GetChild(1).transform.gameObject;
        
        
        // Get tree log base material
        //Renderer logMeshRenderer = gameRules.transform.GetChild(chunkTag.biome).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Renderer>();
        //Material mat_log  = logMeshRenderer.material;
        
        
        // Get tree leaf base material
        //Renderer leafMeshRenderer = gameRules.transform.GetChild(chunkTag.biome).transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Renderer>();
        //Material mat_leaf = leafMeshRenderer.material;
        
        
        
        // Load static objects
        for (int i=0; i < loadChunk.staticCount; i++) {
            
            GameObject newStatic = MonoBehaviour.Instantiate( Resources.Load( loadChunk.staticName[i] )) as GameObject;
            newStatic.name             = loadChunk.staticName[i];
            newStatic.transform.parent = loading_chunk.transform.GetChild(2).gameObject.transform;
            
            newStatic.transform.Translate(loadChunk.staticPosition[i].x, loadChunk.staticPosition[i].y, loadChunk.staticPosition[i].z);
            
            Vector3 rotation = new Vector3(loadChunk.staticRotation[i].x, loadChunk.staticRotation[i].y, loadChunk.staticRotation[i].z);
            newStatic.transform.localRotation = Quaternion.Euler(rotation);
            
            newStatic.transform.localScale = new Vector3(loadChunk.staticScale[i].x, loadChunk.staticScale[i].y, loadChunk.staticScale[i].z);
            
            //newStatic.SetActive(false);
            
            
            
            if (newStatic.name == "log") {
            
            Renderer staticMeshRenderer = newStatic.GetComponent<Renderer>();
            if (staticMeshRenderer == null) continue;
            
            //staticMeshRenderer.material = mat_log;
            
            }
            
            if (newStatic.name == "leaves") {
            
            Renderer staticMeshRenderer = newStatic.GetComponent<Renderer>();
            if (staticMeshRenderer == null) continue;
            
            //staticMeshRenderer.material = mat_leaf;
            
            }
            
        }
        
        
        
        // Load dynamic entities
        for (int i=0; i < loadChunk.entityCount; i++) {
            
            // Generate an entity
            GameObject newEntity = MonoBehaviour.Instantiate( Resources.Load( loadChunk.entityName[i] )) as GameObject;
            if (newEntity == null) 
                continue;
            
            newEntity.SetActive(false);
            
            newEntity.name = loadChunk.entityName[i];
            newEntity.transform.Translate(new Vector3(loadChunk.entityPosition[i].x, loadChunk.entityPosition[i].y, loadChunk.entityPosition[i].z));
            newEntity.transform.localRotation = Quaternion.Euler( new Vector3(loadChunk.entityRotation[i].x, loadChunk.entityRotation[i].y, loadChunk.entityRotation[i].z) );
            
            newEntity.transform.parent = newEntitiesList.transform;
            
            //
            // Tag data
            EntityTag entityTag   = newEntity.gameObject.GetComponent<EntityTag>();
            
            entityTag.Name        = loadChunk.entityName[i];
            entityTag.Health      = loadChunk.Health[i];
            entityTag.Armor       = loadChunk.Armor[i];
            entityTag.Age         = loadChunk.Age[i];
            entityTag.Team        = loadChunk.Team[i];
            entityTag.Speed       = loadChunk.Speed[i];
            entityTag.isGenetic   = loadChunk.isGenetic[i];
            entityTag.useAI       = loadChunk.useAI[i];
            
            // AI
            ActorTag actorTag = newEntity.gameObject.GetComponent<ActorTag>();
            actorTag.AI_initiate();
            
            actorTag.chanceToChangeDirection = loadChunk.chanceToChangeDirection[i];
            actorTag.chanceToWalk            = loadChunk.chanceToWalk[i];
            actorTag.chanceToFocusOnPlayer   = loadChunk.chanceToFocusOnPlayer[i];
            actorTag.chanceToFocusOnEntity   = loadChunk.chanceToFocusOnEntity[i];
            actorTag.chanceToAttackPlayer    = loadChunk.chanceToAttackPlayer[i];
            
            actorTag.distanceToFocusOnPlayer = loadChunk.distanceToFocusOnPlayer[i];
            actorTag.distanceToFocusOnEntity = loadChunk.distanceToFocusOnEntity[i];
            actorTag.distanceToAttackPlayer  = loadChunk.distanceToAttackPlayer[i];
            actorTag.distanceToWalk          = loadChunk.distanceToWalk[i];
            
            actorTag.heightPreferenceMin     = loadChunk.heightPreferenceMin[i];
            actorTag.heightPreferenceMax     = loadChunk.heightPreferenceMax[i];
            
            actorTag.distanceToAttack        = loadChunk.distanceToAttack[i];
            actorTag.distanceToFlee          = loadChunk.distanceToFlee[i];
            
            // Emotional state
            actorTag.love                    = loadChunk.love[i];
            actorTag.fear                    = loadChunk.fear[i];
            actorTag.stress                  = loadChunk.stress[i];
            actorTag.hunger                  = loadChunk.hunger[i];
            
            // Animation
            actorTag.limbAxis            = loadChunk.limbAxis[i];
            actorTag.limbAxisInvert      = loadChunk.limbAxisInvert[i];
            actorTag.limbCycleRate       = loadChunk.limbCycleRate[i];
            actorTag.limbCycleRange      = loadChunk.limbCycleRange[i];
            
            actorTag.consumptionTimer     = loadChunk.consumptionTimer[i];
            
            
            
            // Genetic state
            if (entityTag.isGenetic) {
            
            GeneTag entityGeneticTag = newEntity.gameObject.GetComponent<GeneTag>();
            
            entityGeneticTag.AdultSizeMul  = loadChunk.AdultSizeMul[i];
            
            entityGeneticTag.BodyO         = new Vector3(loadChunk.BodyO[i].x,   loadChunk.BodyO[i].y,   loadChunk.BodyO[i].z);
            entityGeneticTag.HeadO         = new Vector3(loadChunk.HeadO[i].x,   loadChunk.HeadO[i].y,   loadChunk.HeadO[i].z);
            entityGeneticTag.LimbFLO       = new Vector3(loadChunk.LimbFLO[i].x, loadChunk.LimbFLO[i].y, loadChunk.LimbFLO[i].z);
            entityGeneticTag.LimbFRO       = new Vector3(loadChunk.LimbFRO[i].x, loadChunk.LimbFRO[i].y, loadChunk.LimbFRO[i].z);
            entityGeneticTag.LimbRLO       = new Vector3(loadChunk.LimbRLO[i].x, loadChunk.LimbRLO[i].y, loadChunk.LimbRLO[i].z);
            entityGeneticTag.LimbRRO       = new Vector3(loadChunk.LimbRRO[i].x, loadChunk.LimbRRO[i].y, loadChunk.LimbRRO[i].z);
            
            entityGeneticTag.BodyP         = new Vector3(loadChunk.BodyP[i].x,   loadChunk.BodyP[i].y,   loadChunk.BodyP[i].z);
            entityGeneticTag.HeadP         = new Vector3(loadChunk.HeadP[i].x,   loadChunk.HeadP[i].y,   loadChunk.HeadP[i].z);
            entityGeneticTag.LimbFLP       = new Vector3(loadChunk.LimbFLP[i].x, loadChunk.LimbFLP[i].y, loadChunk.LimbFLP[i].z);
            entityGeneticTag.LimbFRP       = new Vector3(loadChunk.LimbFRP[i].x, loadChunk.LimbFRP[i].y, loadChunk.LimbFRP[i].z);
            entityGeneticTag.LimbRLP       = new Vector3(loadChunk.LimbRLP[i].x, loadChunk.LimbRLP[i].y, loadChunk.LimbRLP[i].z);
            entityGeneticTag.LimbRRP       = new Vector3(loadChunk.LimbRRP[i].x, loadChunk.LimbRRP[i].y, loadChunk.LimbRRP[i].z);
            
            entityGeneticTag.BodyR         = new Vector3(loadChunk.BodyR[i].x,   loadChunk.BodyR[i].y,   loadChunk.BodyR[i].z);
            entityGeneticTag.HeadR         = new Vector3(loadChunk.HeadR[i].x,   loadChunk.HeadR[i].y,   loadChunk.HeadR[i].z);
            entityGeneticTag.LimbFLR       = new Vector3(loadChunk.LimbFLR[i].x, loadChunk.LimbFLR[i].y, loadChunk.LimbFLR[i].z);
            entityGeneticTag.LimbFRR       = new Vector3(loadChunk.LimbFRR[i].x, loadChunk.LimbFRR[i].y, loadChunk.LimbFRR[i].z);
            entityGeneticTag.LimbRLR       = new Vector3(loadChunk.LimbRLR[i].x, loadChunk.LimbRLR[i].y, loadChunk.LimbRLR[i].z);
            entityGeneticTag.LimbRRR       = new Vector3(loadChunk.LimbRRR[i].x, loadChunk.LimbRRR[i].y, loadChunk.LimbRRR[i].z);
            
            entityGeneticTag.BodyS         = new Vector3(loadChunk.BodyS[i].x,   loadChunk.BodyS[i].y,   loadChunk.BodyS[i].z);
            entityGeneticTag.HeadS         = new Vector3(loadChunk.HeadS[i].x,   loadChunk.HeadS[i].y,   loadChunk.HeadS[i].z);
            entityGeneticTag.LimbFLS       = new Vector3(loadChunk.LimbFLS[i].x, loadChunk.LimbFLS[i].y, loadChunk.LimbFLS[i].z);
            entityGeneticTag.LimbFRS       = new Vector3(loadChunk.LimbFRS[i].x, loadChunk.LimbFRS[i].y, loadChunk.LimbFRS[i].z);
            entityGeneticTag.LimbRLS       = new Vector3(loadChunk.LimbRLS[i].x, loadChunk.LimbRLS[i].y, loadChunk.LimbRLS[i].z);
            entityGeneticTag.LimbRRS       = new Vector3(loadChunk.LimbRRS[i].x, loadChunk.LimbRRS[i].y, loadChunk.LimbRRS[i].z);
            
            entityGeneticTag.BodyC         = new Vector3(loadChunk.BodyC[i].x,   loadChunk.BodyC[i].y,   loadChunk.BodyC[i].z);
            entityGeneticTag.HeadC         = new Vector3(loadChunk.HeadC[i].x,   loadChunk.HeadC[i].y,   loadChunk.HeadC[i].z);
            entityGeneticTag.LimbFLC       = new Vector3(loadChunk.LimbFLC[i].x, loadChunk.LimbFLC[i].y, loadChunk.LimbFLC[i].z);
            entityGeneticTag.LimbFRC       = new Vector3(loadChunk.LimbFRC[i].x, loadChunk.LimbFRC[i].y, loadChunk.LimbFRC[i].z);
            entityGeneticTag.LimbRLC       = new Vector3(loadChunk.LimbRLC[i].x, loadChunk.LimbRLC[i].y, loadChunk.LimbRLC[i].z);
            entityGeneticTag.LimbRRC       = new Vector3(loadChunk.LimbRRC[i].x, loadChunk.LimbRRC[i].y, loadChunk.LimbRRC[i].z);
            
            // Update physical form from genetic markers
            entityGeneticTag.updateGenetics();
            
            }
            
        }
        
        loading_chunk = null;
        
        return;
	}
	
	
	
	
	
	
	
	public void saveWorldData() {
        
        if (!System.IO.Directory.Exists(worldsPath)) System.IO.Directory.CreateDirectory(worldsPath);
        
        TickUpdate gameRulesObject   = GameObject.Find("GameRules").GetComponent<TickUpdate>();
        GameObject playerObject      = GameObject.Find("Player");
        GameObject mainCamera        = GameObject.Find("Main Camera");
        MouseLook mouseLook          = mainCamera.GetComponent<MouseLook>();
        
        // Save inventory
        Inventory inventory       = GameObject.Find("HUD").GetComponent<Inventory>();
        Interface interfaceScript = GameObject.Find("HUD").GetComponent<Interface>();
        
        // Setup the save data
        WorldData worldData = new WorldData();
        
        worldData.seed         = gameRulesObject.worldSeed;
        worldData.sunAngle     = gameRulesObject.dayNightCycleCurrent;
        
        worldData.playerPosition = new Vec3(playerObject.transform.position.x, playerObject.transform.position.y, playerObject.transform.position.z);
        
        worldData.health      = inventory.health;
        worldData.hunger      = inventory.hunger;
        worldData.saturation  = inventory.saturation;
        
        worldData.facingPitch = mouseLook.xRotation;
        worldData.facingYaw = playerObject.transform.rotation.eulerAngles.y;
        
        worldData.doDayNightCycle = gameRulesObject.doDayNightCycle;
        worldData.doWeatherCycle  = gameRulesObject.doWeatherCycle;
        
        worldData.TickCounter     = gameRulesObject.TickCounter;
        worldData.TickRate        = gameRulesObject.TickRate;
        
        // Save inventory state
        worldData.inv_selector = interfaceScript.selectedSlot;
        
        for (int i=0; i < 8; i++) {
            worldData.inv_name[i]  = inventory.Name[i];
            worldData.inv_count[i] = inventory.Stack[i];
        }
        
        // Setup the version data
        WorldVersion worldVersion = new WorldVersion();
        worldVersion.clientVersion = clientVersion;
        
        // Save world data
        fileStream_save = System.IO.File.Open(worldsPath + worldName + "/world", System.IO.FileMode.Create, System.IO.FileAccess.Write);
        
        formatter.Serialize(fileStream_save, worldData);
        fileStream_save.Close();
        
        
        // Version data
        fileStream_save = System.IO.File.Open(worldsPath + worldName + "/version", System.IO.FileMode.Create, System.IO.FileAccess.Write);
        
        formatter.Serialize(fileStream_save, worldVersion);
        fileStream_save.Close();
        
	}
	
	
	
	
	public bool loadWorldData() {
        
        if (!System.IO.File.Exists(worldsPath + worldName + "/world")) return false;
        
        // Check world client version
        WorldVersion worldVersion = new WorldVersion();
        
        fileStream_load = System.IO.File.Open(worldsPath + worldName + "/version", System.IO.FileMode.Open, System.IO.FileAccess.Read);
        
        worldVersion = formatter.Deserialize(fileStream_load) as WorldVersion;
        
        if (worldVersion.clientVersion != clientVersion) return false;
        
        
        // Read world data
        WorldData worldData = new WorldData();
        
        fileStream_load = System.IO.File.Open(worldsPath + worldName + "/world", System.IO.FileMode.Open, System.IO.FileAccess.Read);
        
        worldData = formatter.Deserialize(fileStream_load) as WorldData;
        
        fileStream_load.Close();
        
        
        TickUpdate gameRulesObject   = GameObject.Find("GameRules").GetComponent<TickUpdate>();
        GameObject playerObject      = GameObject.Find("Player");
        GameObject mainCamera        = GameObject.Find("Main Camera");
        MouseLook mouseLook          = mainCamera.GetComponent<MouseLook>();
        
        
        // Load the world data
        gameRulesObject.worldSeed             = worldData.seed;
        gameRulesObject.dayNightCycleCurrent  = worldData.sunAngle;
        
        gameRulesObject.doDayNightCycle       = worldData.doDayNightCycle;
        gameRulesObject.doWeatherCycle        = worldData.doWeatherCycle;
        gameRulesObject.TickCounter           = worldData.TickCounter;
        gameRulesObject.TickRate              = worldData.TickRate;
        
        mouseLook.xRotation                   = worldData.facingPitch;
        
        // Load inventory
        Inventory inventory       = GameObject.Find("HUD").GetComponent<Inventory>();
        Interface interfaceScript = GameObject.Find("HUD").GetComponent<Interface>();
        
        inventory.clear();
        
        for (int i=0; i < 8; i++) {
            
            inventory.Name[i]  = worldData.inv_name[i];
            inventory.Stack[i] = worldData.inv_count[i];
            
            if (inventory.Name[i] != "") 
            inventory.State[i] = true;
            
        }
        
        inventory.updateInventory();
        
        // Update the inventory
        interfaceScript.selectedSlot = worldData.inv_selector;
        interfaceScript.setSelection();
        interfaceScript.updateInHand();
        
        
        // Player stats
        inventory.setHealth(worldData.health);
        inventory.setHunger(worldData.hunger);
        inventory.setSaturation(worldData.saturation);
        
        // Place the player
        playerObject.transform.Translate(new Vector3(worldData.playerPosition.x, worldData.playerPosition.y, worldData.playerPosition.z));
        playerObject.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, worldData.facingYaw, 0.0f));
        
        return true;
	}
	
	
	
	
	
	public bool createWorld(string world_name) {
        
        if (!System.IO.Directory.Exists(worldsPath)) System.IO.Directory.CreateDirectory(worldsPath);
        
        if (!System.IO.Directory.Exists(worldsPath + world_name)) {
            System.IO.Directory.CreateDirectory(worldsPath + world_name);
            
            worldName = world_name;
            
            if (!System.IO.Directory.Exists(worldsPath + worldName + chunksPath)) System.IO.Directory.CreateDirectory(worldsPath + worldName + chunksPath);
            
            return true;
            
        } else {return false;}
        
	}
	
	
	
	public void destroyWorld(string world_name) {
        
        if (System.IO.Directory.Exists(worldsPath + world_name)) { System.IO.Directory.Delete(worldsPath + world_name, true); }
        
	}
	
	
	
	public bool openWorld(string world_name) {
        
        if (System.IO.Directory.Exists(worldsPath + world_name)) {
            
            worldName = world_name;
            
            return true;
            
        } else {return false;}
        
	}
	
	
	
	public void closeWorld() {
        
        for(int i = 0; i < chunkList.gameObject.transform.childCount; i++) {
            GameObject chunkObject = chunkList.gameObject.transform.GetChild(i).gameObject;
            MonoBehaviour.Destroy(chunkObject);
        }
        
	}
	
	
	
	
	public bool chunkExists(float chunk_x, float chunk_z) {
        
        if (System.IO.File.Exists(worldsPath + worldName + chunksPath + chunk_x + "_" + chunk_z)) {
            return true;
        } else {
            return false;
        }
        
	}
	
	
	GameObject getChunk(float chunk_x, float chunk_z) {
        
        string chunkName = chunk_x + "_" + chunk_z;
        
        for (int i=0; i < chunkList.gameObject.transform.childCount; i++) {
            
            string chunk_name = chunkList.gameObject.transform.GetChild(i).gameObject.name;
            
            if (chunk_name == chunkName) {
            
            return chunkList.gameObject.transform.GetChild(i).gameObject;
            }
            
        }
        
        return null;
	}
	
	
}


















