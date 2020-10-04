using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour {

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("Ghost")) {
            audioSource.Play();
            collision.BroadcastMessage("Freeze");
            FindObjectOfType<LevelLoader>().LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
