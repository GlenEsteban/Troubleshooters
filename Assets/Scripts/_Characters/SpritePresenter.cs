using UnityEngine;

public class SpritePresenter : MonoBehaviour {
    [SerializeField] private Transform _characterSprites;
    [SerializeField] private Animator _clawAnimator;
    [SerializeField] private Animator _displayAnimator;

    private LookOrientation _lookOrientation;

    private void Awake() {
        _lookOrientation = GetComponent<LookOrientation>();
    }

    private void OnEnable() {
        _lookOrientation.OrientationChanged += UpdateSpriteLookOrientation;
    }

    private void OnDisable() {
        _lookOrientation.OrientationChanged -= UpdateSpriteLookOrientation;
    }

    public void UpdateSpriteLookOrientation(Vector2 direction) {
        if (direction.x == 0) return;

        if (direction.x > 0) {
            _characterSprites.localScale = new Vector3(1, 1, 1);
        }
        else {
            _characterSprites.localScale = new Vector3(-1, 1, 1);
        }
    } 

    public void SetClawAnimation(bool state) {
        _clawAnimator.SetBool("IsUsingClaw", state);
    }

    public void SetDisplayAnimation(Expression expression) {
        switch (expression) {
            case Expression.Happy:
                _displayAnimator.SetBool("IsHappy", true);
                break;
            case Expression.Neutral:
                _displayAnimator.SetBool("IsNeutral", true);
                break;
            case Expression.Upset:
                _displayAnimator.SetBool("IsUpset", true);
                break;
            case Expression.SuperHappy:
                _displayAnimator.SetBool("IsSuperHappy", true);
                break;
        }
    }
}

public enum Expression {
    Happy,
    Neutral,
    Upset,
    SuperHappy
}