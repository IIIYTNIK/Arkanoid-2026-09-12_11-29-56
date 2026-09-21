using System.Collections;
using UnityEngine;

/// <summary>
/// Добавьте этот компонент прямо на GameObject Ball (или дайте
/// SpeedBallPowerUp.cs добавить его автоматически при первом подборе).
/// Работает НЕЗАВИСИМО от вашего текущего скрипта движения мяча — просто
/// "прижимает" модуль скорости к нужному значению каждый физический кадр,
/// сохраняя то направление, которое вычисляет ваша логика отскоков.
///
/// Важно: если ваш текущий скрипт мяча сам жёстко задаёт rb.velocity каждый
/// FixedUpdate ПОСЛЕ этого компонента (порядок Script Execution Order), эффект
/// может перетираться. В таком случае просто вызывайте ApplyBoost() напрямую
/// из вашего скрипта мяча, либо скажите мне — интегрирую точечно.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public sealed class BallSpeedPowerUpHandler : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private float duration = 8f;

    private Rigidbody rb;
    private float preBoostSpeed;
    private float boostedSpeed;
    private bool isBoosted;
    private Coroutine activeRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyBoost()
    {
        if (!isBoosted)
        {
            // "Нормальная" скорость запоминается только при первом подборе —
            // повторные PowerUp продлевают эффект, а не накапливают его
            // (иначе 1.5 * 1.5 = 2.25x и так до бесконечности).
            preBoostSpeed = rb.linearVelocity.magnitude;
        }

        boostedSpeed = preBoostSpeed * speedMultiplier;
        isBoosted = true;

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }

        activeRoutine = StartCoroutine(BoostTimer());
    }

    private IEnumerator BoostTimer()
    {
        yield return new WaitForSeconds(duration);
        isBoosted = false;
        rb.linearVelocity = rb.linearVelocity.normalized * preBoostSpeed;
        activeRoutine = null;
    }

    private void FixedUpdate()
    {
        if (!isBoosted || rb.linearVelocity.sqrMagnitude < 0.0001f)
        {
            return;
        }

        rb.linearVelocity = rb.linearVelocity.normalized * boostedSpeed;
    }
}
