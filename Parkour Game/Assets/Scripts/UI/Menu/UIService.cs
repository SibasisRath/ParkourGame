using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIService : MonoBehaviour
{
    [System.Serializable]
    public class PlayerButtonInfo
    {
        public PlayerType type;
        public Button playerButton;
    }

    [System.Serializable]
    public class LevelButtonInfo
    {
        public LevelType type;
        public Button levelButton;
    }

    [SerializeField] private List<PlayerButtonInfo> playerButtons = new();
    [SerializeField] private List<LevelButtonInfo> levelButtons = new();
    [SerializeField] private Button startGameButton;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        foreach (PlayerButtonInfo playerButton in playerButtons)
        {
            playerButton.playerButton.onClick.AddListener(() => SelectPlayer(playerButton.type));
        }
        foreach (LevelButtonInfo levelButton in levelButtons)
        {
            levelButton.levelButton.onClick.AddListener(() => SelectLevel(levelButton.type));
        }
        startGameButton.onClick.AddListener(GameSceneMangaer.Instance.LoadGameScene);
    }
    //PlayerType player
    public void SelectPlayer(PlayerType player)
    {
        GameSceneMangaer.Instance.SetPlayer(player);
    }

    public void SelectLevel(LevelType level)
    {
        GameSceneMangaer.Instance.SetLevel(level);
    }
    public void OnQuit()
    {
        Application.Quit();
    }
}


