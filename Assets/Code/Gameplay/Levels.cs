using AndreaFrigerio.Framework;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Sandwich
{
    [HideMonoScript]
    public class Levels : MonoBehaviour
    {
        const string NAME_DATA_FILE = "SandwichData";

        #region Fields

        [Title("References")]
        [SerializeField, Required] private TMP_Text m_levelText = null;
        [SerializeField, Required] private List<SpawnInGrid> m_spawnInGrids = new();

        [Title("Debug")]
        [SerializeField] private bool m_debug = false;

        [ShowInInspector, ReadOnly]
        private int m_index = 0;

        [Button("Reload Level")]
        public void ReloadCurrentLevel() => ReloadLevel();

        [Button("Change Level")]
        public void NextLevel() => OnChangeLevel();

        #endregion

        GridManager gridManager;
        ObjectPooler objectPooler;

        #region Callbacks Methods

        private void OnEnable()
        {
            ServiceLocator.Register<Levels>(this);

            GridManager.OnGridGenerated += OnInizialize;
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<Levels>();

            GridManager.OnGridGenerated -= OnInizialize;
        }

        #endregion

        #region Private Methods

        private void OnInizialize()
        {
            if (m_spawnInGrids.IsNullOrEmpty())
            {
                Debug.LogError("No levels found");
                return;
            }

            if (SaveData.Exists(NAME_DATA_FILE))
            {
                m_index = SaveData.Load<int>(NAME_DATA_FILE);
            }

            gridManager = ServiceLocator.Get<GridManager>();
            objectPooler = ServiceLocator.Get<ObjectPooler>();

            OnInizializeLevel(m_spawnInGrids[0]);
        }

        private void OnChangeLevel()
        {
            m_index++;
            if (m_spawnInGrids.Count <= m_index)
            {
                Debug.LogError("No more levels");
                return;
            }

            SaveData.Save<int>(m_index, NAME_DATA_FILE);

            if (m_levelText != null) m_levelText.text = "" + (m_index + 1);

            ServiceLocator.Get<Interactions>().ResetUndoData();

            OnInizializeLevel(m_spawnInGrids[m_index]);
        }

        private void OnInizializeLevel(GameObject[,] grid)
        {
            RemoveAllObjects();

            if (m_debug) Debug.Log("Inizializing level: " + (m_index + 1));

            for (int x = 0; x < gridManager.GridSize.x; x++)
            {
                for (int y = 0; y < gridManager.GridSize.y; y++)
                {
                    if (m_debug) Debug.Log("Creating object: " + grid[x, y]);

                    if (grid[x, y] != null)
                    {
                        var node = gridManager.GridNodes[x, y];
                        var obj = objectPooler.Get(grid[x, y].name);
                        obj.transform.position = node.Position;
                        node.Storaged.Add(obj);
                    }
                }
            }
        }

        private void RemoveAllObjects()
        {
            if (ServiceLocator.Get<ObjectPooler>().IsPoolEmpty())
            {
                return;
            }

            for (int x = 0; x < gridManager.GridSize.x; x++)
            {
                for (int y = 0; y < gridManager.GridSize.y; y++)
                {
                    var node = gridManager.GridNodes[x, y];
                    node.Storaged.Clear();
                }
            }

            ServiceLocator.Get<ObjectPooler>().ReturnAllObjectsToPool();
        }

        #endregion

        #region Public Methods

        public void ReloadLevel()
        {
            OnInizializeLevel(m_spawnInGrids[m_index]);
        }

        // reset to last change

        #endregion

    }
}