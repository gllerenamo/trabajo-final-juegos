using UnityEngine;
using TMPro;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Info")]
    public string checkpointName = "Checkpoint 1";

    [TextArea(3, 5)]
    public string message = "Mensaje del checkpoint";

    [Header("UI")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    public float displayTime = 3f;

    [Header("Effects")]
    public ParticleSystem checkpointEffect;
    public AudioClip checkpointSound;

    private bool activated = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;
            ActivateCheckpoint();
        }
    }

    void ActivateCheckpoint()
    {
        Debug.Log($"Checkpoint activado: {checkpointName}");

        // Guardar progreso
        if (GameManager.instance != null)
        {
            Debug.Log($"Progreso guardado en {checkpointName}");
        }

        // Efectos visuales
        if (checkpointEffect != null)
        {
            checkpointEffect.Play();
        }

        // Sonido
        if (audioSource != null && checkpointSound != null)
        {
            audioSource.PlayOneShot(checkpointSound);
        }

        // Mostrar mensaje
        ShowMessage();
    }

    void ShowMessage()
    {
        if (messagePanel != null && messageText != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
            StartCoroutine(HideMessageAfterDelay());
        }
    }

    IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 3,
            checkpointName,
            new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = Color.green },
                fontSize = 14
            }
        );
#endif
    }
}