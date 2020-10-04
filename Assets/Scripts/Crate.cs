using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour {

    private Rigidbody2D rb;
    private new Collider2D collider;
    private AudioSource audioSource;

    private int historyIndex;
    private List<float> historyTimestamp;
    private List<Vector3> historyPosition;
    private List<Quaternion> historyRotation;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        historyTimestamp = new List<float>();
        historyPosition = new List<Vector3>();
        historyRotation = new List<Quaternion>();
    }

    // Update is called once per frame
    void Update() {
        if (GameController.main.gameStatePrevious != GameController.main.gameState) {
            if (GameController.main.gameState == GameController.GameState.Play) {

                transform.position = historyPosition[0];
                transform.rotation = historyRotation[0];

                rb.isKinematic = false;
                collider.enabled = true;

                historyTimestamp = new List<float>();
                historyPosition = new List<Vector3>();
                historyRotation = new List<Quaternion>();
            }
            else if (GameController.main.gameState == GameController.GameState.Rewind) {
                rb.isKinematic = true;
                collider.enabled = false;

                historyIndex = historyTimestamp.Count - 1;
            }
        }

        if (GameController.main.gameState == GameController.GameState.Play) {
            historyTimestamp.Add(GameController.main.gameTime);
            historyPosition.Add(transform.position);
            historyRotation.Add(transform.rotation);
        }
        else if (GameController.main.gameState == GameController.GameState.Rewind) {
            while (historyIndex > 0 && historyTimestamp[historyIndex - 1] >= GameController.main.gameTime) {
                historyIndex--;
            }
            transform.position = historyPosition[historyIndex];
            transform.rotation = historyRotation[historyIndex];
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collider.CompareTag("Ground")) {
            audioSource.Play();
        }
    }
}
