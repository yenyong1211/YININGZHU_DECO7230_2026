using UnityEngine;
using TMPro;

public class Task1Manager : MonoBehaviour
{
    public GameObject task1Panel;
    public TMP_Text feedbackText;

    public Task2Manager task2Manager;

    private bool taskCompleted = false;

    public void StartTask1()
    {
        task1Panel.SetActive(true);
        feedbackText.text = "";

        if (audioSource != null && taskAudio != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(taskAudio);
        }
    }

    public void CheckObject(GameObject selectedObject)
    {
        if (taskCompleted)
        {
            return;
        }

        if (selectedObject.CompareTag("Computer"))
        {
            taskCompleted = true;

            feedbackText.text = "太棒了！\nGreat job!";

            Invoke(nameof(StartTask2), 1.5f);
        }
        else
        {
            feedbackText.text = "再试一次\nTry again";
        }
    }

    private void StartTask2()
    {
        task1Panel.SetActive(false);

        if (task2Manager != null)
        {
            task2Manager.StartTask2();
        }
    }
}