using System.Collections;
using Arkanoid.Core;
using Arkanoid.Core.Interfaces;
using Arkanoid.Paddle;
using UnityEngine;

namespace Arkanoid.Ball
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class BallController : MonoBehaviour
    {
        [SerializeField] private BallConfig config;
        [SerializeField] private float bottomBoundaryZ = -10f;

        private Rigidbody rb;
        private float currentBaseSpeed;
        private float activeSpeedMultiplier = 1f;
        private Coroutine speedEffectRoutine;

        // Храним вектор направления и состояние запуска напрямую, 
        // чтобы PhysX не мог занулить направление шарика при контакте.
        private Vector3 currentDirection;
        private bool isLaunched;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;

            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void Start()
        {
            // Если шарик лежит прямо на сцене (а не заспавнен через BallManager),
            // запускаем его автоматически при нажатии Play.
            if (!isLaunched)
            {
                Launch(Vector3.forward);
            }
        }

        public void Launch(Vector3 direction)
        {
            currentBaseSpeed = config != null ? config.InitialSpeed : 8f;

            var normalized = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;
            currentDirection = ClampDeflection(normalized);
            isLaunched = true;

            rb.linearVelocity = currentDirection * GetTargetSpeed();
        }

        private void FixedUpdate()
        {
            if (!isLaunched) return;

            // Гарантированно поддерживаем заданное направление и скорость
            rb.linearVelocity = currentDirection * GetTargetSpeed();

            if (transform.position.z < bottomBoundaryZ)
            {
                HandleFellBelowBoundary();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // 1. Наносим урон блоку, если столкнулись с ним
            if (collision.collider.TryGetComponent<Blocks.BlockController>(out var block))
            {
                block.ApplyDamage(1);
            }

            // 2. Отскок от платформы
            if (collision.collider.TryGetComponent<PaddleController>(out var paddle))
            {
                ReflectOffPaddle(paddle, collision);
                return;
            }

            // 3. Отскок от остальных объектов (блоки, стены)
            ReflectOffSurface(collision);
        }

        private void ReflectOffPaddle(PaddleController paddle, Collision collision)
        {
            var hitPoint = collision.GetContact(0).point;
            var offset = paddle.GetHitOffset(hitPoint); // -1 .. +1

            var newDirection = new Vector3(offset, 0f, 1f).normalized;
            currentDirection = ClampDeflection(newDirection);

            rb.linearVelocity = currentDirection * GetTargetSpeed();
        }

        private void ReflectOffSurface(Collision collision)
        {
            if (collision.contactCount == 0) return;

            var contact = collision.GetContact(0);

            // Математический отскок от нормали
            var reflectDirection = Vector3.Reflect(currentDirection, contact.normal);
            reflectDirection.y = 0f;

            currentDirection = ClampDeflection(reflectDirection);
            rb.linearVelocity = currentDirection * GetTargetSpeed();
        }

        private float GetTargetSpeed()
        {
            if (config == null) return currentBaseSpeed * activeSpeedMultiplier;
            return Mathf.Clamp(currentBaseSpeed * activeSpeedMultiplier, config.MinSpeed, config.MaxSpeed);
        }

        private Vector3 ClampDeflection(Vector3 direction)
        {
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
            {
                return Vector3.forward;
            }

            direction.Normalize();

            var minAngleDegrees = config != null ? config.MinDeflectionAngleDegrees : 8f;
            var minRad = minAngleDegrees * Mathf.Deg2Rad;
            var maxRad = (90f * Mathf.Deg2Rad) - minRad;

            var angleFromXAxis = Mathf.Atan2(Mathf.Abs(direction.z), Mathf.Abs(direction.x));
            var clampedAngle = Mathf.Clamp(angleFromXAxis, minRad, maxRad);

            var xSign = direction.x >= 0f ? 1f : -1f;
            var zSign = direction.z >= 0f ? 1f : -1f;

            direction.x = Mathf.Cos(clampedAngle) * xSign;
            direction.z = Mathf.Sin(clampedAngle) * zSign;

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