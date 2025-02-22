using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Sandwich
{
    public partial class GridManager : SerializedMonoBehaviour
    {
        #region Debug

        [Title("Debug")]
        [SerializeField] private bool m_drawGizmos = true;

        [FoldoutGroup("Gizmo")]
        [SerializeField, ColorPalette] private Color m_gridColor = Color.white;

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!m_drawGizmos) return;

            // Draw grid
            Gizmos.color = m_gridColor;
#if UNITY_EDITOR
            Handles.color = m_gridColor;
#endif

            if (GridNodes != null)
            {
                // Iterate through the 2D array GridNodes instead of the Nodes list
                for (int x = 0; x < m_gridSize.x; x++)
                {
                    for (int y = 0; y < m_gridSize.y; y++)
                    {
                        Node node = GridNodes[x, y]; // Access node from the 2D array
#if UNITY_EDITOR
                        Handles.Label(node.Position, $"({x}, {y})");
#endif

                        switch (m_wordSpace)
                        {
                            case WordSpace.XYZ:
                                Gizmos.DrawWireCube(node.Position, Vector3.one * m_cellSize);
                                break;
                            case WordSpace.XY:
                                Gizmos.DrawWireCube(node.Position, new Vector3(m_cellSize, m_cellSize, 0));
                                break;
                            case WordSpace.XZ:
                                Gizmos.DrawWireCube(node.Position, new Vector3(m_cellSize, 0, m_cellSize));
                                break;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
