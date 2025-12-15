using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialLanding : MonoBehaviour
{
    [Header("Settings")]
    public string menuSceneName = "MainMenu";

    [Header("UI References")]
    public GameObject completionPanel;
    public TextMeshProUGUI completionText;
    public Button menuButton;

    [Header("Options")]
    public bool autoReturnToMenu = false;
    public float autoReturnDelay = 5f;

    private bool completed = false;

    void Start()
    {
        // Ocultar panel al inicio
        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }

        // Conectar botón
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(ReturnToMenu);
            menuButton.gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !completed)
        {
            completed = true;
            StartCoroutine(CompleteTutorial());
        }
    }

    IEnumerator CompleteTutorial()
    {
        Debug.Log("¡Tutorial completado!");

        // Desactivar PlayerController
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

        // Mostrar panel
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }

        // Mostrar texto inicial
        if (completionText != null)
        {
            completionText.text =
                "<b><size=48><color=#00FF00>¡TUTORIAL COMPLETADO!</color></size></b>\n\n" +
                "<size=32>Ya estás listo para la misión real</size>";
        }

        yield return new WaitForSeconds(2f);

        // Mostrar botón
        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(true);
        }

        // Actualizar texto con instrucción
        if (completionText != null)
        {
            completionText.text =
                "<b><size=48><color=#00FF00>¡TUTORIAL COMPLETADO!</color></size></b>\n\n" +
                "<size=32>Ya estás listo para la misión real</size>\n\n" +
                "<size=24><color=#AAAAAA>Presiona el botón para continuar</color></size>";
        }

        // Opción: retorno automático
        if (autoReturnToMenu)
        {
            yield return new WaitForSeconds(autoReturnDelay);
            ReturnToMenu();
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("Volviendo al menú principal desde tutorial");
        SceneManager.LoadScene(menuSceneName);
    }
}