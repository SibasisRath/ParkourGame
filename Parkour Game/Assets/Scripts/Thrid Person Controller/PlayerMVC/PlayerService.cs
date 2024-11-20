using System.Collections.Generic;
using UnityEngine;

public class PlayerService : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public PlayerType playerType;
        public PlayerView playerView;
        public PlayerData playerData;
    }

    [SerializeField] private List<PlayerInfo> playerInfos = new ();
    public PlayerInfo SelectedPlayer {  get; private set; }

    private PlayerView playerView;
    private PlayerData playerData;
    private PlayerController playerController;

    private LevelService levelService;
    private EventService eventService;
    private InGameUI gameUI;

    public PlayerController CurrentPlayer {  get; private set; }

    // Later here we can handel multiple playable characters. 
    public void SettingPlayerInfos(PlayerType playerType = PlayerType.Ninja)
    {
        foreach (PlayerInfo playerInfo in playerInfos)
        {
            if (playerInfo.playerType == playerType)
            {
                SelectedPlayer = playerInfo;
            }
        }
        if (SelectedPlayer == null) { Debug.Log("no player"); }
    }

    public void Init(LevelService levelService, EventService eventService, InGameUI inGameUI)
    {
        this.levelService = levelService;
        this.eventService = eventService;
        this.gameUI = inGameUI;
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }
   


    public void Initialize()
    {
        if (levelService == null || SelectedPlayer == null)
        {
            Debug.LogError("PlayerService dependencies are not properly initialized.");
            return;
        }
        GameObject player = Instantiate(SelectedPlayer.playerView.gameObject, levelService.SelectedLevel.levelData.playerSapwnPosition + levelService.transform.position, Quaternion.identity);
        playerController = new(player.GetComponent<PlayerView>(), SelectedPlayer.playerData);
        playerController.Init(eventService);
        CurrentPlayer = playerController;
    }
}
