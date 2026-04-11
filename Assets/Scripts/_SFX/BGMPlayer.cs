using UnityEngine;

/// <summary>
/// Triggers background music playback and delegates it to the AudioManager.
/// </summary>
public class BGMPlayer: MonoBehaviour {
    [Header("Main BGM")]
    [SerializeField] private AudioClip mainBGM;
    [SerializeField] private bool loopMainBGM = true;

    [Header("Win BGM")]
    [SerializeField] private AudioClip gameWinBGM;
    [SerializeField] private bool loopGameWinBGM = true;

    public void PlayMainBGM() {
        if (mainBGM == null) { return; }

        AudioManager.Instance.PlayMusic(mainBGM, loopMainBGM);
    }
        
    public void PlayGameWinBGM() {
        if (gameWinBGM == null) { return; }

        AudioManager.Instance.PlayMusic(gameWinBGM, loopGameWinBGM);
    }
}