using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTag : MonoBehaviour {
    
    //   name=value;name=value;
    public string data;
    public int    lifeTime = -1;
    
    
    public void addTag(string name, string value) {
        if (data == "") {
            data = name +"="+ value;
        } else {
            data += ";"+name +"="+ value;
        }
    }
    
    public string getTag(string name) {
        
        string[] pairList = data.Split(';');
        
        if (pairList.Length == 0) {
            pairList = new string[1];
            pairList[0] = data;
        }
        
        for (int i=0; i < pairList.Length; i++) {
            
            string[] nameValueSet = pairList[i].Split('=');
            
            if (nameValueSet[0] != name) 
                continue;
            
            return nameValueSet[1];
        }
        
        return "";
    }
    
    public bool removeTag(string name) {
        
        string[] pairList = data.Split(';');
        
        if (pairList.Length == 0) {
            pairList = new string[1];
            pairList[0] = data;
        }
        
        clearTag();
        
        for (int i=0; i < pairList.Length; i++) {
            
            string[] nameValueSet = pairList[i].Split('=');
            
            if (nameValueSet[0] == name) 
                continue;
            
            addTag(nameValueSet[0], nameValueSet[1]);
        }
        
        return true;
    }
    
    public void clearTag() {
        data = "";
    }
    
}
