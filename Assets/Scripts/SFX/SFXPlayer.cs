using UnityEngine;

public class SFXPlayer : MonoBehaviour {
    [SerializeField] AudioClip _sfx;
    private AudioSource _audioSource;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }
    public void PlaySFX() {
        _audioSource.loop = false;
        _audioSource.clip = _sfx;
        _audioSource.Play();
    }

    public void StopSFX() {
        _audioSource.Stop();
    }
}