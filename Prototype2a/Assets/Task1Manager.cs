using UnityEngine;
using TMPro;

public class Task1Manager : MonoBehaviour
{
    public GameObject task1Panel;
    public TMP_Text feedbackText;
    public Task2Manager task2Manager;

    public void StartTask1()
    {
        task1Panel.SetActive(true);
        feedbackText.text = "";
    }

    public void CheckObject(GameObject selectedObject)
    {
        if (selectedObject.CompareTag("Computer"))
        {
            feedbackText.text = "太棒了！ Great job!";

            task1Panel.SetActive(false);

            task2Manager.StartTask2();
        }
        else
        {
            feedbackText.text = "再试一次 Try again";
        }
    }
}