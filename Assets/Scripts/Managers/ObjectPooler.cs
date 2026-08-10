using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace ArusMerah.Managers
{
    [System.Serializable]
    public class PoolItemConfig
    {
        public string poolTagKey; // Nama identifikasi pool (misal: "FishSmall", "Trash")
        public GameObject prefabPoolObject; // Prefab yang akan dipooling
        public int numberOfItemsToPrewarm = 15; // Jumlah awal yang dibuat di awal game
        public bool isExpandableIfExhausted = true; // Apakah boleh membuat item baru jika stok pool habis
    }

    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set;  }

        [Header("Konfigurasi daftar pool")]
        [SerializeField] private List<PoolItemConfig> poolItemConfigsList;

        private Dictionary<string, Queue<GameObject>> poolDictionary; // Dictionary internal untuk menyimpan Queue (antrian) GameObject yang sedang tidak aktif

        private Dictionary<GameObject, string> activeObjectTagMapping; // Dictionary pendukung untuk mencatat asal prefab tiap GameObject agar mudah dikembalikan

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            InitializeObjectPools();
        }

        private void InitializeObjectPools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>(); // Ngebuat dictionary baru untuk nyimpen pool diawal game
            activeObjectTagMapping = new Dictionary<GameObject, string>(); // Ngebuat dictionary baru untuk nyimpen mapping tag tiap objek yang aktif

            // Melakukan iterasi atau looping untuk setiap konfigurasi pool yang sudah ditentukan di inspector
            foreach (PoolItemConfig poolConfig in poolItemConfigsList)
            {
                // Setup sistem FIFO untuk setiap pool, sehingga objek yang paling lama tidak aktif akan diambil pertama kali
                Queue<GameObject> objectsInstanceQueue = new Queue<GameObject>();

                //if (FlowManager.instance != null && FlowManager.i)
                /*int prewarmCount =*/  // Pastikan jumlah prewarm tidak negatif

                for (int i = 0; i < poolConfig.numberOfItemsToPrewarm; i++)
                {
                    GameObject instantiatedObject = Instantiate(poolConfig.prefabPoolObject);
                    instantiatedObject.transform.SetParent(this.transform);
                    instantiatedObject.SetActive(false);

                    // Masukin objek yang sudah diinstansiasi ke dalam antrian pool/sistem FIFO
                    objectsInstanceQueue.Enqueue(instantiatedObject);
                }

                // Sistem FIFO yang sudah diisi dengan objek-objek yang sudah diinstansiasi dimasukkan ke dalam dictionary poolDictionary dengan key poolTagKey
                poolDictionary.Add(poolConfig.poolTagKey, objectsInstanceQueue);
            }
        }

        // Mengambil GameObject dari Pool berdasarkan poolTagKey
        public GameObject SpawnFromPool(string poolTagKey, Vector3 spawnWorldPoint, Quaternion spawnWorldRotation)
        {
            if (!poolDictionary.ContainsKey(poolTagKey))
            {
                Debug.LogWarning($"[ObjectPooler]: dengan tag pool '{poolTagKey}' tidak ditemukan!");
                return null;
            }

            Queue<GameObject> targetQueue = poolDictionary[poolTagKey];
            GameObject objectToSpawn = null;

            if (targetQueue.Count == 0) // Kalau objek yang mau di spawn habis
            {
                PoolItemConfig poolConfig = poolItemConfigsList.Find(config => config.poolTagKey == poolTagKey); // Mencari tag yang sesuai dari dictionary list config(Config yang sudah mati)

                if (poolConfig != null && poolConfig.isExpandableIfExhausted) // Kalau confignya ada dan bisa ditambahkan jumlahnya
                {
                    objectToSpawn = Instantiate(poolConfig.prefabPoolObject);
                    objectToSpawn.transform.SetParent(this.transform);
                }
                else
                {
                    Debug.LogWarning($"[ObjectPooler]: Stok pool '{poolTagKey}' habis dan tidak expandable!");
                    return null;
                }
            }
            else // Kalau objek yang mau dispawn masih ada, maka cukup spawn yg paling depan
            {
                objectToSpawn = targetQueue.Dequeue();
            }

            // Atur posisi, rotasi, dan hidupkan objek kembali
            objectToSpawn.transform.position = spawnWorldPoint;
            objectToSpawn.transform.rotation = spawnWorldRotation;
            objectToSpawn.SetActive(true);

            // Kalau objek yang mau dispawn belum ada di dictionary mapping, maka tambahkan ke dictionary mapping agar bisa dikembalikan ke pool yang sesuai nanti
            if (!activeObjectTagMapping.ContainsKey(objectToSpawn))
            {
                activeObjectTagMapping.Add(objectToSpawn, poolTagKey);
            }

            return objectToSpawn;
        }

        public void ReturnToPool(GameObject gameObjectToReturn)
        {
            if (gameObjectToReturn == null) return;

            gameObjectToReturn.SetActive(false); // Matiin visual objek yang sudah ditangkap
            gameObjectToReturn.transform.SetParent(this.transform); // mengembalikan objek ke posisi object pooler

            // Bagian pengecekan tag objek yang ingin dikembalikan ke pool
            if (activeObjectTagMapping.TryGetValue(gameObjectToReturn, out string poolTagKey))
            {
                // Mencari pool dengan key yang sesuai, kalau ada maka masukkan kembali ke antrian pool
                if (poolDictionary.ContainsKey(poolTagKey))
                {
                    // Masukkan kembali objek ke dalam antrian pool
                    poolDictionary[poolTagKey].Enqueue(gameObjectToReturn);
                }
            }
            else
            {
                Destroy(gameObjectToReturn); // Kalau keynya tidak terdaftar langsung hancurkan saja objeknya
            }
        }
    }
}
