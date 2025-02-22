using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sandwich
{
    public class PlayerInput : MonoBehaviour
    {
        [Title("Swipe Settings")]
        [SerializeField] private float swipeThreshold = 50f; // Minimum swipe distance in pixels
        [SerializeField] private float timeThreshold = 0.3f; // Maximum time for a valid swipe
        [SerializeField] private float swipeTollerance = 0.5f;

        [SerializeField, Required] private InputActionAsset inputActions;

        private InputAction playerAction; // Input action for detecting swipe
        private Vector2 startTouchPosition; // Starting position of the swipe
        private Vector2 endTouchPosition; // Ending position of the swipe
        private float startTime; // Time when the swipe starts
        private bool isSwiping; // Track whether a swipe is ongoing

        private void Awake()
        {
            // Reference the Player input action
            playerAction = inputActions["Swipe"]; // Ensure the action name is "Player"
        }

        private void OnEnable()
        {
            // Enable the action
            playerAction.Enable();
            playerAction.started += OnTouchStart;
            playerAction.performed += OnTouchMove;
            playerAction.canceled += OnTouchEnd;
        }

        private void OnDisable()
        {
            // Disable the action
            playerAction.Disable();
            playerAction.started -= OnTouchStart;
            playerAction.performed -= OnTouchMove;
            playerAction.canceled -= OnTouchEnd;
        }

        private void OnTouchStart(InputAction.CallbackContext context)
        {
            // Start recording the swipe
            startTouchPosition = context.ReadValue<Vector2>();
            startTime = Time.time;
            isSwiping = true;

            Ray ray = Camera.main.ScreenPointToRay(startTouchPosition);

            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log(hit.transform.name);
            }
        }

        private void OnTouchMove(InputAction.CallbackContext context)
        {
            // Update the end position if a swipe is ongoing
            if (isSwiping)
            {
                endTouchPosition = context.ReadValue<Vector2>();
            }
        }

        private void OnTouchEnd(InputAction.CallbackContext context)
        {
            // End the swipe
            endTouchPosition = context.ReadValue<Vector2>();
            float endTime = Time.time;

            isSwiping = false;

            // Validate and process the swipe
            if (IsValidSwipe(startTouchPosition, endTouchPosition, startTime, endTime))
            {
                Vector2 swipeDirection = endTouchPosition - startTouchPosition;
                DetermineSwipeDirection(swipeDirection);
            }
        }

        private bool IsValidSwipe(Vector2 start, Vector2 end, float startTime, float endTime)
        {
            // Check if the swipe distance exceeds the threshold
            if (Vector2.Distance(start, end) < swipeThreshold)
                return false;

            // Check if the swipe time is within the threshold
            if (endTime - startTime > timeThreshold)
                return false;

            return true;
        }

        private void DetermineSwipeDirection(Vector2 swipeDirection)
        {
            swipeDirection.Normalize(); // Normalize the swipe vector

            if (Vector2.Dot(swipeDirection, Vector2.down) > swipeTollerance)
            {
                Debug.Log("Swipe Up");
                OnSwipe(SwipeDirection.Up);
            }
            else if (Vector2.Dot(swipeDirection, Vector2.up) > swipeTollerance)
            {
                Debug.Log("Swipe Down");
                OnSwipe(SwipeDirection.Down);
            }
            else if (Vector2.Dot(swipeDirection, Vector2.left) > swipeTollerance)
            {
                Debug.Log("Swipe Right");
                OnSwipe(SwipeDirection.Right);
            }
            else if (Vector2.Dot(swipeDirection, Vector2.right) > swipeTollerance)
            {
                Debug.Log("Swipe Left");
                OnSwipe(SwipeDirection.Left);
            }
        }

        private void OnSwipe(SwipeDirection direction)
        {
            // Implement game logic for swipe here
            Debug.Log($"Swiped: {direction}");
        }
    }
}
