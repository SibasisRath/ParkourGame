using System.Collections.Generic;
using UnityEngine;

public class CollectableService
{
    private ObjectPool objectPool;
    private List<Vector3> collectablesSpawnPositions;
    private List<CollectableView> collectables;
    public int CollectableCount {  get; private set; }
    public int DestroyedCollectableCount { get; private set; } = 0;
    public int PickedCollectableCount { get; private set; } = 0;

    private Vector3 levelPosition;

    private List<int> usedPositions = new ();

    private EventService eventService;

    public CollectableService(List<Vector3> collectablesSpawnPositions, List<CollectableView> collectables, int collectableCount, Transform levelLocation)
    {
        objectPool = new ObjectPool();
        this.CollectableCount = collectableCount;
        this.collectablesSpawnPositions = collectablesSpawnPositions;
        this.collectables = collectables;
        this.levelPosition = levelLocation.position;
    }

    public void SpawnCollectables()
    {
        for (int i = 0; i < CollectableCount; i++)
        {
            
            int positionIndex = GetUniqueRandomPositionIndex();
            Vector3 spawnPosition = collectablesSpawnPositions[positionIndex];
            
            CollectableView prefab = collectables[i % collectables.Count];
            GameObject collectableInstance = objectPool.GetObject(prefab);
            collectableInstance.GetComponent<CollectableView>().Initialize(this, eventService);

            collectableInstance.transform.position = spawnPosition + levelPosition;
            collectableInstance.SetActive(true);
        }

        // Clear used positions after spawning
        usedPositions.Clear();
    }

    private int GetUniqueRandomPositionIndex()
    {
        int index;
        do
        {
            index = Random.Range(0, collectablesSpawnPositions.Count);
        } while (usedPositions.Contains(index));

        usedPositions.Add(index);
        return index;
    }

    public void OnCollectablePicked(CollectableView collectableView)
    {
        Debug.Log("Collectable picked up.");
        PickedCollectableCount++;
        objectPool.ReturnObject(collectableView);
        eventService.OnCollectablePickedUp.InvokeEvent();
    }

    public void OnCollectableSubmerged(CollectableView collectableView)
    {
        Debug.Log("Collectable submerged.");
        DestroyedCollectableCount++;
        objectPool.ReturnObject(collectableView);
        eventService.OnCollectableDestroyed.InvokeEvent();
    }

    public bool DoesAllCatsAreSaved()
    {
        return PickedCollectableCount == CollectableCount;
    }

    public void Init(EventService eventService)
    {
        this.eventService = eventService;
    }

    public bool DoesAllCatsAreDied()
    {
        return PickedCollectableCount == 0;
    }
    public int RestCollectablesCount()
    {
        return CollectableCount - DestroyedCollectableCount;
    }
}