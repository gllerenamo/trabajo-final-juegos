using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MovingObstacle : MonoBehaviour
{
    [Header("Movement Type")]
    [Tooltip("Tipo de movimiento del obstáculo")]
    public MovementType movementType = MovementType.Horizontal;

    public enum MovementType
    {
        Horizontal,
        Vertical,
        Circular,
        PingPong
    }

    [Header("Movement Settings")]
    [Tooltip("Velocidad del movimiento")]
    [Range(0.5f, 5f)]
    public float moveSpeed = 2f;

    [Tooltip("Distancia de movimiento desde el punto inicial")]
    [Range(1f, 10f)]
    public float moveRange = 4f;

    [Tooltip("Invertir dirección del movimiento")]
    public bool invertDirection = false;

    [Header("Visual")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.yellow;

    private Vector3 startPosition;
    private float timeCounter = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        timeCounter += Time.deltaTime * moveSpeed;

        float direction = invertDirection ? -1f : 1f;

        switch (movementType)
        {
            case MovementType.Horizontal:
                MoveHorizontal(direction);
                break;

            case MovementType.Vertical:
                MoveVertical(direction);
                break;

            case MovementType.Circular:
                MoveCircular(direction);
                break;

            case MovementType.PingPong:
                MovePingPong(direction);
                break;
        }
    }

    void MoveHorizontal(float direction)
    {
        float xOffset = Mathf.Sin(timeCounter) * moveRange * direction;
        transform.position = startPosition + new Vector3(xOffset, 0, 0);
    }

    void MoveVertical(float direction)
    {
        float yOffset = Mathf.Sin(timeCounter) * moveRange * direction;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    void MoveCircular(float direction)
    {
        float x = Mathf.Cos(timeCounter * direction) * moveRange;
        float y = Mathf.Sin(timeCounter * direction) * moveRange;
        transform.position = startPosition + new Vector3(x, y, 0);
    }

    void MovePingPong(float direction)
    {
        float offset = Mathf.PingPong(timeCounter * direction, moveRange * 2f) - moveRange;

        // Decidir si se mueve horizontal o vertical según la configuración
        if (Mathf.Abs(startPosition.x) > Mathf.Abs(startPosition.y))
        {
            transform.position = startPosition + new Vector3(0, offset, 0);
        }
        else
        {
            transform.position = startPosition + new Vector3(offset, 0, 0);
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Vector3 pos = Application.isPlaying ? startPosition : transform.position;

        Gizmos.color = gizmoColor;

        switch (movementType)
        {
            case MovementType.Horizontal:
                Gizmos.DrawLine(pos + Vector3.left * moveRange, pos + Vector3.right * moveRange);
                break;

            case MovementType.Vertical:
                Gizmos.DrawLine(pos + Vector3.down * moveRange, pos + Vector3.up * moveRange);
                break;

            case MovementType.Circular:
                DrawCircleGizmo(pos, moveRange);
                break;

            case MovementType.PingPong:
                Gizmos.DrawLine(pos + Vector3.left * moveRange, pos + Vector3.right * moveRange);
                Gizmos.DrawLine(pos + Vector3.down * moveRange, pos + Vector3.up * moveRange);
                break;
        }
    }

    void DrawCircleGizmo(Vector3 center, float radius)
    {
        int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}