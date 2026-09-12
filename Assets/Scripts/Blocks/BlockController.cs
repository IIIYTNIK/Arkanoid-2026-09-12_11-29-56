using Arkanoid.Core;
using Arkanoid.Core.Interfaces;
using UnityEngine;

namespace Arkanoid.Blocks
{
    /// <summary>
    /// Блок не знает про уровни или победу — только сообщает ILevelManager
    /// "я уничтожен" (требование п.24.13 ТЗ: BlockController не управляет уровнем).
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class BlockController : MonoBehaviour
    {
        [SerializeField] private BlockData data;
        [SerializeField] private MeshRenderer meshRenderer;

        private int currentHp;

        private void Awake()
        {
            currentHp = data.MaxHp;
            RefreshVisual();
        }

        public void ApplyDamage(int amount)
        {
            currentHp -= amount;

            if (currentHp <= 0)
            {
                DestroyBlock();
                return;
            }

            RefreshVisual();
        }

        private void RefreshVisual()
        {
            if (data.TryGetStateForHp(currentHp, out var state))
            {
                meshRenderer.material = state.material;
            }
        }

        private void DestroyBlock()
        {
            if (data.TryGetStateForHp(1, out var finalState)
                && GameServices.TryGet<IScoreManager>(out var scoreManager))
            {
                scoreManager.AddScore(finalState.scoreValue);
            }

            if (GameServices.TryGet<ILevelManager>(out var levelManager))
            {
                levelManager.NotifyBlockDestroyed();
            }

            if (Random.value <= data.PowerUpDropChance
                && GameServices.TryGet<IPowerUpService>(out var powerUpService))
            {
                // Конкретный тип PowerUp выбирается PowerUpManager (Developer 2)
                // через весовую drop-таблицу — BlockController лишь просит "заспавни что-нибудь".
                powerUpService.SpawnRandomPowerUp(transform.position);
            }

            Destroy(gameObject);
        }
    }
}
