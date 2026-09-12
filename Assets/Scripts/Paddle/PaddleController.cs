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

        private float currentWidthMultiplier = 1f;
        private Coroutine widthEffectRoutine;

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
            var input = moveAction != null ? moveAction.action.ReadValue<float>() : 0f;
            var delta = input * config.MoveSpeed * Time.deltaTime;

            var halfWidth = (config.BaseWidth * currentWidthMultiplier) * 0.5f;
            var minX = -config.FieldHalfWidth + halfWidth;
            var maxX = config.FieldHalfWidth - halfWidth;

            var newX = Mathf.Clamp(transform.position.x + delta, minX, maxX);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
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
