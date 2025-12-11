using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;
using static UnityEngine.Rendering.GPUSort;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Fuerza de propulsión hacia adelante")]
    public float thrustForce = 10f;

    [Tooltip("Fuerza de propulsión hacia atrás")]
    public float reverseForce = 6f;  // NUEVO: Retroceso más débil que avanzar

    [Tooltip("Fuerza de rotación")]
    public float rotationForce = 5f;

    [Tooltip("Velocidad máxima")]
    public float maxSpeed = 8f;

    [Tooltip("Fuerza anti-gravedad automática")]
    public float antiGravityForce = 4f;

    [Header("References")]
    public ParticleSystem thrusterEffect;
    public ParticleSystem reverseThrusterEffect;  // NUEVO: Efecto para retroceso
    public AudioSource thrusterAudio;

    [Header("Rotation Limits")]
    [Tooltip("Velocidad angular máxima en grados por segundo")]
    public float maxAngularVelocity = 100f;

    private Rigidbody rb;
    private bool isThrusting = false;
    private bool isReversing = false;  // NUEVO: Estado de retroceso

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("¡NO HAY RIGIDBODY!");
        }
        else
        {
            Debug.Log("✓ Rigidbody encontrado");

            // Configurar física para mejor control
            rb.linearDamping = 1.5f;
            rb.angularDamping = 4f;
        }

        if (thrusterEffect != null)
        {
            thrusterEffect.Stop();
        }

        if (reverseThrusterEffect != null)
        {
            reverseThrusterEffect.Stop();
        }

        Debug.Log("✓ PlayerController iniciado correctamente");
    }

    void Update()
    {
        if (rb == null) return;

        // Detectar inputs
        isThrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        isReversing = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);  // NUEVO

        // ANTI-GRAVEDAD AUTOMÁTICA
        rb.AddForce(Vector3.up * antiGravityForce);

        // PROPULSIÓN HACIA ADELANTE
        if (isThrusting)
        {
            rb.AddForce(transform.up * thrustForce);
        }

        // PROPULSIÓN HACIA ATRÁS (RETROCESO) - NUEVO
        if (isReversing)
        {
            rb.AddForce(-transform.up * reverseForce);  // Negativo = hacia atrás
        }

        // ROTACIÓN IZQUIERDA
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddTorque(Vector3.forward * rotationForce);
        }

        // ROTACIÓN DERECHA
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddTorque(Vector3.back * rotationForce);
        }

        // LIMITAR VELOCIDAD MÁXIMA
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // LIMITAR VELOCIDAD ANGULAR
        if (rb.angularVelocity.magnitude > maxAngularVelocity * Mathf.Deg2Rad)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity * Mathf.Deg2Rad;
        }

        // EFECTOS VISUALES
        HandleEffects();
    }

    void HandleEffects()
    {
        // Partículas de propulsión hacia adelante
        if (thrusterEffect != null)
        {
            if (isThrusting && !isReversing && !thrusterEffect.isPlaying)
            {
                thrusterEffect.Play();
            }
            else if ((!isThrusting || isReversing) && thrusterEffect.isPlaying)
            {
                thrusterEffect.Stop();
            }
        }

        // Partículas de propulsión hacia atrás - NUEVO
        if (reverseThrusterEffect != null)
        {
            if (isReversing && !isThrusting && !reverseThrusterEffect.isPlaying)
            {
                reverseThrusterEffect.Play();
            }
            else if ((!isReversing || isThrusting) && reverseThrusterEffect.isPlaying)
            {
                reverseThrusterEffect.Stop();
            }
        }

        // Audio
        if (thrusterAudio != null)
        {
            if ((isThrusting || isReversing) && !thrusterAudio.isPlaying)
            {
                thrusterAudio.Play();
            }
            else if (!isThrusting && !isReversing && thrusterAudio.isPlaying)
            {
                thrusterAudio.Stop();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("¡Colisión con obstáculo!");
            GameManager.instance?.OnObstacleHit();
        }
        else if (collision.gameObject.CompareTag("LandingPlatform"))
        {
            Debug.Log("¡Aterrizaje exitoso!");
            GameManager.instance?.OnLevelComplete();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fuel"))
        {
            Debug.Log("¡Combustible recogido!");
            GameManager.instance?.AddFuel(2);
            Destroy(other.gameObject);
        }
    }
}