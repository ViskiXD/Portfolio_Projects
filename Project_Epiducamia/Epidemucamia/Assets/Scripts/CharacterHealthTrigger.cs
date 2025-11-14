using UnityEngine;

public class CharacterHealthTrigger : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Cutscene Settings")]
    [SerializeField] private CutsceneManager cutsceneManager;
    [SerializeField] private bool triggerOnce = true; // Trigger cutscene only once
    private bool hasTriggered = false;

    [Header("Optional: Specific Health Threshold")]
    [SerializeField] private bool useHealthThreshold = false;
    [SerializeField] private float healthThreshold = 50f; // Trigger when health drops below this

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Call this method when the character takes damage
    public void TakeDamage(float damage)
    {
        if (hasTriggered && triggerOnce)
            return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {currentHealth}");

        // Check if we should trigger the cutscene
        if (useHealthThreshold)
        {
            // Trigger when health drops below threshold
            if (currentHealth <= healthThreshold && !hasTriggered)
            {
                TriggerCutscene();
            }
        }
        else
        {
            // Trigger immediately when shot (any damage)
            if (!hasTriggered)
            {
                TriggerCutscene();
            }
        }

        // Optional: Destroy or disable character when health reaches 0
        if (currentHealth <= 0)
        {
            OnCharacterDeath();
        }
    }

    // This method is called when the character is hit by a raycast (hitscan weapon)
    public void OnHitByRaycast(float damage)
    {
        TakeDamage(damage);
    }

    // This method can be called by collision detection (projectile weapons)
    private void OnCollisionEnter(Collision collision)
    {
        // Check if hit by a bullet/projectile
        if (collision.gameObject.CompareTag("Bullet"))
        {
            float damage = 10f; // Default damage, or get from bullet script
            
            // Try to get damage from bullet script if it exists
            var bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                damage = bullet.damage;
            }

            TakeDamage(damage);

            // Destroy the bullet
            Destroy(collision.gameObject);
        }
    }

    private void TriggerCutscene()
    {
        if (cutsceneManager != null)
        {
            hasTriggered = true;
            Debug.Log("Triggering cutscene for " + gameObject.name);
            cutsceneManager.StartCutscene();
        }
        else
        {
            Debug.LogError("CutsceneManager is not assigned on " + gameObject.name);
        }
    }

    private void OnCharacterDeath()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Add death logic here (ragdoll, animation, etc.)
    }

    // Public method to manually trigger the cutscene (for testing)
    public void ManualTriggerCutscene()
    {
        if (!hasTriggered || !triggerOnce)
        {
            TriggerCutscene();
        }
    }
}

