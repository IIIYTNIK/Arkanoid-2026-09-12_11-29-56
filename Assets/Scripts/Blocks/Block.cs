using UnityEngine;

/// <summary>
/// Минимальный разрушаемый блок. Требует, чтобы у объекта Ball был Tag "Ball"
/// (Unity → Inspector → Tag → Add Tag, если его ещё нет в списке).
/// Повесьте на Prefab блока (например, Cube с Collider не-триггером).
/// </summary>
public sealed class Block : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int hp = 1;
    [SerializeField] private int scoreValue = 10;

    [Header("Drops")]
    [SerializeField, Range(0f, 1f)] private float powerUpDropChance = 0.15f;
    [SerializeField, Range(0f, 1f)] private float coinDropChance = 0.25f;
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private GameObject coinPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Ball"))
        {
            return;
        }

        hp--;
        if (hp <= 0)
        {
            DestroyBlock();
        }
    }

    private void DestroyBlock()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        // Шансы не пересекаются: сначала проверяем PowerUp, затем — в оставшемся
        // диапазоне — монетку. Так сумма шансов не может превысить 100%.
        var roll = Random.value;
        if (roll <= powerUpDropChance && powerUpPrefab != null)
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
        else if (roll <= powerUpDropChance + coinDropChance && coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
