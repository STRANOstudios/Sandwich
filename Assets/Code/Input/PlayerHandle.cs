using Sandwich;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AndreaFrigerio.Framework
{
    public class PlayerHandle : MonoBehaviour, IUpdateService
    {
        #region Fields

        [Title("Settings")]
        [SerializeField] private float m_swipeTollerance = 0.5f;

        [Title("Debug")]
        [SerializeField] private bool m_debug = false;
        [ShowIfGroup("m_debug")]
        [ShowInInspector, ReadOnly] private Vector2 m_startPos;
        [ShowIfGroup("m_debug")]
        [ShowInInspector, ReadOnly] private Vector2 m_endPos;
        [ShowIfGroup("m_debug")]
        [ShowInInspector, ReadOnly] private SwipeDirection m_swipeDirection = SwipeDirection.None;
        [ShowIfGroup("m_debug")]
        [ShowInInspector, ReadOnly]
        private Vector2Int m_selectedCell = -Vector2Int.one;

        [SerializeField] private bool m_debugLog = false;

        #endregion

        const float m_raycastDistance = 20f;
        const float m_ingredientHeight = 0.5f;

        private GridManager m_gridManager = null;

        #region Callbacks Methods

        private void OnEnable()
        {
            ServiceLocator.Register<PlayerHandle>(this);

            GridManager.OnGridGenerated += OnInizialize;
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<PlayerHandle>();

            GridManager.OnGridGenerated -= OnInizialize;
        }

        #endregion

        #region Interface Methods

        public void OnUpdate()
        {
            HandlePlayerTurn();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
#endif
        }

        #endregion

        #region Private Methods

        private void OnInizialize()
        {
            m_gridManager = ServiceLocator.Get<GridManager>();
        }

        private void HandlePlayerTurn()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        BeganTouch(touch);
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        EndTouch(touch);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                //if (m_debugLog) Debug.Log("Touch lost (no active touches)");
            }
        }

        private void BeganTouch(Touch touch)
        {
            if (m_debugLog) Debug.Log("Touching");
            m_startPos = touch.position;

            m_selectedCell = DetermineCell(touch);
        }

        private void EndTouch(Touch touch)
        {
            if (m_debugLog) Debug.Log("Swiping");

            m_endPos = touch.position;

            Vector2 swipeDirectionScreen = m_endPos - m_startPos;
            DetermineSwipeDirection(swipeDirectionScreen);

            if (m_debugLog) Debug.Log(m_selectedCell);

            if (m_selectedCell != -Vector2Int.one)
            {
                ServiceLocator.Get<Interactions>().Interact(m_selectedCell, m_swipeDirection);
            }
        }

        private void DetermineSwipeDirection(Vector2 swipeDirection)
        {
            if (swipeDirection.sqrMagnitude < m_swipeTollerance) return;

            OnSwipe(Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y)
                ? (swipeDirection.x > 0 ? SwipeDirection.Right : SwipeDirection.Left)
                : (swipeDirection.y > 0 ? SwipeDirection.Up : SwipeDirection.Down));
        }

        private void OnSwipe(SwipeDirection direction)
        {
            m_swipeDirection = direction;
            if (m_debugLog) Debug.Log($"Detected swipe: {direction}");
        }

        private Vector2Int DetermineCell(Touch touch)
        {
            if (m_debugLog) Debug.Log("Determine Cell");

            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            Vector3 pos = Utils.MathUtils.IntersectRayPlane(ray, Vector3.up, m_gridManager.transform.position);

            Debug.DrawLine(pos, pos + Vector3.up, Color.green, 5f);

            Vector2Int cell = GetCellFromIntersection(pos);

            if (cell == -Vector2Int.one)
            {
                return -Vector2Int.one;
            }

            Debug.DrawLine(m_gridManager.GridNodes[cell.x, cell.y].Position, m_gridManager.GridNodes[cell.x, cell.y].Position + Vector3.up, Color.red, 5f);

            return cell;
        }

        private Vector2Int GetCellFromIntersection(Vector3 intersectionPoint)
        {
            Vector3 offset = intersectionPoint;

            offset.x += m_gridManager.CellSize * (m_gridManager.GridSize.x / 2f);
            offset.z += m_gridManager.CellSize * (m_gridManager.GridSize.y / 2f);

            int xIndex = Mathf.FloorToInt(offset.x / m_gridManager.CellSize);
            int yIndex = Mathf.FloorToInt(offset.z / m_gridManager.CellSize);

            if (xIndex < 0 || xIndex >= m_gridManager.GridSize.x ||
                yIndex < 0 || yIndex >= m_gridManager.GridSize.y)
            {
                return -Vector2Int.one;
            }

            return new Vector2Int(xIndex, yIndex);
        }

        #endregion
    }
}
