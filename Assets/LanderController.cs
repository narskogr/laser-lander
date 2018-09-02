using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanderController : MonoBehaviour
{

    private float lastSoundPlay = 0f;

    public float health = 1f;

    public Canvas UICanvas;
    public RectTransform healthBar;

    public Transform explosionPrefab;
    public AudioClip softLandingSound;
    public AudioClip hardLandingSound;
    public Camera mainCamera;


    void Start()
    {
        // Randomize position
        Vector3 randomPosition = new Vector3(Random.Range(4f, -4f), Random.Range(3f, 4f), 0f);
        transform.position = randomPosition;
        lastSoundPlay += 1 * Time.deltaTime;
    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    void LateUpdate()
    {
        int healthPercent = Mathf.RoundToInt(health / 1f * 100);
        healthBar.sizeDelta = new Vector2(healthPercent, healthBar.sizeDelta.y);
        Vector2 landerScreenPosition = RectTransformUtility.WorldToScreenPoint(mainCamera, transform.position);
        Vector2 healthBarPosition = new Vector2(Mathf.Round(landerScreenPosition.x), Mathf.Round(landerScreenPosition.y + 40));
        healthBar.position = healthBarPosition;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        float magnitude = collision.relativeVelocity.magnitude;
        if (collision.gameObject.tag != "LandingPad")
        {
            magnitude = magnitude * 5;
        }

        if (magnitude < 0.5)
        {
            lastSoundPlay = 0;
            AudioSource.PlayClipAtPoint(hardLandingSound, transform.position, magnitude + 0.5f);
            health -= magnitude / 5;
        }
        else
        {
            lastSoundPlay = 0;
            AudioSource.PlayClipAtPoint(softLandingSound, transform.position, magnitude);
            health -= magnitude / 5;
        }
    }

    void Die()
    {
        healthBar.transform.GetComponent<Image>().enabled = false;
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
