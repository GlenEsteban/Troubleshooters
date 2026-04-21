using UnityEngine;

/// <summary>
/// Updates display animations based on emotion state and special animation triggers.
/// </summary>
[RequireComponent(typeof(Emotion))]
public class DisplayAnimator : MonoBehaviour {

    [SerializeField] private Animator displayAnimator;

    private Emotion emotion;

    private static readonly int NoneHash = Animator.StringToHash("None");
    private static readonly int AngryHash = Animator.StringToHash("Angry");
    private static readonly int SadHash = Animator.StringToHash("Sad");
    private static readonly int NeutralHash = Animator.StringToHash("Neutral");
    private static readonly int HappyHash = Animator.StringToHash("Happy");
    private static readonly int EcstaticHash = Animator.StringToHash("Ecstatic");
    private static readonly int SurprisedHash = Animator.StringToHash("Surprised");

    private static readonly int AlertHash = Animator.StringToHash("Alert");

    private DisplayAnimation currentAnimation;

    private void Awake() {
        emotion = GetComponent<Emotion>();
    }

    private void OnEnable() {
        emotion.EmotionChanged += UpdateDisplayAnimation;
    }

    private void OnDisable() {
        emotion.EmotionChanged -= UpdateDisplayAnimation;
    }

    private void Update() {
        UpdateDisplayAnimation(emotion.CurrentEmotion);
    }

    public void UpdateDisplayAnimation(DisplayAnimation animation) {
        if (currentAnimation == animation) { return; }
        if (displayAnimator == null) { return; }

        currentAnimation = animation;

        switch (animation) {
            case DisplayAnimation.None:
                displayAnimator.CrossFade(NoneHash, 0f);
                break;
            case DisplayAnimation.Angry:
                displayAnimator.CrossFade(AngryHash, 0f);
                break;
            case DisplayAnimation.Sad:
                displayAnimator.CrossFade(SadHash, 0f);
                break;
            case DisplayAnimation.Neutral:
                displayAnimator.CrossFade(NeutralHash, 0f);
                break;
            case DisplayAnimation.Happy:
                displayAnimator.CrossFade(HappyHash, 0f);
                break;
            case DisplayAnimation.Ecstatic:
                displayAnimator.CrossFade(EcstaticHash, 0f);
                break;
            case DisplayAnimation.Surprised:
                displayAnimator.CrossFade(SurprisedHash, 0f);
                break;
            case DisplayAnimation.Alert:
                displayAnimator.CrossFade(AlertHash, 0f);
                break;
        }
    }
}