using UnityEngine;

/// <summary>
/// Triggers sound effect playback by selecting an audio clip
/// and delegating playback to the AudioManager.
/// </summary>
public class SFXPlayer : MonoBehaviour {
    [SerializeField, Tooltip("One or more clips; plays randomly if multiple are assigned")]
    private AudioClip[] audioClips; 
    public void PlaySFX() {
        if (audioClips == null || audioClips.Length == 0) { return; }

        if (audioClips.Length == 1) {
            AudioManager.Instance.PlayAudioClip(audioClips[0]);
        }
        else {
            AudioManager.Instance.PlayRandomAudioClip(audioClips);
        }
    }
}