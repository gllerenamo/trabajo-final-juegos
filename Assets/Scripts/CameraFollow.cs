using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // El Player

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 0, -15);
    public float smoothSpeed = 0.125f;
    public bool followX = true;
    public bool followY = true;

    [Header("Limits")]
    public bool useLimits = true;
    public float minX = -25f;
    public float maxX = 20f;

    void LateUpdate()
    {
        if (target == null) return;

        // Calcular posición deseada
        Vector3 desiredPosition = target.position + offset;

        // Aplicar límites si están activados
        if (useLimits)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        }

        // Solo seguir en ejes seleccionados
        if (!followX) desiredPosition.x = transform.position.x;
        if (!followY) desiredPosition.y = transform.position.y;

        // Suavizar movimiento
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aplicar
        transform.position = smoothedPosition;
    }
}