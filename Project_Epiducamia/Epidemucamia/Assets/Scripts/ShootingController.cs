using UnityEngine;

public class ShootingController : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private ShootingMode shootingMode = ShootingMode.Raycast;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.5f;
    
    [Header("Projectile Settings (for Projectile mode)")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    
    [Header("Camera")]
    [SerializeField] private Camera playerCamera;

    private float nextFireTime = 0f;

    public enum ShootingMode
    {
        Raycast,    // Instant hit (hitscan)
        Projectile  // Bullet that travels
    }

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (firePoint == null && playerCamera != null)
        {
            firePoint = playerCamera.transform;
        }
    }

    private void Update()
    {
        // Check for shoot input
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        // Play muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (shootingMode == ShootingMode.Raycast)
        {
            ShootRaycast();
        }
        else if (shootingMode == ShootingMode.Projectile)
        {
            ShootProjectile();
        }
    }

    private void ShootRaycast()
    {
        RaycastHit hit;
        Vector3 shootDirection = playerCamera.transform.forward;

        if (Physics.Raycast(playerCamera.transform.position, shootDirection, out hit, range))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);

            // Check if we hit a character
            CharacterHealthTrigger character = hit.collider.GetComponent<CharacterHealthTrigger>();
            if (character != null)
            {
                character.OnHitByRaycast(damage);
                Debug.Log("Character hit! Damage dealt: " + damage);
            }

            // Spawn impact effect
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }

            // Optional: Draw debug line to show raycast
            Debug.DrawLine(playerCamera.transform.position, hit.point, Color.red, 1f);
        }
    }

    private void ShootProjectile()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is not assigned!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("Fire point is not assigned!");
            return;
        }

        // Instantiate bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // Set bullet damage if it has a Bullet component
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = damage;
        }
    }

    // Optional: Gizmo to show shooting range in editor
    private void OnDrawGizmosSelected()
    {
        if (playerCamera != null && shootingMode == ShootingMode.Raycast)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * range);
        }
    }
}

