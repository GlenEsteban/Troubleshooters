using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AntigravityObject : MonoBehaviour , IUsableObject{

    private Rigidbody2D rb;
    private float defaultGravityScale;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        defaultGravityScale = rb.gravityScale;
    }

    public void Use() {
        ToggleGravity();
    }

    public void ToggleGravity() {
        if (rb == null) return;

        if (rb.gravityScale != 0) {
            rb.gravityScale = 0;
        }
        else {
            rb.gravityScale = defaultGravityScale;
        }
    }
}
