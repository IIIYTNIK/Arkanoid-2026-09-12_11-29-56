using System;
using Arkanoid.Blocks;
using UnityEngine;

namespace Arkanoid.Levels
{
    [Serializable]
    public struct BlockPlacement
    {
        public BlockData blockData;
        public Vector3 localPosition;
    }

    /// <summary>
    /// Уровень — данные, а не сцена. Добавление нового уровня = новый .asset,
    /// без изменения LevelManager (требование п.22 ТЗ).
    /// </summary>
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Arkanoid/Level Config")]
    public sealed class LevelConfig : ScriptableObject
    {
        [SerializeField] private BlockPlacement[] blockPlacements;

        public BlockPlacement[] BlockPlacements => blockPlacements;
    }
}
