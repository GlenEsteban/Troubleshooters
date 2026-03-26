using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Loads game scenes
public class SceneLoader : MonoBehaviour {
    private void Update() {
        // TEMP: INPUT FOR SCENE RELOAD AND LEVEL SELECT. IT WILL IMPLEMENTED ON MENU SYSTEM
        if (Input.GetKeyUp(KeyCode.R)) {
            StartCoroutine(ReloadScene(0));
        }
        if (Input.GetKeyUp(KeyCode.N)) {
            StartCoroutine(LoadNextScene(0));
        }
    }

    public IEnumerator ReloadScene(float delay) {
        yield return new WaitForSeconds(delay);
        var currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneBuildIndex);
    }

    public IEnumerator LoadNextScene(float delay) {
        yield return new WaitForSeconds(delay);
        var nextSceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneBuildIndex < SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(nextSceneBuildIndex);
        }
    }
}