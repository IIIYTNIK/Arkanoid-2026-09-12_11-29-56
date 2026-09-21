using System;
using UnityEngine;

/// <summary>
/// Простой singleton для счёта очков. Повесьте этот скрипт на пустой GameObject
/// "ScoreManager" в сцене Game — он переживёт разные обращения к нему из Block/Coin.
/// </summary>
public sealed class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int CurrentScore { get; private set; }

    /// <summary>Подписывайтесь на это событие в UI, чтобы обновлять текст счёта.</summary>
    public event Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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
