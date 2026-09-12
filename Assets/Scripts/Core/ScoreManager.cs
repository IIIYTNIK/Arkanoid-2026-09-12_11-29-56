using System;
using Arkanoid.Core.Interfaces;
using UnityEngine;

namespace Arkanoid.Core
{
    public sealed class ScoreManager : MonoBehaviour, IScoreManager
    {
        public int CurrentScore { get; private set; }
        public event Action<int> OnScoreChanged;

        public void AddScore(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentScore += amount;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}
