using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central manager responsible for tracking and classifying all characters.
/// Other systems should query this class instead of storing direct references
/// to multiple characters.
/// </summary>
public class CharacterManager : MonoBehaviour {
    public static CharacterManager Instance { get; private set; }

    public IReadOnlyList<Character> PlayerCharacters => playerCharacters.AsReadOnly();
    public IReadOnlyList<Character> EnemyCharacters => enemyCharacters.AsReadOnly();
    public IReadOnlyList<Character> NPCCharacters => npcCharacters.AsReadOnly();

    [Header("Characters")]
    [SerializeField] private List<Character> playerCharacters = new List<Character>();
    [SerializeField] private List<Character> enemyCharacters = new List<Character>();
    [SerializeField] private List<Character> npcCharacters = new List<Character>();

    private void Awake() {
        // Ensure only one instance exists
        if (Instance != null &&  Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Registers a character with the manager based on its type.
    /// </summary>
    public void RegisterCharacter(Character character) {
        switch (character.Type) {
            case CharacterType.Player:
                if (!playerCharacters.Contains(character)) {
                    playerCharacters.Add(character);
                }
                break;
            case CharacterType.Enemy:
                if (!enemyCharacters.Contains(character)) {
                    enemyCharacters.Add(character);
                }
                break;
            case CharacterType.NPC:
                if (!npcCharacters.Contains(character)) {
                    npcCharacters.Add(character);
                }
                break;
            case CharacterType.None:
                break;
        }
    }

    /// <summary>
    /// Unregisters a character from the manager.
    /// </summary>
    public void RemoveCharacter(Character character) {
        switch (character.Type) {
            case CharacterType.Player:
                if (!playerCharacters.Contains(character)) {
                    playerCharacters.Remove(character);
                }
                break;
            case CharacterType.Enemy:
                if (!enemyCharacters.Contains(character)) {
                    enemyCharacters.Remove(character);
                }
                break;
            case CharacterType.NPC:
                if (!npcCharacters.Contains(character)) {
                    npcCharacters.Remove(character);
                }
                break;
            case CharacterType.None:
                break;
        }
    }
}