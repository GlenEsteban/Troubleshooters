using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {
    public static CharacterManager Instance { get; private set; }

    [SerializeField] private List<Character> _players = new List<Character>();
    [SerializeField] private List<Character> _enemies = new List<Character>();
    [SerializeField] private List<Character> _NPCs = new List<Character>();

    public IReadOnlyList<Character> Followers => _players.AsReadOnly();
    public IReadOnlyList<Character> Enemies => _enemies.AsReadOnly();
    public IReadOnlyList<Character> NPCs => _NPCs.AsReadOnly();


    public void Add(Character character, CharacterType type) {
        switch (type) {
            case CharacterType.Player:
                _players.Add(character);
                break;
            case CharacterType.Enemy:
                _enemies.Add(character);
                break;
            case CharacterType.NPC:
                _NPCs.Add(character);
                break;
            case CharacterType.None:
                break;
        }
    }
    public void Remove(Character character) {
        if (_players.Contains(character)) {
            _players.Remove(character);
        } else if (_enemies.Contains(character)) {
            _enemies.Remove(character);
        } else if (_NPCs.Contains(character)) {
            _NPCs.Remove(character);
        }
    }

    private void Awake() {
        if (Instance != null &&  Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }
    }
}

public enum CharacterType{
    Player,
    Enemy,
    NPC,
    None
}