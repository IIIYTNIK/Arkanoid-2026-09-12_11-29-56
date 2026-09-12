using System;

namespace Arkanoid.Core.Interfaces
{
    /// <summary>
    /// BlocksRemaining — атомарный счётчик, декрементируемый ровно один раз на блок.
    /// Это гарантирует, что победа (OnLevelComplete) не сработает дважды, даже если
    /// несколько шариков одновременно уничтожают последний блок в одном кадре
    /// (edge case #8 в ТЗ).
    /// </summary>
    public interface ILevelManager
    {
        int CurrentLevelIndex { get; }
        int BlocksRemaining { get; }

        event Action OnLevelComplete;

        void LoadLevel(int levelIndex);

        /// <summary>Вызывается BlockSpawner при создании каждого разрушаемого блока.</summary>
        void RegisterBlock();

        /// <summary>Вызывается BlockController в момент уничтожения. Идемпотентно ниже нуля не уходит.</summary>
        void NotifyBlockDestroyed();
    }
}
