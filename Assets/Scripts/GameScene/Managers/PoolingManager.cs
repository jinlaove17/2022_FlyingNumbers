using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PoolObject
{
    public Transform group;
    public string tag;
    public GameObject prefab;
    public int initSize;
}

public class PoolingManager : MonoBehaviour
{
    private static PoolingManager instance;

    [SerializeField]
    private PoolObject[] poolObjects;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (PoolObject poolObject in poolObjects)
            {
                poolDictionary.Add(poolObject.tag, new Queue<GameObject>());

                for (int i = 0; i < poolObject.initSize; ++i)
                {
                    CreateNewObject(poolObject.group, poolObject.tag, poolObject.prefab);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameObject Spawn(string tag, Vector3 position)
    {
        return instance.SpawnFromPool(tag, position, Quaternion.identity);
    }

    public static GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
    {
        return instance.SpawnFromPool(tag, position, rotation);
    }

    public static T Spawn<T>(string tag, Vector3 position) where T : Component
    {
        GameObject obj = instance.SpawnFromPool(tag, position, Quaternion.identity);

        if (obj.TryGetComponent(out T component))
        {
            return component;
        }

        throw new Exception("컴포넌트가 존재하지 않습니다.");
    }

    public static T Spawn<T>(string tag, Vector3 position, Quaternion rotation) where T : Component
    {
        GameObject obj = instance.SpawnFromPool(tag, position, rotation);

        if (obj.TryGetComponent(out T component))
        {
            return component;
        }

        throw new Exception("컴포넌트가 존재하지 않습니다.");
    }

    public static void Return(GameObject obj)
    {
        if (!instance.poolDictionary.ContainsKey(obj.name))
        {
            throw new Exception("poolDictionary에 해당 Key가 없습니다.");
        }

        instance.poolDictionary[obj.name].Enqueue(obj);
    }

    private GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            throw new Exception("poolDictionary에 해당 Key가 없습니다.");
        }

        Queue<GameObject> poolQueue = poolDictionary[tag];

        if (poolQueue.Count <= 0)
        {
            PoolObject poolObject = Array.Find(poolObjects, (PoolObject poolObj) => poolObj.tag == tag);
            GameObject newObject = CreateNewObject(poolObject.group, poolObject.tag, poolObject.prefab);

            poolQueue.Enqueue(newObject);
        }

        GameObject objectToSpawn = poolQueue.Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    private GameObject CreateNewObject(Transform group, string tag, GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, group);

        newObject.name = tag;
        newObject.SetActive(false);

        return newObject;
    }
}
