using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhost : MonoBehaviour {

    // Components
    private Rigidbody2D rb;
    private new Collider2D collider;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    // Input/movement/animation
    public float movementXSpeed;
    public float movementJumpForce;
    public float movementJumpTime;
    public LayerMask movementGround;
    public Vector3 movementJumpFeetPos = new Vector3(0, -0.5f);
    public float movementJumpFeetRadius = 0.1f;

    private bool movementJumpGrounded;
    private bool movementJumping;
    private float movementJumpTimer;
    private string animatorCurrent;

    // Position timestamps
    private int historyInputIndex;
    public List<float> historyInputTimestamp;
    public List<float> historyInputX;
    public List<bool> historyInputJump;

    private int historyPlaybackIndex;
    public List<float> historyPlaybackTimestamp;
    public List<Vector3> historyPlaybackPosition;
    public List<string> historyPlaybackAnimation;
    public List<bool> historyPlaybackFlip;

    // Start is called before the first frame update
    void Start() {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Set to the last recorded frame
        historyPlaybackIndex = historyPlaybackTimestamp.Count - 1;
    }

    // Update is called once per frame
    void Update() {
        // If the game state has changed
        if (GameController.main.gameStatePrevious != GameController.main.gameState) {
            if (GameController.main.gameState == GameController.GameState.Play) {
                historyInputIndex = 0;
                transform.position = historyPlaybackPosition[0];
                rb.isKinematic = false;
                rb.velocity = Vector2.zero;
                collider.enabled = true;

                historyPlaybackTimestamp = new List<float>();
                historyPlaybackPosition = new List<Vector3>();
                historyPlaybackAnimation = new List<string>();
                historyPlaybackFlip = new List<bool>();
            }
            else if (GameController.main.gameState == GameController.GameState.Rewind) {
                rb.isKinematic = true;
                collider.enabled = false;

                historyPlaybackIndex = historyPlaybackTimestamp.Count - 1;
            }
        }

        // If the game is in play mode
        if (GameController.main.gameState == GameController.GameState.Play) {
            movementJumpGrounded = Physics2D.OverlapCircle(transform.position + movementJumpFeetPos, movementJumpFeetRadius, movementGround);

            while (historyInputIndex < historyInputTimestamp.Count - 1 && historyInputTimestamp[historyInputIndex + 1] <= GameController.main.gameTime) {
                historyInputIndex++;
            }

            historyPlaybackTimestamp.Add(GameController.main.gameTime);
            historyPlaybackPosition.Add(transform.position);
            historyPlaybackAnimation.Add(animatorCurrent);
            historyPlaybackFlip.Add(spriteRenderer.flipX);

            if (movementJumpGrounded) {
                if (Mathf.Abs(historyInputX[historyInputIndex]) > 0.1f)
                    ChangeAnimation("Player_Run");
                else
                    ChangeAnimation("Player_Idle");
            }
            else {
                if (rb.velocity.y > 0)
                    ChangeAnimation("Player_Jump");
                else
                    ChangeAnimation("Player_Land");
            }

            if (historyInputX[historyInputIndex] != 0.0f)
                spriteRenderer.flipX = historyInputX[historyInputIndex] < 0.0f;
        }

        // If the game is in rewind mode
        else if (GameController.main.gameState == GameController.GameState.Rewind) {
            while (historyPlaybackIndex > 0 && historyPlaybackTimestamp[historyPlaybackIndex - 1] >= GameController.main.gameTime) {
                historyPlaybackIndex--;
            }

            transform.position = historyPlaybackPosition[historyPlaybackIndex];
            ChangeAnimation(historyPlaybackAnimation[historyPlaybackIndex]);
            spriteRenderer.flipX = historyPlaybackFlip[historyPlaybackIndex];
        }
    }

    public void FixedUpdate() {
        if (GameController.main.gameState == GameController.GameState.Play) {
            rb.velocity = new Vector2(historyInputX[historyInputIndex] * movementXSpeed, rb.velocity.y);

            if (movementJumpGrounded && historyInputJump[historyInputIndex]) {
                movementJumping = true;
                movementJumpTimer = movementJumpTime;
                rb.velocity = new Vector2(rb.velocity.x, movementJumpForce);
                audioSource.Play();
            }

            if (movementJumping && historyInputJump[historyInputIndex]) {
                if (movementJumpTimer > 0) {
                    rb.velocity = new Vector2(rb.velocity.x, movementJumpForce);
                    movementJumpTimer -= Time.deltaTime;
                }
                else {
                    movementJumping = false;
                }
            }

            if (!historyInputJump[historyInputIndex])
                movementJumping = false;
        }
    }

    public void ChangeAnimation(string animation) {
        if (animation != animatorCurrent) {
            animator.Play(animation);
            animatorCurrent = animation;
        }
    }

    public void Freeze() {

    }

    public void OnDrawGizmos() {
        if (movementJumpGrounded)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + movementJumpFeetPos, movementJumpFeetRadius);
    }
}
