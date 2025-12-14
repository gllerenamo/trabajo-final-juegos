using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game State")]
    public bool gameActive = true;

    [Header("Fuel Settings")]
    public float initialFuel = 10f;
    public float fuelConsumptionRate = 0.1f;
    [HideInInspector] public float currentFuel;

    [Header("Score Settings")]
    public float initialScore = 100f;
    public float collisionPenalty = 20f;
    public int maxCollisions = 5;
    [HideInInspector] public float currentScore;
    [HideInInspector] public int collisionCount = 0;

    [Header("Death Zone")]
    public float deathZoneY = -10f;
    public float deathZoneTop = 15f;
    public float deathZoneLeft = -30f;
    public float deathZoneRight = 25f;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI collisionsText;
    public Slider fuelSlider;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public TextMeshProUGUI gameOverReasonText;

    [Header("Audio")]
    public AudioClip fuelPickupSound;
    public AudioClip obstacleHitSound;
    public AudioClip landingSound;
    public AudioClip deathSound;

    private AudioSource audioSource;
    private Transform playerTransform;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        LoadGameData();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        FindSceneReferences();

        gameActive = true;

        if (fuelSlider != null)
        {
            fuelSlider.maxValue = initialFuel;
            fuelSlider.value = currentFuel;
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        UpdateUI();

        Debug.Log($"Nivel iniciado - Combustible: {currentFuel:F1}, Puntaje: {currentScore:F0}%, Colisiones: {collisionCount}");
    }

    void FindSceneReferences()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            scoreText = FindComponentByName<TextMeshProUGUI>(canvas.transform, "ScoreText");
            fuelText = FindComponentByName<TextMeshProUGUI>(canvas.transform, "FuelText");
            collisionsText = FindComponentByName<TextMeshProUGUI>(canvas.transform, "CollisionsText");
            fuelSlider = FindComponentByName<Slider>(canvas.transform, "FuelSlider");

            Transform gameOverTransform = FindTransformByName(canvas.transform, "GameOverPanel");
            if (gameOverTransform != null)
            {
                gameOverPanel = gameOverTransform.gameObject;
                gameOverReasonText = FindComponentByName<TextMeshProUGUI>(gameOverTransform, "GameOverReasonText");
            }

            Transform victoryTransform = FindTransformByName(canvas.transform, "VictoryPanel");
            if (victoryTransform != null)
            {
                victoryPanel = victoryTransform.gameObject;
            }
        }
    }

    T FindComponentByName<T>(Transform parent, string name) where T : Component
    {
        Transform found = FindTransformByName(parent, name);
        if (found != null)
        {
            return found.GetComponent<T>();
        }
        return null;
    }

    Transform FindTransformByName(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

    void Update()
    {
        if (!gameActive) return;

        currentFuel -= fuelConsumptionRate * Time.deltaTime;

        if (currentFuel <= 0)
        {
            currentFuel = 0;
            GameOver("¡Te quedaste sin combustible!");
            return;
        }

        if (playerTransform != null)
        {
            if (playerTransform.position.y < deathZoneY)
            {
                GameOver("¡La nave se estrelló!");
                return;
            }

            if (playerTransform.position.y > deathZoneTop)
            {
                GameOver("¡Nave fuera de límites!");
                return;
            }

            if (playerTransform.position.x < deathZoneLeft)
            {
                GameOver("¡Nave fuera de límites!");
                return;
            }

            if (playerTransform.position.x > deathZoneRight)
            {
                GameOver("¡Nave fuera de límites!");
                return;
            }
        }

        UpdateUI();
        SaveGameData();
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;
        if (currentFuel > initialFuel)
        {
            currentFuel = initialFuel;
        }

        PlaySound(fuelPickupSound);
        SaveGameData();
        Debug.Log($"Combustible recogido! Total: {currentFuel:F1}");
    }

    public void OnObstacleHit()
    {
        if (!gameActive) return;

        collisionCount++;
        currentScore -= collisionPenalty;

        if (currentScore < 0)
        {
            currentScore = 0;
        }

        PlaySound(obstacleHitSound);
        SaveGameData();

        Debug.Log($"¡Colisión! Total: {collisionCount}/{maxCollisions}, Puntaje: {currentScore:F0}%");

        if (collisionCount >= maxCollisions)
        {
            GameOver("¡Demasiadas colisiones!");
        }
    }

    public void OnLevelComplete()
    {
        if (!gameActive) return;

        gameActive = false;
        PlaySound(landingSound);
        SaveGameData();

        // Desactivar PlayerController
        DisablePlayer();

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        Debug.Log($"¡Nivel completado! Puntaje: {currentScore:F0}%, Combustible: {currentFuel:F1}, Colisiones: {collisionCount}");
    }

    public void GameOver(string reason)
    {
        if (!gameActive) return;

        gameActive = false;
        PlaySound(deathSound);

        // Desactivar PlayerController
        DisablePlayer();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverReasonText != null)
        {
            gameOverReasonText.text = reason;
        }

        Debug.Log($"Game Over: {reason}");
    }

    void DisablePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.enabled = false;
            }

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    public void NextLevel()
    {
        gameActive = true;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        SaveGameData();

        Debug.Log($"Siguiente nivel. Combustible: {currentFuel:F1}, Puntaje: {currentScore:F0}, Colisiones: {collisionCount}");

        if (nextSceneIndex < totalScenes)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log($"¡JUEGO COMPLETADO! Puntaje Final: {currentScore:F0}%, Colisiones: {collisionCount}");
            ResetGame();
            SceneManager.LoadScene(0);
        }
    }

    public void RestartLevel()
    {
        ResetGame();
        gameActive = true;
        SceneManager.LoadScene(0);

        Debug.Log("Reiniciando desde nivel 1");
    }

    public void MainMenu()
    {
        ResetGame();
        SceneManager.LoadScene(0);
    }

    public void ResetGame()
    {
        currentFuel = initialFuel;
        currentScore = initialScore;
        collisionCount = 0;
        gameActive = true;

        SaveGameData();

        Debug.Log("Juego reiniciado");
    }

    void SaveGameData()
    {
        PlayerPrefs.SetFloat("CurrentFuel", currentFuel);
        PlayerPrefs.SetFloat("CurrentScore", currentScore);
        PlayerPrefs.SetInt("CollisionCount", collisionCount);
        PlayerPrefs.Save();
    }

    void LoadGameData()
    {
        if (PlayerPrefs.HasKey("CurrentFuel"))
        {
            currentFuel = PlayerPrefs.GetFloat("CurrentFuel");
            currentScore = PlayerPrefs.GetFloat("CurrentScore");
            collisionCount = PlayerPrefs.GetInt("CollisionCount");

            Debug.Log($"Datos cargados - Combustible: {currentFuel:F1}, Puntaje: {currentScore:F0}, Colisiones: {collisionCount}");
        }
        else
        {
            currentFuel = initialFuel;
            currentScore = initialScore;
            collisionCount = 0;
            SaveGameData();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Puntaje: {currentScore:F0}%";
        }

        if (fuelText != null)
        {
            fuelText.text = $"Combustible: {currentFuel:F1}";
        }

        if (collisionsText != null)
        {
            collisionsText.text = $"Colisiones: {collisionCount}/{maxCollisions}";
        }

        if (fuelSlider != null)
        {
            fuelSlider.value = currentFuel;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void OnDrawGizmos()
    {
        float left = deathZoneLeft;
        float right = deathZoneRight;
        float bottom = deathZoneY;
        float top = deathZoneTop;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(left, bottom, 0), new Vector3(left, top, 0));
        Gizmos.DrawLine(new Vector3(right, bottom, 0), new Vector3(right, top, 0));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(left, top, 0), new Vector3(right, top, 0));
        Gizmos.DrawLine(new Vector3(left, bottom, 0), new Vector3(right, bottom, 0));

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        float centerX = (left + right) / 2f;
        float centerY = (bottom + top) / 2f;
        Gizmos.DrawWireCube(
            new Vector3(centerX, centerY, 0),
            new Vector3(right - left, top - bottom, 0.1f)
        );
    }
}