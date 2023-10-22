using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	
	[Space(10)]
    [Header("Save game slots")]
	[Space(5)]
    public int currentWorldsPage = 0;
    
	public TMPro.TextMeshProUGUI SaveSlot0;
	public TMPro.TextMeshProUGUI SaveSlot1;
	public TMPro.TextMeshProUGUI SaveSlot2;
	public TMPro.TextMeshProUGUI SaveSlot3;
	
	public TMPro.TextMeshProUGUI SaveSlot4;
	public TMPro.TextMeshProUGUI SaveSlot5;
	public TMPro.TextMeshProUGUI SaveSlot6;
	public TMPro.TextMeshProUGUI SaveSlot7;
	
	[Space(10)]
    [Header("New world generation")]
	[Space(5)]
    
	public InputField   NameInput;
	public InputField   SeedInput;
	
	public Toggle       addWorldDecorations;
	public Toggle       addWorldEntities;
	public Toggle       generateFlatWorld;
	
	
	[Space(10)]
    [Header("Render distance")]
	[Space(5)]
    
    public Slider  RenderDistanceSlider;
	public Text    RenderDistanceAmmount;
	public Slider  EntityDistanceSlider;
	public Text    EntityDistanceAmmount;
	
	
	[Space(10)]
    [Header("Pause menu")]
	[Space(5)]
    
    public Slider  PauseMenuRenderDistanceSlider;
	public Text    PauseMenuRenderDistanceAmmount;
	public Slider  PauseMenuEntityDistanceSlider;
	public Text    PauseMenuEntityDistanceAmmount;
	
	
    
	[Space(10)]
    [Header("Menu sounds")]
	[Space(5)]
    
    public AudioSource audioSource;
    public AudioClip   menu_hover;
    public AudioClip   menu_click;
    
    
    
	[Space(10)]
    [Header("Menu objects")]
	[Space(5)]
    
	public TMPro.TextMeshProUGUI WorldNameText;
	
	public TickUpdate tickUpdate;
	
	
	public GameObject RootMenu;
	
    public GameObject Panel;
	public GameObject BackPanel;
	
	public GameObject NewWorldMenu;
	public GameObject LoadWorldMenu;
	public GameObject SettingsMenu;
	public GameObject DestroyCheckMenu;
	
	public GameObject LoadButton;
	public GameObject DestroyButton;
	
	public GameObject LoadScrollUp;
	public GameObject LoadScrollDown;
	
	public GameObject PauseMenu;
	
	public GameObject canvasMainMenu;
	public GameObject canvasLoading;
	public GameObject canvasGenerating;
	
	public GameObject ControlObject;
	public GameObject WorldObject;
	
	public GameObject MainMenuObject;
	
	
	
	
	
	public int LoadCounter;
	
	public Material menuSkybox;
	
	public Text clientVersionText;
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	void Start() {
        
        QualitySettings.vSyncCount  = 0;
        Application.targetFrameRate = 30;
        
        tickUpdate.worldName = "";
        
        int major = 0;
        int minor = tickUpdate.clientVersion;
        while (minor > 99) {minor -= 100; major++;}
        
        
        if (minor < 10) {
            clientVersionText.text = major + ".0" + minor;
        } else {
            clientVersionText.text = major + "." + minor;
        }
        
        
        clearWorldList();
        checkSaveState();
        clearWorldNameText();
        
        canvasMainMenu.SetActive(true);
        
        PauseMenu.SetActive(true);
        
        LoadCounter = 0;
        
        
        // Initiate directories
        if (!System.IO.Directory.Exists("worlds")) System.IO.Directory.CreateDirectory("worlds");
        if (!System.IO.Directory.Exists("structures")) System.IO.Directory.CreateDirectory("structures");
        
        
        // Load settings menu state
        
        if (!System.IO.File.Exists("settings")) return;
        
        SettingsMenu settingsState = new SettingsMenu();
        
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter;
        formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        
        System.IO.FileStream fileStream_load = System.IO.File.Open("settings", System.IO.FileMode.Open, System.IO.FileAccess.Read);
        settingsState = formatter.Deserialize(fileStream_load) as SettingsMenu;
        fileStream_load.Close();
        
        tickUpdate.RenderDistance = settingsState.RenderDistance;
        tickUpdate.StaticDistance = settingsState.StaticDistance;
        tickUpdate.EntityDistance = settingsState.EntityDistance;
        
        
        RenderDistanceSlider.value = tickUpdate.RenderDistance - 1;
        RenderDistanceAmmount.text = tickUpdate.RenderDistance.ToString() + " chunks";
        
        
        EntityDistanceSlider.value = tickUpdate.EntityDistance;
        EntityDistanceAmmount.text = tickUpdate.EntityDistance.ToString() + "%";
        
        
	}
	
	
	
	
	
	
	
	
	void Update() {
        
        if (LoadWorldMenu.activeInHierarchy) {
            
            Vector2 scroll = Input.mouseScrollDelta;
            
            if (scroll.y ==  1) scrollWorldListUp();
            if (scroll.y == -1) scrollWorldListDown();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            
            RootMenu.SetActive(true);
            Panel.SetActive(true);
            BackPanel.SetActive(true);
            
            canvasMainMenu.SetActive(true);
            
            NewWorldMenu.SetActive(false);
            LoadWorldMenu.SetActive(false);
            
            if (SettingsMenu.activeInHierarchy) 
                saveSettingsMenu();
            
            SettingsMenu.SetActive(false);
            
            LoadButton.SetActive(false);
            DestroyButton.SetActive(false);
            
            DestroyCheckMenu.SetActive(false);
        }
        
        
        
        
        if (Input.GetKeyDown(KeyCode.Return)) {
            
            //
            // Enter new world
            
            if (NewWorldMenu.activeInHierarchy) {
                
                if (NameInput.text != "") {
                    
                    canvasMainMenu.    SetActive(false);
                    RootMenu.          SetActive(true);
                    canvasGenerating.  SetActive(true);
                    
                    NewWorldMenu.      SetActive(false);
                    
                    createWorld();
                    
                    setWorldSeed();
                    startLoadingWorld();
                    
                    return;
                }
                
            }
            
            
            
            //
            // Enter load world
            
            if (LoadWorldMenu.activeInHierarchy) {
                
                if (tickUpdate.worldName != "") {
                    
                    canvasMainMenu.  SetActive(false);
                    RootMenu.        SetActive(true);
                    canvasLoading.   SetActive(true);
                    
                    LoadWorldMenu.   SetActive(false);
                    LoadButton.      SetActive(false);
                    DestroyButton.   SetActive(false);
                    
                    setWorldSeed();
                    startLoadingWorld();
                    
                    return;
                }
                
            }
            
            
        }
        
        
        int RenderDist = (int)RenderDistanceSlider.value + 1;
        tickUpdate.RenderDistance = RenderDist;
        RenderDistanceAmmount.text = RenderDist.ToString() + " chunks";
        
        
        float EntityDist = EntityDistanceSlider.value;
        tickUpdate.EntityDistance = EntityDist * 0.01f;
        
        EntityDistanceAmmount.text = EntityDist.ToString() + "%";
        
        
        PauseMenuRenderDistanceSlider.value = RenderDistanceSlider.value;
        PauseMenuRenderDistanceAmmount.text = RenderDistanceAmmount.text;
        
        PauseMenuEntityDistanceSlider.value = EntityDistanceSlider.value;
        PauseMenuEntityDistanceAmmount.text = EntityDistanceAmmount.text;
        
        
        RenderSettings.skybox = menuSkybox;
        
        if (LoadCounter == 0) return;
            LoadCounter++;
        
        if (LoadCounter == 10) {
            
            tickUpdate.chunkGenerator.addWorldDecorations = addWorldDecorations.GetComponent<Toggle>().isOn;
            tickUpdate.chunkGenerator.addWorldEntities    = addWorldEntities.GetComponent<Toggle>().isOn;
            tickUpdate.chunkGenerator.generateFlatWorld   = generateFlatWorld.GetComponent<Toggle>().isOn;
            
            ControlObject.SetActive(true);
            WorldObject.SetActive(true);
            
            MainMenuObject.SetActive(false);
            
            canvasLoading.SetActive(false);
            canvasGenerating.SetActive(false);
            canvasMainMenu.SetActive(true);
            
            tickUpdate.initiateWorld();
            
            LoadCounter=0;
            
        }
        
        
	}
	
	
	
	
	
	
    public void playHoverSound() {
        audioSource.PlayOneShot(menu_hover);
    }
    
    public void playClickSound() {
        audioSource.PlayOneShot(menu_click);
    }
    
    
    
    
    
	
	public void Activate() {
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        tickUpdate.isPaused = false;
        
	}
	
	
	
	
	
	
	
	public void saveSettingsMenu() {
        
        SettingsMenu settingsState = new SettingsMenu();
        
        
        settingsState.RenderDistance = tickUpdate.RenderDistance;
        settingsState.StaticDistance = tickUpdate.StaticDistance;
        settingsState.EntityDistance = tickUpdate.EntityDistance * 100f;
        
        
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter;
        formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        
        System.IO.FileStream fileStream_save = System.IO.File.Open("settings", System.IO.FileMode.Create, System.IO.FileAccess.Write);
        formatter.Serialize(fileStream_save, settingsState);
        fileStream_save.Close();
        
	}
	
	
	
	public void startLoadingWorld() {
        
        LoadCounter = 1;
        
	}
	
	
	
	
	
	
	
	
	
	public void selectWorld0() {
        
        tickUpdate.worldName = SaveSlot0.text;
        checkSaveState();
        
	}
	
	public void selectWorld1() {
        
        tickUpdate.worldName = SaveSlot1.text;
        checkSaveState();
        
	}
	
	public void selectWorld2() {
        
        tickUpdate.worldName = SaveSlot2.text;
        checkSaveState();
        
	}
	
	public void selectWorld3() {
        
        tickUpdate.worldName = SaveSlot3.text;
        checkSaveState();
        
	}
	
	public void selectWorld4() {
        
        tickUpdate.worldName = SaveSlot4.text;
        checkSaveState();
        
	}
	
	public void selectWorld5() {
        
        tickUpdate.worldName = SaveSlot5.text;
        checkSaveState();
        
	}
	
	public void selectWorld6() {
        
        tickUpdate.worldName = SaveSlot6.text;
        checkSaveState();
        
	}
	
	public void selectWorld7() {
        
        tickUpdate.worldName = SaveSlot7.text;
        checkSaveState();
        
	}
	
	
	
	public void destroyInitiate() {
        
        WorldNameText.text = tickUpdate.worldName;
        
	}
	
	
	
	public void checkSaveState() {
        
        if (tickUpdate.worldName == "") {
            LoadButton.SetActive(false);
            DestroyButton.SetActive(false);
        } else {
            LoadButton.SetActive(true);
            DestroyButton.SetActive(true);
        }
        
        
	}
	
	
	
	public void destroyWorld() {
        
        if (System.IO.Directory.Exists("worlds/" + tickUpdate.worldName)) { System.IO.Directory.Delete("worlds/" + tickUpdate.worldName, true); }
        
        tickUpdate.worldName = "";
        
        updateWorldsList();
        
	}
	
	
	
	
	public void createWorld() {
        
        tickUpdate.worldName = NameInput.text;
        setWorldSeed();
        
	}
	
	public void updateWorldsList() {
        
        clearWorldList();
        
        string[] listOfWorlds = Directory.GetDirectories("worlds");
        int numberOfPages = listOfWorlds.Length;
        
        if (numberOfPages == 0) {
            LoadScrollDown.SetActive(false);
            LoadScrollUp.SetActive(false);
            currentWorldsPage=0;
            return;
        }
        
        LoadScrollDown.SetActive(true);
        LoadScrollUp.SetActive(true);
        
        if (currentWorldsPage < 1) {
            currentWorldsPage = 0;
            
            LoadScrollUp.SetActive(false);
        }
        
        if (numberOfPages > 7) {
            if (currentWorldsPage > numberOfPages-8) {
                currentWorldsPage = numberOfPages-8;
                
                LoadScrollDown.SetActive(false);
            }
        } else {
            LoadScrollDown.SetActive(false);
            LoadScrollUp.SetActive(false);
            currentWorldsPage=0;
        }
        
        
        
        int index = currentWorldsPage;
        
        if (listOfWorlds.Length > index+0) SaveSlot0.text = Path.GetFileNameWithoutExtension(listOfWorlds[index+0]);
        if (listOfWorlds.Length > index+1) SaveSlot1.text = Path.GetFileNameWithoutExtension(listOfWorlds[index+1]);
        if (listOfWorlds.Length > index+2) SaveSlot2.text = Path.GetFileNameWithoutExtension(listOfWorlds[index+2]);
        if (listOfWorlds.Length > index+3) SaveSlot3.text = Path.GetFileNameWithoutExtension(listOfWorlds[index+3]);
        if (listOfWorlds.Length > index+4) SaveSlot4.text = Path.GetFileNameWithoutExtension(listOfWorlds[index+4]);
        if (listOfWorlds.Length > index+5) SaveSlot5.text = Path.GetFileNameWithoutExtension(listOfWorlds[index+5]);
        if (listOfWorlds.Length > index+6) SaveSlot6.text = Path.GetFileNameWithoutExtension(listOfWorlds[index+6]);
        if (listOfWorlds.Length > index+7) SaveSlot7.text = Path.GetFileNameWithoutExtension(listOfWorlds[index+7]);
        
	}
	
	
	public void clearWorldList() {
        
        SaveSlot0.text = "";
        SaveSlot1.text = "";
        SaveSlot2.text = "";
        SaveSlot3.text = "";
        SaveSlot4.text = "";
        SaveSlot5.text = "";
        SaveSlot6.text = "";
        SaveSlot7.text = "";
        
        LoadButton.      SetActive(false);
        DestroyButton.   SetActive(false);
        
	}
	
	
	
	public void clearWorldNameText() {
        
        Random.InitState( (int)(System.DateTime.Now.Second * System.DateTime.Now.Minute * 100f) );
        int random_seed = Random.Range(100000000, 1000000000);
        
        // Find a good name
        NameInput.text = "New world";
        for (int i=0; i < 1000; i++) {
            
            if (!System.IO.Directory.Exists("worlds/" + NameInput.text)) 
                break;
            
            NameInput.text = "New world(" + (i+1) + ")";
        }
        
        SeedInput.text = random_seed.ToString();
        
        SeedInput.text = SeedInput.text.Replace("6", "7");
	}
	
	
	
	
	public void setWorldSeed() {
        
        int newWorldSeed = System.Convert.ToInt32( SeedInput.text );
        
        tickUpdate.worldSeed = newWorldSeed;
        tickUpdate.chunkGenerator.worldSeed = newWorldSeed;
        
	}
	
	
	
	public void quitGame() {
        
        Application.Quit();
        
	}
	
	
	
	
	public void scrollWorldListUp() {
        currentWorldsPage--;
        
        updateWorldsList();
    }
	
	
	
	
	public void scrollWorldListDown() {
        currentWorldsPage++;
        
        updateWorldsList();
    }
	
	
	
}


















