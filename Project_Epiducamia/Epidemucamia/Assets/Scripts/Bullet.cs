using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float damage = 10f;
    [SerializeField] private float bulletSpeed = 50f;
    [SerializeField] private float lifetime = 5f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
        }

        // Destroy bullet after lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // The damage will be handled by CharacterHealthTrigger
        // Just destroy the bullet
        Destroy(gameObject);
    }
}

