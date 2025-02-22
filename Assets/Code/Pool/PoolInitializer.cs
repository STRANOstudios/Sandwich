using Sirenix.OdinInspector;
using UnityEngine;

namespace AndreaFrigerio.Framework
{
    [HideMonoScript]
    public class PoolInitializer : MonoBehaviour
    {
        [System.Serializable]
        public class PoolItem
        {
            public GameObject prefab;
            public int amount;
        }

        [Header("Pool Settings")]
        [SerializeField] private ObjectPooler pooler;
        [SerializeField] private PoolItem[] itemsToPool;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            foreach (var item in itemsToPool)
            {
                if (item.prefab != null)
                    pooler.InitializePool(item.prefab, item.amount);
            }
        }

        public void ClearPool()
        {
            foreach (var item in itemsToPool)
            {
                if (item.prefab != null)
                    pooler.RemoveObjectFromPool(item.prefab);
            }
        }
    }
}
