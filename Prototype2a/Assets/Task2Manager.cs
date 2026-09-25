using UnityEngine;
using TMPro;

public class Task2Manager : MonoBehaviour
{
    public GameObject task2Panel;
    public TMP_Text feedbackText;

    public AudioSource audioSource;
    public AudioClip taskAudio;
    public AudioClip successAudio;

    private bool taskCompleted = false;

    public void StartTask2()
    {
        task2Panel.SetActive(true);
        feedbackText.text = "";
        taskCompleted = false;

        if (audioSource != null && taskAudio != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(taskAudio);
        }
    }

    public void BookPickedUp()
    {
        if (taskCompleted)
        {
            return;
        }

        taskCompleted = true;

        feedbackText.text = "太棒了！\nGreat job!";

        if (audioSource != null && successAudio != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(successAudio);
        }

        Debug.Log("Task 2 completed.");
    }
}