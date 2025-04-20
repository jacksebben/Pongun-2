using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Paddle : MonoBehaviour
{
    protected const float paddleSpeed = 8f;
    protected const float specialInterval = 5f;
    protected const float specialDuration = 0.1f;

    [Header("Audio")]
	[SerializeField] private AudioClip specialUse;

    [Header("Components")]
    [SerializeField] protected PaddleGun gun;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected AudioSource sounds;

    [SerializeField] private SpriteRenderer outline;

    protected bool canAction = false;
    protected bool canSpecial = false;

    protected float lastTimeSpecial;

    private bool roundStarting = false;

    private void Awake()
    {
        // Initialize the paddle's gun.
        gun.Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!roundStarting && !canAction && Time.time - lastTimeSpecial > specialDuration)
        {
            canAction = true;
        }
        
        if (canAction) {
            Fire();

            if (!canSpecial && Time.time - lastTimeSpecial >= specialInterval)
            {
                EnableSpecial(true);
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

        EnableSpecial(false, false);
        lastTimeSpecial = Time.time - 1.5f;

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

    protected virtual void EnableSpecial(bool enabled, bool playSounds = true)
    {
        canSpecial = enabled;

        if (enabled)
        {
            outline.color = Color.yellow;
        }
        else
        {
            // Disable the paddle's ability to move.
            canAction = false;

            // Update the last blink time.
            lastTimeSpecial = Time.time;

            outline.color = Color.black;
        }
    }

    protected void DoSpecial()
    {
        if (!canSpecial) { return; }

        if (!Ball.Instance.TryFlipVelocity(transform.position.x < 0 ? -1 : 1))
            return;

        EnableSpecial(false);

        sounds.PlayOneShot(specialUse);
    }
}
