using UnityEngine;

/// <summary>
/// Plays claw sound effects in response to grab and release events.
/// </summary>
[RequireComponent(typeof(Claw))]
public class ClawSFX : MonoBehaviour {
    [SerializeField] private AudioClip clawClackOpen;
    [SerializeField] private AudioClip clawClackClosed;

    private Claw claw;

    private void Awake() {
        claw = GetComponent<Claw>();
    }
    private void OnEnable() {
        claw.ClawClosed += PlayClawClackClosed;
        claw.ClawOpened += PlayClawClackOpen;
    }

    private void OnDisable() {
        claw.ClawClosed -= PlayClawClackClosed;
        claw.ClawOpened -= PlayClawClackOpen;
    }

    public void PlayClawClackOpen() {
        AudioManager.Instance.PlayAudioClip(clawClackOpen);
    }

    public void PlayClawClackClosed() {
        AudioManager.Instance.PlayAudioClip(clawClackClosed);
    }
}