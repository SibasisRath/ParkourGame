public class EventService
{
    public EventController<ResultType> OnPlayerDied {get; private set;}
    public EventController<ResultType> OnPlayerCompleteLevel { get; private set; }
    public EventController OnPlayerTriggerCalamity {get; private set;}
    public EventController OnPlayerQuitsLevel { get; private set; }
    public EventController OnCollectablePickedUp { get; private set; }
    public EventController OnCollectableDestroyed { get; private set; }




    public EventService()
    {
        OnPlayerDied = new();
        OnPlayerCompleteLevel = new();
        OnPlayerTriggerCalamity = new();
        OnPlayerQuitsLevel = new();
        OnCollectablePickedUp = new();
        OnCollectableDestroyed = new();
    }
}
