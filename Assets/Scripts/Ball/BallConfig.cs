using UnityEngine;

namespace Arkanoid.Ball
{
    [CreateAssetMenu(fileName = "BallConfig", menuName = "Arkanoid/Ball Config")]
    public sealed class BallConfig : ScriptableObject
    {
        [Header("Speed")]
        [SerializeField] private float initialSpeed = 8f;
        [SerializeField] private float minSpeed = 5f;
        [SerializeField] private float maxSpeed = 20f;

        [Header("Reflection")]
        [Tooltip("Минимальный угол отклонения от горизонтали/вертикали в градусах. " +
                 "Не даёт шарику двигаться строго горизонтально/вертикально бесконечно (edge case #7 ТЗ).")]
        [SerializeField, Range(1f, 20f)] private float minDeflectionAngleDegrees = 8f;

        public float InitialSpeed => initialSpeed;
        public float MinSpeed => minSpeed;
        public float MaxSpeed => maxSpeed;
        public float MinDeflectionAngleDegrees => minDeflectionAngleDegrees;
    }
}
