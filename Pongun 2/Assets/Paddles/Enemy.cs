using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Paddle
{
    // Store different difficulties of enemy.
    [System.Serializable] public struct Difficulty
    {
        // The name of the difficulty.
        public string name;
        
        // Determines how far away the ball needs to be for the enemy to follow it.
        // This only applies to the y-axis.
        public float distanceNeededToFollow;
        
        // Determines how often the enemy fires.
        // The max amount of random time between shots, not counting shooting delay.
        public float maxFiringDelay;

        // Determines how accurate the enemy is when leading its shots.
        // 0 is perfect accuracy, 1 is no accuracy.
        public float leadingShotInaccuracy;
    }
    public Difficulty[] difficulties;
    Difficulty currentDifficulty;

    Vector2 ballPosition;

    private float fireableTimepoint;

    private void Start()
    {
        // Test code
        //currentDifficulty = difficulties[difficultySelector];

        currentDifficulty = difficulties[MainMenu.EnemyDifficulty];
    }

    protected override void OnUpdate()
    {
        // Store the ball's position
        ballPosition = Ball.Instance.GetRigidbody().position;

        // Predict the ball's position and point the gun at it.
        Vector2 aimPosition = Ball.Instance.PositionAfterSeconds(Vector2.Distance(ballPosition, gun.SpawnPosition) / Bullet.bulletSpeed);
        gun.PointGunAtPosition(aimPosition);

        // If the ball is far away enough, vertically, but close enough, horizontally, then blink.
        //if (Mathf.Abs(ballPosition.y - rb.position.y) > 1f
        //    && Mathf.Abs(ballPosition.x - rb.position.x) < 1f)
        //{
        //    DoSpecial();
        //}
    }

    public override void Actionable()
    {
        fireableTimepoint = Time.time + currentDifficulty.maxFiringDelay;

        base.Actionable();
    }

    protected override void Move()
    {
        // The vertical difference/distance between the ball and the paddle.
        float verticalDistance = ballPosition.y - rb.position.y;

        // If the ball is far enough away, vertically...
        if (Mathf.Abs(verticalDistance) > currentDifficulty.distanceNeededToFollow)
        {
            // Have the rigidbody follow y-position of the ball at a certain speed.
            Vector3 newPos = rb.position + (paddleSpeed * Time.fixedDeltaTime * new Vector2(0f, verticalDistance).normalized);
            newPos.x = distFromCenter;
			rb.MovePosition(newPos);
        }
    }

	protected override void Fire()
    {
        if (Time.time - fireableTimepoint < 0) { return; }

        fireableTimepoint = Time.time + Random.Range(0f, currentDifficulty.maxFiringDelay);

        // Predict the ball's position and point the gun at it.
        Vector2 aimPosition = Ball.Instance.PositionAfterSeconds(Vector2.Distance(ballPosition, gun.SpawnPosition) / Bullet.bulletSpeed, currentDifficulty.leadingShotInaccuracy);
        gun.PointGunAtPosition(aimPosition);

        gun.Fire();
    }
}
