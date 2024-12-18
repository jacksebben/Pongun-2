using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    public static Ball Instance;

    public static UnityEvent<Speed, SpeedInfo> OnBallSpeedIncrease = new();

    public enum Speed
    {
        Normal,
        Fast,
        Ludicrous,
        Overdrive,
        Unprecedented
    }
    public struct SpeedInfo
    {
        public float startingSpeed;
        public float growthRate;

        public Color color;
        public float particleLifetime;

    }
    private Speed currentSpeedState = Speed.Normal;
    private readonly Dictionary<Speed, SpeedInfo> speedDict = new()
    {
        {Speed.Normal, new SpeedInfo() {
            startingSpeed = 6.0f,
            growthRate = 1.5f,
            color = Color.clear,
            particleLifetime = 0f,
        }},
        {Speed.Fast, new SpeedInfo() {
            startingSpeed = 10f,
            growthRate = 0.7f,
            color = Color.yellow,
            particleLifetime = 0.1f,
        }},
        {Speed.Ludicrous, new SpeedInfo() {
            startingSpeed = 16f,
            growthRate = 0.5f,
            color = Color.red,
            particleLifetime = 0.15f,
        }},
        {Speed.Overdrive, new SpeedInfo() {
            startingSpeed = 20f,
            growthRate = 0.3f,
            color = Color.magenta,
            particleLifetime = 0.2f,
        }},
        {Speed.Unprecedented, new SpeedInfo()
        {
            startingSpeed = 25f,
            growthRate = 0.01f,
            color = Color.cyan,
            particleLifetime = 0.25f,
        }}
    };

    [SerializeField] private AudioClip collisionSfx;
    [SerializeField] private AudioClip speedIncreaseSfx;


    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AudioSource sounds;
    [SerializeField] private ParticleSystem trailParticles;

    private float currentMaxSpeed;
    private float currentSpeedGrowthRate;

    private bool isStarting;

	private void Awake()
    {
        Instance = Instance != null ? Instance : this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarting) { return; }
        
        // Ensure that the ball is moving no faster than the current max speed.
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, currentMaxSpeed);

        // Add slight force to the ball to ensure that it doesn't get stuck.
        // The flight force will pull it towards the horizontal middle of the stage, or the x-axis.
        float yPosition = rb.position.y;
        rb.AddForce(new Vector2(0f, -yPosition * Mathf.Abs(yPosition) * 0.002f));
    }

    /// <summary>
    /// Resets many of the ball's properties, as well as its velocity/position.
    /// </summary>
    public void Reset()
    {
        isStarting = true;
        
        // Reset the current speed state of the ball.
        SetSpeedState(Speed.Normal);

        // Reset the ball's velocity and position.
        rb.velocity = Vector2.zero;

        // Give the ball a random spin.
        float randomDirection = Random.Range(0, 2) * 2 - 1;
        rb.angularVelocity = Random.Range(180f, 360f) * randomDirection;
    }

    public void Shoot()
    {
        isStarting = false;
        
        // Reset the current max speed and growth rate.
        // PS move this back to reset when done testing speeds.
        currentMaxSpeed = speedDict[Speed.Normal].startingSpeed;
        currentSpeedGrowthRate = speedDict[Speed.Normal].growthRate;

        // Use rigidbody to shoot ball in random direction.
        float randomDirection = Random.Range(0, 2) * 2 - 1;
        rb.velocity = new Vector2(Random.Range(-0.5f, 0.5f), randomDirection).normalized * currentMaxSpeed;
    }

    public Vector2 PositionAfterSeconds(float seconds, float inaccuracy = 0f)
    {
        Vector2 ballPosition = rb.position;

        // Predict the ball's position and point the gun at it
        Vector2 aimPosition = ballPosition + rb.velocity * seconds;

        // If ball is going to bounce off of wall, do some Vector math to predict where it will bounce.
        if (Mathf.Abs(aimPosition.y) > 4.2f)
        {
            // Find the bounce position, which is between the ball position and the aim position.
            Vector2 bouncePosition = Vector2.Lerp(ballPosition, aimPosition, (4.2f - Mathf.Abs(ballPosition.y)) / Mathf.Abs(aimPosition.y - ballPosition.y));

            aimPosition = new Vector2(aimPosition.x, aimPosition.y - (2f * (aimPosition.y - bouncePosition.y)));
        }

		// Apply random inaccuracy to the aim (unit circle between 0-3 depending on inaccuracy arg.)
        aimPosition += Random.insideUnitCircle * (inaccuracy * 3);

		return aimPosition;
    }

    public float GetMaxSpeed()
    {
        return currentMaxSpeed;
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    void SetSpeedState(Speed newSpeed)
    {
        // Set the current speed state.
        currentSpeedState = newSpeed;

        // Change the ball's growth rate.
        currentSpeedGrowthRate = speedDict[newSpeed].growthRate;

        // Change the ball's trail particles.
        ParticleSystem.MainModule tpMain = trailParticles.main;
        tpMain.startColor = speedDict[newSpeed].color;
        tpMain.startLifetime = speedDict[newSpeed].particleLifetime;

        OnBallSpeedIncrease.Invoke(newSpeed, speedDict[newSpeed]);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isStarting) { return; }

        // If the ball hits a paddle or a bullet, increase the max speed.
        bool speedStateIncrease = false;
        float newMaxSpeed = currentMaxSpeed;
        if (collision.transform.root.TryGetComponent(out Bullet bullet))
        {
            newMaxSpeed += 0.2f * currentSpeedGrowthRate;

        }
        else if (collision.transform.root.TryGetComponent(out Paddle paddle))
        {
            newMaxSpeed += 0.4f * currentSpeedGrowthRate;
        }
        else
        {
            newMaxSpeed += 0.1f * currentSpeedGrowthRate;
        }

        // Determine if the ball's speed state should change.
        Speed nextState = (Speed)((int)currentSpeedState + 1);
        if (speedDict.ContainsKey(nextState))
        {
            if (newMaxSpeed >= speedDict[nextState].startingSpeed)
            {
                speedStateIncrease = true;
                SetSpeedState(nextState);
            }
        }

        // Set the ball's new max speed.
        currentMaxSpeed = newMaxSpeed;

        // Play sounds for collisions.
        if (!sounds.isPlaying || sounds.time >= 0.02f)
        {
            int speedStateMagnitude = (int)currentSpeedState;

            sounds.pitch = Random.Range(0.7f, 0.85f) + (speedStateMagnitude * 0.15f);
            sounds.PlayOneShot(speedStateIncrease ? speedIncreaseSfx : collisionSfx);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isStarting) { return; }
        
        // If the ball was hit by a bullet, always make it hit away from the bullet.
        if (collision.transform.root.TryGetComponent(out Bullet bullet))
        {
            // Get the bullet's owner.
            Paddle bulletOwner = bullet.BulletOwner;

            // Get either 1 or -1, depending on which side of the paddle the ball is on.
            float side = Mathf.Sign(rb.position.x - bulletOwner.transform.position.x);

            // Set the ball's velocity.
            rb.velocity = new Vector2(Mathf.Abs(rb.velocity.x) * side, rb.velocity.y);
        }
        else if (collision.transform.root.TryGetComponent(out Paddle paddle))
        {
            // Get either 1 or -1, depending on which side of the paddle the ball is on.
            float side = Mathf.Sign(rb.position.x - paddle.transform.position.x);

            // Set the ball's velocity.
            rb.velocity = new Vector2(currentMaxSpeed * side, rb.velocity.y);
        }
    }
}
