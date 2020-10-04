using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

    public TextMeshProUGUI textTimeremaining;
    public Image[] imageRewinds;
    public Image imageRewinding;
    public TextMeshProUGUI textLevel;
    public Button buttonQuit;

    public Sprite spriteRewindsUnused;
    public Sprite spriteRewindsUsed;

    // Start is called before the first frame update
    void Start() {
        // TODO: Non-jam code for this pls
        if (GameController.main == null)
            GameController.main = FindObjectOfType<GameController>();

        for (int i = 0; i < imageRewinds.Length; i++)
            imageRewinds[i].enabled = i < GameController.main.levelRewindsMax;

        textLevel.text = "LEVEL " + SceneManager.GetActiveScene().buildIndex.ToString();

        buttonQuit.enabled = Application.platform != RuntimePlatform.WebGLPlayer;
    }

    // Update is called once per frame
    void Update() {
        textTimeremaining.text = "RELOOP IN " + Mathf.FloorToInt((GameController.main.levelTime - GameController.main.gameTime)).ToString() + " SECONDS";
        imageRewinding.enabled = GameController.main.gameState == GameController.GameState.Rewind;

        for (int i = 0; i < GameController.main.levelRewindsMax; i++) {
            if (GameController.main.levelRewindsLeft > i)
                imageRewinds[i].sprite = spriteRewindsUnused;
            else
                imageRewinds[i].sprite = spriteRewindsUsed;
        }
    }

    public void UIQuit() {
        Application.Quit();
    }
}
