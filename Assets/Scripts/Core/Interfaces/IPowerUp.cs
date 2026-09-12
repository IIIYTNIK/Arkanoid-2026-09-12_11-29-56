using Arkanoid.Core.Interfaces;
using Arkanoid.Paddle;
using UnityEngine;

namespace Arkanoid.Core.Interfaces
{
    /// <summary>
    /// Контекст, который PowerUp получает при активации. Даёт доступ ТОЛЬКО
    /// к тому, что нужно для применения эффекта — не к менеджерам напрямую,
    /// чтобы конкретные PowerUp (пишет Dev2) не могли произвольно менять
    /// чужие системы в обход их API (требование п.24.14 ТЗ).
    /// </summary>
    public interface IPowerUpContext
    {
        IBallManager Balls { get; }
        ILifeManager Lives { get; }
        PaddleController Paddle { get; }
    }

    /// <summary>Реализуется каждым конкретным PowerUp (ExpandPlatform, SpeedBall, ...).</summary>
    public interface IPowerUp
    {
        string EffectId { get; }
        float Duration { get; }
        void Apply(IPowerUpContext context);
    }

    /// <summary>
    /// Ядро системы PowerUp (реализует Dev1). Отвечает за спавн, физическое падение,
    /// подбор платформой и жизненный цикл временных эффектов.
    /// </summary>
    public interface IPowerUpService
    {
        /// <summary>
        /// Блок уже решил, что дроп случился (PowerUpDropChance прошёл) — этот метод
        /// сам выбирает КАКОЙ именно PowerUp выпадет, используя весовую drop-таблицу
        /// (PowerUpManager, Developer 2). BlockController не знает про типы PowerUp.
        /// </summary>
        void SpawnRandomPowerUp(Vector3 spawnPosition);

        /// <summary>
        /// Регистрирует активный временный эффект. Если effectId уже активен,
        /// ПРОДЛЕВАЕТ его длительность, а НЕ складывает/накапливает (edge case #5 ТЗ).
        /// </summary>
        void RegisterActiveEffect(string effectId, float duration);
    }
}
