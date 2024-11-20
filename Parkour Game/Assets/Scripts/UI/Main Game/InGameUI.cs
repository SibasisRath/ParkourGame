using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    private bool isGamePaused = false;
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenu;

    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TextMeshProUGUI collectablescountText;
    private int totalPickedCollectableCount = 0;
    private int totalRestCollectableCount = 0;

    [Header("Message")]
    [SerializeField] private OnScreenMessageController onScreenMessage;

    [Header("Result")]
    [SerializeField] private ResultUI resultUI; 
 
    private EventService eventService;
    private LevelService levelService;
    private PlayerService playerService;

    public void Init(EventService eventService, LevelService levelService, PlayerService playerService)
    {
        this.eventService = eventService;
        this.levelService = levelService;
        this.playerService = playerService;
        SubscribeToEvents();
        
    }

    private void SubscribeToEvents()
    {
        eventService.OnPlayerQuitsLevel.AddListener(BackToMainMenu);
        eventService.OnPlayerCompleteLevel.AddListener(ActivatingResultUI);
        eventService.OnPlayerDied.AddListener(ActivatingResultUI);
        eventService.OnPlayerTriggerCalamity.AddListener(DisplayCalamityTriggerMessage);
        eventService.OnCollectablePickedUp.AddListener(DisplayPickUpMessage);
        eventService.OnCollectablePickedUp.AddListener(UpdatePickedUpCollectablesCount);
        eventService.OnCollectableDestroyed.AddListener(DisplayCollectableDestroyMessage);
        eventService.OnCollectableDestroyed.AddListener(UpdateRestOfCollectablesCount);

    }

    private void DisplayCalamityTriggerMessage()
    {
        onScreenMessage.gameObject.SetActive(true);
        onScreenMessage.DisplayText(OnScreenMessageType.CalamityTriggerHit);
    }

    private void DisplayCollectableDestroyMessage()
    {
        onScreenMessage.gameObject.SetActive(true);
        onScreenMessage.DisplayText(OnScreenMessageType.CollectableDestroyed);

    }

    private void DisplayPickUpMessage()
    {
        onScreenMessage.gameObject.SetActive(true);
        onScreenMessage.DisplayText(OnScreenMessageType.CollectablePickedUp);
    }

    private void ActivatingResultUI(ResultType resultType)
    {
        OnPause();
        resultUI.gameObject.SetActive(true);
        resultUI.Init(levelService,playerService);
        switch (resultType)
        {
            case ResultType.Winner:
                resultUI.OnPlayerWins();
                break;
            case ResultType.Did_Not_Win:
                resultUI.OnPlayerLooses(); ;
                break;
        }        
    }

    private void OnDisable()
    {
        eventService.OnPlayerCompleteLevel.RemoveListener(ActivatingResultUI);
        eventService.OnPlayerDied.RemoveListener(ActivatingResultUI);
        eventService.OnPlayerQuitsLevel.RemoveListener(BackToMainMenu);
        eventService.OnCollectablePickedUp.RemoveListener(UpdatePickedUpCollectablesCount);
        eventService.OnCollectablePickedUp.RemoveListener(DisplayPickUpMessage);
        eventService.OnCollectableDestroyed.RemoveListener(UpdateRestOfCollectablesCount);
        eventService.OnCollectableDestroyed.RemoveListener(DisplayCollectableDestroyMessage);
    }

    // Start is called before the first frame update
    void Start()
    {
        OnResume();
        UpdateRestOfCollectablesCount();
        UpdatePickedUpCollectablesCount();
        resumeButton.onClick.AddListener(OnResume);
        mainMenu.onClick.AddListener(BackToMainMenu);
    }

    private void UpdateCollectableCountText()
    {
        collectablescountText.text = totalPickedCollectableCount.ToString()
            + " / " + totalRestCollectableCount.ToString();
    }

    private void UpdateRestOfCollectablesCount()
    {
        totalRestCollectableCount = levelService.CurrentLevel.CollectableService.RestCollectablesCount();
        UpdateCollectableCountText();
    }

    private void UpdatePickedUpCollectablesCount()
    {
        totalPickedCollectableCount = levelService.CurrentLevel.CollectableService.PickedCollectableCount ;
        UpdateCollectableCountText();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isGamePaused) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseAndInfo();
            OnPause();
        }
    }

    private void PauseAndInfo()
    {
        instructionPanel.SetActive(true);
        OnPause();
    }

    private void OnPause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isGamePaused = true;
        
        Time.timeScale = 0f;
    }

    private void OnResume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isGamePaused = false;
        instructionPanel.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void BackToMainMenu()
    {
        GameSceneMangaer.Instance.LoadMenuScene();
    }
}
