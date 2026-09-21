using UnityEngine;

/// <summary>
/// Повесьте на Prefab монетки: небольшой меш (Sphere/Cylinder) + Collider с
/// галочкой Is Trigger. Требует Tag "Paddle" на платформе (и опционально "Ball").
/// </summary>
public sealed class Coin : MonoBehaviour
{
    [SerializeField] private int scoreValue = 5;
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float despawnBelowY = -10f;

    private void Update()
    {
        // Ось падения — Y (вниз по полю). Если ваше поле развёрнуто в другой
        // плоскости (например, XZ), замените Vector3.down на нужное направление.
        transform.position += Vector3.down * (fallSpeed * Time.deltaTime);

        if (transform.position.y < despawnBelowY)
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
