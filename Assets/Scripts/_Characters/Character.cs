using UnityEngine;

/// <summary>
/// Represents a character in the scene and handles registration
/// with the CharacterManager.
/// </summary>
public class Character : MonoBehaviour {
    public CharacterType Type => characterType;
    public string Name => characterName;

    [Header("Character Info")]
    [SerializeField] private CharacterType characterType;
    [SerializeField] private string characterName;

    private void OnEnable() {
        CharacterManager.Instance.RegisterCharacter(this);
    }

    private void OnDisable() {
        CharacterManager.Instance.RemoveCharacter(this);
    }
}