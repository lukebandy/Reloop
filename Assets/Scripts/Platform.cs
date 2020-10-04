using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    private new BoxCollider2D collider;

    private Vector2 positionClosed;
    public Vector2 positionOpen;

    private bool open;
    public float speed;

    private int historyIndex;
    private List<float> historyTimestamp;
    private List<Vector3> historyPosition;

    // Start is called before the first frame update
    void Start() {
        positionClosed = transform.position;

        historyTimestamp = new List<float>();
        historyPosition = new List<Vector3>();
    }

    // Update is called once per frame
    public void Update() {
        if (GameController.main.gameStatePrevious != GameController.main.gameState) {
            if (GameController.main.gameState == GameController.GameState.Play) {
                transform.position = historyPosition[0];

                historyTimestamp = new List<float>();
                historyPosition = new List<Vector3>();
            }
            else if (GameController.main.gameState == GameController.GameState.Rewind) {
                historyIndex = historyTimestamp.Count - 1;
            }
        }

        if (GameController.main.gameState == GameController.GameState.Play) {
            if (open)
                transform.position = Vector3.MoveTowards(transform.position, positionClosed + positionOpen, Time.deltaTime * speed);
            else
                transform.position = Vector3.MoveTowards(transform.position, positionClosed, Time.deltaTime * speed);

            historyTimestamp.Add(GameController.main.gameTime);
            historyPosition.Add(transform.position);
        }
        else if (GameController.main.gameState == GameController.GameState.Rewind) {
            while (historyIndex > 0 && historyTimestamp[historyIndex - 1] >= GameController.main.gameTime) {
                historyIndex--;
            }
            transform.position = historyPosition[historyIndex];
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") || collision.CompareTag("Ghost")) {
            collision.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if ((collision.transform.CompareTag("Player") || collision.transform.CompareTag("Ghost")) && collision.gameObject.activeSelf) {
            collision.transform.SetParent(null);
        }
    }

    public void Open(bool open) {
        this.open = open;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        // TODO: Make this work better in play mode
        if (collider == null)
            collider = GetComponent<BoxCollider2D>();
        if (positionClosed == null)
            Gizmos.DrawWireCube(transform.position + new Vector3(positionOpen.x, positionOpen.y), collider.size);
    }
}
