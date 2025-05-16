using System.Collections.Generic; // For HashSet
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float damageAmount = 25f; // Base damage of the sword
    public float knockbackForce = 10f; // Force applied for knockback
    public LayerMask knockbackLayer; // Layer for objects that can be knocked back
    public DamageType damageType = DamageType.Physical; // Default damage type
    public Transform owner; // The owner of the sword (e.g., player or enemy)
    public Collider swordCollider; // Reference to the collider, which can be on another GameObject

    [SerializeField]
    public HashSet<Collider> hitObjects = new HashSet<Collider>(); // Tracks objects already hit
    public bool isSwinging = false; // Indicates whether the sword is actively swinging
    public LayerMask targetLayer;

    private void Start()
    {
        // Ensure the collider is assigned
        // if (swordCollider == null)
        // {
        //     swordCollider = GetComponentInChildren<Collider>();
        //     if (swordCollider == null)
        //     {
        //         Debug.LogError("Sword collider is not assigned and no child collider found!");
        //     }
        // }
    }

    private void OnEnable()
    {
        if (swordCollider != null)
        {
            swordCollider.isTrigger = true; // Ensure it's a trigger
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSwinging) return; // Only deal damage if the sword is swinging
        if (hitObjects.Contains(other)) return; // Skip if already hit during this swing

        if ((1 << other.gameObject.layer & knockbackLayer.value) != 0) // Check if the layer matches
        {
            // Check if the collided object has a Rigidbody for knockback
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 forceDirection = (hitPoint - transform.position).normalized; // Direction of the force

                // Apply force at the hit point
                rb.AddForceAtPosition(forceDirection * knockbackForce, hitPoint, ForceMode.Impulse);
            }
        }

        // Check if the collided object is on the target layer
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0) return;

        // Check if the collided object has a DamageReceiver
        DamageReceiver damageReceiver = other.GetComponent<DamageReceiver>();
        if (damageReceiver != null)
        {
            // Create DamageData
            DamageData damageData = new DamageData(
                damageAmount,
                other.ClosestPoint(transform.position), // Position of the hit
                other.transform.up, // Normal of the hit
                owner != null ? owner.gameObject : null, // Attacker
                damageType // Type of damage
            );

            // Apply damage
            damageReceiver.TakeDamage(damageData);

            // Add the object to the hit list
            hitObjects.Add(other);
        }
    }

    /// <summary>
    /// Starts the sword swing, allowing damage to be dealt.
    /// </summary>
    public void StartSwing()
    {
        isSwinging = true;
        hitObjects.Clear(); // Clear the list of hit objects for a new swing
    }

    /// <summary>
    /// Ends the sword swing, preventing further damage.
    /// </summary>
    public void EndSwing()
    {
        isSwinging = false;
        hitObjects.Clear(); // Clear the list after the swing ends
    }
}
