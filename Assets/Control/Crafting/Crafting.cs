using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Crafting : MonoBehaviour {
    
	[Space(10)]
	[Header("Crafting slots")]
	[Space(5)]
    
	public string[]       Name            = new string[9];
	public bool[]         State           = new bool[9];
	
	public int[]          Stack           = new int[9];
	public int[]          StackMax        = new int[9];
	public int[]          Durability      = new int[9];
	public string[]       Data            = new string[9];
	
	[Space(20)]
	
    public GameObject[]   crafting_slots  = new GameObject[9];
	public RawImage[]     slot_image      = new RawImage[9];
	public Text[]         slot_count      = new Text[9];
	
	
	[Space(10)]
	[Header("Crafting result")]
	[Space(5)]
    
	public GameObject   crafting_result;
	public RawImage     result_image;
	public Text         result_count;
	
	public bool         result_state = false;
	
	public string       result_name;
	public int          result_stack;
	public int          result_stackMax;
	public int          result_durability;
	public string       result_data;
	
}
