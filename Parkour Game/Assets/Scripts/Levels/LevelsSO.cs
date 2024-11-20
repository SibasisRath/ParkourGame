using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelSO/New Level")]
public class LevelsSO : ScriptableObject
{
    public LevelType levelType = LevelType.FreePlay;
    public Vector3 playerSapwnPosition;
    public int maxParkourCount;

    [Header("Collectable setting")]
    public List<Vector3> collectablesSapwnPositions;
    public List<CollectableView> collectables;
    public int collectableCount = 3;

    [Header("Sound setting")]
    public bool isThereAnySound = true;
    public SoundType soundType = SoundType.Air;
}
