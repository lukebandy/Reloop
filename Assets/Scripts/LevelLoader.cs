using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    Animator animator;
    public float transitionTime;

    // Start is called before the first frame update
    void Start() {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void LoadLevel(int levelIndex) {
        animator.SetBool("Exit", true);
        StartCoroutine(LoadLevelCoroutine(levelIndex));
    }

    public IEnumerator LoadLevelCoroutine(int levelIndex) {
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
