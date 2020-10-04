using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public Button uiButtonQuit;

    // Start is called before the first frame update
    void Start() {
        //uiButtonQuit.enabled = Application.platform != RuntimePlatform.WebGLPlayer;
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void UIStart() {
        FindObjectOfType<LevelLoader>().LoadLevel(1);
    }

    public void UIQuit() {
        Application.Quit();
    }
}
