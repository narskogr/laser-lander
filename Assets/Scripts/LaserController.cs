using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    private bool firing = false;
    public AudioSource laserFire;
    public AudioSource laserBeam;
    public Transform hitPrefab;

    public Camera mainCamera;
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;
    public float laserMaxLength = 5f;
    public float laserPower = 50f;
    public Transform laserBarrelPoint;

    void Start()
    {
        mainCamera = Camera.main;
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = 0;
    }

    void Update()
    {
        // Point towards mouse
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        // Lock so it can't go 180 degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle > 0)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }

        // Start firing
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0)
        {
            laserFire.Play();
            laserBeam.Play();
            firing = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            firing = false;
            laserBeam.Stop();
        }

        // Fire
        if (firing)
        {
            ShootLaser(laserBarrelPoint.position, laserMaxLength);
            laserLineRenderer.enabled = true;
        }
        else
        {
            laserLineRenderer.enabled = false;
        }

    }

    void ShootLaser(Vector3 sourcePosition, float length)
    {

        // Cast Ray
        Ray2D ray = new Ray2D(sourcePosition, transform.up);
        RaycastHit2D hit;
        hit = Physics2D.Raycast(ray.origin, transform.up, length);
        Vector2 startPosition = ray.origin;
        Vector2 endPosition = ray.GetPoint(length);

        // Get beam length for visuals
        float beamLength = Vector2.Distance(startPosition, endPosition);
        float beamLengthRatio = beamLength / laserMaxLength;

        // Add force to hit
        if (hit.collider)
        {
            // What are we hitting?
            string colliderName = hit.collider.transform.name;

            // Recalculate length variabls
            endPosition = hit.point;
            beamLength = Vector2.Distance(startPosition, endPosition);
            beamLengthRatio = beamLength / laserMaxLength;

            // Add force
            Rigidbody2D targetRigidBody = hit.transform.gameObject.GetComponent<Rigidbody2D>();
            if (targetRigidBody)
            {

                // Add appropriate torque if hitting wings
                float torqueMultiplier = 0f;
                float forceMultiplier = 0f;

                switch (colliderName)
                {
                    case "LeftWingBottomCollider":
                        torqueMultiplier = -1;
                        break;
                    case "LeftWingTopCollider":
                        torqueMultiplier = 1;
                        break;
                    case "RightWingBottomCollider":
                        torqueMultiplier = 1;
                        break;
                    case "RightWingTopCollider":
                        torqueMultiplier = -1;
                        break;
                    case "LanderReflector":
                        forceMultiplier = 1;
                        break;
                    default:
                        forceMultiplier = -1;
                        break;
                }

                Vector2 targetPosition = hit.transform.position;
                Vector3 hitVector = hit.transform.InverseTransformDirection(targetPosition - hit.point);
                hitVector.Normalize();

                // Torque ignores hitpower intentionally
                targetRigidBody.AddTorque(1 * torqueMultiplier * laserPower * Time.deltaTime);
                targetRigidBody.AddForce(hit.transform.up * forceMultiplier * laserPower * Time.deltaTime);

                // Spawn particles!
                Transform laserHitEffect = Instantiate(hitPrefab, hit.point, Quaternion.LookRotation(hitVector, Vector3.up));
                Destroy(laserHitEffect.gameObject, 0.12f);
            }

            // Drain health
            Health healthComponent = hit.transform.gameObject.GetComponent<Health>();
            if (healthComponent && colliderName == "LanderBody")
            {
                healthComponent.TakeDamage(Time.deltaTime);
            }
        }

        // Draw Line
        Debug.DrawLine(startPosition, endPosition);
        laserLineRenderer.endWidth = laserWidth * (1f - beamLengthRatio);
        laserLineRenderer.positionCount = 2;
        laserLineRenderer.SetPosition(0, startPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }
}
