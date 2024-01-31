using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleGun : MonoBehaviour
{
    const float fireDelay = 0.52f;
    
    [SerializeField] private GameObject bulletPrefab;

    public Paddle Owner { get; private set; }

    public Vector2 SpawnPosition { get; private set;}

    private float lastFireTime;
    
    // Start is called before the first frame update
    void Start()
    {
        lastFireTime = Time.time;
    }

    public void Init(Paddle owner)
    {
        Owner = owner;
    }

    private void Update()
    {
        // Get position slightly in front of the gun, on lower layer.
        SpawnPosition = transform.position + (transform.right * 0.5f) + new Vector3(0f, 0f, 3f);
    }

    public void PointGunAtPosition(Vector3 position)
    {
        // Point the tip of the gun towards the position.
        Vector3 direction = position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Fire()
    {
        if (Time.time - lastFireTime < fireDelay) { return; }
        
        lastFireTime = Time.time;
        
        // Spawns a bullet at the tip of the gun.
        Instantiate(bulletPrefab, SpawnPosition, transform.rotation).GetComponent<Bullet>().Init(Owner);
    }
}
