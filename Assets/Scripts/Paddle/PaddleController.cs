using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Arkanoid.Paddle
{
    /// <summary>
    /// Платформа отвечает только за собственное движение и границы (требование п.6 ТЗ).
    /// Изменение размера от PowerUp применяется через ApplyWidthMultiplier — платформа
    /// не знает, что такое PowerUp, только "измени ширину на время".
    /// </summary>
    public sealed class PaddleController : MonoBehaviour
    {
        [SerializeField] private PaddleConfig config;
        [SerializeField] private InputActionReference moveAction;

        private Rigidbody rb;
        private float currentInput;
        private float currentWidthMultiplier = 1f;
        private Coroutine widthEffectRoutine;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            rb.isKinematic = true;
            rb.useGravity = false;
        }

        private void OnEnable()
        {
            moveAction?.action.Enable();
        }

        private void OnDisable()
        {
            moveAction?.action.Disable();
        }

        private void Update()
        {
            // Считываем ввод в Update
            currentInput = moveAction != null ? moveAction.action.ReadValue<float>() : 0f;
        }
        private void FixedUpdate()
        {
            // Перемещаем физически в FixedUpdate
            var delta = currentInput * config.MoveSpeed * Time.fixedDeltaTime;

            var halfWidth = (config.BaseWidth * currentWidthMultiplier) * 0.5f;
            var minX = -config.FieldHalfWidth + halfWidth;
            var maxX = config.FieldHalfWidth - halfWidth;

            var newX = Mathf.Clamp(rb.position.x + delta, minX, maxX);
            rb.MovePosition(new Vector3(newX, rb.position.y, rb.position.z));
        }

        /// <summary>Возвращает -1..+1: где на платформе шарик ударил (для BallController.ReflectOffPaddle).</summary>
        public float GetHitOffset(Vector3 worldHitPoint)
        {
            var halfWidth = (config.BaseWidth * currentWidthMultiplier) * 0.5f;
            var localX = worldHitPoint.x - transform.position.x;
            return Mathf.Clamp(localX / halfWidth, -1f, 1f);
        }

        public void ApplyWidthMultiplier(float multiplier, float duration)
        {
            if (widthEffectRoutine != null)
            {
                StopCoroutine(widthEffectRoutine);
            }

            widthEffectRoutine = StartCoroutine(WidthEffectRoutine(multiplier, duration));
        }

        private IEnumerator WidthEffectRoutine(float multiplier, float duration)
        {
            currentWidthMultiplier = multiplier;
            transform.localScale = new Vector3(multiplier, 1f, 1f);
            yield return new WaitForSeconds(duration);
            currentWidthMultiplier = 1f;
            transform.localScale = Vector3.one;
            widthEffectRoutine = null;
        }
    }
}
