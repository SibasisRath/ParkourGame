using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GameWinTriggerScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerView playerView = other.GetComponent<PlayerView>();
        if (playerView != null)
        {
            playerView.PlayerFinishedTheGameTrigger();
            //Debug.Log("Game Win trigger.");
        }
    }
}
