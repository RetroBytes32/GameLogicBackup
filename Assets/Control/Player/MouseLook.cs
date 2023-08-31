using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {
	
	public float mouseSensitivityX = 220.0f;
	public float mouseSensitivityY = 220.0f;
	
	public Transform playerBody;
	
	public float xRotation = 0.0f;
	
	public TickUpdate tickUpdate;
	
	
	
	void Update () {
        
        if (tickUpdate.doMouseLook == false) 
            return;
        
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;
        
        // Apply movement offset to the players orientation
        playerBody.Rotate(Vector3.up * mouseX);
        
        // Clamp the pitch angles
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);
        
        transform.localRotation = Quaternion.Euler(xRotation, 0f, tickUpdate.damageIndicationOffset);
        
	}
	
	void Start () {
	  
	  Cursor.lockState = CursorLockMode.Locked;
	  
	  tickUpdate = GameObject.Find("GameRules").GetComponent<TickUpdate>();
	  
	}
	
	
	
}
