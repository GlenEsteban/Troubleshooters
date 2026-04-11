using UnityEngine;

/// <summary>
/// Plays claw sound effects in response to grab and release events.
/// </summary>
[RequireComponent(typeof(ClawAttachment))]
public class ClawAttachmentSFX : MonoBehaviour {
    [Header("Claw Sound Effects")]
    [SerializeField] private AudioClip clawClackOpen;
    [SerializeField] private AudioClip clawClackClosed;

    private ClawAttachment clawAttachment;

    private void Awake() {
        clawAttachment = GetComponent<ClawAttachment>();
    }
    private void OnEnable() {
        clawAttachment.ClawClosed += PlayClawClackClosed;
        clawAttachment.ClawOpened += PlayClawClackOpen;
    }

    private void OnDisable() {
        clawAttachment.ClawClosed -= PlayClawClackClosed;
        clawAttachment.ClawOpened -= PlayClawClackOpen;
    }

    public void PlayClawClackOpen() {
        AudioManager.Instance.PlayAudioClip(clawClackOpen);
    }

    public void PlayClawClackClosed() {
        AudioManager.Instance.PlayAudioClip(clawClackClosed);
    }
}