using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIFlowManager : MonoBehaviour
{
    public GameObject introPanel, learningPanel, readyPanel, startTaskButton;

    private GameObject controlsPanel;

    void Start() { gameObject.AddComponent<PlayerHeight>(); ShowIntro(); }

    void ShowPanel(GameObject panel) // Show only the requested onboarding screen.
    {
        if (controlsPanel != null) controlsPanel.SetActive(panel == controlsPanel);
        introPanel.SetActive(panel == introPanel);
        learningPanel.SetActive(panel == learningPanel);
        readyPanel.SetActive(panel == readyPanel);
        startTaskButton.SetActive(panel == null);
    }

    public void ShowIntro() // Reset both tasks before returning to the introduction.
    {
        var task1 = FindFirstObjectByType<Task1Manager>();
        var task2 = FindFirstObjectByType<Task2Manager>();
        if (task1 != null) task1.ResetTask();
        if (task2 != null) task2.ResetTask();
        ShowPanel(introPanel);
    }

    public void StartLearning() // Reuse the introduction layout for the controller guide.
    {
        if (controlsPanel == null)
        {
            controlsPanel = Instantiate(introPanel, introPanel.transform.parent);
            controlsPanel.name = "Controller Instructions";
            foreach (var text in controlsPanel.GetComponentsInChildren<TMP_Text>(true))
            {
                bool title = text.text == "Chinese Learning Room";
                bool button = text.GetComponentInParent<Button>() != null;
                text.text = button ? "Continue" : title ? "Controller Guide" : "MOVE: Push the left thumbstick.\nHEIGHT: Hold X to lower your view, Y to raise it.\nSELECT: Point the ray and press the index trigger\nto click buttons or activate the computer.\nGRAB: Hold the side grip button with your middle finger.\nRELEASE: Let go of the grip button to put the book down.\nTASK 2: Put the red book inside the bag, then release it.\nLOOK: Turn your head to look around.";
                if (!button) { text.rectTransform.anchoredPosition = new Vector2(0, title ? 220 : 0); text.rectTransform.sizeDelta = new Vector2(850, title ? 70 : 340); text.enableAutoSizing = true; text.fontSizeMin = 22; text.fontSizeMax = title ? 40 : 28; text.raycastTarget = false; }
            }
            foreach (var button in controlsPanel.GetComponentsInChildren<Button>(true))
            {
                ((RectTransform)button.transform).anchoredPosition = new Vector2(0, -230);
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(BeginVocabulary);
            }
        }
        ShowPanel(controlsPanel);
    }

    public void BeginVocabulary() // Open the vocabulary screen at the first word.
    {
        ShowPanel(learningPanel);
        var cards = learningPanel.GetComponentInChildren<LearningCardManager>();
        if (cards != null) cards.ResetCards();
    }

    public void ShowReady() { ShowPanel(readyPanel); }
    public void StartMission() { ShowPanel(null); } // Hide onboarding and reveal the existing task-start button.
}
