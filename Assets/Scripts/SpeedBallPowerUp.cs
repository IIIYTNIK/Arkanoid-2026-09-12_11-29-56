using UnityEngine;

/// <summary>
/// Повесьте на Prefab PowerUp'а: визуально отличимый меш (например, Capsule
/// другого цвета) + Collider с галочкой Is Trigger. Требует Tag "Paddle" на
/// платформе и Tag "Ball" на мяче.
/// </summary>
public sealed class SpeedBallPowerUp : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float despawnBelowY = -10f;

    private void Update()
    {
        transform.position += Vector3.down * (fallSpeed * Time.deltaTime);

        if (transform.position.y < despawnBelowY)
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
