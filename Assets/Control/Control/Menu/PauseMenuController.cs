using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour {
	
	public GameObject Canvas;
	public TickUpdate tickUpdate;
	public GameObject mainMenu;
	
	
	public GameObject RootMenu;
	public GameObject SettingsMenu;
	
	
	public Slider  RenderDistanceSlider;
	public Text    RenderDistanceAmmount;
	
	public Slider  EntityDistanceSlider;
	public Text    EntityDistanceAmmount;
	
	public Slider  MainMenuRenderDistanceSlider;
	public Text    MainMenuRenderDistanceAmmount;
	
	public Slider  MainMenuEntityDistanceSlider;
	public Text    MainMenuEntityDistanceAmmount;
	
	
	
	
	
	
	
	
	
	void Update() {
	  
	  if (!tickUpdate.isPaused) return;
	  
	  if (Input.GetKeyDown(KeyCode.Escape)) {
	    
	    if (SettingsMenu.activeInHierarchy) 
	      mainMenu.GetComponent<MainMenu>().saveSettingsMenu();
	    
	  }
	  
	  int RenderDist = (int)RenderDistanceSlider.value + 1;
	  tickUpdate.RenderDistance = RenderDist;
	  RenderDistanceAmmount.text = RenderDist.ToString() + " chunks";
	  
	  
	  float EntityDist = EntityDistanceSlider.value;
	  tickUpdate.EntityDistance = EntityDist * 0.01f;
	  
	  EntityDistanceAmmount.text = EntityDist.ToString() + "%";
	  
	  MainMenuRenderDistanceAmmount.text = RenderDistanceAmmount.text;
	  MainMenuEntityDistanceAmmount.text = EntityDistanceAmmount.text;
	  
	  MainMenuRenderDistanceSlider.value = RenderDistanceSlider.value;
	  MainMenuEntityDistanceSlider.value = EntityDistanceSlider.value;
	  
	}
	
	
	
	void Start() {
	  
	  Canvas.SetActive(false);
	  Time.timeScale = 1;
	  
	  Cursor.lockState = CursorLockMode.None;
	  Cursor.visible = true;
	  
	  tickUpdate.isPaused = false;
	  
	}
	
	
	
	public void Activate() {
	  
	  Canvas.SetActive(true);
	  Time.timeScale = 0;
	  
	  RootMenu.SetActive(true);
	  
	  SettingsMenu.SetActive(false);
	  
	}
	
	
	
	public void Deactivate() {
	  
	  Canvas.SetActive(false);
	  Time.timeScale = 1;
	  
	  Cursor.lockState = CursorLockMode.Locked;
	  Cursor.visible = false;
	  
	  tickUpdate.isPaused = false;
	  
	}
	
	
	
	public void saveWorld() {
	  
	  tickUpdate.saveWorld();
	  
	  Cursor.lockState = CursorLockMode.Locked;
	  Cursor.visible = false;
	  
	  tickUpdate.isPaused = false;
	  
	}
	
	
	
	public void initiateSaveQuit() {
	  
	  // Save and clear the world
	  tickUpdate.saveWorld();
	  tickUpdate.purgeWorld();
	  tickUpdate.isPaused = false;
	  
	  // Reset the menu
	  mainMenu.SetActive(true);
	  
	  Destroy( GameObject.Find("Player") );
	  
	  // Disable the world
	  GameObject.Find("Control").SetActive(false);
	  GameObject.Find("World").SetActive(false);
	  
	  Cursor.lockState = CursorLockMode.None;
	  Cursor.visible = true;
	  
	}
	
	
	
	public void initiateQuitNoSave() {
	  
	  // Save and clear the world
	  tickUpdate.purgeWorld();
	  tickUpdate.isPaused = false;
	  
	  // Reset the menu
	  mainMenu.SetActive(true);
	  
	  Destroy(GameObject.Find("Player"));
	  
	  // Disable the world
	  GameObject.Find("Control").SetActive(false);
	  GameObject.Find("World").SetActive(false);
	  
	  Cursor.lockState = CursorLockMode.None;
	  Cursor.visible = true;
	  
	}
	
	
	
	
}
