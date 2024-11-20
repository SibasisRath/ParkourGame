using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button mainMenu;

    private LevelService levelService;
    private PlayerService playerService;

    private void Start()
    {
        mainMenu.onClick.AddListener(BackToMainMenu);
    }
    private void BackToMainMenu()
    {
        GameSceneMangaer.Instance.LoadMenuScene();
    }
    public void Init(LevelService levelService, PlayerService playerService)
    {
        this.levelService = levelService;
        this.playerService = playerService;
    }
    public void OnPlayerWins()
    {
        List<ResultType> results = new ();
        results.Add(ResultType.Winner);
        // Check win-specific conditions
        if (DoesPlayerSurvivedWithoutCats())
            results.Add(ResultType.Not_A_Cat_Person);
        if (DoesPlayerSavedAllCats())
            results.Add(ResultType.Cat_Person);
        if (DoesPlayerCompletedLastMinutes())
            results.Add(ResultType.Last_Minutes_Survivor);

        // Check general gameplay conditions
        AddCommonConditions(results);

        // Display results
        DisplayResults(results);
    }

    private bool DoesPlayerCompletedLastMinutes()
    {
        return playerService.CurrentPlayer.PlayerView.Calamity.IsPlayerALastMinuteSurvivor();
    }

    private bool DoesPlayerSavedAllCats()
    {
        return levelService.CurrentLevel.CollectableService.DoesAllCatsAreSaved();
    }

    private bool DoesPlayerSurvivedWithoutCats()
    {
        return levelService.CurrentLevel.CollectableService.DoesAllCatsAreDied();
    }

    public void OnPlayerLooses()
    {
        List<ResultType> results = new ();
        results.Add(ResultType.Did_Not_Win);
        // Check lose-specific conditions
        if (DoesPlayerPerformedAnyParkourActions())
            results.Add(ResultType.Tried_And_Died);

        // Check general gameplay conditions
        AddCommonConditions(results);

        // Display results
        DisplayResults(results);
    }

    private bool DoesPlayerPerformedAnyParkourActions()
    {
        return playerService.CurrentPlayer.PlayerModel.DashCounter > 0;
    }

    private void AddCommonConditions(List<ResultType> results)
    {
        if (playerService.CurrentPlayer.PlayerModel.DashCounter == 0) // no hurry
            results.Add(ResultType.Chill_Player);
        if (playerService.CurrentPlayer.WasPlayerInHurry()) // player was in hurry
            results.Add(ResultType.Quick_Moves);
        if (playerService.CurrentPlayer.PlayerModel.parkourCounter > levelService.CurrentLevel.LevelData.maxParkourCount) // Replace with actual threshold
            results.Add(ResultType.Parkour_Addict);
    }

    private void DisplayResults(List<ResultType> results)
    {
        resultText.text = ""; // Clear previous text
        foreach (ResultType result in results)
        {
            resultText.text += $"{result.ToString().Replace('_', ' ')}\n";
        }
    }
}