using System;
using System.Collections.Generic;
using Arkanoid.Ball;
using UnityEngine;

namespace Arkanoid.Core.Interfaces
{
    /// <summary>
    /// Единственный источник правды о том, сколько шариков сейчас активно.
    /// LifeManager подписывается на OnAllBallsLost и НИКОГДА не считает шарики сам.
    /// </summary>
    public interface IBallManager
    {
        IReadOnlyList<BallController> ActiveBalls { get; }

        /// <summary>Срабатывает ровно один раз в момент, когда последний шарик покинул поле.</summary>
        event Action OnAllBallsLost;

        void RegisterBall(BallController ball);
        void UnregisterBall(BallController ball);

        BallController SpawnBall(Vector3 position, Vector3 initialDirection);

        /// <summary>
        /// Применяет множитель скорости ко ВСЕМ активным шарикам и новым, заспавненным
        /// пока эффект активен. Множитель абсолютный (currentSpeed = baseSpeed * multiplier),
        /// чтобы повторные PowerUp не накапливались бесконечно (см. edge case #5 в ТЗ).
        /// </summary>
        void ApplySpeedMultiplierToAll(float multiplier, float duration);
    }
}
