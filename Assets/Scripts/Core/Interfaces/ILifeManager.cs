using System;

namespace Arkanoid.Core.Interfaces
{
    /// <summary>
    /// Управляет жизнями игрока. LoseLife() вызывается ТОЛЬКО в ответ на
    /// IBallManager.OnAllBallsLost — никогда напрямую из BallController.
    /// </summary>
    public interface ILifeManager
    {
        int CurrentLives { get; }
        int MaxLives { get; }

        event Action<int> OnLivesChanged;
        event Action OnGameOver;

        void LoseLife();

        /// <summary>Не превышает MaxLives (edge case #6 в ТЗ).</summary>
        void AddLife();
    }
}
