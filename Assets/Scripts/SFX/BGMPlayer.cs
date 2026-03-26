using UnityEngine;

public class BGMPlayer: MonoBehaviour {
    [SerializeField] AudioClip _mainBGM;
    [SerializeField] AudioClip _gameWinBGM;

    AudioSource bgmAudioSource;

    private void Awake() {
        bgmAudioSource = GetComponent<AudioSource>();
    }

    public void PlayMainBGM() {
        bgmAudioSource.loop = true;
        bgmAudioSource.clip = _mainBGM;
        bgmAudioSource.Play();
    }
    public void PlayGameWinBGM() {
        bgmAudioSource.loop = true;
        bgmAudioSource.clip = _gameWinBGM;
        bgmAudioSource.Play();
    }

    public void StopMusic() {
        bgmAudioSource.Stop();
    }
}