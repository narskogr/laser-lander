using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;

    public List<Vector2> edgeVertices = new List<Vector2>();

    public int leftEdge = -10;
    public int rightEdge = 10;

    [Range(1, 1000)]
    public int segments = 20;

    [Range(0, 100)]
    public float majorPower = 25f;
    [Range(0, 10)]
    public float minorPower = 10f;
    [Range(0, 1)]
    public float microPower = 5f;
    [Range(0.1f, 100)]
    public float majorScale = 28f;
    [Range(0.1f, 10)]
    public float minorScale = 3f;
    [Range(0.1f, 1)]
    public float microScale = 1f;

    public float offset = 10f;

    // public bool snapEdges = false;
    // public float snapLeftHeight = 0f;
    // public float snapRightHeight = 0f;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
    }

    public void GenerateLine()
    {
        int pointCount = segments + 1;
        float length = Mathf.Abs(leftEdge - rightEdge);

        lineRenderer.positionCount = pointCount;

        // Generate line
        for (int i = 0; i < pointCount; i++)
        {
            float x = leftEdge + (length / segments * i);

            float majorHeight = Mathf.PerlinNoise(x / majorScale + offset, 1 / majorScale) * majorPower;
            float minorHeight = Mathf.PerlinNoise(x / minorScale + offset, 1 / minorScale) * minorPower;
            float microHeight = Mathf.PerlinNoise(x / microScale + offset, 1 / microScale) * microPower;
            float height = majorHeight + minorHeight + microHeight;

            Vector2 point = new Vector2(x, height);
            lineRenderer.SetPosition(i, point);
            edgeVertices.Add(point);
        }

        // Snap left and right edges
        // if (snapEdges)
        // {
        //     Vector2 leftPoint = new Vector2(leftEdge, snapLeftHeight);
        //     Vector2 rightPoint = new Vector2(rightEdge, snapRightHeight);
        //     lineRenderer.SetPosition(0, leftPoint);
        //     lineRenderer.SetPosition(segments + 1, rightPoint);
        // }

        // Set Edge Collider
        edgeCollider.points = edgeVertices.ToArray();
    }
}
