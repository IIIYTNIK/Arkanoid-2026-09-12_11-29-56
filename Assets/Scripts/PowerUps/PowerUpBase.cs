using Arkanoid.Core.Interfaces;
using UnityEngine;

namespace Arkanoid.PowerUps
{
    [CreateAssetMenu(fileName = "PowerUpConfig", menuName = "Arkanoid/PowerUp Config")]
    public sealed class PowerUpConfig : ScriptableObject
    {
        [SerializeField] private string effectId;
        [SerializeField] private float duration = 10f;
        [SerializeField] private GameObject visualPrefab;
        [SerializeField, Range(0f, 1f)] private float dropWeight = 0.25f;

        public string EffectId => effectId;
        public float Duration => duration;
        public GameObject VisualPrefab => visualPrefab;
        public float DropWeight => dropWeight;
    }

    /// <summary>
    /// Базовый класс для конкретных PowerUp (ExpandPlatform, SpeedBall, MultiBall, ExtraLife).
    /// Developer 2 наследуется отсюда и реализует только ApplyEffect —
    /// весь "сантехнический" код (EffectId/Duration из конфига) уже готов.
    /// </summary>
    public abstract class PowerUpBase : IPowerUp
    {
        protected readonly PowerUpConfig Config;

        protected PowerUpBase(PowerUpConfig config)
        {
            Config = config;
        }

        public string EffectId => Config.EffectId;
        public float Duration => Config.Duration;

        public void Apply(IPowerUpContext context) => ApplyEffect(context);

        protected abstract void ApplyEffect(IPowerUpContext context);
    }
}
