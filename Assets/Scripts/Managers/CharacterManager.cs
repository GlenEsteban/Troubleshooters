using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central manager responsible for tracking and classifying all characters.
/// Other systems should query this class instead of storing direct references
/// to multiple characters.
/// </summary>
public class CharacterManager : MonoBehaviour {
    public static CharacterManager Instance { get; private set; }

    [SerializeField] private List<Character> players = new List<Character>();
    [SerializeField] private List<Character> enemies = new List<Character>();
    [SerializeField] private List<Character> npcs = new List<Character>();

    public IReadOnlyList<Character> Followers => players.AsReadOnly();
    public IReadOnlyList<Character> Enemies => enemies.AsReadOnly();
    public IReadOnlyList<Character> NPCs => npcs.AsReadOnly();

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
                if (!players.Contains(character)) {
                    players.Add(character);
                }
                break;
            case CharacterType.Enemy:
                if (!enemies.Contains(character)) {
                    enemies.Add(character);
                }
                break;
            case CharacterType.NPC:
                if (!npcs.Contains(character)) {
                    npcs.Add(character);
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
                if (!players.Contains(character)) {
                    players.Remove(character);
                }
                break;
            case CharacterType.Enemy:
                if (!enemies.Contains(character)) {
                    enemies.Remove(character);
                }
                break;
            case CharacterType.NPC:
                if (!npcs.Contains(character)) {
                    npcs.Remove(character);
                }
                break;
            case CharacterType.None:
                break;
        }
    }
}