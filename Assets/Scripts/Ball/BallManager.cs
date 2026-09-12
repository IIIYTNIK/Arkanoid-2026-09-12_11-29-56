using System;
using System.Collections.Generic;
using Arkanoid.Core.Interfaces;
using UnityEngine;

namespace Arkanoid.Ball
{
    public sealed class BallManager : MonoBehaviour, IBallManager
    {
        [SerializeField] private BallController ballPrefab;

        private readonly List<BallController> activeBalls = new();

        public IReadOnlyList<BallController> ActiveBalls => activeBalls;
        public event Action OnAllBallsLost;

        public void RegisterBall(BallController ball)
        {
            if (!activeBalls.Contains(ball))
            {
                activeBalls.Add(ball);
            }
        }

        public void UnregisterBall(BallController ball)
        {
            if (!activeBalls.Remove(ball))
            {
                return;
            }

            // Единственное место, где проверяется "шариков больше нет".
            // Это гарантирует, что при потере одного из нескольких шариков
            // (edge case #1, #4 из ТЗ) жизнь не теряется, пока список не опустеет.
            if (activeBalls.Count == 0)
            {
                OnAllBallsLost?.Invoke();
            }
        }

        public BallController SpawnBall(Vector3 position, Vector3 initialDirection)
        {
            var ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.Launch(initialDirection);
            RegisterBall(ball);
            return ball;
        }

        public void ApplySpeedMultiplierToAll(float multiplier, float duration)
        {
            foreach (var ball in activeBalls)
            {
                ball.ApplySpeedMultiplier(multiplier, duration);
            }
        }
    }
}
