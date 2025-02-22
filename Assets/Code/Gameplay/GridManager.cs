using Sirenix.OdinInspector;
using System;
using UnityEngine;
using AndreaFrigerio.Framework;
using System.Linq;
using System.Collections.Generic;

namespace Sandwich
{
    [HideMonoScript]
    public partial class GridManager : SerializedMonoBehaviour
    {
        public static event Action OnGridGenerated;

        #region Fields

        [Title("Grid Settings")]
        [SerializeField, MinValue(1)] private Vector2Int m_gridSize = new(4, 4);
        [SerializeField, MinValue(0)] private float m_cellSize = 1f;
        /*[SerializeField, MinValue(0)] */
        private float m_cellSpacing = 0f;
        /*[SerializeField]*/
        private WordSpace m_wordSpace = WordSpace.XZ;
        /*[SerializeField] */
        private GridType m_gridType = GridType.Rectangular;

        [Button("Generate Grid")]
        private void GenerateGrid() => InizializeGrid();

        #endregion

        #region Debugging and Visualizations

        [ShowInInspector, ReadOnly, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
        private List<Node> GridNodesList
        {
            get
            {
                return GridNodes.Cast<Node>().ToList();
            }
        }

        #endregion

        #region Variables

        [HideInInspector]
        public Node[,] GridNodes = new Node[4, 4]; // 2D Array for grid nodes

        #endregion

        #region Callbacks Methods

        private void Start() => InizializeGrid();

        private void OnEnable()
        {
            ServiceLocator.Register<GridManager>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<GridManager>();
        }

        #endregion

        #region Private Methods

        private void InizializeGrid()
        {
            // Initialize the 2D array with the grid size
            GridNodes = new Node[m_gridSize.x, m_gridSize.y];

            Vector3 gridOffset = new Vector3(
                -(m_gridSize.x * (m_cellSize + m_cellSpacing)) / 2f + (m_cellSize + m_cellSpacing) / 2f,
                0,
                -(m_gridSize.y * (m_cellSize + m_cellSpacing)) / 2f + (m_cellSize + m_cellSpacing) / 2f
            );

            for (int x = 0; x < m_gridSize.x; x++)
            {
                for (int y = 0; y < m_gridSize.y; y++)
                {
                    Vector3 cellPosition = CalculateCellPosition(gridOffset, x, y);

                    // Assign the node at the [x, y] position in the 2D array
                    GridNodes[x, y] = new Node(cellPosition);
                }
            }

            OnGridGenerated?.Invoke();
        }

        private Vector3 CalculateCellPosition(Vector3 gridOffset, int x, int y)
        {
            Vector3 basePosition = gridOffset + new Vector3(
                x * (m_cellSize + m_cellSpacing),
                0,
                y * (m_cellSize + m_cellSpacing)
            );

            if (m_gridType == GridType.Hexagonal)
            {
                if (y % 2 == 1)
                {
                    basePosition.x += (m_cellSize + m_cellSpacing) / 2f;
                }
            }

            return basePosition;
        }

        #endregion

        #region Getters and Setters

        public Vector2Int GridSize => m_gridSize;
        public float CellSize => m_cellSize;

        public int HowManyCellsIsNotEmpty()
        {
            return GridNodes.Cast<Node>().Count(node => node.Storaged.Count > 0);
        }

        #endregion

    }
}
