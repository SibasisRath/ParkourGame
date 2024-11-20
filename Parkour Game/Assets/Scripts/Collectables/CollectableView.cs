using UnityEngine;

// currently i only have cat ,
// so i am making this collectable view.
// If we have more than cat and they have different logics,
// then we can use different view with ICollectable.
public class CollectableView : MonoBehaviour, ICollectable, ISubmergable
{
    CollectableStates collectableState;
    private CollectableService collectableService;
    private EventService eventService;

    public void Initialize(CollectableService colleactableService, EventService eventService)
    {
        this.collectableService = colleactableService;
        this.eventService = eventService;
    }
    private void OnEnable()
    {
        collectableState = CollectableStates.NotPicked;
    }
    public void Interact()
    {
        collectableState = CollectableStates.PickedUp;
        this.gameObject.SetActive(false);

        //Debug.Log("Collectable is collected");


        collectableService.OnCollectablePicked(this);
    }

    public void OnFullySubmerged()
    {
        if (collectableState != CollectableStates.Died)
        {
            collectableState = CollectableStates.Died;
            this.gameObject.SetActive(false);
            //Debug.Log("Collectable is fully submerged. Collectable is destroyed.");

            collectableService.OnCollectableSubmerged(this);
        }
        
    }

    public void OnPartiallySubmerged()
    {
        collectableState = CollectableStates.panic;
        //Debug.Log("Collectable is partially submerged.");
    }
}

