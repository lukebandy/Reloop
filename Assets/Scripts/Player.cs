using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    // Input/movement/animation
    public float movementXSpeed;
    public float movementJumpForce;
    public float movementJumpTime;
    public LayerMask movementGround;

    private bool grounded;
    private float inputMove;
    private bool inputJump;
    private bool movementJumping;
    private float movementJumpTimer;
    private Vector3 movementJumpFeetPos = new Vector3(0, -0.5f);
    private float movementJumpFeetRadius = 0.1f;
    private string animatorCurrent;

    // Ghosting
    public GameObject prefabGhost;

    private List<float> historyTimestamps;
    private List<float> historyInputX;
    private List<bool> historyInputJump;
    private List<Vector3> historyPosition;
    private List<string> historyAnimation;
    private List<bool> historyFlip;

    // Start is called before the first frame update
    private void Start() {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>(); 
    }
    
    // Called when GameObject enabled
    private void OnEnable() {
        // Don't do this at start
        if (historyPosition != null && historyPosition.Count > 0)
            transform.position = historyPosition[0];

        // Reset timestamps
        historyInputX = new List<float>();
        historyInputJump = new List<bool>();
        historyPosition = new List<Vector3>();
        historyTimestamps = new List<float>();
        historyAnimation = new List<string>();
        historyFlip = new List<bool>();
    }

    // Update is called once per frame
    private void Update() {
        if (GameController.main.gameState == GameController.GameState.Play) {
            // Change animation
            if (grounded) {
                if (Mathf.Abs(inputMove) > 0.1f)
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

            if (inputMove != 0.0f)
                spriteRenderer.flipX = inputMove < 0.0f;
        }
        else if (GameController.main.gameState == GameController.GameState.Rewind) {
            // Spawn ghost
            GameObject ghost = Instantiate(prefabGhost, transform.position, Quaternion.identity);
            PlayerGhost ghostScript = ghost.GetComponent<PlayerGhost>();

            ghostScript.movementXSpeed = movementXSpeed;
            ghostScript.movementJumpForce = movementJumpForce;
            ghostScript.movementJumpTime = movementJumpTime;
            ghostScript.movementGround = movementGround;

            ghostScript.movementJumpFeetPos = movementJumpFeetPos;
            ghostScript.movementJumpFeetRadius = movementJumpFeetRadius;

            ghostScript.historyInputTimestamp = historyTimestamps;
            ghostScript.historyInputX = historyInputX;
            ghostScript.historyInputJump = historyInputJump;
            ghostScript.historyPlaybackTimestamp = historyTimestamps;
            ghostScript.historyPlaybackPosition = historyPosition;
            ghostScript.historyPlaybackAnimation = historyAnimation;
            ghostScript.historyPlaybackFlip = historyFlip;

            gameObject.SetActive(false);
        }
        else if (GameController.main.gameState == GameController.GameState.Paused) {
            // TODO: Test this and resume on play
            animator.StopPlayback();
        }
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(inputMove * movementXSpeed, rb.velocity.y);
        grounded = Physics2D.OverlapCircle(transform.position + movementJumpFeetPos, movementJumpFeetRadius, movementGround);

        if (grounded && inputJump) {
            movementJumping = true;
            movementJumpTimer = movementJumpTime;
            rb.velocity = new Vector2(rb.velocity.x, movementJumpForce);
            audioSource.Play();
        }

        if (movementJumping && inputJump) {
            if (movementJumpTimer > 0) {
                rb.velocity = new Vector2(rb.velocity.x, movementJumpForce);
                movementJumpTimer -= Time.deltaTime;
            }
            else {
                movementJumping = false;
            }
        }

        if (!inputJump)
            movementJumping = false;

        // Record timestamp
        // TODO: Record separate logs for position, animation and flips
        historyTimestamps.Add(GameController.main.gameTime);
        historyInputX.Add(inputMove);
        historyInputJump.Add(inputJump);
        historyPosition.Add(transform.position);
        historyAnimation.Add(animatorCurrent);
        historyFlip.Add(spriteRenderer.flipX);
    }

    public void InputMove(InputAction.CallbackContext context) {
        inputMove = context.ReadValue<Vector2>().x;
        Debug.Log(inputMove);
    }

    public void InputJump(InputAction.CallbackContext context) {
        inputJump = context.ReadValue<float>() > 0.0f;
    }

    public void ChangeAnimation(string animation) {
        if (animation != animatorCurrent) {
            animator.Play(animation);
            animatorCurrent = animation;
        }
    }

    public void OnDrawGizmos() {
        bool grounded = Physics2D.OverlapCircle(transform.position + movementJumpFeetPos, movementJumpFeetRadius, movementGround);
        if (grounded)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + movementJumpFeetPos, movementJumpFeetRadius);
    }
}
