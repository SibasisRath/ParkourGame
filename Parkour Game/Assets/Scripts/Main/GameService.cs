using UnityEngine;

public class GameService : MonoBehaviour
{   
    [SerializeField] private PlayerService playerService;
    [SerializeField] private LevelService levelService;
    [SerializeField] private InGameUI gameUI;
    private EventService eventService;
    private GameData gameData;

    void Awake()
    { 
        this.gameData = GameSceneMangaer.GameData;
        eventService = new();
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Explicitly call initialization methods in the correct order
        levelService.SettingLevelInfos(gameData.SelectedLevel);
        playerService.SettingPlayerInfos(gameData.SelectedPlayer);

        levelService.Init(playerService,eventService);
        playerService.Init(levelService, eventService, gameUI);
        gameUI.Init(eventService, levelService, playerService);

        playerService.Initialize();
    }

}
