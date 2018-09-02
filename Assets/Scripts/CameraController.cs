using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Camera cameraComponent;
    public Transform target;
    public Transform anchor;
    float bottom = -4.0f;
    float above = 1.5f;
    float minView = 7.5f;

    void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target)
        {
            float height = target.position.y + above - bottom;
            height = Mathf.Max(height, minView) / 2.0f;
            cameraComponent.orthographicSize = height;
            transform.position = new Vector3(target.position.x - anchor.position.x, bottom + height, -10);
        }
    }
}
