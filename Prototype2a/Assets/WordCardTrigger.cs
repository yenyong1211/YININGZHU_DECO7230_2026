using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WordCardTrigger : MonoBehaviour
{
    public Task1Manager task1Manager;
    private Task2Manager task2Manager;
    private XRBaseInteractable interactable;
    private float lastInteraction = -1f;

    void Awake() // Resolve the task managers and this object’s XR interactable.
    {
        if (task1Manager == null) task1Manager = FindFirstObjectByType<Task1Manager>();
        task2Manager = FindFirstObjectByType<Task2Manager>();
        interactable = GetComponent<XRBaseInteractable>();
    }

    void OnEnable() // Subscribe to XR activation while enabled.
    {
        if (interactable != null) interactable.activated.AddListener(OnActivated);
    }

    void OnDisable() // Remove the listener to avoid duplicate subscriptions.
    {
        if (interactable != null) interactable.activated.RemoveListener(OnActivated);
    }

    void OnActivated(ActivateEventArgs args) { ShowCard(); }

    public void ShowCard() // Keep the original callback name; check tasks without opening word cards.
    {
        if (Time.unscaledTime - lastInteraction < 0.25f) return; // Ignore duplicate interactions within 0.25 seconds.
        lastInteraction = Time.unscaledTime;
        if (task1Manager != null) task1Manager.CheckObject(gameObject);
        if (task2Manager != null) task2Manager.CheckObject(gameObject);
    }
}
