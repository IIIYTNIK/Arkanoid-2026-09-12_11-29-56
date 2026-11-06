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
            BuildFruitVisuals();
        }

        private void BuildFruitVisuals()
        {
            // Прямо меняем меш блока на сферу (фрукт), чтобы кубик стал круглым
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            var sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            meshFilter.sharedMesh = sphereObj.GetComponent<MeshFilter>().sharedMesh;
            Destroy(sphereObj);

            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            if (meshRenderer != null)
            {
                // Яркие цвета фруктов: Яблоко, Арбуз, Банан, Виноград
                var fruitColors = new Color[] {
                    new Color(1.0f, 0.2f, 0.2f),   // Красное яблоко
                    new Color(0.2f, 0.9f, 0.3f),   // Зеленый арбуз
                    new Color(1.0f, 0.9f, 0.1f),   // Желтый банан
                    new Color(0.6f, 0.2f, 0.9f)    // Фиолетовый виноград
                };
                int fruitIndex = Mathf.Abs(gameObject.GetInstanceID()) % fruitColors.Length;

                // Создаем новый материал с ярким цветом фрукта
                var fruitMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                fruitMaterial.color = fruitColors[fruitIndex];
                meshRenderer.material = fruitMaterial;
            }
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