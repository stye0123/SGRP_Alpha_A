using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [Header("物件池配置")]
    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"物件池標籤 {tag} 不存在！");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        
        // 如果池子空了，創建新物件
        if (pool.Count == 0)
        {
            Pool poolConfig = pools.Find(p => p.tag == tag);
            if (poolConfig != null)
            {
                GameObject newObj = Instantiate(poolConfig.prefab);
                pool.Enqueue(newObj);
            }
        }

        GameObject objectToSpawn = pool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"物件池標籤 {tag} 不存在！");
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }
}

// 物件池介面
public interface IPooledObject
{
    void OnObjectSpawn();
} 