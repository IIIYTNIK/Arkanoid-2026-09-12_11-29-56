using UnityEngine;

/// <summary>
/// Игровое поле в плоскости XZ (см. BallController). "Падение" вниз к платформе —
/// это движение по УБЫВАНИЮ Z, а не по Y.
/// </summary>
public sealed class SpeedBallPowerUp : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float despawnBelowZ = -10f; // Проверка удаления по оси Z
    [SerializeField] private float rotationSpeed = 180f;

    private void Update()
    {
        // 1. Падение вдоль поля к платформе (по убыванию Z)
        transform.Translate(Vector3.back * fallSpeed * Time.deltaTime, Space.World);

        // 2. Вращение вокруг своей оси Y
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);

        // 3. Проверка выхода за нижнюю границу поля (ось Z)
        if (transform.position.z < despawnBelowZ)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Paddle"))
        {
            return;
        }

        var ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null)
        {
            var booster = ball.GetComponent<BallSpeedPowerUpHandler>();
            if (booster == null)
            {
                booster = ball.AddComponent<BallSpeedPowerUpHandler>();
            }

            booster.ApplyBoost();
        }

        Destroy(gameObject);
    }
}