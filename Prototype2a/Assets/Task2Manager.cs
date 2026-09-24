using UnityEngine;
using TMPro;

public class Task2Manager : MonoBehaviour
{
    public GameObject task2Panel;
    public TMP_Text feedbackText;

    private bool taskCompleted = false;

    public void StartTask2()
    {
        task2Panel.SetActive(true);
        feedbackText.text = "";
        taskCompleted = false;
    }

    public void BookPickedUp()
    {
        if (taskCompleted)
        {
            return;
        }

        taskCompleted = true;

        feedbackText.text = "太棒了！";

        Debug.Log("Task 2 completed.");
    }
}