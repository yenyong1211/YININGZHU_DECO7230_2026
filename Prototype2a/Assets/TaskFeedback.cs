using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskFeedback : MonoBehaviour
{
    private CanvasGroup group;
    private RectTransform panel;
    private TMP_Text label;
    private AudioSource speaker;
    private Coroutine dismiss;
    private Camera view;

    public static TaskFeedback For(MonoBehaviour owner) // Reuse existing feedback, or add it to the caller if missing.
    {
        var feedback = FindFirstObjectByType<TaskFeedback>();
        return feedback != null ? feedback : owner.gameObject.AddComponent<TaskFeedback>();
    }

    void Awake()
    {
        speaker = gameObject.AddComponent<AudioSource>();
        speaker.playOnAwake = false;
        speaker.spatialBlend = 0f;
        speaker.volume = 0.65f;
        label = CreatePanel(transform, "Player Task Feedback", new Vector2(720, 200), new Vector2(24, 16), 38, 100, new Color(.025f, .04f, .07f, .95f));
        panel = (RectTransform)label.transform.parent;
        group = panel.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0;
        group.interactable = group.blocksRaycasts = false; // Prevent hidden or transitioning UI from intercepting input.
    }

    public static TMP_Text CreatePanel(Transform parent, string name, Vector2 size, Vector2 padding, float fontSize, int order, Color colour) // Build reusable world-space UI for instructions and feedback.
    {
        var root = new GameObject(name, typeof(RectTransform), typeof(Canvas));
        var rect = root.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.sizeDelta = size;
        rect.localScale = Vector3.one * .001f;
        var canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = order;
        var background = root.AddComponent<Image>();
        background.color = colour;
        background.raycastTarget = false;
        var textObject = new GameObject("Message", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(rect, false);
        var textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = padding;
        textRect.offsetMax = -padding;
        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        return text;
    }

    public void Show(bool success, string message, TMP_Text fontSource) // Display a result, play its sound and schedule a fade.
    {
        if (fontSource != null && fontSource.font != null) label.font = fontSource.font;
        label.text = message;
        label.color = success ? new Color(0.45f, 1f, 0.65f) : new Color(1f, 0.75f, 0.35f);
        group.alpha = 1;
        LateUpdate();
        Play(success ? "success" : "retry");
        if (dismiss != null) StopCoroutine(dismiss);
        dismiss = StartCoroutine(Dismiss());
    }

    public void Play(string sound)
    {
        var clip = Resources.Load<AudioClip>("Feedback/" + sound);
        if (clip != null) { speaker.Stop(); speaker.PlayOneShot(clip); }
    }

    public void Clear() // Cancel the fade, hide feedback and stop its sound.
    {
        if (dismiss != null) StopCoroutine(dismiss);
        dismiss = null;
        group.alpha = 0;
        speaker.Stop();
    }

    void LateUpdate() // Position visible feedback in front of the player.
    {
        if (group == null || group.alpha == 0) return;
        if (view == null) view = Camera.main;
        if (view == null) return;

        panel.position = view.transform.position + view.transform.forward * 1.4f - view.transform.up * 0.12f;
        panel.rotation = view.transform.rotation;
    }

    IEnumerator Dismiss() // Wait briefly, then fade out without using scaled game time.
    {
        yield return new WaitForSecondsRealtime(2f);
        for (float t = 0; t < 0.25f; t += Time.unscaledDeltaTime)
        {
            group.alpha = 1f - t / 0.25f;
            yield return null;
        }
        group.alpha = 0;
        dismiss = null;
    }
}
