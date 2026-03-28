using UnityEngine;

public class Character : MonoBehaviour {
    [SerializeField] private CharacterType type;

    public CharacterType Type => type;

    private void Awake() {
        SetCharacterTypeViaTag();
    }
    private void Start() {
        CharacterManager.Instance.RegisterCharacter(this);
    }

    private void OnDisable() {
        CharacterManager.Instance.RemoveCharacter(this);
    }

    private void SetCharacterTypeViaTag() {
        string layerName = LayerMask.LayerToName(gameObject.layer);

        switch (layerName) {
            case "Player":
                type = CharacterType.Player;
                break;
            case "NPC":
                type = CharacterType.NPC;
                break;
            case "Hazards":
                type = CharacterType.Enemy;
                break;
            default:
                type = CharacterType.None;
                break;
        }
    }
}