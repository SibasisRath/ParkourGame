using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelService : MonoBehaviour
{
    [System.Serializable]
    public class LevelInfo
    {
        public LevelType levelType;
        public LevelView levelView;
        public LevelsSO levelData;
    }
    [SerializeField] private List<LevelInfo> levelInfos = new ();
    private CollectableService collectableService;
    public LevelInfo SelectedLevel { get; private set; }

    public LevelController CurrentLevel {  get; private set; }

    private PlayerService playerService;
    private EventService eventService;

    public void SettingLevelInfos(LevelType levelType = LevelType.FreePlay)
    {
        foreach (LevelInfo levelInfo in levelInfos)
        {
            if (levelInfo.levelType == levelType)
            {
                SelectedLevel = levelInfo;
            }
        }
        if (SelectedLevel == null)
        {
            Debug.LogError("SelectedLevel is null in LevelService");
        }
        else if (SelectedLevel.levelData == null)
        {
            Debug.LogError("SelectedLevel.levelData is null in LevelService");
        }
    }
    public void Init(PlayerService playerService, EventService eventService)
    {
        this.eventService = eventService;
        this.playerService = playerService;
    }
    private void Start()
    {
        GameObject level = Instantiate(SelectedLevel.levelView.gameObject, this.transform.position, Quaternion.identity);
        LevelView levelView= level.GetComponent<LevelView>();
        CurrentLevel = new (SelectedLevel.levelData, levelView, eventService);
    }
}
