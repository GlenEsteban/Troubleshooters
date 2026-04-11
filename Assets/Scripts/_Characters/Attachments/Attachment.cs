using UnityEngine;

/// <summary>
/// Base class for a single attachment that can be used.
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