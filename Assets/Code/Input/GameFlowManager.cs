using Sirenix.OdinInspector;
using UnityEngine;
using AndreaFrigerio.Framework;

namespace Sandwich
{
    [HideMonoScript]
    public class GameFlowManager : MonoBehaviour
    {
        [EnumToggleButtons, SerializeField, Tooltip("Current state of the game flow.")]
        private GameFlowState gameFlowState;

        private void OnEnable()
        {
            ServiceLocator.Register<GameFlowManager>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<GameFlowManager>();
        }

        /// <summary>
        /// Gets the current game flow state.
        /// </summary>
        public GameFlowState CurrentState => gameFlowState;

        /// <summary>
        /// Checks if a specific state or combination of states is active.
        /// </summary>
        /// <param name="state">The state to check.</param>
        /// <returns>True if the state is active, false otherwise.</returns>
        public bool IsStateActive(GameFlowState state)
        {
            return (gameFlowState & state) == state;
        }

        /// <summary>
        /// Sets the current game flow state.
        /// </summary>
        /// <param name="newState">The new state to set.</param>
        public void SetGameState(GameFlowState newState)
        {
            gameFlowState = newState;
            Debug.Log($"Game state updated to: {gameFlowState}");
        }
    }

    [System.Flags]
    public enum GameFlowState
    {
        None = 0,
        Menu = 1 << 0,          // Binary 0001
        PlayerInput = 1 << 1,   // Binary 0010
        Animations = 1 << 2,    // Binary 0100
    }
}
