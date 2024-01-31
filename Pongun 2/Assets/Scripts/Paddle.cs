using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Paddle : MonoBehaviour
{
    protected const float paddleSpeed = 8f;
    protected const float blinkInterval = 5f;
    protected const float blinkDuration = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioClip blinkUse;

    [Header("Components")]
    [SerializeField] protected PaddleGun gun;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected AudioSource sounds;

    [SerializeField] private SpriteRenderer outline;

    protected bool canAction = false;
    protected bool canBlink = false;

    protected float lastBlinkTime;

    private bool roundStarting = false;

    private void Awake()
    {
        // Initialize the paddle's gun.
        gun.Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!roundStarting && !canAction && Time.time - lastBlinkTime > blinkDuration)
        {
            canAction = true;
        }
        
        if (canAction) {
            Fire();

            if (!canBlink && Time.time - lastBlinkTime >= blinkInterval)
            {
                EnableBlink(true);
            }
        }

        OnUpdate();
    }

    protected virtual void OnUpdate() { }

    void FixedUpdate()
    {
        if (canAction) {
            Move();
        }

        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate() { }

    /// <summary>
    /// Enable or disable the paddle's ability to move.
    /// </summary>
    /// <param name="canAction">Whether or not the paddle can move.</param>
    public void SetCanAction(bool canAction)
    {
        this.canAction = canAction;
    }

    public void Reset()
    {
        roundStarting = true;
        canAction = false;

        EnableBlink(false, false);
        lastBlinkTime = Time.time - 1.5f;

        rb.velocity = Vector2.zero;
    }

    public virtual void Actionable()
    {
        roundStarting = false;
        canAction = true;
    }

    /// <summary>
    /// Move the paddle. This is called in FixedUpdate, and is implemented in child classes.
    /// </summary>
    protected abstract void Move();

    /// <summary>
    /// Fire the paddle's gun.
    /// </summary>
    protected abstract void Fire();

    protected virtual void EnableBlink(bool enabled, bool playSounds = true)
    {
        canBlink = enabled;

        if (enabled)
        {
            outline.color = Color.cyan;
        }
        else
        {
            // Disable the paddle's ability to move.
            canAction = false;

            // Update the last blink time.
            lastBlinkTime = Time.time;

            outline.color = Color.black;
        }
    }

    protected void Blink()
    {
        if (!canBlink) { return; }

        EnableBlink(false);

        sounds.PlayOneShot(blinkUse);

        //  Move the paddle to horizontally aligned with the ball.
        float ballY = Ball.Instance.GetRigidbody().position.y;
        rb.MovePosition(new Vector2(rb.position.x, ballY));
    }
}
