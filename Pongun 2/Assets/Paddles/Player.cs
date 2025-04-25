using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : Paddle
{   
    [Header("Audio")]
    [SerializeField] private AudioClip blinkReady;
    
    private Vector2 inputDirection;

    // Update is called once per frame
    protected override void OnUpdate()
    {
        // Use W and S to move the pong paddle up and down.
        inputDirection = new Vector2(0, Input.GetAxisRaw("Vertical"));

        // Get mouse position in world space and send it to the visuals.
        gun.PointGunAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoSpecial();
        }
    }

    protected override void Move()
    {
        // Use the paddle's rigidbody to move the paddle.
        Vector3 newPos = rb.position + (paddleSpeed * Time.fixedDeltaTime * inputDirection);
        newPos.x = -distFromCenter;

		rb.MovePosition(newPos);
    }

	protected override void Fire()
    {
        // When click, fire the gun.
        if (Input.GetMouseButtonDown(0))
        {
            gun.Fire();
        }
    }

    protected override void EnableSpecial(bool enabled, bool playSounds = true)
    {
        base.EnableSpecial(enabled, playSounds);

        if (playSounds)
        {
            if (enabled)
            {
                sounds.PlayOneShot(blinkReady);
            }
        }
    }
}
