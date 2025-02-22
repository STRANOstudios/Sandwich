using AndreaFrigerio.Framework;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Sirenix.Utilities;
using System.Collections.Generic;

namespace Sandwich
{
    [HideMonoScript]
    public class Interactions : MonoBehaviour
    {
        #region Fields

        [Title("Debug")]
        [SerializeField] private bool m_debugLog = true;
        private GridManager m_gridManager;

        private Node m_nodeFromBackUp = null;
        private Node m_nodeToBackup = null;
        private Vector2Int m_nodeFromPoint;
        private Vector2Int m_nodeToPoint;

        #endregion

        #region Callback Methods

        private void OnEnable()
        {
            ServiceLocator.Register<Interactions>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<Interactions>();
        }

        #endregion

        #region Public Methods

        public void Interact(Vector2Int selectedCell, SwipeDirection swipeDirection)
        {
            m_gridManager = ServiceLocator.Get<GridManager>();

            Node oldNode = m_gridManager.GridNodes[selectedCell.x, selectedCell.y];
            if (oldNode.Storaged.IsNullOrEmpty())
            {
                if (m_debugLog) Debug.Log("Selected cell is empty. Interaction aborted.");
                HighlightInvalidInteraction(oldNode);
                return;
            }

            Vector2Int cellInSwipeDirection = DetermineCellInSwipeDirection(selectedCell, swipeDirection);
            if (cellInSwipeDirection == -Vector2Int.one)
            {
                if (m_debugLog) Debug.Log("Invalid swipe direction.");
                HighlightInvalidInteraction(oldNode);
                return;
            }

            Node newNode = m_gridManager.GridNodes[cellInSwipeDirection.x, cellInSwipeDirection.y];

            if (newNode.Storaged.IsNullOrEmpty())
            {
                if (m_debugLog) Debug.Log("Destination cell is empty. Interaction aborted.");
                HighlightInvalidInteraction(oldNode);
                return;
            }

            if (CanMoveBread() || !oldNode.Storaged.Any(obj => obj.GetComponent<Bread>() != null))
            {
                m_nodeFromPoint = selectedCell;
                m_nodeToPoint = cellInSwipeDirection;

                MoveIngredients(oldNode, newNode);
            }
            else
            {
                HighlightInvalidInteraction(oldNode);
            }
        }

        public void UndoLastInteraction()
        {
            if (m_nodeFromBackUp == null || m_nodeToBackup == null)
            {
                if (m_debugLog) Debug.Log("No previous interaction to undo.");
                return;
            }

            if (m_debugLog) Debug.Log("Undoing last interaction.");

            Node nodeFrom = m_gridManager.GridNodes[m_nodeFromPoint.x, m_nodeFromPoint.y];
            Node nodeTo = m_gridManager.GridNodes[m_nodeToPoint.x, m_nodeToPoint.y];

            nodeFrom.Storaged = m_nodeFromBackUp.Storaged;
            nodeTo.Storaged = m_nodeToBackup.Storaged;

            Move(nodeFrom.Storaged.Count, nodeFrom.Position, nodeFrom.Storaged);

            ResetUndoData();
        }

        public void ResetUndoData()
        {
            m_nodeFromBackUp = null;
            m_nodeToBackup = null;
        }

        #endregion

        #region Private Methods

        private bool CanMoveBread()
        {
            if (m_debugLog) Debug.Log("Checking if bread can be moved.");

            return m_gridManager.HowManyCellsIsNotEmpty() == 2;
        }

        private bool IsSandwichClosing(Node node)
        {
            if (m_debugLog) Debug.Log("Checking if sandwich is closing.");

            // Check if the first and last object in the node are the bread
            return node.Storaged.First().GetComponent<Bread>() && node.Storaged.Last().GetComponent<Bread>();
        }

        private void MoveIngredients(Node fromCell, Node toCell)
        {
            m_nodeFromBackUp = fromCell.Clone();
            m_nodeToBackup = toCell.Clone();

            if (m_debugLog) Debug.Log("Moving ingredients.");

            // Reverse the list of objects if more than one is in the source cell
            if (fromCell.Storaged.Count > 1)
            {
                fromCell.Storaged.Reverse();
            }

            Move(toCell.Storaged.Count, toCell.Position, fromCell.Storaged, () =>
            {
                if (IsSandwichClosing(toCell))
                {
                    if (m_debugLog) Debug.Log("Sandwich closed! Proceeding to the next level.");
                    ServiceLocator.Get<Levels>().NextLevel();
                }
            });

            toCell.Storaged.AddRange(fromCell.Storaged);
            fromCell.Storaged.Clear();
        }

        private void Move(int index, Vector3 pos, List<GameObject> fromCell, System.Action onComplete = null)
        {
            pos.y = index * 0.5f;

            int tweenCount = fromCell.Count;
            foreach (GameObject obj in fromCell)
            {
                obj.transform.DOJump(pos, 1f, 1, 0.5f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        tweenCount--;

                        if (tweenCount == 0)
                        {
                            onComplete?.Invoke();
                        }
                    }
                    );
                //obj.transform.position = pos;
                pos.y += 0.5f;
            }
        }

        private Vector2Int DetermineCellInSwipeDirection(Vector2Int selectedCell, SwipeDirection swipeDirection)
        {
            Vector2Int gridSize = m_gridManager.GridSize;
            Vector2Int newCell = selectedCell;

            switch (swipeDirection)
            {
                case SwipeDirection.Up:
                    newCell.y += 1;
                    break;
                case SwipeDirection.Down:
                    newCell.y -= 1;
                    break;
                case SwipeDirection.Left:
                    newCell.x -= 1;
                    break;
                case SwipeDirection.Right:
                    newCell.x += 1;
                    break;
                case SwipeDirection.None:
                    return -Vector2Int.one;
            }

            return newCell.x < 0 || newCell.x >= gridSize.x || newCell.y < 0 || newCell.y >= gridSize.y
                ? -Vector2Int.one
                : newCell;
        }

        private void HighlightInvalidInteraction(Node cell)
        {
            if (m_debugLog) Debug.Log("Invalid interaction detected. Highlighting cell.");

            foreach (GameObject obj in cell.Storaged)
            {
                MeshRenderer renderer = obj.GetComponentInChildren<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = new Material(renderer.material);
                    Color originalColor = renderer.material.color;

                    renderer.material.DOColor(Color.red, 0.3f)
                        .OnComplete(() => renderer.material.DOColor(originalColor, 0.3f));
                }
            }
        }

        #endregion
    }
}
