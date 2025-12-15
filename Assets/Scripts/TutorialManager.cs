using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;

    [Header("Settings")]
    public string menuSceneName = "MainMenu";
    public bool autoReturnToMenu = true;

    private bool tutorialActive = true;
    private bool practiceMode = false;

    void Start()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        StartCoroutine(RunTutorial());
    }

    void Update()
    {
        // Presionar ESC para volver al menú
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }

        // Mostrar ayuda en modo práctica
        if (practiceMode && Input.GetKeyDown(KeyCode.H))
        {
            ShowPracticeHelp();
        }
    }

    IEnumerator RunTutorial()
    {
        // ====== BIENVENIDA ======
        yield return ShowMessage(
            "<b><size=48>¡BIENVENIDO, PILOTO!</size></b>\n\n" +
            "<size=32>Academia de Limpieza Orbital</size>\n\n" +
            "Esta es tu zona de entrenamiento básico\n\n" +
            "<color=yellow><size=24>Presiona ESPACIO para continuar</size></color>",
            true
        );

        // ====== PROPULSIÓN ADELANTE ======
        yield return ShowMessage(
            "<b><size=40>PROPULSIÓN ADELANTE</size></b>\n\n" +
            "Mantén presionada:\n" +
            "<color=yellow><size=44>W</size></color> o <color=yellow><size=44>↑</size></color>\n\n" +
            "<size=24>Intenta propulsarte ahora</size>",
            false
        );
        yield return WaitForInput(3f, KeyCode.W, KeyCode.UpArrow);
        yield return new WaitForSeconds(0.5f);

        yield return ShowMessage(
            "<b><color=green>¡EXCELENTE!</color></b>\n\n" +
            "Mantén la tecla presionada para\n" +
            "continuar propulsándote\n\n" +
            "<size=20>La propulsión consume combustible</size>",
            false
        );
        yield return new WaitForSeconds(3f);

        // ====== ROTACIÓN ======
        yield return ShowMessage(
            "<b><size=40>CONTROL DE DIRECCIÓN</size></b>\n\n" +
            "<color=yellow><size=44>A</size></color> o <color=yellow><size=44>←</size></color> = Rotar IZQUIERDA\n\n" +
            "<color=yellow><size=44>D</size></color> o <color=yellow><size=44>→</size></color> = Rotar DERECHA\n\n" +
            "<size=24>Prueba rotar tu nave</size>",
            false
        );
        yield return WaitForInput(3f, KeyCode.A, KeyCode.LeftArrow, KeyCode.D, KeyCode.RightArrow);
        yield return new WaitForSeconds(0.5f);

        yield return ShowMessage(
            "<b><color=green>¡PERFECTO!</color></b>\n\n" +
            "Combina propulsión con rotación\n" +
            "para controlar tu trayectoria\n\n" +
            "<size=20>Practica moverte un poco</size>",
            false
        );
        yield return new WaitForSeconds(3f);

        // ====== RETROCESO ======
        yield return ShowMessage(
            "<b><size=40>PROPULSIÓN INVERSA</size></b>\n\n" +
            "Para maniobras precisas o frenar:\n\n" +
            "<color=yellow><size=44>S</size></color> o <color=yellow><size=44>↓</size></color>\n\n" +
            "<size=24>Intenta retroceder</size>",
            false
        );
        yield return WaitForInput(3f, KeyCode.S, KeyCode.DownArrow);
        yield return new WaitForSeconds(0.5f);

        yield return ShowMessage(
            "<b><color=green>¡BIEN HECHO!</color></b>\n\n" +
            "El retroceso es más débil que\n" +
            "la propulsión adelante\n\n" +
            "<size=20>Úsalo con precaución</size>",
            false
        );
        yield return new WaitForSeconds(3f);

        // ====== COMBUSTIBLE ======
        yield return ShowMessage(
            "<b><size=40>⚡ GESTIÓN DE ENERGÍA</size></b>\n\n" +
            "Tu <color=yellow>COMBUSTIBLE</color> se consume constantemente\n\n" +
            "Recoge las cápsulas <color=yellow>AMARILLAS</color>\n" +
            "para recargar energía\n\n" +
            "<size=20>Observa la barra superior derecha</size>",
            false
        );
        yield return new WaitForSeconds(5f);

        // ====== OBSTÁCULOS ======
        yield return ShowMessage(
            "<b><size=40>⚠️ ESCOMBROS ESPACIALES</size></b>\n\n" +
            "Evita colisionar con obstáculos\n" +
            "<color=grey>GRISES</color> y <color=red>ROJOS</color>\n\n" +
            "<color=red>Cada colisión: -20% puntaje</color>\n" +
            "<color=red>Máximo: 5 colisiones</color>\n\n" +
            "<size=20>Pilotaje suave = Mejor puntaje</size>",
            false
        );
        yield return new WaitForSeconds(5f);

        // ====== OBJETIVO ======
        yield return ShowMessage(
            "<b><size=40>🎯 OBJETIVO</size></b>\n\n" +
            "Llega a la plataforma AZUL\n" +
            "al final de cada zona\n\n" +
            "<size=28>Meta: Completar con el mayor puntaje posible</size>\n\n" +
            "<size=20>Intenta aterrizar en esta zona de práctica</size>",
            false
        );
        yield return new WaitForSeconds(4f);

        // ====== PRÁCTICA LIBRE ======
        yield return ShowMessage(
            "<b><size=44><color=green>¡ENTRENAMIENTO COMPLETO!</color></size></b>\n\n" +
            "Ahora practica libremente\n\n" +
            "Aterriza en la plataforma AZUL cuando estés listo\n\n" +
            "<size=22><color=yellow>H</color> = Ver ayuda | <color=yellow>ESC</color> = Volver al menú</size>",
            false
        );
        yield return new WaitForSeconds(4f);

        // Ocultar panel y activar modo práctica
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        tutorialActive = false;
        practiceMode = true;
    }

    IEnumerator ShowMessage(string message, bool waitForSpace)
    {
        if (tutorialText != null)
        {
            tutorialText.text = message;
        }

        if (waitForSpace)
        {
            yield return WaitForSpace();
        }
    }

    IEnumerator WaitForSpace()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator WaitForInput(float minTime, params KeyCode[] keys)
    {
        float elapsed = 0f;
        bool inputDetected = false;

        while (elapsed < minTime || !inputDetected)
        {
            foreach (KeyCode key in keys)
            {
                if (Input.GetKey(key))
                {
                    inputDetected = true;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
    }

    void ShowPracticeHelp()
    {
        if (tutorialPanel != null && tutorialText != null)
        {
            tutorialPanel.SetActive(true);
            tutorialText.text =
                "<b>CONTROLES</b>\n\n" +
                "<color=yellow>W/↑</color> = Propulsión adelante\n" +
                "<color=yellow>S/↓</color> = Propulsión atrás\n" +
                "<color=yellow>A/←</color> = Rotar izquierda\n" +
                "<color=yellow>D/→</color> = Rotar derecha\n\n" +
                "<color=yellow>ESC</color> = Volver al menú\n\n" +
                "<size=20>Presiona <color=yellow>H</color> para ocultar</size>";

            StartCoroutine(HideHelpAfterDelay());
        }
    }

    IEnumerator HideHelpAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        if (tutorialPanel != null && practiceMode)
        {
            tutorialPanel.SetActive(false);
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("Volviendo al menú principal");
        SceneManager.LoadScene(menuSceneName);
    }
}