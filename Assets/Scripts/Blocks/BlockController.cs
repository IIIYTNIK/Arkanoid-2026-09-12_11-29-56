using Arkanoid.Core;
using Arkanoid.Core.Interfaces;
using UnityEngine;

namespace Arkanoid.Blocks
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class BlockController : MonoBehaviour
    {
        [SerializeField] private BlockData data;
        [SerializeField] private MeshRenderer meshRenderer;

        private int currentHp;

        private void Awake()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            if (data != null)
            {
                currentHp = data.MaxHp;
            }

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
            if (data != null && data.TryGetStateForHp(currentHp, out var state))
            {
                if (meshRenderer != null && state.material != null)
                {
                    meshRenderer.material = state.material;
                }
            }

            // Процедурный окрас блоков под фрукты (яблоки, арбузы, грибы, ягоды)
            if (meshRenderer != null)
            {
                var fruitColors = new Color[] {
                    new Color(0.9f, 0.15f, 0.1f),  // Красное яблоко
                    new Color(0.15f, 0.8f, 0.3f),   // Арбуз
                    new Color(0.85f, 0.55f, 0.2f),  // Лесной гриб
                    new Color(0.65f, 0.2f, 0.85f)   // Лесные ягоды
                };
                int fruitIndex = Mathf.Abs(gameObject.GetInstanceID()) % fruitColors.Length;
                meshRenderer.material.color = fruitColors[fruitIndex];
            }
        }

        private void DestroyBlock()
        {
            if (data != null && data.TryGetStateForHp(1, out var finalState)
                && GameServices.TryGet<IScoreManager>(out var scoreManager))
            {
                scoreManager.AddScore(finalState.scoreValue);
            }

            if (GameServices.TryGet<ILevelManager>(out var levelManager))
            {
                levelManager.NotifyBlockDestroyed();
            }

            if (data != null && Random.value <= data.PowerUpDropChance
                && GameServices.TryGet<IPowerUpService>(out var powerUpService))
            {
                powerUpService.SpawnRandomPowerUp(transform.position);
            }

            Destroy(gameObject);
        }
    }
}