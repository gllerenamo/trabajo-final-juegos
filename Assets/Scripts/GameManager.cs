using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    public float initialFuel = 10f;
    public float fuelConsumptionRate = 0.1f;
    public int maxCollisions = 5;
    public float initialScore = 100f;
    public float scoreDeductionPerHit = 20f;

    [Header("Death Zone")]
    public float deathZoneY = -10f; // Límite inferior
    public float deathZoneTop = 15f; // NUEVO: Límite superior
    public float deathZoneLeft = -30f;
    public float deathZoneRight = 25f;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI collisionsText;
    public Slider fuelSlider;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public TextMeshProUGUI gameOverReasonText; // NUEVO

    [Header("Audio")]
    public AudioClip fuelPickupSound;
    public AudioClip obstacleHitSound;
    public AudioClip landingSound;
    public AudioClip deathSound; // NUEVO

    private float currentFuel;
    private float currentScore;
    private int collisionCount = 0;
    private bool gameActive = true;
    private AudioSource audioSource;
    private Transform playerTransform;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Buscar el player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        currentFuel = initialFuel;
        currentScore = initialScore;
        collisionCount = 0;
        gameActive = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        if (!gameActive) return;

        // Consumir combustible
        currentFuel -= fuelConsumptionRate * Time.deltaTime;

        if (currentFuel <= 0)
        {
            currentFuel = 0;
            GameOver("¡Te quedaste sin combustible!");
        }

        // Verificar límites
        if (playerTransform != null)
        {
            // Límite inferior
            if (playerTransform.position.y < deathZoneY)
            {
                GameOver("¡La nave se estrelló!");
            }

            // NUEVO: Límite superior
            if (playerTransform.position.y > deathZoneTop)
            {
                GameOver("¡Nave fuera de límites!");
            }

            // Límite izquierdo
            if (playerTransform.position.x < deathZoneLeft)
            {
                GameOver("¡Nave fuera de límites!");
            }

            // Límite derecho
            if (playerTransform.position.x > deathZoneRight)
            {
                GameOver("¡Nave fuera de límites!");
            }
        }

        UpdateUI();
    }

    public void AddFuel(float amount)
    {
        currentFuel += amount;
        PlaySound(fuelPickupSound);
        UpdateUI();
    }

    public void OnObstacleHit()
    {
        if (!gameActive) return;

        collisionCount++;
        currentScore -= scoreDeductionPerHit;

        if (currentScore < 0) currentScore = 0;

        PlaySound(obstacleHitSound);

        if (collisionCount >= maxCollisions)
        {
            GameOver("¡Demasiadas colisiones!");
        }

        UpdateUI();
    }

    public void OnLevelComplete()
    {
        if (!gameActive) return;

        gameActive = false;
        PlaySound(landingSound);

        // Bonus por combustible y sin colisiones
        currentScore += currentFuel * 2;
        currentScore += (maxCollisions - collisionCount) * 10;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Opcional: Congelar el tiempo o la nave
        Time.timeScale = 0f;

        Debug.Log($"¡Nivel completado! Score final: {currentScore}");
    }

    void GameOver(string reason)
    {
        if (!gameActive) return;

        gameActive = false;

        PlaySound(deathSound);

        Debug.Log($"Game Over: {reason}");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Mostrar razón del Game Over
            if (gameOverReasonText != null)
            {
                gameOverReasonText.text = reason;
            }
        }

        // Opcional: Congelar el juego
        // Time.timeScale = 0f;
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
            fuelSlider.maxValue = initialFuel + 10;
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

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Restaurar tiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("¡Has completado todos los niveles!");
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}