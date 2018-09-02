using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanderController : MonoBehaviour
{

    private float lastSoundPlay = 0f;
    private Health health;

    public AudioClip softLandingSound;
    public AudioClip hardLandingSound;


    void Start()
    {
        lastSoundPlay += 1 * Time.deltaTime;
        health = GetComponent<Health>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        float magnitude = collision.relativeVelocity.magnitude;
        if (collision.gameObject.tag != "LandingPad")
        {
            magnitude = magnitude * 10;
        }

        if (magnitude > 0.5)
        {
            lastSoundPlay = 0;
            AudioSource.PlayClipAtPoint(hardLandingSound, transform.position, magnitude + 0.5f);
            health.TakeDamage(magnitude / 2);
        }
        else
        {
            lastSoundPlay = 0;
            AudioSource.PlayClipAtPoint(softLandingSound, transform.position, magnitude);
            health.TakeDamage(magnitude / 4);
        }
    }
}
