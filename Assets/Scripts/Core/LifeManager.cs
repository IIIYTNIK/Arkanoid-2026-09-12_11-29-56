using System;
using Arkanoid.Core.Interfaces;
using UnityEngine;

namespace Arkanoid.Core
{
    /// <summary>
    /// Реальная реализация ILifeManager. Подписывается на IBallManager.OnAllBallsLost —
    /// это единственный триггер потери жизни (требование п.8 ТЗ: жизнь не должна
    /// уменьшаться, пока есть хотя бы один активный шарик).
    /// </summary>
    public sealed class LifeManager : MonoBehaviour, ILifeManager
    {
        [SerializeField] private int maxLives = 3;
        [SerializeField] private BallManager ballManager;

        public int CurrentLives { get; private set; }
        public int MaxLives => maxLives;

        public event Action<int> OnLivesChanged;
        public event Action OnGameOver;

        private void Awake()
        {
            CurrentLives = maxLives;
        }

        private void OnEnable()
        {
            ballManager.OnAllBallsLost += LoseLife;
        }

        private void OnDisable()
        {
            ballManager.OnAllBallsLost -= LoseLife;
        }

        public void LoseLife()
        {
            if (CurrentLives <= 0)
            {
                // Уже Game Over — не даём уйти в отрицательные значения
                // (защита на случай повторного вызова в одном кадре, edge case #3).
                return;
            }

            CurrentLives--;
            OnLivesChanged?.Invoke(CurrentLives);

            if (CurrentLives <= 0)
            {
                OnGameOver?.Invoke();
                return;
            }

            var spawnPoint = Vector3.zero; // TODO Developer 1: заменить на реальную точку спавна
            ballManager.SpawnBall(spawnPoint, Vector3.up);
        }

        public void AddLife()
        {
            if (CurrentLives >= maxLives)
            {
                // Edge case #6: Extra Life при Lives == MaxLives не должен ничего менять.
                return;
            }

            CurrentLives++;
            OnLivesChanged?.Invoke(CurrentLives);
        }
    }
}
