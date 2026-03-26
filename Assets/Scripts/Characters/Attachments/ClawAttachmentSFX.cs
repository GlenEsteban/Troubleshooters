using System.Collections;
using UnityEngine;

public class ClawAttachmentSFX : MonoBehaviour {
    // References    
    [SerializeField] AudioClip _clawClackOpen;
    [SerializeField] AudioClip _clawClackClosed;
    private AudioSource _audioSource;

    public void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClawClackOpen() {
        _audioSource.PlayOneShot(_clawClackOpen);
    }

    public void PlayClawClackClosed() {
        _audioSource.PlayOneShot(_clawClackClosed);
    }
}