using UnityEngine;

/// <summary>
/// Updates character visuals based on orientation and emotion state.
/// </summary>
[RequireComponent(typeof(LookOrientation))]
[RequireComponent(typeof(Emotion))]
public class CharacterVisuals : MonoBehaviour {
    [Header("Visual Components")]
    [SerializeField] private Transform characterVisuals;
    [SerializeField] private Animator emotionDisplayAnimator;

    private LookOrientation lookOrientation;
    private Emotion emotion;

    private void Awake() {
        lookOrientation = GetComponent<LookOrientation>();
        emotion = GetComponent<Emotion>();
    }

    private void OnEnable() {
        lookOrientation.LookDirectionChanged += UpdateVisualsOrientation;
        emotion.EmotionChanged += UpdateEmotionDisplayAnimation;
    }

    private void OnDisable() {
        lookOrientation.LookDirectionChanged -= UpdateVisualsOrientation;
        emotion.EmotionChanged -= UpdateEmotionDisplayAnimation;
    }

    public void UpdateVisualsOrientation(Vector2 direction) {
        if (characterVisuals == null) { return; }
        if (direction.x == 0) { return; }

        if (direction.x < 0) {
            characterVisuals.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else {
            characterVisuals.localRotation = Quaternion.Euler(0, 0, 0);
        }
    } 

    public void UpdateEmotionDisplayAnimation(EmotionType emotion) {
        if (emotionDisplayAnimator == null) { return; }

        emotionDisplayAnimator.SetInteger("Emotion", (int)emotion);
    }
}