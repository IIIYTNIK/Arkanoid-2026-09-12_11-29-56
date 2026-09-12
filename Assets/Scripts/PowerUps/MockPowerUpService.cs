using Arkanoid.Core.Interfaces;
using UnityEngine;

namespace Arkanoid.PowerUps
{
    /// <summary>
    /// Временная реализация IPowerUpService — просто логирует, ничего не спавнит.
    /// Позволяет Developer 1 тестировать BlockController.DestroyBlock() до того,
    /// как Developer 2 закончит реальный PowerUpManager (см. п.26 ТЗ).
    /// </summary>
    public sealed class MockPowerUpService : MonoBehaviour, IPowerUpService
    {
        public void SpawnRandomPowerUp(Vector3 spawnPosition)
        {
            Debug.Log($"[MockPowerUpService] Would spawn a power-up at {spawnPosition}");
        }

        public void RegisterActiveEffect(string effectId, float duration)
        {
            Debug.Log($"[MockPowerUpService] Would register effect '{effectId}' for {duration}s");
        }
    }
}
