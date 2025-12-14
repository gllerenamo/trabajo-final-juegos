using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class FinalVictory : MonoBehaviour
{
    [Header("UI References")]
    public GameObject finalVictoryPanel;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI creatorsText;
    public CanvasGroup canvasGroup;

    [Header("Effects")]
    public ParticleSystem celebrationParticles;

    [Header("Creators Names")]
    [TextArea(5, 10)]
    public string creatorsNames = "Piero Mejía\nGonzalo Llerena\nNombre 3\nNombre 4\nNombre 5";

    [Header("Audio")]
    public AudioClip victoryMusic;

    [Header("Animation Settings")]
    public float fadeInDuration = 1f;
    public float scaleUpDuration = 0.8f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private AudioSource audioSource;
    private bool victoryTriggered = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (finalVictoryPanel != null)
        {
            finalVictoryPanel.SetActive(false);

            // Configurar CanvasGroup si no existe
            if (canvasGroup == null)
            {
                canvasGroup = finalVictoryPanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = finalVictoryPanel.AddComponent<CanvasGroup>();
                }
            }
        }

        if (celebrationParticles != null)
        {
            celebrationParticles.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !victoryTriggered)
        {
            victoryTriggered = true;
            ShowFinalVictory();
        }
    }

    public void ShowFinalVictory()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.gameActive = false;
        }

        StartCoroutine(VictorySequence());
    }

    IEnumerator VictorySequence()
    {
        // Slow motion épico
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(1.2f);

        // Restaurar tiempo
        Time.timeScale = 1f;

        // Música de victoria
        if (audioSource != null && victoryMusic != null)
        {
            audioSource.PlayOneShot(victoryMusic);
        }

        // Activar partículas
        if (celebrationParticles != null)
        {
            celebrationParticles.Play();
        }

        yield return new WaitForSeconds(0.3f);

        // Mostrar panel con animación
        if (finalVictoryPanel != null)
        {
            finalVictoryPanel.SetActive(true);

            // Preparar para animación
            finalVictoryPanel.transform.localScale = Vector3.zero;
            canvasGroup.alpha = 0f;

            // Llenar datos antes de mostrar
            UpdateStats();

            if (creatorsText != null)
            {
                creatorsText.text = creatorsNames;
            }

            // Animar aparición
            StartCoroutine(AnimatePanelAppearance());
        }
    }

    IEnumerator AnimatePanelAppearance()
    {
        float elapsedTime = 0f;

        // Animación combinada de escala y fade
        while (elapsedTime < Mathf.Max(scaleUpDuration, fadeInDuration))
        {
            elapsedTime += Time.deltaTime;

            // Escala con curva suave
            if (elapsedTime < scaleUpDuration)
            {
                float scaleProgress = elapsedTime / scaleUpDuration;
                float curveValue = scaleCurve.Evaluate(scaleProgress);

                // Efecto de "bounce" al final
                float overshoot = 1f;
                if (scaleProgress > 0.7f)
                {
                    overshoot = 1f + (Mathf.Sin((scaleProgress - 0.7f) * Mathf.PI * 3f) * 0.1f);
                }

                finalVictoryPanel.transform.localScale = Vector3.one * curveValue * overshoot;
            }
            else
            {
                finalVictoryPanel.transform.localScale = Vector3.one;
            }

            // Fade in
            if (elapsedTime < fadeInDuration)
            {
                canvasGroup.alpha = elapsedTime / fadeInDuration;
            }
            else
            {
                canvasGroup.alpha = 1f;
            }

            yield return null;
        }

        // Asegurar valores finales
        finalVictoryPanel.transform.localScale = Vector3.one;
        canvasGroup.alpha = 1f;
    }

    void UpdateStats()
    {
        if (statsText == null || GameManager.instance == null) return;

        float finalScore = GameManager.instance.currentScore;
        float finalFuel = GameManager.instance.currentFuel;
        int totalCollisions = GameManager.instance.collisionCount;

        string grade = GetGrade(finalScore);
        string gradeColor = GetGradeColor(finalScore);

        // AÑADIR ESPACIO ARRIBA DE LA CALIFICACIÓN
        string statsDisplay = "";
        statsDisplay += "\n\n";  // ← Espacio extra arriba
        statsDisplay += $"<size=40><color={gradeColor}>CALIFICACION: {grade}</color></size>\n\n";
        statsDisplay += $"<b>Puntaje Final:</b> <color=#FFD700>{finalScore:F0}%</color>\n\n";
        statsDisplay += $"<b>Combustible Restante:</b> <color=#00FF00>{finalFuel:F1}</color>\n\n";
        statsDisplay += $"<b>Colisiones Totales:</b> <color=#FF6666>{totalCollisions}</color>";

        statsText.text = statsDisplay;
    }

    string GetGrade(float score)
    {
        if (score >= 100f) return "S(PERFECTO)";
        if (score >= 80f) return "A(EXCELENTE)";
        if (score >= 60f) return "B(BIEN)";
        if (score >= 40f) return "C(REGULAR)";
        return "D(MEJORABLE)";
    }

    string GetGradeColor(float score)
    {
        if (score >= 100f) return "#FFD700"; // Dorado
        if (score >= 80f) return "#00FF00";  // Verde
        if (score >= 60f) return "#00FFFF";  // Cyan
        if (score >= 40f) return "#FFA500";  // Naranja
        return "#FF4444";                     // Rojo
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;

        // Detener partículas
        if (celebrationParticles != null)
        {
            celebrationParticles.Stop();
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGame();
        }

        SceneManager.LoadScene(0);
    }
}
