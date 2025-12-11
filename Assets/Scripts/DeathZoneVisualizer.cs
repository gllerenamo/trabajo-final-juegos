using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class DeathZoneVisualizer : MonoBehaviour
{
    [Header("Settings")]
    public bool showLines = true;
    public float lineWidth = 0.15f;
    public float blinkSpeed = 1.5f;

    [Header("Colors")]
    public Color sideColor = Color.red;
    public Color topBottomColor = Color.yellow;

    private LineRenderer[] lines;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.instance;

        if (gameManager != null && showLines)
        {
            CreateLines();
        }
    }

    void CreateLines()
    {
        lines = new LineRenderer[4];
        string[] names = { "LeftLine", "RightLine", "TopLine", "BottomLine" };

        for (int i = 0; i < 4; i++)
        {
            GameObject lineObj = new GameObject(names[i]);
            lineObj.transform.parent = transform;

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;

            // Material simple
            lr.material = new Material(Shader.Find("Sprites/Default"));

            // No queremos que afecte la iluminación
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;

            lines[i] = lr;
        }

        UpdateLines();
    }

    void Update()
    {
        if (lines == null || !showLines) return;

        // Efecto de parpadeo suave
        float alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * blinkSpeed);

        // Líneas laterales (rojas)
        Color sideColorWithAlpha = sideColor;
        sideColorWithAlpha.a = alpha;
        lines[0].startColor = sideColorWithAlpha;
        lines[0].endColor = sideColorWithAlpha;
        lines[1].startColor = sideColorWithAlpha;
        lines[1].endColor = sideColorWithAlpha;

        // Líneas arriba/abajo (amarillas)
        Color topBottomColorWithAlpha = topBottomColor;
        topBottomColorWithAlpha.a = alpha;
        lines[2].startColor = topBottomColorWithAlpha;
        lines[2].endColor = topBottomColorWithAlpha;
        lines[3].startColor = topBottomColorWithAlpha;
        lines[3].endColor = topBottomColorWithAlpha;
    }

    void UpdateLines()
    {
        if (gameManager == null || lines == null) return;

        float left = gameManager.deathZoneLeft;
        float right = gameManager.deathZoneRight;
        float bottom = gameManager.deathZoneY;
        float top = gameManager.deathZoneTop;
        float z = 2f; // Delante de fondo pero detrás de objetos

        // Línea izquierda
        lines[0].SetPosition(0, new Vector3(left, bottom, z));
        lines[0].SetPosition(1, new Vector3(left, top, z));

        // Línea derecha
        lines[1].SetPosition(0, new Vector3(right, bottom, z));
        lines[1].SetPosition(1, new Vector3(right, top, z));

        // Línea superior
        lines[2].SetPosition(0, new Vector3(left, top, z));
        lines[2].SetPosition(1, new Vector3(right, top, z));

        // Línea inferior
        lines[3].SetPosition(0, new Vector3(left, bottom, z));
        lines[3].SetPosition(1, new Vector3(right, bottom, z));
    }
}