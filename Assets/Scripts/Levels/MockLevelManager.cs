using System;
using Arkanoid.Core.Interfaces;
using UnityEngine;

namespace Arkanoid.Levels
{
    /// <summary>
    /// Временная реализация ILevelManager. Регистрируйте её в GameServices вместо
    /// реального LevelManager, пока Developer 2 не закончит Phase 5 (см. п.26 ТЗ:
    /// "минимальная рабочая реализация/mock, позволяющая второму разработчику
    /// продолжать работу"). Удалить регистрацию из bootstrap-сцены, когда
    /// LevelManager готов — интерфейс не меняется, так что переключение безопасно.
    /// </summary>
    public sealed class MockLevelManager : MonoBehaviour, ILevelManager
    {
        [SerializeField] private int mockBlockCount = 10;

        public int CurrentLevelIndex { get; private set; } = 1;
        public int BlocksRemaining { get; private set; }

        public event Action OnLevelComplete;

        private void Awake()
        {
            BlocksRemaining = mockBlockCount;
        }

        public void LoadLevel(int levelIndex)
        {
            CurrentLevelIndex = levelIndex;
            BlocksRemaining = mockBlockCount;
        }

        public void RegisterBlock() => BlocksRemaining++;

        public void NotifyBlockDestroyed()
        {
            if (BlocksRemaining <= 0)
            {
                return;
            }

            BlocksRemaining--;
            if (BlocksRemaining == 0)
            {
                OnLevelComplete?.Invoke();
            }
        }
    }
}
