using UnityEngine;

//this script is attached to objects of levels environment
[RequireComponent (typeof(Collider))]
public class CalamityTriggerObject : MonoBehaviour
{
    [SerializeField] private CalamityScript calamity;   

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerView>(out var playerView))
        {
            Debug.Log("calamity trigger.");
            calamity.StartCalamity();
            playerView.PlayerTriggerCalamity(calamity);
        }
    }
}
