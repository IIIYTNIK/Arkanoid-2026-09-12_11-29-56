using UnityEngine;

public class MenuCubeController : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(30f, 45f, 20f);

    [SerializeField] private Vector3 movementAmplitude = new Vector3(3f, 2f, 2f);
    [SerializeField] private Vector3 movementSpeed = new Vector3(0.7f, 0.9f, 0.5f);

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);

        float x = Mathf.Sin(Time.time * movementSpeed.x) * movementAmplitude.x;
        float y = Mathf.Sin(Time.time * movementSpeed.y) * movementAmplitude.y;
        float z = Mathf.Sin(Time.time * movementSpeed.z) * movementAmplitude.z;

        transform.position = startPosition + new Vector3(x, y, z);
    }
}