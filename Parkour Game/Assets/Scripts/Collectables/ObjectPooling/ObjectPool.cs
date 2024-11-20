using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Dictionary<CollectableView, Queue<CollectableView>> pool = new Dictionary<CollectableView, Queue<CollectableView>>();

    public GameObject GetObject(CollectableView prefab)
    {
        if (!pool.ContainsKey(prefab))
        {
            pool[prefab] = new Queue<CollectableView>();
        }

        if (pool[prefab].Count > 0)
        {
            CollectableView instance = pool[prefab].Dequeue();
            instance.gameObject.SetActive(true);
            return instance.gameObject;
        }
        else
        {
            // Instantiate new if the pool is empty for this prefab
            return Object.Instantiate(prefab.gameObject);
        }
    }

    public void ReturnObject(CollectableView instance)
    {
        instance.gameObject.SetActive(false);
        if (!pool.ContainsKey(instance))
        {
            pool[instance] = new Queue<CollectableView>();
        }
        pool[instance].Enqueue(instance);
    }
}