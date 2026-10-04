using UnityEngine;

/// <summary>
/// Игровое поле в плоскости XZ (см. BallController). "Падение" вниз к платформе —
/// это движение по УБЫВАНИЮ Z, а не по Y.
/// </summary>
public sealed class Coin : MonoBehaviour
{
    [SerializeField] private int scoreValue = 5;
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float despawnBelowZ = -10f; // Проверка удаления по оси Z
    [SerializeField] private float rotationSpeed = 180f; // Скорость вращения в градусах в секунду

    private void Update()
{
    // 1. Падение вдоль поля к платформе (по убыванию Z)
    transform.Translate(Vector3.back * fallSpeed * Time.deltaTime, Space.World);

    // 2. Вращение вокруг собственной оси Z (монетка крутится как «колесо»/ребром)
    transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);

    // 3. Проверка выхода за нижнюю границу поля (ось Z)
    if (transform.position.z < despawnBelowZ)
    {
        Destroy(gameObject);
    }
}
    

    private void OnTriggerEnter(Collider other)
{
    Debug.Log($"Монетка коснулась объекта: {other.gameObject.name} с тегом: {other.tag}");

    if (!other.CompareTag("Paddle") && !other.CompareTag("Ball"))
    {
        return;
    }

    if (ScoreManager.Instance != null)
    {
        Debug.Log($"Прибавляем очки через ScoreManager! Текущее значение очков: {scoreValue}");
        ScoreManager.Instance.AddScore(scoreValue);
    }
    else
    {
        Debug.LogError("ScoreManager.Instance НЕ найден на сцене!");
    }

    Destroy(gameObject);
}
}