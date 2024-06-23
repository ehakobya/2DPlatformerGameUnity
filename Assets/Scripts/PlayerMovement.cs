using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask wallLayerMask;

    // player object references

    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    // Get reference of the objects
    // player movement script is attached to player object,
    // GetComponent will search the player object and return the Rigidbody2D or others
    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update() {

        horizontalInput = Input.GetAxis("Horizontal");

        // flip character to towards right
        if (horizontalInput > 0.01f) {
            transform.localScale = Vector3.one;
        }
        // flip character towards left
        else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // wall jump logic
        if (wallJumpCooldown > 0.2f) {

            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            if (isOnWall() && !isOnGround()) {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else {
                body.gravityScale = 7;
            }

            if (Input.GetKey(KeyCode.Space)) {
                Jump();
            }

        }
        else {
            wallJumpCooldown += Time.deltaTime;
        }


        // Set animation parameters when there is a key press on horizontal axis
        animator.SetBool("isRunning", horizontalInput != 0);
        animator.SetBool("isOnGround", isOnGround());
    }

    private void Jump() {
        if (isOnGround()) {
            // maintain current velocity in x, apply speed on y dir
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("isJumping");
        }
        else if (isOnWall() && !isOnGround()) {
            if (horizontalInput == 0) {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            }
            wallJumpCooldown = 0;
        }

    }

    // this function is called every time the collider in this (player) touches
    // another collider
    private void OnCollisionEnter2D(Collision2D collision) {

    }

    public bool isOnGround() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayerMask);
        return raycastHit.collider != null;
    }

    public bool isOnWall() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayerMask);
        return raycastHit.collider != null;
    }
}
