using System.Collections;
using Arkanoid.Core;
using Arkanoid.Core.Interfaces;
using Arkanoid.Paddle;
using UnityEngine;

namespace Arkanoid.Ball
{
    /// <summary>
    /// Шарик управляет только своей собственной физикой и уведомляет BallManager
    /// о рождении/смерти. НЕ трогает жизни напрямую (требование п.24.12 ТЗ) —
    /// это ответственность LifeManager, реагирующего на IBallManager.OnAllBallsLost.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class BallController : MonoBehaviour
    {
        [SerializeField] private BallConfig config;
        [SerializeField] private float bottomBoundaryZ = -6f;

        private Rigidbody rb;
        private float currentBaseSpeed;
        private float activeSpeedMultiplier = 1f;
        private Coroutine speedEffectRoutine;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY; // 2D-геймплей на 3D поле
        }

        public void Launch(Vector3 direction)
        {
            currentBaseSpeed = config.InitialSpeed;
            var normalized = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;
            rb.linearVelocity = ClampDeflection(normalized) * currentBaseSpeed;
        }

        private void FixedUpdate()
        {
            // Стабильная скорость: после любых столкновений вектор может измениться
            // по направлению, но модуль всегда возвращается к currentBaseSpeed * activeSpeedMultiplier.
            var targetSpeed = Mathf.Clamp(currentBaseSpeed * activeSpeedMultiplier, config.MinSpeed, config.MaxSpeed);
            var direction = ClampDeflection(rb.linearVelocity.normalized);
            rb.linearVelocity = direction * targetSpeed;

            if (transform.position.z < bottomBoundaryZ) // на 3D-поле "низ" — по факту одна из горизонтальных осей;
            {                                            // финальная ось задаётся ориентацией игрового поля в сцене.
                HandleFellBelowBoundary();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent<PaddleController>(out var paddle))
            {
                ReflectOffPaddle(paddle, collision);
            }
            // Отражение от стен/блоков обрабатывается физическим Collider'ом Unity —
            // FixedUpdate выше нормализует результирующую скорость и угол.
        }

        private void ReflectOffPaddle(PaddleController paddle, Collision collision)
        {
            var hitPoint = collision.GetContact(0).point;
            var offset = paddle.GetHitOffset(hitPoint); // -1 (левый край) .. +1 (правый край)

            var newDirection = new Vector3(offset, 0f, Mathf.Abs(rb.linearVelocity.z) > 0.01f ? 1f : 1f).normalized;
            rb.linearVelocity = ClampDeflection(newDirection) * currentBaseSpeed * activeSpeedMultiplier;
        }

        /// <summary>
        /// Не даёт направлению стать почти строго горизонтальным/вертикальным —
        /// прямая защита от edge case #7 в ТЗ (бесконечные одинаковые столкновения).
        /// </summary>
        private Vector3 ClampDeflection(Vector3 direction)
        {
            var minRad = config.MinDeflectionAngleDegrees * Mathf.Deg2Rad;
            var angleFromAxis = Mathf.Asin(Mathf.Clamp(Mathf.Abs(direction.z), 0f, 1f));

            if (angleFromAxis < minRad)
            {
                var sign = direction.z >= 0f ? 1f : -1f;
                direction.z = Mathf.Sin(minRad) * sign;
                direction.x = Mathf.Sqrt(Mathf.Max(0f, 1f - direction.z * direction.z)) * Mathf.Sign(direction.x == 0 ? 1 : direction.x);
            }

            return direction.normalized;
        }

        public void ApplySpeedMultiplier(float multiplier, float duration)
        {
            if (speedEffectRoutine != null)
            {
                StopCoroutine(speedEffectRoutine);
            }

            speedEffectRoutine = StartCoroutine(SpeedMultiplierRoutine(multiplier, duration));
        }

        private IEnumerator SpeedMultiplierRoutine(float multiplier, float duration)
        {
            // Абсолютное присвоение, а не умножение на предыдущий множитель —
            // защита от бесконечного накопления скорости (edge case #5 ТЗ).
            activeSpeedMultiplier = multiplier;
            yield return new WaitForSeconds(duration);
            activeSpeedMultiplier = 1f;
            speedEffectRoutine = null;
        }

        private void HandleFellBelowBoundary()
        {
            if (GameServices.TryGet<IBallManager>(out var ballManager))
            {
                ballManager.UnregisterBall(this);
            }

            Destroy(gameObject);
        }
    }
}
