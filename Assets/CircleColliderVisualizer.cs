using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[ExecuteAlways]
public class CircleColliderVisualizer : MonoBehaviour
{
    [SerializeField] private int segments = 64;
    [SerializeField] private float lineWidth = 0.04f;
    [SerializeField] private Color lineColor = Color.white;

    private CircleCollider2D circleCollider;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        EnsureLineRenderer();
        UpdateCircle();
    }

    private void OnValidate()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        EnsureLineRenderer();
        UpdateCircle();
    }

    private void LateUpdate()
    {
        UpdateCircle();
    }

    private void EnsureLineRenderer()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.textureMode = LineTextureMode.Stretch;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.numCapVertices = 4;
        lineRenderer.numCornerVertices = 4;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.sortingOrder = 100;
    }

    private void UpdateCircle()
    {
        if (circleCollider == null || lineRenderer == null)
            return;

        int pointCount = Mathf.Max(segments, 3);
        lineRenderer.positionCount = pointCount;

        Vector3 center = transform.TransformPoint(circleCollider.offset);
        float radius = circleCollider.radius * GetMaxScale();

        for (int i = 0; i < pointCount; i++)
        {
            float angle = i / (float)pointCount * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(center.x + x, center.y + y, center.z));
        }
    }

    private float GetMaxScale()
    {
        Vector3 scale = transform.lossyScale;
        return Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
    }

    private void OnDrawGizmosSelected()
    {
        if (circleCollider == null)
            circleCollider = GetComponent<CircleCollider2D>();

        if (circleCollider == null)
            return;

        Gizmos.color = lineColor;
        DrawWireCircle(
            transform.TransformPoint(circleCollider.offset),
            circleCollider.radius * GetMaxScale(),
            segments);
    }

    private static void DrawWireCircle(Vector3 center, float radius, int pointCount)
    {
        Vector3 previousPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= pointCount; i++)
        {
            float angle = i / (float)pointCount * Mathf.PI * 2f;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, center.z);
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}
