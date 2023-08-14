using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Vec3 {
	
	public float x, y, z;
	
	public Vec3(float xx, float yy, float zz) {
        x = xx;
        y = yy;
        z = zz;
	}
	
	public Vec3(Vector3 vec) {
        x = vec.x;
        y = vec.y;
        z = vec.z;
	}
	
}




[System.Serializable]
public class WorldVersion {
	
	public int clientVersion;
	
}





[System.Serializable]
public class SettingsMenu {
	
	public int   RenderDistance;
	public float StaticDistance;
	public float EntityDistance;
	
}



[System.Serializable]
public class WorldData {
	
	public int seed;
	
	public float sunAngle;
	
	public Vec3  playerPosition;
	
	public int  health;
	public int  hunger;
	public int  saturation;
	
	public float facingYaw;
	public float facingPitch;
	
	// World settings
	public bool doDayNightCycle;
	public bool doWeatherCycle;
	
	public int TickRate;
	public int TickCounter; // World up time
	
	// Inventory state
	public string[]  inv_name      = new string[8];
	public int[]     inv_count     = new int[8];
	public int       inv_selector;
	
}






[System.Serializable]
public class ChunkData {
	
	// Biome type
	public int        biome;
	
	// Terrain grid layout
	public float[,]   vertexGrid;
	public Vec3[,]    colorGrid;
	
	// Static chunk objects
	public int        staticCount;
	public string[]   staticName;
	
	public Vec3[]     staticPosition;
	public Vec3[]     staticRotation;
	public Vec3[]     staticScale;
	
	// Dynamic chunk entities
	public int        entityCount;
	public string[]   entityName;
	
	public Vec3[]     entityPosition;
	public Vec3[]     entityRotation;
	
	public float[]    Health;
	public float[]    Armor;
	
	public int[]      Age;
	public int[]      Team;
	public float[]    Speed;
	
	public bool[]     isGenetic;
	public bool[]     useAI;
	
	
	//
	// AI
	//
	
	public float[] chanceToChangeDirection;
	public float[] chanceToWalk;
	public float[] chanceToFocusOnPlayer;
	public float[] chanceToFocusOnEntity;
	public float[] chanceToAttackPlayer;
    
	public float[] distanceToFocusOnPlayer;
	public float[] distanceToFocusOnEntity;
	public float[] distanceToAttackPlayer;
	public float[] distanceToWalk;
    
    public float[] heightPreferenceMin;
	public float[] heightPreferenceMax;
    
	public float[] distanceToAttack;
	public float[] distanceToFlee;
    
    // Emotional state
    public int[] love;
	public int[] fear;
	public int[] stress;
	public int[] hunger;
	
    // Animation
	public int[]   limbAxis;
	public bool[]  limbAxisInvert;
	public float[] limbCycleRate;
	public float[] limbCycleRange;
	
	public int[]   consumptionTimer;
	
	
	//
	// Genetic expression
	//
	
	public float[] AdultSizeMul;
	
	public Vec3[]  BodyO;
	public Vec3[]  HeadO;
	public Vec3[]  LimbFLO;
	public Vec3[]  LimbFRO;
	public Vec3[]  LimbRLO;
	public Vec3[]  LimbRRO;
	
	public Vec3[]  BodyP;
	public Vec3[]  HeadP;
	public Vec3[]  LimbFLP;
	public Vec3[]  LimbFRP;
	public Vec3[]  LimbRLP;
	public Vec3[]  LimbRRP;
	
	public Vec3[]  BodyR;
	public Vec3[]  HeadR;
	public Vec3[]  LimbFLR;
	public Vec3[]  LimbFRR;
	public Vec3[]  LimbRLR;
	public Vec3[]  LimbRRR;
	
	public Vec3[]  BodyS;
	public Vec3[]  HeadS;
	public Vec3[]  LimbFLS;
	public Vec3[]  LimbFRS;
	public Vec3[]  LimbRLS;
	public Vec3[]  LimbRRS;
	
	public Vec3[]   BodyC;
	public Vec3[]   HeadC;
	public Vec3[]   LimbFLC;
	public Vec3[]   LimbFRC;
	public Vec3[]   LimbRLC;
	public Vec3[]   LimbRRC;
	
	
	
	
	public ChunkData(int static_count, int dynamic_count) {
        
        // Chunk biome type
        biome           = 0;
        
        // Terrain grid array
        vertexGrid      = new float[101, 101];
        colorGrid       = new Vec3 [101, 101];
        
        // Static objects
        staticCount     = static_count;
        staticName      = new string[staticCount];
        
        staticPosition  = new Vec3[staticCount];
        staticRotation  = new Vec3[staticCount];
        staticScale     = new Vec3[staticCount];
        
        
        // Dynamic entities
        entityCount     = dynamic_count;
        entityName      = new string[entityCount];
        
        entityPosition  = new Vec3[entityCount];
        entityRotation  = new Vec3[entityCount];
        
        Health          = new float[entityCount];
        Armor           = new float[entityCount];
        
        Age             = new int[entityCount];
        Team            = new int[entityCount];
        Speed           = new float[entityCount];
        
        isGenetic       = new bool[entityCount];
        useAI           = new bool[entityCount];
        
        
        // AI
        chanceToChangeDirection = new float[entityCount];
        chanceToWalk            = new float[entityCount];
        chanceToFocusOnPlayer   = new float[entityCount];
        chanceToFocusOnEntity   = new float[entityCount];
        chanceToAttackPlayer    = new float[entityCount];
        
        distanceToFocusOnPlayer = new float[entityCount];
        distanceToFocusOnEntity = new float[entityCount];
        distanceToAttackPlayer  = new float[entityCount];
        distanceToWalk          = new float[entityCount];
        
        heightPreferenceMin     = new float[entityCount];
        heightPreferenceMax     = new float[entityCount];
        
        distanceToAttack        = new float[entityCount];
        distanceToFlee          = new float[entityCount];
        
        // Emotional state
        love                    = new int[entityCount];
        fear                    = new int[entityCount];
        stress                  = new int[entityCount];
        hunger                  = new int[entityCount];
        
        
        // Animation
        limbAxis                = new int[entityCount];
        limbAxisInvert          = new bool[entityCount];
        limbCycleRate           = new float[entityCount];
        limbCycleRange          = new float[entityCount];
        
        consumptionTimer         = new int[entityCount];
        
        
        // Entity genetic expression
        AdultSizeMul   = new float[entityCount];
        
        BodyO      = new Vec3[entityCount];
        HeadO      = new Vec3[entityCount];
        LimbFLO    = new Vec3[entityCount];
        LimbFRO    = new Vec3[entityCount];
        LimbRLO    = new Vec3[entityCount];
        LimbRRO    = new Vec3[entityCount];
        
        BodyP      = new Vec3[entityCount];
        HeadP      = new Vec3[entityCount];
        LimbFLP    = new Vec3[entityCount];
        LimbFRP    = new Vec3[entityCount];
        LimbRLP    = new Vec3[entityCount];
        LimbRRP    = new Vec3[entityCount];
        
        BodyR      = new Vec3[entityCount];
        HeadR      = new Vec3[entityCount];
        LimbFLR    = new Vec3[entityCount];
        LimbFRR    = new Vec3[entityCount];
        LimbRLR    = new Vec3[entityCount];
        LimbRRR    = new Vec3[entityCount];
        
        BodyS      = new Vec3[entityCount];
        HeadS      = new Vec3[entityCount];
        LimbFLS    = new Vec3[entityCount];
        LimbFRS    = new Vec3[entityCount];
        LimbRLS    = new Vec3[entityCount];
        LimbRRS    = new Vec3[entityCount];
        
        BodyC      = new Vec3[entityCount];
        HeadC      = new Vec3[entityCount];
        LimbFLC    = new Vec3[entityCount];
        LimbFRC    = new Vec3[entityCount];
        LimbRLC    = new Vec3[entityCount];
        LimbRRC    = new Vec3[entityCount];
        
	}
	
	
	
};








