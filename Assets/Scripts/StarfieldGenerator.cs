using UnityEngine;

public class StarfieldGenerator : MonoBehaviour
{
    [Header("Starfield Settings")]
    [Range(500, 5000)]
    public int numberOfStars = 2000;

    [Range(0.05f, 0.5f)]
    public float starSize = 0.15f;

    public Color starColor = Color.white;

    [Range(0.1f, 1f)]
    public float brightnessVariation = 0.5f;

    [Range(0.5f, 5f)]
    public float twinkleSpeed = 2f;

    public bool animateTwinkling = true;

    [Header("Coverage Area")]
    [Tooltip("Área horizontal que cubrirán las estrellas")]
    public float horizontalRange = 80f;

    [Tooltip("Área vertical que cubrirán las estrellas")]
    public float verticalRange = 40f;

    [Tooltip("Centro de las estrellas (ajustar si el nivel no está en 0,0)")]
    public Vector2 centerPosition = Vector2.zero;

    [Header("Color Variation")]
    public bool useColorVariation = true;
    public Color blueStarTint = new Color(0.7f, 0.8f, 1f);
    public Color yellowStarTint = new Color(1f, 0.95f, 0.8f);

    private GameObject starContainer;

    void Start()
    {
        GenerateStarfield();
    }

    void GenerateStarfield()
    {
        // Crear contenedor para organizar
        starContainer = new GameObject("Stars_Container");
        starContainer.transform.parent = transform;
        starContainer.transform.localPosition = Vector3.zero;

        // Crear estrellas
        for (int i = 0; i < numberOfStars; i++)
        {
            CreateStar(i);
        }

        Debug.Log($"✓ {numberOfStars} estrellas generadas");
    }

    void CreateStar(int index)
    {
        GameObject star = GameObject.CreatePrimitive(PrimitiveType.Quad);
        star.name = "Star_" + index;
        star.transform.parent = starContainer.transform;

        // Posición aleatoria en el área definida
        float x = Random.Range(-horizontalRange / 2f, horizontalRange / 2f) + centerPosition.x;
        float y = Random.Range(-verticalRange / 2f, verticalRange / 2f) + centerPosition.y;
        float z = transform.position.z;

        star.transform.position = new Vector3(x, y, z);

        // Tamaño aleatorio
        float size = starSize * Random.Range(0.5f, 1.5f);
        star.transform.localScale = new Vector3(size, size, 1f);

        // Color de la estrella
        Color finalColor = starColor;

        if (useColorVariation)
        {
            float colorChoice = Random.Range(0f, 1f);

            if (colorChoice < 0.7f)
            {
                // 70% estrellas blancas
                finalColor = starColor;
            }
            else if (colorChoice < 0.9f)
            {
                // 20% estrellas azuladas
                finalColor = blueStarTint;
            }
            else
            {
                // 10% estrellas amarillentas
                finalColor = yellowStarTint;
            }
        }

        // Brillo aleatorio
        finalColor.a = Random.Range(brightnessVariation, 1f);

        // Crear material
        Material mat = new Material(Shader.Find("Unlit/Transparent"));
        mat.color = finalColor;
        star.GetComponent<Renderer>().material = mat;

        // Remover collider (no necesario)
        Destroy(star.GetComponent<Collider>());

        // Añadir parpadeo
        if (animateTwinkling)
        {
            StarTwinkle twinkle = star.AddComponent<StarTwinkle>();
            twinkle.speed = twinkleSpeed * Random.Range(0.8f, 1.2f);
        }
    }

    // Visualizar área en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(centerPosition.x, centerPosition.y, transform.position.z);
        Gizmos.DrawWireCube(center, new Vector3(horizontalRange, verticalRange, 0.1f));
    }
}

// Script de parpadeo
public class StarTwinkle : MonoBehaviour
{
    public float speed = 2f;
    private Material material;
    private float baseAlpha;
    private float timeOffset;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        baseAlpha = material.color.a;
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float t = Time.time * speed + timeOffset;
        float alpha = baseAlpha * (0.3f + 0.7f * (0.5f + 0.5f * Mathf.Sin(t)));

        Color color = material.color;
        color.a = alpha;
        material.color = color;
    }
}