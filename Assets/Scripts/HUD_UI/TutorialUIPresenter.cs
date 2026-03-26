using UnityEngine;

public class TutorialUIPresenter : MonoBehaviour {
    [SerializeField] Animator _moveTutorialUIAnimator;

    public void DisplayMoveTutorialUI(bool isDisplayed) {
        _moveTutorialUIAnimator.SetBool("IsVisible", isDisplayed);
    }
}