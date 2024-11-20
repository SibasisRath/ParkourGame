using System.Collections.Generic;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    private GameObject collectiblePrefab;
    private Transform levelEnvironment;
    private LevelController levelController;

    public void SetLevelController(LevelController levelController)
    {
        this.levelController = levelController;
    }

    private void Start()
    {
        levelController.CollectableService.SpawnCollectables();
    }

    public void SpawnCollectibles(Vector3[] positions)
    {
        foreach (Vector3 position in positions)
        {
            GameObject collectible = Instantiate(collectiblePrefab, levelEnvironment);

            collectible.transform.localPosition = position;
        }
    }
}
