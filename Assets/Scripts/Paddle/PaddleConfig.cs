using UnityEngine;

namespace Arkanoid.Paddle
{
    [CreateAssetMenu(fileName = "PaddleConfig", menuName = "Arkanoid/Paddle Config")]
    public sealed class PaddleConfig : ScriptableObject
    {
        [SerializeField] private float baseWidth = 2f;
        [SerializeField] private float moveSpeed = 12f;
        [SerializeField] private float fieldHalfWidth = 8f;

        public float BaseWidth => baseWidth;
        public float MoveSpeed => moveSpeed;
        public float FieldHalfWidth => fieldHalfWidth;
    }
}
