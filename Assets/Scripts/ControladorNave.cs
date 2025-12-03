using UnityEngine;

public class ControladorNave : MonoBehaviour
{
    Rigidbody rigidBody;
    Transform transform;
    AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        Propulse();
        Rotate();
    }

    private void Propulse()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.freezeRotation = true;
            rigidBody.AddRelativeForce(Vector3.up);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

        }
    	else
        {
            audioSource.Stop();
        }
        rigidBody.freezeRotation = false;
    }

    private void Rotate()
    {
        var rotationStep = Time.deltaTime * 50;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationStep);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationStep);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            case "ColisionSegura":
                print("Colision Segura.");
                break;
            case "ColisionPeligrosa":
                print("Colision peligrosa.");
                break;
            default:
                break;
        }
    }
}
