using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public TextMeshProUGUI textTimeremaining;
    public Image[] imageRewinds;
    public Image imageRewinding;

    public Sprite spriteRewindsUnused;
    public Sprite spriteRewindsUsed;

    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < imageRewinds.Length; i++)
            imageRewinds[i].enabled = i < GameController.main.levelRewindsMax;
    }

    // Update is called once per frame
    void Update() {
        textTimeremaining.text = Mathf.Round((GameController.main.levelTime - GameController.main.gameTime)).ToString();
        imageRewinding.enabled = GameController.main.gameState == GameController.GameState.Rewind;

        for (int i = 0; i < GameController.main.levelRewindsMax; i++) {
            if (GameController.main.levelRewindsLeft > i)
                imageRewinds[i].sprite = spriteRewindsUnused;
            else
                imageRewinds[i].sprite = spriteRewindsUsed;
        }
    }
}
