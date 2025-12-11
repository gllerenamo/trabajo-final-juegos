using System;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.Rendering.GPUSort;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

public class BoosterZone : MonoBehaviour
{
    [Header("Boost Settings")]
    [Tooltip("Dirección del impulso")]
    public Vector3 boostDirection = Vector3.right;

    [Tooltip("Fuerza del impulso")]
    public float boostForce = 500f;

    [Header("Visual")]
    public Color glowColor = Color.cyan;
    public float pulseSpeed = 2f;

    [Header("Audio")]
    public AudioClip boostSound;

    private Renderer meshRenderer;
    private Material material;
    private AudioSource audioSource;
    private Color originalEmissionColor;

    void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            material = meshRenderer.material;
            if (material.HasProperty("_EmissionColor"))
            {
                originalEmissionColor = material.GetColor("_EmissionColor");
            }
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Efecto de pulso en la emisión
        if (material != null && material.HasProperty("_EmissionColor"))
        {
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            Color emissionColor = originalEmissionColor * (1f + pulse);
            material.SetColor("_EmissionColor", emissionColor);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Aplicar impulso
                rb.AddForce(boostDirection.normalized * boostForce, ForceMode.Impulse);

                Debug.Log("¡BOOST activado!");

                // Reproducir sonido
                if (audioSource != null && boostSound != null)
                {
                    audioSource.PlayOneShot(boostSound);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // Visualizar dirección del boost en el editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, boostDirection.normalized * 3f);
    }
}