using UnityEngine;

public class UIFlowManager : MonoBehaviour
{
    public GameObject introPanel;
    public GameObject learningPanel;
    public GameObject readyPanel;

    public GameObject startTaskButton;

    void Start()
    {
        startTaskButton.SetActive(false);
        ShowIntro();
    }

    public void ShowIntro()
    {
        introPanel.SetActive(true);
        learningPanel.SetActive(false);
        readyPanel.SetActive(false);

        startTaskButton.SetActive(false);
    }

    public void StartLearning()
    {
        introPanel.SetActive(false);
        learningPanel.SetActive(true);
        readyPanel.SetActive(false);

        startTaskButton.SetActive(false);
    }

    public void ShowReady()
    {
        introPanel.SetActive(false);
        learningPanel.SetActive(false);
        readyPanel.SetActive(true);

        startTaskButton.SetActive(false);
    }

    public void StartMission()
    {
        introPanel.SetActive(false);
        learningPanel.SetActive(false);
        readyPanel.SetActive(false);

        startTaskButton.SetActive(true);
    }
}