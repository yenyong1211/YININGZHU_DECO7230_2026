using UnityEngine;
using TMPro;

public class Task2Manager : MonoBehaviour
{
    public GameObject task2Panel;
    public TMP_Text feedbackText;
    public TMP_Text instructionText;
    public AudioClip taskInstructionAudio;
    private AudioSource instructionSpeaker;
    private bool running;
    private DeskAdventure adventure;

    void Awake() // Use a separate audio source so feedback cannot interrupt narration.
    {
        instructionSpeaker = gameObject.AddComponent<AudioSource>();
        instructionSpeaker.playOnAwake = false;
        instructionSpeaker.spatialBlend = 0f;
        var preview = GameObject.Find("DeskAdventureModels");
        if (preview != null) preview.SetActive(false);
        ResetTask();
    }

    public void ResetTask() // Stop narration and clean up the active round.
    {
        if (instructionSpeaker != null) instructionSpeaker.Stop();
        running = false;
        if (adventure != null) adventure.StopRound();
        if (task2Panel != null) task2Panel.SetActive(false);
    }

    public void StartTask2() // Start the catch-and-pack game and play its instruction.
    {
        if (running) return;
        running = true;
        if (task2Panel != null) task2Panel.SetActive(true);
        if (adventure == null) adventure = gameObject.GetComponent<DeskAdventure>();
        if (adventure == null) adventure = gameObject.AddComponent<DeskAdventure>();
        adventure.Begin(this);
        if (taskInstructionAudio != null && adventure.CurrentStage != DeskAdventure.Stage.Idle)
        {
            instructionSpeaker.Stop();
            instructionSpeaker.PlayOneShot(taskInstructionAudio);
        }
    }

    public void RestartAdventure() // Reset before starting another round.
    {
        ResetTask();
        StartTask2();
    }

    public void CompleteAdventure() { running = false; if (instructionSpeaker != null) instructionSpeaker.Stop(); }

    public void CheckObject(GameObject selectedObject) // Forward wrong-object selections to the active game.
    {
        if (!running || selectedObject == null || adventure == null) return;
        var word = selectedObject.GetComponentInParent<LookWord>();
        if (word == null || word.english != "book") adventure.WrongObject();
    }

    public void BookPickedUp() // Forward the original red book grab event.
    {
        if (running && adventure != null) adventure.BookGrabbed();
    }
}
