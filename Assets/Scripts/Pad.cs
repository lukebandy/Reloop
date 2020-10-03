using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour {

    public Sprite spriteUnweighted;
    public Sprite spriteWeighted;

    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    bool weighted = false;

    public GameObject[] control;

    // Start is called before the first frame update
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(0.8f, 0.8f), 0.0f, Vector2.zero, 0.0f);
        bool weightedNew = hits.Length > 0;

        if (weighted != weightedNew) {
            audioSource.Play();
            if (weightedNew)
                spriteRenderer.sprite = spriteWeighted;
            else
                spriteRenderer.sprite = spriteUnweighted;
            foreach (GameObject gameObject in control)
                gameObject.SendMessage("Open", weightedNew);
        }

        weighted = weightedNew;
    }
}
