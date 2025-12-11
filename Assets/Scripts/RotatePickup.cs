using UnityEngine;

public class RotatePickup : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("Bob Animation")]
    public bool enableBobbing = true;
    public float bobbingSpeed = 2f;
    public float bobbingAmount = 0.3f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Rotación constante
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);

        // Movimiento de "bobbing" arriba y abajo
        if (enableBobbing)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}