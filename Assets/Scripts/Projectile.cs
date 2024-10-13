using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Particle effect that will be instantiated upon hitting the target
    public GameObject FX;

    // The target object the projectile will move towards
    public GameObject target;

    // Indicates whether the projectile is a critical hit
    public bool IsCrit;

    // Amount of damage the projectile will deal
    public int damage;

    // Duration the projectile will exist before being destroyed
    public float lifetime;

    // Indicates whether the target is a player
    public bool istargetPlayer;

    // Indicates whether the projectile will deal damage upon hitting the target
    public bool DealDamage;

    void Start()
    {
        // Check if a target is assigned
        if (target != null)
        {
            // Move the projectile to the target's position over 1 second
            LeanTween.move(gameObject, target.transform.position, 1f).setOnComplete(() =>
            {
                // Instantiate the visual effect if it exists
                if (FX != null)
                {
                    GameObject o = Instantiate(FX, target.transform.position, Quaternion.identity);
                }

                // Deal damage if enabled
                if (DealDamage)
                {
                    // If the target is a player, deal damage to the opponent
                    if (istargetPlayer)
                        PVPManager.Get().DealDamageToOpponent(damage);

                    // If the target has a BattleCardDisplay component, deal damage through it
                    if (target.GetComponent<BattleCardDisplay>())
                        target.GetComponent<BattleCardDisplay>().DealDamage(damage);
                }

                // Set the pet attacking state to false
                SpellManager.IsPetAttacking = false;

                // Destroy the projectile after a short delay
                Destroy(gameObject, 0.1f);
            });
        }
    }

    public void Update()
    {
        // If the target is null (not assigned or destroyed)
        if (target == null)
        {
            // Increment the lifetime of the projectile
            lifetime += Time.deltaTime;

            // If the lifetime exceeds 1 second, log an error and destroy the projectile
            if (lifetime > 1f)
            {
                Debug.LogError("Projectile Destroyed");
                Destroy(gameObject);
            }
        }
    }
}

