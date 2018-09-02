using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    public float healthCurrent = 10f;
    public float healthMax = 10f;

    public GameObject explosion;

    public void TakeDamage(float amount)
    {
        healthCurrent -= amount;
        if (healthCurrent <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
