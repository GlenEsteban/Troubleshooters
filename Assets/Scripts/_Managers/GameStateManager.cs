using System.Collections;
using UnityEngine;

/// <summary>
/// Central manager responsible for controlling game flow and handling game state changes.
/// Other systems should use this class to change or query the current game state.
/// </summary>
public class GameStateManager : MonoBehaviour {
    public static GameStateManager Instance { get; private set; }

    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private BGMPlayer bgmPlayer; 
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject levelIntroUI;
    [SerializeField] private SFXPlayer levelIntroSFX;
    [SerializeField] private GameObject reloadLevelTransitionUI;

    private bool hasStartedLevel = false;

    private GameState currentGameState = GameState.None;

    public GameState CurrentGameState => currentGameState;

    private void Awake() {
        // Ensure only one instance exists
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start() {
        ChangeGameState(GameState.GameStart);
    }

    private void Update() {
        // TEMP: INPUT FOR TESTING GAME STATE CHANGES
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ChangeGameState(GameState.GameStart);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ChangeGameState(GameState.GameOver);
        }
    }

    /// <summary>
    /// Changes the current game state and runs the logic associated with the new state.
    /// </summary>
    /// <param name="state">The game state to switch to.</param>
    public void ChangeGameState(GameState state) {
        if (currentGameState == state) { return; }

        currentGameState = state;

        switch (state) {
            case GameState.GameStart:
                RunGameStart();
                break;
            case GameState.GameWin:
                RunGameWin();
                break;
            case GameState.GameOver:
                RunGameOver();
                break;
        }
    }
    private void RunGameStart() {
        if (!hasStartedLevel) {
            StartCoroutine(LevelIntro());

            hasStartedLevel = true;
        }
        else {
            levelIntroUI.SetActive(false);
            levelIntroSFX.PlaySFX();

            reloadLevelTransitionUI.SetActive(true);

            StartCoroutine(LevelReload());
        }
    }

    private void RunGameWin() {
        bgmPlayer.PlayGameWinBGM();

       sceneLoader.LoadNextScene(3f);

        hasStartedLevel = false;
    }

    private void RunGameOver() {
        AudioManager.Instance.StopMusic();

        sceneLoader.ReloadScene(3f);

        hasStartedLevel = false;
    }

    /// <summary>
    /// Plays the intro sequence before gameplay begins.
    /// </summary>
    private IEnumerator LevelIntro() {
        levelIntroUI.SetActive(true);
        reloadLevelTransitionUI.SetActive(false);
        playerController.enabled = false;

        yield return new WaitForSeconds(0.5f);

        levelIntroSFX.PlaySFX();

        yield return new WaitForSeconds(4f);

        bgmPlayer.PlayMainBGM();

        playerController.enabled = true;
    }

    /// <summary>
    /// Plays the reload transition before gameplay resumes.
    /// </summary>
    private IEnumerator LevelReload() {
        levelIntroUI.SetActive(false);

        reloadLevelTransitionUI.SetActive(true);

        playerController.enabled = false;

        yield return new WaitForSeconds(1f);

        bgmPlayer.PlayMainBGM();

        playerController.enabled = true;
    }

}