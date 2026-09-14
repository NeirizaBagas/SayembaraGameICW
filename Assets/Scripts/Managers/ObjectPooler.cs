using System.Collections.Generic;
using UnityEngine;
using ArusMerah.Data;

namespace ArusMerah.Managers
{
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set; }

        private Dictionary<ItemTypeSO, Queue<GameObject>> poolDictionary = new Dictionary<ItemTypeSO, Queue<GameObject>>();
        private Dictionary<GameObject, ItemTypeSO> activeObjectMapping = new Dictionary<GameObject, ItemTypeSO>();
        private Dictionary<ItemTypeSO, Transform> poolContainerMapping = new Dictionary<ItemTypeSO, Transform>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            FlowManager.OnInitializePoolLevels += InitializePoolFromLevels;
        }

        private void OnDisable()
        {
            FlowManager.OnInitializePoolLevels -= InitializePoolFromLevels;
        }

        public void InitializePoolFromLevels(List<LevelDataSO> levelDataList)
        {
            if (levelDataList == null) return;

            Dictionary<ItemTypeSO, int> maxItemCounts = new Dictionary<ItemTypeSO, int>();
            foreach (LevelDataSO level in levelDataList)
            {
                if (level == null || level.randomSpawnItems == null) continue;

                foreach (RandomSpawnItem item in level.randomSpawnItems)
                {
                    if (item.itemTypeSO == null || item.itemTypeSO.itemPrefab == null) continue;

                    if (maxItemCounts.TryGetValue(item.itemTypeSO, out int currentMax))
                    {
                        if (item.spawnCount > currentMax) maxItemCounts[item.itemTypeSO] = item.spawnCount;
                    }
                    else
                    {
                        maxItemCounts[item.itemTypeSO] = item.spawnCount;
                    }
                }
            }

            foreach (KeyValuePair<ItemTypeSO, int> itemSpawnPair in maxItemCounts)
            {
                ItemTypeSO itemType = itemSpawnPair.Key;
                int targetCount = itemSpawnPair.Value;

                if (!poolContainerMapping.TryGetValue(itemType, out Transform container) || container == null)
                {
                    GameObject containerObj = new GameObject($"Pool_{itemType.name}");
                    containerObj.transform.SetParent(transform);
                    container = containerObj.transform;
                    poolContainerMapping[itemType] = container;
                }

                if (!poolDictionary.TryGetValue(itemType, out Queue<GameObject> queue))
                {
                    queue = new Queue<GameObject>();
                    poolDictionary[itemType] = queue;
                }

                int needed = targetCount - queue.Count;
                for (int i = 0; i < needed; i++)
                {
                    GameObject instantiated = Instantiate(itemType.itemPrefab, container);
                    instantiated.SetActive(false);
                    queue.Enqueue(instantiated);
                }
            }
        }

        public GameObject SpawnFromPool(ItemTypeSO itemTypeSO, Vector3 spawnWorldPoint, Quaternion spawnWorldRotation)
        {
            if (itemTypeSO == null)
            {
                Debug.LogError("[ObjectPooler] itemTypeSO is null.");
                return null;
            }

            if (!poolContainerMapping.TryGetValue(itemTypeSO, out Transform container) || container == null)
            {
                GameObject containerObj = new GameObject($"Pool_{itemTypeSO.name}");
                containerObj.transform.SetParent(transform);
                container = containerObj.transform;
                poolContainerMapping[itemTypeSO] = container;
            }

            if (!poolDictionary.TryGetValue(itemTypeSO, out Queue<GameObject> targetQueue))
            {
                targetQueue = new Queue<GameObject>();
                poolDictionary[itemTypeSO] = targetQueue;
            }

            GameObject objectToSpawn;
            if (targetQueue.Count == 0)
            {
                if (itemTypeSO.itemPrefab == null)
                {
                    Debug.LogError($"[ObjectPooler] itemPrefab on {itemTypeSO.name} is null.");
                    return null;
                }
                objectToSpawn = Instantiate(itemTypeSO.itemPrefab, container);
            }
            else
            {
                objectToSpawn = targetQueue.Dequeue();
            }

            objectToSpawn.transform.position = spawnWorldPoint;
            objectToSpawn.transform.rotation = spawnWorldRotation;
            objectToSpawn.SetActive(true);

            activeObjectMapping[objectToSpawn] = itemTypeSO;
            return objectToSpawn;
        }

        public void ReturnToPool(GameObject gameObjectToReturn)
        {
            if (gameObjectToReturn == null) return;

            if (activeObjectMapping.TryGetValue(gameObjectToReturn, out ItemTypeSO itemType))
            {
                activeObjectMapping.Remove(gameObjectToReturn);
                gameObjectToReturn.SetActive(false);

                Transform targetContainer = poolContainerMapping.TryGetValue(itemType, out Transform foundContainer) && foundContainer != null
                    ? foundContainer
                    : transform;
                gameObjectToReturn.transform.SetParent(targetContainer);

                if (poolDictionary.TryGetValue(itemType, out Queue<GameObject> queue))
                {
                    queue.Enqueue(gameObjectToReturn);
                }
                else
                {
                    Queue<GameObject> newQueue = new Queue<GameObject>();
                    newQueue.Enqueue(gameObjectToReturn);
                    poolDictionary[itemType] = newQueue;
                }
            }
            else
            {
                Destroy(gameObjectToReturn);
            }
        }
    }
}
