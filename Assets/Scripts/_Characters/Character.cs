using UnityEngine;

/// <summary>
/// Represents a character in the scene and handles registration
/// with the CharacterManager.
/// </summary>
public class Character : MonoBehaviour {
    public CharacterType Type => type;

    [SerializeField] private CharacterType type;

    private void OnEnable() {
        CharacterManager.Instance.RegisterCharacter(this);
    }

    private void OnDisable() {
        CharacterManager.Instance.RemoveCharacter(this);
    }
}