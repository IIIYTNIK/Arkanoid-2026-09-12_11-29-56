using System;
using UnityEngine;

namespace Arkanoid.Blocks
{
    [Serializable]
    public struct BlockHpVisualState
    {
        public int hp;
        public Material material;
        public int scoreValue;
    }

    /// <summary>
    /// Конфигурация одного типа блока. Список HP→визуал/очки произвольной длины —
    /// добавление блока с HP=10 не требует изменений в коде (требование п.9 ТЗ).
    /// </summary>
    [CreateAssetMenu(fileName = "BlockData", menuName = "Arkanoid/Block Data")]
    public sealed class BlockData : ScriptableObject
    {
        [SerializeField] private BlockHpVisualState[] hpStates;
        [SerializeField, Range(0f, 1f)] private float powerUpDropChance = 0.2f;

        public int MaxHp => hpStates.Length > 0 ? hpStates[^1].hp : 1;
        public float PowerUpDropChance => powerUpDropChance;

        public bool TryGetStateForHp(int hp, out BlockHpVisualState state)
        {
            foreach (var s in hpStates)
            {
                if (s.hp == hp)
                {
                    state = s;
                    return true;
                }
            }

            state = default;
            return false;
        }
    }
}
