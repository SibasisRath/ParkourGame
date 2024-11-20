using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneMangaer : MonoBehaviour
{
    public static GameSceneMangaer Instance { get; private set; }
    public static GameData GameData { get; private set; } = new GameData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance
        }
    }

    public void SetPlayer(PlayerType player = PlayerType.Ninja)
    {
        GameData.SelectedPlayer = player;
    }

    public void SetLevel(LevelType level = LevelType.FreePlay)
    {
        GameData.SelectedLevel = level;
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("Levels");
    }
    public void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
}

public enum GameSceneType { Menu, MainGame }

public class GameData
{
    public PlayerType SelectedPlayer { get ; set; } = PlayerType.Ninja;
    public LevelType SelectedLevel { get; set; } = LevelType.FreePlay;
}
public enum PlayerType
{
    Ninja
}

public enum LevelType
{
    FreePlay,
    Level1,
    Level2,
    Level3
}

