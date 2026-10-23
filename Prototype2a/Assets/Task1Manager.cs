using System.Collections;
using UnityEngine;
using TMPro;

public class Task1Manager : MonoBehaviour
{
    public GameObject task1Panel;
    public TMP_Text feedbackText;
    public GameObject startTaskButton;
    public Task2Manager task2Manager;
    public float successPause = 2.1f;
    public float fadeDuration = 0.25f;
    private bool running;
    private bool taskCompleted;

    void Awake() { if (task1Panel != null) task1Panel.SetActive(false); }

    public void ResetTask() // Cancel unfinished transitions and clear the previous task state.
    {
        StopAllCoroutines();
        TaskFeedback.For(this).Clear();
        running = false;
        taskCompleted = false;
        if (task1Panel != null)
        {
            task1Panel.SetActive(false);
            var group = task1Panel.GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = 1f;
                group.interactable = group.blocksRaycasts = true;
            }
        }
    }

    public void StartTask1() // Start the computer-selection task once.
    {
        if (running) return;
        ResetTask();
        if (task2Manager != null) task2Manager.ResetTask();
        running = true;
        if (startTaskButton != null) startTaskButton.SetActive(false);
        if (task1Panel != null) task1Panel.SetActive(true);
        if (feedbackText != null) feedbackText.text = "";
    }

    public void CheckObject(GameObject selectedObject) // Accept the computer tag or LookWord identifier.
    {
        if (!running || taskCompleted || selectedObject == null) return; // Ignore inactive tasks, repeated success and missing selections.
        var word = selectedObject.GetComponentInParent<LookWord>();
        if (selectedObject.tag == "Computer" || (word != null && word.english == "computer"))
        {
            taskCompleted = true;
            if (feedbackText != null) feedbackText.text = "太棒了！\nGreat job!";
            TaskFeedback.For(this).Show(true, "太棒了！找到电脑了\nCorrect! You found the computer.", feedbackText);
            StartCoroutine(Advance());
        }
        else
        {
            if (feedbackText != null) feedbackText.text = "再试一次\nTry again";
            TaskFeedback.For(this).Show(false, "再试一次：请找到电脑\nTry again: find the computer.", feedbackText);
        }
    }

    IEnumerator Advance() // Show success briefly, then fade into Task2.
    {
        yield return new WaitForSecondsRealtime(successPause);
        yield return FadePanel(task1Panel, 1f, 0f, fadeDuration);
        if (task1Panel != null) task1Panel.SetActive(false);
        running = false;
        if (task2Manager != null)
        {
            TaskFeedback.For(this).Play("transition");
            task2Manager.StartTask2();
            yield return FadePanel(task2Manager.task2Panel, 0f, 1f, fadeDuration);
        }
    }

    public static IEnumerator FadePanel(GameObject panel, float from, float to, float duration) // Fade the panel without blocking the game.
    {
        if (panel == null) yield break;
        var group = panel.GetComponent<CanvasGroup>();
        if (group == null) group = panel.AddComponent<CanvasGroup>();
        group.alpha = from;
        group.interactable = group.blocksRaycasts = false; // Prevent hidden or transitioning UI from intercepting input.
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            group.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, elapsed / duration));
            yield return null;
        }
        group.alpha = to;
        group.interactable = group.blocksRaycasts = to > 0f;
    }
}
