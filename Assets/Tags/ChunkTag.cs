using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class ChunkTag : MonoBehaviour {
	
	public bool isLoaded = false;
	public bool shouldUpdateVoxels = false;
	
	public int biome = 0;
	
	public float[,]  vertex_grid = new float[101, 101];
	public Vec3[,]   color_grid  = new Vec3[101, 101];
	
	//public char[,,] voxel_grid = new char[101, 20, 101];
    
	
	
	public void update_mesh() {
        
        Vector3[] vertex_array = new Vector3[ 101 * 101 ];
        
        int index=0;
        for (int z=0; z <= 100; z++) {
            
            for (int x=0; x <= 100; x++) {
                
                float height = vertex_grid[x, z];
                
                vertex_array[index] = new Vector3(x, height, z);
                
                index++;
            }
            
        }
        
        
        // Upload new mesh data
        
        Mesh mesh_base = transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        MeshCollider coll_base = transform.GetChild(0).GetComponent<MeshCollider>();
        
        //mesh_base.Clear();
        mesh_base.vertices  = vertex_array;
        mesh_base.RecalculateNormals();
        
        // Generate mesh collider
        coll_base.sharedMesh = mesh_base;
        
        return;
	}
	
	
	
	
	public void update_color() {
        
        Color[] color_array  = new Color[101 * 101];
        
        int index=0;
        for (int z=0; z <= 100; z++) {
            
            for (int x=0; x <= 100; x++) {
                
                Vec3 value = color_grid[x, z];
                color_array[index] = new Color(value.x, value.y, value.z, 1f);
                
                index++;
            }
            
        }
        
        // Upload new mesh color data
        Mesh mesh_base = transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh;
        
        mesh_base.colors = color_array;
        
        return;
	}
	
}
