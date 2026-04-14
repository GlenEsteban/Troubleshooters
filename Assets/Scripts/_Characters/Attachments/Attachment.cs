using UnityEngine;

/// <summary>
/// Base class for equippable attachments, providing logic for equip/unequip
/// behavior and a common interface for usage.
/// </summary>
public abstract class Attachment: MonoBehaviour {
    public virtual void OnEquip() {
        //Equip base logic
    }

    public virtual void OnUnequip() { 
        //Unequip base logic
    }

    public abstract void Use();
}