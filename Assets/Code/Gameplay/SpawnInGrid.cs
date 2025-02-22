using UnityEngine;
using Sirenix.OdinInspector;

namespace Sandwich
{
    [HideMonoScript]
    public class SpawnInGrid : SerializedMonoBehaviour
    {
        [TableMatrix(SquareCells = true)]
        [SerializeField] private GameObject[,] m_grid = new GameObject[4, 4];

        public static implicit operator GameObject[,] (SpawnInGrid spawnInGrid) => spawnInGrid.m_grid;
    }
}