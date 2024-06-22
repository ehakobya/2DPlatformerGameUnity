using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private float speed;

    // player object references
    private Rigidbody2D body;
    private Animator animator;

    // animator parameters
    private bool isOnGround;

    // Get reference of the objects
    // player movement script is attached to player object,
    // GetComponent will search the player object and return the Rigidbody2D or others
    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update() {

        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        // flip character to towards right
        if (horizontalInput > 0.01f) {
            transform.localScale = Vector3.one;
        }
        // flip character towards left
        else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKey(KeyCode.Space) && isOnGround) {
            HandlePlayerJump();
        }

        // Set animation parameters when there is a key press on horizontal axis
        animator.SetBool("isRunning", horizontalInput != 0);
        animator.SetBool("isOnGround", isOnGround);
    }

    private void HandlePlayerJump() {
        // maintain current velocity in x, apply speed on y dir
        body.velocity = new Vector2(body.velocity.x, speed);
        animator.SetTrigger("isJumping");
        isOnGround = false;
    }

    // this function is called every time the collider in this (player) touches
    // another collider
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ground") {
            isOnGround = true;
        }
    }
}
