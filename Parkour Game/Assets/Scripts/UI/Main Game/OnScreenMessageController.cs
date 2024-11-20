using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnScreenMessageController : MonoBehaviour
{
    [System.Serializable]
    public class MessageInfo
    {
        public OnScreenMessageType type;
        public string messageString;
    }
    [SerializeField] private List<MessageInfo> messageInfos = new ();
    public string textToBeDisplayed { get; set; } = "";

    [Header("Settings")]
    [SerializeField] private float letterDelay = 0.05f;
    [SerializeField] private float readDelay = 0.5f;
    [SerializeField] private GameObject displaypanel;
    [SerializeField] private TextMeshProUGUI displayTextArea;

    private Coroutine typingCoroutine; // Reference to the running coroutine.

    public void DisplayText(OnScreenMessageType onScreenMessageType)
    {
        foreach (MessageInfo info in messageInfos) 
        {
            if (info.type == onScreenMessageType) textToBeDisplayed = info.messageString;
        }

        // Stop any existing typing coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Start the new typing coroutine
        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        displayTextArea.text = ""; // Clear the text display initially.

        foreach (char letter in textToBeDisplayed)
        {
            displayTextArea.text += letter; // Add the next letter.
            yield return new WaitForSeconds(letterDelay); // Wait before showing the next letter.
        }

        yield return new WaitForSeconds(readDelay);
        gameObject.SetActive(false);
    }
}

public enum OnScreenMessageType
{
    None,
    CollectablePickedUp,
    CollectableDestroyed,
    CalamityTriggerHit
}
