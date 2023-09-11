using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : MonoBehaviour {
    
    
	[Space(10)]
    [Header("Menu sounds")]
	[Space(5)]
    
    public AudioSource audioSource;
    public AudioClip   menu_hover;
    public AudioClip   menu_click;
    
    
    
    
    public void playHoverSound() {
        audioSource.PlayOneShot(menu_hover);
    }
    
    public void playClickSound() {
        audioSource.PlayOneShot(menu_click);
    }
    
    
}
