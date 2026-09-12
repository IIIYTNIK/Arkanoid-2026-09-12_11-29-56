using System;

namespace Arkanoid.Core.Interfaces
{
    public interface IScoreManager
    {
        int CurrentScore { get; }
        event Action<int> OnScoreChanged;
        void AddScore(int amount);
    }
}
