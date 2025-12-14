using UnityEngine;

public class LevelConfig : MonoBehaviour
{
    [Header("Death Zone Limits")]
    public float deathZoneY = -10f;
    public float deathZoneTop = 15f;
    public float deathZoneLeft = -30f;
    public float deathZoneRight = 25f;

    void Start()
    {
        // Aplicar configuración al GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.deathZoneY = deathZoneY;
            GameManager.instance.deathZoneTop = deathZoneTop;
            GameManager.instance.deathZoneLeft = deathZoneLeft;
            GameManager.instance.deathZoneRight = deathZoneRight;

            Debug.Log($"Configuración de nivel aplicada: Límites [{deathZoneLeft}, {deathZoneRight}] x [{deathZoneY}, {deathZoneTop}]");
        }
    }
}