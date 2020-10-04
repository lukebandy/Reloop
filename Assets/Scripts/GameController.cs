using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController main;

    private AudioSource audioSource;

    public int levelRewindsMax;
    [HideInInspector]public int levelRewindsLeft;
    public float levelTime;

    public enum GameState { Play, Rewind };
    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState gameStatePrevious;
    [HideInInspector] public float gameTime;

    Player player;

    // Start is called before the first frame update
    void Start() {
        main = this;

        audioSource = GetComponent<AudioSource>();

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
                int gameTimeRemainingPrevious = Mathf.FloorToInt(levelTime - gameTime);

                gameTime += Time.deltaTime;

                int gameTimeRemaining = Mathf.FloorToInt(levelTime - gameTime);
                if (gameTimeRemaining != gameTimeRemainingPrevious) {
                    if (gameTimeRemaining == 0 || gameTimeRemaining == 1 || gameTimeRemaining == 2)
                        audioSource.Play();
                }

                if (gameTime >= levelTime) {
                    gameState = GameState.Rewind;
                }
                break;

            case GameState.Rewind:
                gameTime -= Time.deltaTime * 3.0f;
                // Go back to play mode
                if (gameTime <= 0.0f) {
                    // New loop
                    gameTime = 0.0f;
                    gameState = GameState.Play;
                    player.gameObject.SetActive(true);

                    if (levelRewindsLeft > 0) {
                        levelRewindsLeft--;
                    }
                    // Lost game
                    else {
                        foreach (PlayerGhost ghost in FindObjectsOfType<PlayerGhost>())
                            Destroy(ghost.gameObject);
                        levelRewindsLeft = levelRewindsMax;
                    }
                }
                break;
        }
    }
}
