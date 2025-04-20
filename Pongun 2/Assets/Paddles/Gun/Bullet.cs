using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{   
    public static float bulletSpeed = 21f;

    const float maxLifeTime = 2f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D hitbox;

    public Paddle BulletOwner { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, maxLifeTime);
        
        // Initialize the bullet's velocity forward.
        rb.velocity = transform.right * bulletSpeed;
    }

    public void Init(Paddle bulletOwner)
    {
        BulletOwner = bulletOwner;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float timeToDestroy = 0.3f;     // the default time to destroy the bullet
        
        // If bullet hits a ball or bullet, destroy the bullet.
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ball") || collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            // Bullet no longer interacts with anything.
            Destroy(hitbox);
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.1f);
        }
        else
        {
            // Bullet still dies, but after a longer time.
            timeToDestroy = 0.7f;
        }

        Destroy(gameObject, timeToDestroy);
    }
}
