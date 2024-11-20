using UnityEngine;

public class LevelController
{
    public CollectableService CollectableService {  get; private set; }

    public LevelView LevelView {  get; private set; }
    public LevelsSO LevelData { get; private set; }

    private EventService eventService;

    public LevelController(LevelsSO levelsSO, LevelView levelView, EventService eventService) 
    {
        LevelView = levelView;
        LevelData = levelsSO;
        LevelView.SetLevelController(this);
        CollectableService = new (LevelData.collectablesSapwnPositions, LevelData.collectables, LevelData.collectableCount, levelView.transform);
        this.eventService = eventService;
        CollectableService.Init(eventService);
    }
}
