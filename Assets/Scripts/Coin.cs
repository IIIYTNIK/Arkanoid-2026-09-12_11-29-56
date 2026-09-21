using UnityEngine;

/// <summary>
/// Игровое поле в плоскости XZ (см. BallController). "Падение" вниз к платформе —
/// это движение по УБЫВАНИЮ Z, а не по Y.
/// </summary>
public sealed class Coin : MonoBehaviour
{
    [SerializeField] private int scoreValue = 5;
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float despawnBelowZ = -10f;

    private void Update()
    {
        transform.position += Vector3.back * (fallSpeed * Time.deltaTime);

        if (transform.position.z < despawnBelowZ)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Paddle") && !other.CompareTag("Ball"))
        {
            return;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        Destroy(gameObject);
    }
}
