using UnityEngine;

/// <summary>
/// Updates claw and indicator light animations in response to ClawAttachment events.
/// </summary>
[RequireComponent(typeof(ClawAttachment))]
public class ClawAttachmentAnimator : MonoBehaviour {
    [SerializeField] private Animator clawAnimator;
    [SerializeField] private Animator clawIndicatorLightAnimator;

    private ClawAttachment clawAttachment;

    private static readonly int MagentaHash = Animator.StringToHash("Magenta");
    private static readonly int YellowHash = Animator.StringToHash("Yellow");
    private static readonly int CyanHash = Animator.StringToHash("Cyan");

    private void Awake () {
        clawAttachment = GetComponent<ClawAttachment>();
    }

    private void OnEnable () {
        clawAttachment.ClawClosed += AnimateClawClosed;
        clawAttachment.ClawOpened+= AnimateClawOpened;
        clawAttachment.NothingGrabbed += AnimateYellowIndicatorLight;
        clawAttachment.GrabbedObject += AnimateCyanIndicatorLight;
        clawAttachment.ClawOpened += AnimateMagentaIndicatorLight;
        clawAttachment.ReleasedObject += AnimateMagentaIndicatorLight;
        clawAttachment.UnintentionallyReleased += AnimateYellowIndicatorLight;
    }

    private void OnDisable() {
        clawAttachment.ClawClosed -= AnimateClawClosed;
        clawAttachment.ClawOpened -= AnimateClawOpened;
        clawAttachment.NothingGrabbed -= AnimateYellowIndicatorLight;
        clawAttachment.GrabbedObject -= AnimateCyanIndicatorLight;
        clawAttachment.ClawOpened -= AnimateMagentaIndicatorLight;
        clawAttachment.ReleasedObject -= AnimateMagentaIndicatorLight;
        clawAttachment.UnintentionallyReleased -= AnimateYellowIndicatorLight;
    }

    private void AnimateClawClosed() {
        clawAnimator.SetBool("IsUsingClaw", true);
    }

    private void AnimateClawOpened() {
        clawAnimator.SetBool("IsUsingClaw", false);
    }

    private void AnimateMagentaIndicatorLight() {
        clawIndicatorLightAnimator.CrossFade(MagentaHash, 0f);
    }

    private void AnimateYellowIndicatorLight() {
        clawIndicatorLightAnimator.CrossFade(YellowHash, 0f);
    }

    private void AnimateCyanIndicatorLight() {
        clawIndicatorLightAnimator.CrossFade(CyanHash, 0f);
    }
}
