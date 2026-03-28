using UnityEngine;

/// <summary>
/// Central manager responsible for playing sound effects.
/// Other objects should use this class to request audio playback
/// instead of managing AudioSources directly.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource;

    private void Awake() {
        // Ensure only one instance exists
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays a one-shot sound effect using the global AudioManager instance.
    /// </summary>
    public void PlayAudioClip(AudioClip clip) {
        if (clip == null) { return; }

        audioSource.PlayOneShot(clip);
    }
}