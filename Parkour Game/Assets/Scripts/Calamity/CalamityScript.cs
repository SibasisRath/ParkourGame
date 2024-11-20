using System.Collections.Generic;
using UnityEngine;

// currently I only have water.
// Basically we can add this to different calamity types
// like lava, traps(strong wind force field)
[RequireComponent(typeof(Collider))]
public class CalamityScript : MonoBehaviour
{
    [Header("Position Settings")]
    [SerializeField] private Vector3 initialPosition;   
    [SerializeField] private Vector3 finalPosition;    
    [SerializeField] private bool changePosition = true; 

    [Header("Scale Settings")]
    [SerializeField] private Vector3 initialScale; 
    [SerializeField] private Vector3 finalScale;   
    [SerializeField] private bool changeScale = true;  

    [Header("Animation Settings")]
    [SerializeField] private float duration = 1f;   
    [SerializeField] private bool backAndForth = false;

    private const int LastMinutesOfCalamity = 90;

    private float elapsedTime;
    private bool isReversing = false;
    private bool isActive = false;

    private readonly List<ISubmergable> submergables = new();


    private void Start()
    {
        duration = Mathf.Max(0.01f, duration);

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("ontrigger enter calamity");
        if (other.TryGetComponent<ISubmergable>(out var submergable))
        {
            Debug.Log("submergable enter calamity " + submergable);
            submergables.Add(submergable);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ISubmergable>(out var submergable))
        {
            submergables.Remove(submergable);
        }
    }

    private void Update()
    {
        if (!isActive) return;

        if (elapsedTime >= duration)
        {
            if (backAndForth)
            {
                isReversing = !isReversing;
                (initialPosition, finalPosition) = (finalPosition, initialPosition);
                (initialScale, finalScale) = (finalScale, initialScale);
            }
            else
            {
                // Stop the calamity when it reaches the final state.
                isActive = false;
                elapsedTime = duration; // Ensure progress stays at 100%.
                return;
            }
        }
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        if (changePosition)
        {
            transform.position = Vector3.Lerp(initialPosition, finalPosition, t);
        }

        if (changeScale)
        {
            transform.localScale = Vector3.Lerp(initialScale, finalScale, t);
        }

        CheckIfPlayerInside();
    }

    void CheckIfPlayerInside()
    {
        Bounds waterBounds = new (transform.position, transform.localScale);

        foreach (ISubmergable submergable in submergables)
        {
            MonoBehaviour submergableObj = (MonoBehaviour)submergable;

            if (submergableObj != null)
            {
                Collider objCollider = submergableObj.GetComponent<Collider>();
                Bounds objBounds = objCollider.bounds;

                if (waterBounds.Contains(objBounds.min) && waterBounds.Contains(objBounds.max))
                {
                    submergable.OnFullySubmerged();
                }
            }
        }
    }

    public void StartCalamity()
    {
        if (isActive) return;
        isActive = true;
        elapsedTime = 0f; // Reset animation timing
        Debug.Log("calamity started");
    }

    public void StopCalamity()
    {
        isActive = false;
    }

    private float GetCalamityProgress()
    {
        if (!isActive) return 100f;
        return Mathf.Clamp01(elapsedTime / duration) * 100f;
    }

    public bool IsPlayerALastMinuteSurvivor()
    {
        return LastMinutesOfCalamity < GetCalamityProgress();
    }

    private void OnDisable()
    {
        submergables.Clear();
    }
}
