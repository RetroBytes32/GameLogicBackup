using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class StructureItem {
    public string   name;
    public string   data;
    public Vector3  position;
    public Vector3  rotation;
    public Vector3  scale;
}

[System.Serializable]
public class StructureEntity {
    public string   name;
    public Vector3  position;
}





[System.Serializable]
public class Biome {
    
	public string name;
	public int    priority;
	
	[Space(10)]
	[Header("Biome region")]
	[Space(5)]
	
	public float  region_x;
	public float  region_z;
	public float  region_mul_x;
	public float  region_mul_y;
    public float  region_mul_z;
    
    
    
	[Space(10)]
	[Header("Terrain color")]
	[Space(5)]
	
	public Color  base_color;
	
	
	
	[Space(10)]
	[Header("Mountain height")]
	[Space(5)]
	
	public float  Mountain_rise;
	public float  Mountain_mult;
	
	public Color  Mountain_color;
	public float  Mountain_color_mult;
	
	
	
	[Space(10)]
	[Header("Mountain cap")]
	[Space(5)]
	
	public float  Mountain_cap;
	public float  Mountain_cap_mult;
	
	public Color  Mountain_cap_color;
	public float  Mountain_cap_color_mult;
	
	public float  Mountain_cap_cutoff;
	
	
	
	[Space(10)]
	[Header("Minimum biome height")]
	[Space(5)]
	
	public float  Maximum_terrain_height;
	public float  Minimum_terrain_height;
	
	
	
	[Space(10)]
	[Header("Noise octives")]
	[Space(5)]

	public PerlinOctive[]  octives;
	
	
	
	[Space(10)]
	[Header("Static objects")]
	[Space(5)]
	
	public StaticSpawn[]  staticList;
	
	
	
	[Space(10)]
	[Header("Dynamic entities")]
	[Space(5)]
	
	public DynamicSpawn[]  entities;
	
	
	
	[Space(10)]
	[Header("Tree generation")]
	[Space(5)]
	
	public TreeSpawn[]  treeSpawn;
	
	
}


[System.Serializable]
public class StaticSpawn {
    
	public string name;
    
	public float  height_range_min;
	public float  height_range_max;
    
	public int    density;
    
};


[System.Serializable]
public class TreeSpawn {
    
	[Space(10)]
	[Header("Trunk")]
	[Space(5)]
	
	public string name;
    
	public float  height_range_min;
	public float  height_range_max;
    
	public int    density;
    
	public float  stack_height_min;
	public float  stack_height_max;
    
	[Space(10)]
	[Header("Leafs")]
	[Space(5)]
	
    public string leafName;
    public Color  leafColor;
    
    public int    leafDensity;
    
    public float  leafHeight;
    public float  leafSpread;
    
    public float  leafHeightMul;
    public float  leafSpreadMul;
    
    public bool   scaleAndStagger;
    
};


[System.Serializable]
public class DynamicSpawn {
    
	public string name;
    
	public int Density;
    
	public float  height_range_min;
	public float  height_range_max;
    
	public Color[] ColorHead;
	public Color[] ColorBody;
	public Color[] ColorLimbs;
    
};


[System.Serializable]
public class PerlinOctive {
    
	public float amplitude;
	public float frequency;
    
	public float height;
    
}


[System.Serializable]
public class StructureBuild {
    public string structureName;
    
    public int    structureHeight;
    
    public List<StructureItem>    items;
    public List<StructureEntity>  entities;
}



[System.Serializable]
public class ChunkGeneration : MonoBehaviour {
    
	[Space(10)]
	[Header("Current world")]
	[Space(5)]
    
	public int      worldSeed;
    
	public int      currentChunkSeed;
    
	public bool     reloadWorldChunks = false;
    
    
    
	[Space(10)]
	[Header("Decorations")]
	[Space(5)]
    
	public bool     addWorldDecorations;
	public bool     addWorldEntities;
    public bool     generateFlatWorld;
    
    
    
	[Space(10)]
	[Header("World base noise")]
	[Space(5)]
    
	public float noise_x;
	public float noise_y;
	public float noise_z;
    
	public float noise_multiplier;
	public float noise_height;
    
	public PerlinOctive[]  octives;
    
    
	[Space(10)]
	[Header("Water table")]
	[Space(5)]
    
	public Color WaterColor;
	public Color MudColor;
	public float MudColorMult;
    
	public float waterDepthMin;
	public float waterDepthMult;
    
    
    
    
    
	[Space(50)]
	[Header("World biomes")]
	[Space(5)]
    
    public Biome[]   biomes;
    
    
    
    
    [Space(50)]
	[Header("Village structures")]
	[Space(5)]
    
    public float villageHeightMin         = 0f;
    public float villageHeightMax         = 30f;
    
    public float villageNoiseX            = 0.1f;
    public float villageNoiseZ            = 0.1f;
    public float villageNoiseHeightMul = 0f;
    public float villageNoiseThreshhold   = 0f;
    
    public int   chanceToGenerateVillage     = 20;
    public int   villageNumberOfBuilds       = 40;
    public float villageBuildRadius          = 50f;
    public float villageSpaceBetweenBuilds   = 10f;
    
    
    [Space(10)]
	[Header("Village entities")]
	[Space(5)]
    
    public int numberOfEntities   = 20;
    
    public string villageInhabitants;
    
    
    
    [Space(50)]
	[Header("Current structure")]
	[Space(5)]
    
    public bool doSavePlacedObjectsToStructure = false;
    
    public int  structureRotation;
    
    public string structureName;
    
    public int structureHeight;
    
    public List<StructureItem>    items;
    public List<StructureEntity>  entities;
    
    
    
    [Space(10)]
	[Header("Loaded world structures")]
	[Space(5)]
    
    public List<StructureBuild> worldStructures;
    
    
    
    
    
    [Space(50)]
	[Header("Internal")]
	[Space(5)]
    
	public GameObject ChunkList;
	public GameObject gameRules;
    
    public TickUpdate tickUpdate;
    
    
	int           updateCounter = 0;
    
	int[]         index_array_cache;
	Vector2[]     vertex_uv_cache;
    
	GameObject    currentChunk = null;
    
    
    
    //
    // Chunk generation core
    //
    
	public GameObject generateChunk(float chunk_x, float chunk_z, float chunk_sz, bool addWorldDecorations, bool dummyChunk) {
        
        GameObject oldChunk = currentChunk;
        
        // Generate a new chunk
        currentChunk = Instantiate( Resources.Load( "chunk" )) as GameObject;
        currentChunk.name = chunk_x + "_" + chunk_z;
        currentChunk.transform.parent = ChunkList.transform;
        ChunkTag chunkTag = currentChunk.transform.GetChild(0).gameObject.GetComponent<ChunkTag>();
        
        // Setup container objects
        GameObject newEntityContainer = new GameObject();
        newEntityContainer.name = "entities";
        newEntityContainer.transform.parent = currentChunk.transform;
        
        GameObject newStaticContainer = new GameObject();
        newStaticContainer.name = "static";
        newStaticContainer.transform.parent = currentChunk.transform;
        newStaticContainer.SetActive(false);
        
        // Calculate the chunk seed
        currentChunkSeed = worldSeed + ( (int)chunk_x + (int)chunk_z );
        Random.InitState( currentChunkSeed );
        
        
        // Get terrain mesh
        GameObject chunkMeshObject = currentChunk.transform.GetChild(0).gameObject;
        Mesh chunkMesh = currentChunk.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh;
        
        //Renderer chunkRenderer = chunkMeshObject.GetComponent<Renderer>();
        //MeshCollider chunkMeshCollider = chunkMeshObject.GetComponent<MeshCollider>();
        
        
        // Set chunk position
        chunkMeshObject.transform.Translate(-(chunk_sz/2), 0, -(chunk_sz/2));
        
        
        //
        // Generate water table
        
        GameObject waterTableObject = Instantiate( Resources.Load( "WaterTable" )) as GameObject;
        waterTableObject.transform.name = "water";
        waterTableObject.transform.parent = currentChunk.transform;
        waterTableObject.transform.position = new Vector3(currentChunk.transform.position.x, -0.7f, currentChunk.transform.position.z);
        
        waterTableObject.GetComponent<MeshRenderer>().material = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;
        
        
        // Generation values
        int array_size   = (101) * (101) * 6;
        
        int indexVertexArray = 0;
        int indexColorArray = 0;
        
        Vector3[]  vertex_array  = new Vector3[array_size];
        Color[]    color_array   = new Color  [array_size];
        float[,]   biome_array   = new float[101, 101];
        
        
        
        
        //
        // Generate temporary dummy chunk
        //
        
        if (dummyChunk) {
            
            chunkMesh.Clear();
            
            chunkMesh.vertices  = vertex_array;
            chunkMesh.triangles = index_array_cache;
            chunkMesh.colors    = color_array;
            chunkMesh.uv        = vertex_uv_cache;
            
            //chunkTag.update_mesh();
            //chunkTag.update_color();
            
            // Generate mesh collider
            //chunkMesh.RecalculateNormals();
            //chunkMeshCollider.sharedMesh = chunkMesh;
            
            // Scale and position
            currentChunk.transform.GetChild(0).localScale = new Vector3((100 * 0.01f), 1.0f, (100 * 0.01f));
            currentChunk.transform.Translate(new Vector3(chunk_x, 0.0f, chunk_z));
            
            return currentChunk;
        }
        
        
        
        //
        // Check generate a new village in the last chunk
        //
        
        if ((addWorldDecorations) & (Random.Range(0, chanceToGenerateVillage) == 0)) {
            
            if (oldChunk != null) 
                generateVillage(oldChunk, (chunk_x * villageNoiseX), (chunk_z * villageNoiseZ));
            
        }
        
        
        
        
        //
        // Terrain generation
        //
        
        float noise_total = 0;
        Color color_total = Color.white;
        
        Random.InitState( worldSeed );
        
        float x_offset = (Random.Range(1f, 99f) * 0.01f) - (Random.Range(1f, 99f) * 0.01f);
        float z_offset = (Random.Range(1f, 99f) * 0.01f) - (Random.Range(1f, 99f) * 0.01f);
        
        for (int z=0; z <= 100; z++) {
            
            for (int x=0; x <= 100; x++) {
                
                //
                // Perlin noise base generation
                //
                
                float xCoord = ((float)x * noise_x) + (chunk_x * noise_x) + x_offset + 100f;
                float zCoord = ((float)z * noise_z) + (chunk_z * noise_z) + z_offset + 100f;
                
                float perlinSample = Mathf.PerlinNoise(xCoord, zCoord);
                noise_total = ((perlinSample * noise_multiplier) + noise_y) * 2 - 1;
                
                
                if (noise_total > noise_height)
                noise_total = noise_height;
                
                
                //
                // Base perlin octives
                //
                
                if (octives.Length > 0) {
                    
                    for (int a=0; a < octives.Length; a++) {
                        
                        float noise_mod_x = noise_x * octives[a].frequency;
                        float noise_mod_z = noise_z * octives[a].frequency;
                        
                        xCoord = ((float)x * noise_mod_x) + (chunk_x * noise_mod_x) + x_offset;
                        zCoord = ((float)z * noise_mod_z) + (chunk_z * noise_mod_z) + z_offset;
                        
                        perlinSample = Mathf.PerlinNoise(xCoord, zCoord);
                        
                        noise_total += (((perlinSample * 2) - 1) * octives[a].amplitude) + octives[a].height;
                        
                    }
                    
                }
                
                
                
                //
                // Apply biome noise
                //
                
                int  biomeIndex = 0;
                
                if (biomes.Length > 0) {
                    
                    //
                    // Biome region assignment
                    //
                    
                    float biomeNoiseSample=0f;
                    
                    float biomePriority=0f;
                    float biomePrioritySample=0f;
                    
                    for (int a=0; a < biomes.Length; a++) {
                        
                        xCoord = (((float)x * biomes[a].region_mul_x) + (chunk_x * biomes[a].region_mul_x) + x_offset + biomes[a].region_x) + 100f;
                        zCoord = (((float)z * biomes[a].region_mul_z) + (chunk_z * biomes[a].region_mul_z) + z_offset + biomes[a].region_z) + 100f;
                        
                        float sample = ((Mathf.PerlinNoise(xCoord, zCoord) * 2) - 1) * biomes[a].region_mul_y;
                        
                        if (sample <= 0) 
                            continue;
                        
                        if (biomes[a].priority < biomePriority) 
                            continue;
                        
                        biomePriority       = biomes[a].priority;
                        biomePrioritySample = sample;
                        
                        biomeIndex = a;
                        
                    }
                    
                    biomeNoiseSample = biomePrioritySample;
                    
                    biome_array[x, z] = biomeIndex;
                    
                    
                    
                    //
                    // Assign biome color
                    //
                    
                    
                    color_total = biomes[biomeIndex].base_color;
                    
                    
                    
                    //
                    // Biome specific perlin octives
                    //
                    
                    float noise_sub_total = 0f;
                    
                    if (biomes[biomeIndex].octives.Length > 0) {
                        
                        for (int a=0; a < biomes[biomeIndex].octives.Length; a++) {
                        
                        float noise_mod_x = noise_x * biomes[biomeIndex].octives[a].frequency;
                        float noise_mod_z = noise_z * biomes[biomeIndex].octives[a].frequency;
                        
                        xCoord = ((float)x * noise_mod_x) + (chunk_x * noise_mod_x) + x_offset;
                        zCoord = ((float)z * noise_mod_z) + (chunk_z * noise_mod_z) + z_offset;
                        
                        perlinSample = Mathf.PerlinNoise(xCoord, zCoord);
                        float biome_sub = ((perlinSample * 2) - 1) * biomes[biomeIndex].octives[a].amplitude;
                        
                        noise_sub_total += biome_sub;
                        
                        }
                        
                    }
                    
                    
                    
                    //
                    // Mountain multiplier
                    //
                    
                    if (noise_sub_total > biomes[biomeIndex].Mountain_rise) {
                        
                        noise_sub_total += (noise_sub_total - biomes[biomeIndex].Mountain_rise) * (biomeNoiseSample * biomes[biomeIndex].Mountain_mult);
                        color_total = Color.Lerp(biomes[biomeIndex].base_color, biomes[biomeIndex].Mountain_color, (noise_sub_total - biomes[biomeIndex].Mountain_rise) * biomes[biomeIndex].Mountain_color_mult);
                        
                    }
                    
                    
                    //
                    // Mountain height cutoff
                    //
                    
                    if (biomes[biomeIndex].Mountain_cap > 0f) {
                        
                        if (noise_sub_total > biomes[biomeIndex].Mountain_cap) {
                        
                        noise_sub_total += (noise_sub_total - biomes[biomeIndex].Mountain_cap) * (biomeNoiseSample * biomes[biomeIndex].Mountain_cap_mult);
                        color_total = Color.Lerp(biomes[biomeIndex].Mountain_color, biomes[biomeIndex].Mountain_cap_color, (noise_sub_total - biomes[biomeIndex].Mountain_rise) * biomes[biomeIndex].Mountain_cap_color_mult);
                        
                        }
                        
                        
                        if (biomes[biomeIndex].Mountain_cap_cutoff > 0f)
                        if (noise_sub_total > biomes[biomeIndex].Mountain_cap_cutoff)
                            noise_sub_total = biomes[biomeIndex].Mountain_cap_cutoff;
                        
                    }
                    
                    
                    
                    //
                    // Far lands, perhaps??
                    //
                    
                    noise_total += noise_sub_total * biomeNoiseSample;
                    
                    if ((chunk_x > 999999) | (chunk_x < -999999) | 
                        (chunk_z > 999999) | (chunk_z < -999999)) {
                        
                        noise_total *= Random.Range(0, 13);
                    }
                    
                    
                } else {
                    
                    // Dead biome zone?
                    // This code should never run
                    
                    color_total = Color.white;
                    
                }
                
                
                
                
                //
                // Min / max biome height
                //
                
                if (noise_total < biomes[biomeIndex].Minimum_terrain_height) 
                    noise_total = biomes[biomeIndex].Minimum_terrain_height;
                
                if (noise_total > biomes[biomeIndex].Maximum_terrain_height) 
                    noise_total = biomes[biomeIndex].Maximum_terrain_height;
                
                
                
                
                //
                // Water table
                //
                if (!generateFlatWorld) {
                    if (noise_total < 0f)
                        color_total = Color.Lerp(MudColor, biomes[biomeIndex].base_color, 0.8f);
                    if (noise_total < -0.2f)
                        color_total = Color.Lerp(MudColor, biomes[biomeIndex].base_color, 0.6f);
                    if (noise_total < -0.5f)
                        color_total = Color.Lerp(MudColor, biomes[biomeIndex].base_color, 0.4f);
                    if (noise_total < -0.7f)
                        color_total = Color.Lerp(MudColor, biomes[biomeIndex].base_color, 0.2f);
                    if (noise_total < -1f)
                        color_total = Color.Lerp(MudColor, biomes[biomeIndex].base_color, 0.1f);
                }
                
                if (noise_total < 0f)
                    noise_total *= waterDepthMult;
                
                if (noise_total < waterDepthMin)
                    noise_total = waterDepthMin;
                
                
                
                //
                // Height stepping
                //
                
                noise_total = Mathf.Round(noise_total);
                
                
                
                
                //
                // Add static and dynamic biome specific objects
                //
                
                if (noise_total > 0f) {
                    
                    // Check flat world generation
                    if (generateFlatWorld) 
                        noise_total = 0f;
                    
                    float MountainCapHeight = biomes[biomeIndex].Mountain_cap;
                    
                    if (MountainCapHeight == 0f)
                        MountainCapHeight = 1000f;
                    
                    if (noise_total < MountainCapHeight) {
                        
                        if (addWorldDecorations) {
                            if (biomes[biomeIndex].staticList.Length > 0) 
                                generateStaticObject(x, noise_total, z, currentChunk, chunkTag, biomeIndex);
                            
                            if (biomes[biomeIndex].treeSpawn.Length > 0) 
                                generateTreeObject(x, noise_total, z, currentChunk, chunkTag, biomeIndex);
                        }
                        
                        if (addWorldEntities) 
                            generateDymanicEntity(x, noise_total, z, currentChunk, chunkTag, biomeIndex);
                        
                    }
                    
                    
                }
                
                
                //
                // Check flat world generation
                if (generateFlatWorld) {
                    // Flat noise
                    noise_total = 0f;
                    // Default biome
                    color_total = biomes[0].base_color;
                }
                
                
                //
                // Add random color variation
                
                float variation = 0.024f;
                float colorVar = Random.Range(0f, variation) - Random.Range(0f, variation);
                color_total.r += colorVar;
                color_total.g += colorVar;
                color_total.b += colorVar;
                
                
                
                //
                // Apply the vertex and color data to the buffer
                
                chunkTag.vertex_grid[x, z]  = noise_total;
                chunkTag.color_grid [x, z]  = new Vec3(color_total.r, color_total.g, color_total.b);
                
                vertex_array[indexVertexArray] = new Vector3(x, noise_total, z);
                color_array[indexColorArray]   = new Color(color_total.r, color_total.g, color_total.b, 1f);
                
                indexVertexArray++;
                indexColorArray++;
                
                chunkTag.isLoaded = true;
                
                continue;
            }
            
            continue;
        }
        
        
        
        
        
        
        //
        // Finalize chunk buffer data
        
        chunkMesh.Clear();
        
        chunkMesh.vertices  = vertex_array;
        chunkMesh.triangles = index_array_cache;
        chunkMesh.colors    = color_array;
        chunkMesh.uv        = vertex_uv_cache;
        
        
        
        
        
        //chunkTag.update_mesh();
        //chunkTag.update_color();
        
        // Generate mesh collider
        //chunkMesh.RecalculateNormals();
        //chunkMeshCollider.sharedMesh = chunkMesh;
        
        
        //
        Mesh mesh_base = currentChunk.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        MeshCollider coll_base = currentChunk.transform.GetChild(0).GetComponent<MeshCollider>();
        
        // Upload mesh data
        mesh_base.vertices  = vertex_array;
        mesh_base.RecalculateNormals();
        coll_base.sharedMesh = mesh_base;
        
        // Upload color data
        mesh_base.colors = color_array;
        
        
        
        
        
        
        
        // Scale and position
        float chunkScaleX = (100 * 0.01f);
        float chunkScaleZ = (100 * 0.01f);
        currentChunk.transform.GetChild(0).localScale = new Vector3(chunkScaleX, 1.0f, chunkScaleZ);
        
        currentChunk.transform.Translate(new Vector3(chunk_x, 0.0f, chunk_z));
        
        
        return currentChunk;
	}
	
	
	
	
	
	
	
	
	
	
	
	public void initiate() {
        
        //
        // Generate biome material groups
        //
        
        // Water material
        GameObject waterBase  = MonoBehaviour.Instantiate( Resources.Load( "WaterTable" )) as GameObject;
        Renderer   waterBaseRenderer = waterBase.GetComponent<Renderer>();
        waterBase.SetActive(false);
        
        waterBase.transform.parent  = gameRules.transform;
        waterBase.transform.position = new Vector3(0f, -1000f, 0f);
        
        waterBase.name      = "water";
        Material mat_water  = waterBaseRenderer.material;
        mat_water.color     = WaterColor;
        
        
        //
        // Create biome materials for material batching
        
        /*
        for (int i=0; i < biomes.Length; i++) {
            
            GameObject biomeObject = new GameObject();
            biomeObject.name = biomes[i].name;
            biomeObject.transform.parent = gameRules.transform;
            
            //for (int a=0; a < biomes[i].treeSpawn.Length; a++) {
                
                //GameObject entityObject = new GameObject();
                //entityObject.name = biomes[i].treeSpawn[a].name;
                //entityObject.transform.parent = biomeObject.transform;
                
                //leafColor
                
            //}
            
        }
        */
        
        
        // Initiate cached arrays
        int array_size = (101) * (101) * 6;
        index_array_cache = new int[array_size];
        vertex_uv_cache   = new Vector2[array_size];
        
        
        // Generate index array
        int vert=0;
        int tris=0;
        
        for (int z=0; z < 100; z++) {
            
            for (int x=0; x < 100; x++) {
                
                index_array_cache[tris]   = vert + 0;
                index_array_cache[tris+1] = vert + 100 + 1;
                index_array_cache[tris+2] = vert + 1;
                
                index_array_cache[tris+3] = vert + 1;
                index_array_cache[tris+4] = vert + 100 + 1;
                index_array_cache[tris+5] = vert + 100 + 2;
                
                vert++;
                tris+=6;
                
            }
            
            
            vert++;
        }
        
        // Generate UV array
        int index=0;
        for (int u=0; u < 100; u++) {
            
            for (int v=0; v < 100; v++) {
                
                vertex_uv_cache[index] = new Vector2( v/100, u/100 );
                
                index++;
                
            }
        }
        
        
        return;
	}
    
    
    
    
    
    
    //
    // Generate a village in this chunk
    
    public void generateVillage(GameObject chunk, float xCoordVillage, float zCoordVillage) {
        
        float perlinSample = Mathf.PerlinNoise(xCoordVillage, zCoordVillage) * villageNoiseHeightMul;
        
        if (perlinSample < villageNoiseThreshhold) 
            return;
        
        // Structure placement map
        List<Vector3> buildList = new List<Vector3>();
        
        //
        // Generate village buildings
        int randNumberOfFeatures = Random.Range((villageNumberOfBuilds / 2), villageNumberOfBuilds);
        for (int i=0; i < randNumberOfFeatures; i++) {
            
            // Random position
            Vector3 featurePosition = new Vector3(0f, 0f, 0f);
            featurePosition.x = Mathf.Round(Random.Range(-villageBuildRadius, villageBuildRadius));
            featurePosition.y = 500f;
            featurePosition.z = Mathf.Round(Random.Range(-villageBuildRadius, villageBuildRadius));
            
            featurePosition += chunk.transform.position;
            
            // Ray cast the height
            RaycastHit hit_obj;
            Ray ray = new Ray(featurePosition, -Vector3.up);
            
            if (!Physics.Raycast(ray, out hit_obj, 1000f, LayerMask.GetMask("Ground"))) 
                continue;
            
            featurePosition.y = Mathf.Round(hit_obj.point.y);
            
            // Village height preference
            if ((featurePosition.y < villageHeightMin) | (featurePosition.y > villageHeightMax)) 
                continue;
            
            // Random rotation
            structureRotation = 0;
            for (int a=0; a < Random.Range(0, 4); a++) 
                structureRotation += 90;
            
            int randomStructureIndex = Random.Range(0, worldStructures.Count);
            string randomStructure = worldStructures[ randomStructureIndex ].structureName;
            
            // Pass if the build spot is taken
            bool canBuildHere = true;
            for (int a=0; a < buildList.Count; a++) {
                
                // Check build distance
                if (Vector3.Distance(buildList[a], featurePosition) > villageSpaceBetweenBuilds) 
                    continue;
                
                canBuildHere = false;
                break;
            }
            
            if (!canBuildHere)
                continue;
            
            // Mark build position on the map
            buildList.Add( featurePosition );
            
            tickUpdate.placeStructureInWorld(randomStructure, featurePosition);
        }
        
        if (!addWorldEntities) 
            return;
        
        //
        // Generate village entities
        int spawnCount=0;
        for (int i=0; i < numberOfEntities; i++) {
            
            // Random position
            Vector3 entityPosition = new Vector3(0f, 0f, 0f);
            entityPosition.x = Mathf.Round(Random.Range(0f, villageBuildRadius) - Random.Range(0f, villageBuildRadius));
            entityPosition.y = 500f;
            entityPosition.z = Mathf.Round(Random.Range(0f, villageBuildRadius) - Random.Range(0f, villageBuildRadius));
            
            entityPosition += chunk.transform.position;
            
            // Ray cast the height
            RaycastHit hit_obj;
            Ray ray = new Ray(entityPosition, -Vector3.up);
            
            if (!Physics.Raycast(ray, out hit_obj, 1000f, LayerMask.GetMask("Ground"))) 
                continue;
            
            entityPosition.y = Mathf.Round(hit_obj.point.y);
            
            // Village height preference
            if ((entityPosition.y < villageHeightMin) | (entityPosition.y > villageHeightMax)) 
                continue;
            
            // Check if the spot is taken
            /*
            bool canBuildHere = true;
            for (int a=0; a < buildList.Count; a++) {
                
                // Check entity distance from build
                // so as not to spawn inside a build
                if (Vector3.Distance(buildList[a], entityPosition) > (villageSpaceBetweenBuilds * 0.5f)) 
                    continue;
                
                canBuildHere = false;
                break;
            }
            
            if (!canBuildHere) {
                //if (Random.Range(0, 1) == 1) 
                //    i--;
                
                if (i < (numberOfEntities-1))
                    if (spawnCount < (numberOfEntities/4)) i--;
                
                continue;
            }
            */
            
            GameObject newEntity = MonoBehaviour.Instantiate( Resources.Load( villageInhabitants )) as GameObject;
            
            newEntity.transform.position = entityPosition;
            
            newEntity.name = villageInhabitants;
            newEntity.transform.parent = chunk.transform.GetChild(1).gameObject.transform;
            
            ActorTag actorTag = newEntity.GetComponent<ActorTag>();
            string homeChunk = chunk.transform.position.x.ToString() + "." + chunk.transform.position.z.ToString();
            
            actorTag.addMemory("home", homeChunk);
            
            GeneTag geneticTag = newEntity.GetComponent<GeneTag>();
            geneticTag.updateGenetics();
            
            spawnCount++;
        }
        
        return;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
	public bool generateStaticObject(float x, float y, float z, GameObject chunk, ChunkTag chunkTag, int biomeIndex) {
        
        int static_type = Random.Range(0, biomes[biomeIndex].staticList.Length);
        
        if ((y < biomes[biomeIndex].staticList[static_type].height_range_min) | (y > biomes[biomeIndex].staticList[static_type].height_range_max))
            return false;
        
        if (Random.Range(0, 10000) > (biomes[biomeIndex].staticList[static_type].density))
            return false;
        
        GameObject staticObject;
        staticObject = Instantiate( Resources.Load( biomes[biomeIndex].staticList[static_type].name )) as GameObject;
        staticObject.name = biomes[biomeIndex].staticList[static_type].name;
        
        staticObject.transform.parent = chunk.transform.GetChild(2).transform;
        
        staticObject.transform.Translate(x - 50f, y + (staticObject.transform.localScale.y / 2.25f), z - 50f);
        
        /*
        // My sad attempt to change LOD levels
        LODGroup group = staticObject.GetComponent<LODGroup>();
        LOD[] newLOD = new LOD[2];
        newLOD[0] = new LOD(0.1f, null);
        //newLOD[1] = new LOD(0.07f, null);
        group.SetLODs(newLOD);
        group.RecalculateBounds();
        */
        
        return true;
	}
	
	
	
	
	
	
	public bool generateTreeObject(float x, float y, float z, GameObject chunk, ChunkTag chunkTag, int biomeIndex) {
        
        int tree_type = Random.Range(0, biomes[biomeIndex].treeSpawn.Length);
        
        if ((y < biomes[biomeIndex].treeSpawn[tree_type].height_range_min) | (y > biomes[biomeIndex].treeSpawn[tree_type].height_range_max))
            return false;
        
        if (Random.Range(0, 10000) > (biomes[biomeIndex].treeSpawn[tree_type].density))
            return false;
        
        float minStackingHeight = biomes[biomeIndex].treeSpawn[tree_type].stack_height_min;
        float maxStackingHeight = biomes[biomeIndex].treeSpawn[tree_type].stack_height_max;
        
        float stackHeight = Random.Range(minStackingHeight, maxStackingHeight);
        
        if ((maxStackingHeight == 0) & (minStackingHeight == 0)) 
            stackHeight = 1;
        
        // Generate tree trunk logs
        for (int i=0; i < stackHeight; i++) {
            
            GameObject staticObject;
            staticObject = Instantiate( Resources.Load( biomes[biomeIndex].treeSpawn[tree_type].name )) as GameObject;
            staticObject.name = biomes[biomeIndex].treeSpawn[tree_type].name;
            
            staticObject.transform.parent = chunk.transform.GetChild(2).transform;
            
            float stackOffset = staticObject.transform.localScale.y * i;
            
            staticObject.transform.Translate(x - 50f, y + (staticObject.transform.localScale.y / 2.25f) + stackOffset, z - 50f);
            
        }
        
        // Generate tree leaves
        for (int i=0; i < biomes[biomeIndex].treeSpawn[tree_type].leafDensity; i++) {
            
            GameObject staticObject;
            staticObject = Instantiate( Resources.Load( biomes[biomeIndex].treeSpawn[tree_type].leafName )) as GameObject;
            staticObject.name = biomes[biomeIndex].treeSpawn[tree_type].leafName;
            
            staticObject.transform.parent = chunk.transform.GetChild(2).transform;
            
            float stackOffset = stackHeight * 0.87f;
            
            float leafHeight    = biomes[biomeIndex].treeSpawn[tree_type].leafHeight;
            float leafSpread    = biomes[biomeIndex].treeSpawn[tree_type].leafSpread;
            
            float leafHeightMul = biomes[biomeIndex].treeSpawn[tree_type].leafHeightMul;
            float leafSpreadMul = biomes[biomeIndex].treeSpawn[tree_type].leafSpreadMul;
            
            float offsetX=0;
            float offsetY=0;
            float offsetZ=0;
            
            if (biomes[biomeIndex].treeSpawn[tree_type].scaleAndStagger) {
                
                // Translation offset from height
                offsetY = stackHeight + leafHeight + (i * leafHeightMul);
                
                staticObject.transform.Translate(x - 50f + offsetX, y + (staticObject.transform.localScale.y / 2.25f) + offsetY, z - 50f + offsetZ);
                
                // Random rotation
                float rotationX = 0f;
                float rotationY = Random.Range(0f, 360f) - Random.Range(0f, 360f);
                float rotationZ = 0f;
                
                staticObject.transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
                
                // Stagger the scale
                Vector3 newScale;
                newScale.x = leafSpread + staticObject.transform.localScale.x * (i * leafSpreadMul);
                newScale.y = 1f;
                newScale.z = leafSpread + staticObject.transform.localScale.z * (i * leafSpreadMul);
                
                staticObject.transform.localScale = newScale;
                
                if (staticObject.transform.childCount > 0) {
                    Vector3 hitboxScale;
                    hitboxScale.x = -newScale.x / 2f;
                    hitboxScale.y = -newScale.y;
                    hitboxScale.z = -newScale.z / 2f;
                    
                    staticObject.transform.GetChild(0).transform.localScale = hitboxScale;
                    staticObject.transform.GetChild(0).transform.Translate(0f, staticObject.transform.GetChild(0).transform.localScale.y * 2, 0f);
                }
                
            } else {
                
                // Random translation offset
                offsetX =  Random.Range(0f, leafSpreadMul) - Random.Range(0f, leafSpreadMul);
                offsetY = (Random.Range(0f, leafHeightMul) - Random.Range(0f, leafHeightMul)) + leafHeight + stackOffset;
                offsetZ =  Random.Range(0f, leafSpreadMul) - Random.Range(0f, leafSpreadMul);
                
                staticObject.transform.Translate(x - 50f + offsetX, y + (staticObject.transform.localScale.y / 2.25f) + offsetY, z - 50f + offsetZ);
                
                // Random rotation
                float rotationX = 0f;
                float rotationY = Random.Range(0f, 360f) - Random.Range(0f, 360f);
                float rotationZ = 0f;
                
                staticObject.transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
                
            }
            
        }
        
        return true;
	}
	
	
	
	
	
	
	
	
	public void generateDymanicEntity(float x, float y, float z, GameObject chunk, ChunkTag chunkTag, int biomeIndex) {
        
        if (biomes[biomeIndex].entities.Length == 0) return;
        
        int dynamic_type = Random.Range(0, biomes[biomeIndex].entities.Length);
        
        
        if (Random.Range(0, 100000) > biomes[biomeIndex].entities[dynamic_type].Density) return;
        
        if (biomes[biomeIndex].entities.Length == 0) return;
        
        if ((y < biomes[biomeIndex].entities[dynamic_type].height_range_min) | (y > biomes[biomeIndex].entities[dynamic_type].height_range_max))
            return;
        
        GameObject dynamicObject;
        dynamicObject = Instantiate( Resources.Load( biomes[biomeIndex].entities[dynamic_type].name )) as GameObject;
        dynamicObject.name = biomes[biomeIndex].entities[dynamic_type].name;
        dynamicObject.SetActive(false);
        
        //ActorTag actorTag = dynamicObject.GetComponent<ActorTag>();
        
        dynamicObject.transform.parent = chunk.transform.GetChild(1).gameObject.transform;
        
        dynamicObject.transform.position = new Vector3(x - 50f, y + 0.3f, z - 50f);
        GeneTag entityGene = dynamicObject.GetComponent<GeneTag>();
        
        if (biomes[biomeIndex].entities[dynamic_type].ColorBody.Length > 0) {
            
            int randBodyColor = Random.Range(0, biomes[biomeIndex].entities[dynamic_type].ColorBody.Length);
            
            entityGene.BodyC  = new Vector3(biomes[biomeIndex].entities[dynamic_type].ColorBody[randBodyColor].r,  biomes[biomeIndex].entities[dynamic_type].ColorBody[randBodyColor].g,  biomes[biomeIndex].entities[dynamic_type].ColorBody[randBodyColor].b);
            
        }
        
        if (biomes[biomeIndex].entities[dynamic_type].ColorHead.Length > 0) {
            
            int randHeadColor = Random.Range(0, biomes[biomeIndex].entities[dynamic_type].ColorHead.Length);
            
            entityGene.HeadC   = new Vector3(biomes[biomeIndex].entities[dynamic_type].ColorHead[randHeadColor].r,  biomes[biomeIndex].entities[dynamic_type].ColorHead[randHeadColor].g,  biomes[biomeIndex].entities[dynamic_type].ColorHead[randHeadColor].b);
            
        }
        
        if (biomes[biomeIndex].entities[dynamic_type].ColorLimbs.Length > 0) {
            
            int randLimbColor = Random.Range(0, biomes[biomeIndex].entities[dynamic_type].ColorLimbs.Length);
            
            entityGene.LimbFLC = new Vector3(biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].r, biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].g, biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].b);
            entityGene.LimbFRC = new Vector3(biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].r, biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].g, biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].b);
            entityGene.LimbRLC = new Vector3(biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].r, biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].g, biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].b);
            entityGene.LimbRRC = new Vector3(biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].r, biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].g, biomes[biomeIndex].entities[dynamic_type].ColorLimbs[randLimbColor].b);
            
        }
        
        entityGene.updateGenetics();
        entityGene.extractGeneticSequence();
        
        return;
	}
	
	
	
	void Update() {
        
        updateCounter++;
        if (updateCounter < 30) 
            return;
        
        updateCounter = 0;
        
        if (reloadWorldChunks) {
            reloadWorldChunks = false;
            
            for (int i=0; i < ChunkList.transform.childCount; i++) 
                Destroy( ChunkList.transform.GetChild(i).gameObject );
            
        }
        
	}
	
	
	
	public bool checkChunkFree(float chunk_x, float chunk_z) {

        string chunkName = chunk_x + "_" + chunk_z;
        
        for (int i=0; i < ChunkList.gameObject.transform.childCount; i++) {
            
            string chunk_name = ChunkList.gameObject.transform.GetChild(i).gameObject.name;
            
            if (chunk_name == chunkName) {
                
                return false;
            }
            
        }
        
        return true;
	}
    
    
    
    
    
	public GameObject getChunk(float chunk_x, float chunk_z) {
        
        string chunkName = chunk_x + "_" + chunk_z;
        
        for (int i=0; i < ChunkList.gameObject.transform.childCount; i++) {
            
            string chunk_name = ChunkList.gameObject.transform.GetChild(i).gameObject.name;
            
            if (chunk_name == chunkName) {
            
                return ChunkList.gameObject.transform.GetChild(i).gameObject;
            }
            
        }
        
        return null;
	}
    
    
    
    
    
}
















