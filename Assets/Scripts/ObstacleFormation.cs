using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class ObstacleFormation : MonoBehaviour
{
    [Header("Formation Settings")]
    public int numberOfObstacles = 4;
    public float radius = 3f;
    public float rotationSpeed = 30f;
    public bool clockwise = true;

    [Header("Obstacle Prefab")]
    public GameObject obstaclePrefab;

    [Header("Visual")]
    public bool showGizmos = true;

    private GameObject[] obstacles;

    void Start()
    {
        CreateFormation();
    }

    void CreateFormation()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("ObstaclePrefab no asignado!");
            return;
        }

        obstacles = new GameObject[numberOfObstacles];

        for (int i = 0; i < numberOfObstacles; i++)
        {
            float angle = (360f / numberOfObstacles) * i;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            Vector3 position = transform.position + new Vector3(x, y, 0);

            GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity, transform);
            obstacle.name = $"FormationObstacle_{i}";
            obstacles[i] = obstacle;
        }
    }

    void Update()
    {
        // Rotar toda la formación
        float direction = clockwise ? -1f : 1f;
        transform.Rotate(Vector3.forward * rotationSpeed * direction * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.red;

        // Dibujar círculo
        int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = transform.position + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = transform.position + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0
            );
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }

        // Dibujar posiciones de obstáculos
        for (int i = 0; i < numberOfObstacles; i++)
        {
            float angle = (360f / numberOfObstacles) * i * Mathf.Deg2Rad;
            Vector3 pos = transform.position + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0
            );
            Gizmos.DrawWireSphere(pos, 0.5f);
        }
    }
}