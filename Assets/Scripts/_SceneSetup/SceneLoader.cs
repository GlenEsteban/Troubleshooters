using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles scene loading using Unity's scene manager.
/// </summary>
public class SceneLoader : MonoBehaviour {
    private bool isLoading = false;

    private void Update() {
        // TEMP: INPUT FOR TESTING SCENE LOADING
        if (Input.GetKeyUp(KeyCode.R)) {
            ReloadScene(0);
        }
        if (Input.GetKeyUp(KeyCode.N)) {
            LoadNextScene(0);
        }
    }

    public void LoadScene(int buildIndex, float delay) {
        StartCoroutine(LoadSceneCoroutine(buildIndex, delay));
    }

    private IEnumerator LoadSceneCoroutine(int buildIndex, float delay) {
        if (isLoading) yield break;
        isLoading = true;

        yield return new WaitForSeconds(delay);

        if (buildIndex >= 0 && buildIndex < SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(buildIndex);
        }
        else {
            Debug.LogWarning($"Scene build index {buildIndex} is out of range.");
            isLoading = false;
        }
    }

    public void ReloadScene(float delay) {
        int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        LoadScene(currentSceneBuildIndex, delay);
    }

    public void LoadNextScene(float delay) {
        var nextSceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneBuildIndex < SceneManager.sceneCountInBuildSettings) {
            LoadScene(nextSceneBuildIndex, delay);
        }
        else {
            Debug.LogWarning("No next scene exists in Build Settings.");
        }
    }
}