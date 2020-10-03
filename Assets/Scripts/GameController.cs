using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController main;

    public int levelRewindsMax;
    [HideInInspector]public int levelRewindsLeft;
    public float levelTime;

    public enum GameState { Play, Rewind, Paused };
    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState gameStatePrevious;
    [HideInInspector] public float gameTime;

    Player player;

    // Start is called before the first frame update
    void Start() {

        main = this;

        gameState = GameState.Play;
        gameTime = 0.0f;
        levelRewindsLeft = levelRewindsMax;

        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update() {
        gameStatePrevious = gameState;

        switch (gameState) {
            case GameState.Play:
                gameTime += Time.deltaTime;

                if (gameTime >= levelTime) {
                    gameState = GameState.Rewind;
                }
                break;

            case GameState.Rewind:
                gameTime -= Time.deltaTime * 3.0f;
                // Go back to play mode
                if (gameTime <= 0.0f && levelRewindsLeft > 0) {
                    gameTime = 0.0f;
                    gameState = GameState.Play;
                    player.gameObject.SetActive(true);
                    levelRewindsLeft--;
                }
                break;
        }
    }
}
