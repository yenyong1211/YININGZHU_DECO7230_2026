using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DeskAdventure : MonoBehaviour
{
    public enum Stage { Idle, Chase, Pack, Won, TimedOut }
    public Stage CurrentStage { get; private set; }
    public float roundSeconds = 180f; // Round time limit in seconds.
    public XRGrabInteractable TargetBook => book;
    public BoxCollider BagVolume => bagVolume;
    public TMP_Text MissionText => hud;
    private XRGrabInteractable book;
    private Rigidbody bookBody;
    private Transform bookParent, bag, mascot;
    private Vector3 home, bookScale;
    private Quaternion bookRotation;
    private bool originalKinematic, originalGravity, originalThrow;
    private GameObject props;
    private BoxCollider bagVolume;
    private TMP_Text hud;
    private Task2Manager task;
    private Camera view;
    private float remaining, elapsed, settled, nextHintTime;
    private readonly List<Transform> labels = new List<Transform>();

    private readonly Dictionary<Material, Material> frontMaterials = new Dictionary<Material, Material>(); // Cache private foreground materials for reuse.
    void OnEnable() { Canvas.willRenderCanvases += DrawInFront; }
    void OnDisable() { Canvas.willRenderCanvases -= DrawInFront; }
    void DrawInFront() // Render task UI over scene geometry without changing shared materials.
    {
        if (hud == null || !hud.gameObject.activeInHierarchy) return;
        foreach (var graphic in hud.transform.parent.GetComponentsInChildren<Graphic>())
        {
            bool text = graphic is TMP_Text || graphic is TMP_SubMeshUI;
            for (int i = 0; i < graphic.canvasRenderer.materialCount; i++)
            {
                var source = graphic.canvasRenderer.GetMaterial(i);
                if (source == null || source.shader.name.StartsWith("DeskUI/")) continue;
                if (!frontMaterials.TryGetValue(source, out var copy))
                {
                    copy = new Material(source);
                    copy.shader = Resources.Load<Shader>(text ? "DeskUI/TextOverlay" : "DeskUI/PanelOverlay");
                    copy.renderQueue = 4000;
                    frontMaterials.Add(source, copy);
                }
                graphic.canvasRenderer.SetMaterial(copy, i);
            }
        }
    }

    public void Begin(Task2Manager owner) // Restore the previous round, find the red book and create round props.
    {
        StopRound();
        task = owner;
        var stationery = FindFirstObjectByType<DeskStationery>();
        if (stationery != null) stationery.ResetItems();
        foreach (var word in FindObjectsByType<LookWord>(FindObjectsSortMode.None))
        {
            if (word.english != "book") continue;
            book = word.GetComponent<XRGrabInteractable>();
            break;
        }
        if (book == null)
        {
            TaskFeedback.For(this).Show(false, "找不到可抓取的红书\nMissing grabbable red book.", task.feedbackText);
            return;
        }
        bookBody = book.GetComponent<Rigidbody>();
        home = book.transform.position;
        bookRotation = book.transform.rotation;
        bookScale = book.transform.localScale;
        bookParent = book.transform.parent;
        originalKinematic = bookBody.isKinematic;
        originalGravity = bookBody.useGravity;
        originalThrow = book.throwOnDetach;
        book.throwOnDetach = false;
        Transform template = null;
        foreach (var candidate in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (candidate.name == "DeskAdventureModels") { template = candidate; break; }
        }
        if (template == null)
        {
            book.throwOnDetach = originalThrow;
            TaskFeedback.For(this).Show(false, "找不到书包模型\nMissing bag models.", task.feedbackText);
            return;
        }
        props = Instantiate(template.gameObject, template.position, template.rotation, template.parent);
        props.name = "Playful Desk Adventure";
        props.SetActive(true);
        bag = props.transform.Find("Bag");
        mascot = props.transform.Find("多儿");
        FitBag();
        CreateHUD();
        Label(bag, "书包\nBAG", .36f);
        if (mascot != null) Label(mascot, "多儿\nDUO ER", .13f);
        var restart = props.transform.Find("Restart").gameObject;
        Label(restart.transform, "重新开始\nRESTART", .12f);
        restart.AddComponent<XRSimpleInteractable>().selectEntered.AddListener(RestartSelected);
        book.selectExited.AddListener(BookReleased);
        remaining = roundSeconds;
        elapsed = settled = nextHintTime = 0f;
        SetStage(Stage.Chase);
        if (book.isSelected) BookGrabbed();
        else { bookBody.isKinematic = true; bookBody.useGravity = false; }
    }

    void FitBag() // Fit the bag to the book and create its invisible interior target.
    {
        var bounds = ItemBounds(book);
        var scale = bag.localScale;
        scale.x *= Mathf.Max(1f, (bounds.size.x + .04f) / (.26f * Mathf.Abs(bag.lossyScale.x)));
        scale.z *= Mathf.Max(1f, (bounds.size.z + .04f) / (.20f * Mathf.Abs(bag.lossyScale.z)));
        bag.localScale = scale;
        var target = new GameObject("Bag interior target");
        target.transform.SetParent(bag, false);
        target.transform.localPosition = new Vector3(0f, .08f, 0f);
        bagVolume = target.AddComponent<BoxCollider>();
        bagVolume.isTrigger = true;
        bagVolume.size = new Vector3(.26f, .38f, .20f);
    }

    void CreateHUD() // Create an opaque instruction panel using the task font.
    {
        hud = TaskFeedback.CreatePanel(props.transform, "Task2 Instructions", new Vector2(900, 230), new Vector2(16, 8), 32, 110, new Color(.02f, .04f, .08f, 1f));
        if (task.feedbackText != null) hud.font = task.feedbackText.font;
    }

    void Label(Transform target, string text, float height)
    {
        var root = new GameObject("Label", typeof(TextMeshPro));
        root.transform.SetParent(target, false);
        root.transform.localScale = new Vector3(1f / target.lossyScale.x, 1f / target.lossyScale.y, 1f / target.lossyScale.z);
        root.transform.position = target.position + Vector3.up * height;
        var label = root.GetComponent<TextMeshPro>();
        if (task.feedbackText != null) label.font = task.feedbackText.font;
        label.text = text;
        label.fontSize = .7f;
        label.alignment = TextAlignmentOptions.Center;
        label.rectTransform.sizeDelta = new Vector2(.4f, .12f);
        labels.Add(root.transform);
    }

    void RestartSelected(SelectEnterEventArgs args) { task.RestartAdventure(); }

    public void BookGrabbed() // Stop escape movement by switching from Chase to Pack.
    {
        if (CurrentStage != Stage.Chase) return;
        bookBody.useGravity = originalGravity;
        SetStage(Stage.Pack);
    }

    void BookReleased(SelectExitEventArgs args) // Restore the book physics after release.
    {
        if (CurrentStage != Stage.Pack) return;
        bookBody.isKinematic = originalKinematic;
        bookBody.useGravity = originalGravity;
    }

    public void WrongObject()
    {
        if (CurrentStage != Stage.Chase && CurrentStage != Stage.Pack) return;
        TaskFeedback.For(this).Show(false, "请拿红书，放进书包。\nUse the RED book and put it in the bag.", task.feedbackText);
    }

    public string Mission()
    {
        switch (CurrentStage)
        {
            case Stage.Chase: return "抓住逃跑的红书，放进书包，然后松手。\nCatch the RED book, put it in the bag and release.";
            case Stage.Pack: return "把红书放进书包，然后松手。\nPut the RED book inside the bag and release.";
            case Stage.Won: return "任务完成！\nMission complete!";
            case Stage.TimedOut: return "时间到了，请重新开始。\nTime's up. Restart to try again.";
            default: return "准备开始任务。\nReady to start.";
        }
    }

    void SetStage(Stage stage) // Change stage and update both instructions and feedback.
    {
        CurrentStage = stage;
        settled = 0f;
        string message = Mission();
        if (task.instructionText != null) task.instructionText.text = message.Split('\n')[0];
        if (task.feedbackText != null) task.feedbackText.text = message;
        RefreshMission();
        TaskFeedback.For(this).Show(stage != Stage.TimedOut, message, task.feedbackText);
    }

    void RefreshMission()
    {
        if (hud != null) hud.text = "TASK 2 · " + Mathf.CeilToInt(remaining) + "s\n" + Mission();
    }

    void LateUpdate() // Keep instructions and labels facing the player.
    {
        if (props == null) return;
        if (view == null) view = Camera.main;
        if (view == null) return;
        var panel = hud.transform.parent;
        panel.position = view.transform.position + view.transform.forward * .9f + view.transform.up * (CurrentStage == Stage.Won ? 0f : .22f);
        panel.localScale = Vector3.one * .0007f;
        panel.rotation = view.transform.rotation;
        foreach (var label in labels) if (label != null) label.rotation = view.transform.rotation;
    }

    void Update() // Advance time, move the book and check packing success.
    {
        if (props == null) return;
        if (mascot != null) mascot.position = home + new Vector3(Mathf.Sin(Time.time) * .25f, .3f, .2f);
        if (CurrentStage != Stage.Chase && CurrentStage != Stage.Pack) return;
        remaining = Mathf.Max(0f, remaining - Time.deltaTime);
        elapsed += Time.deltaTime;
        RefreshMission();
        if (remaining <= 0f) { SetStage(Stage.TimedOut); RestoreBook(); return; }
        if (CurrentStage == Stage.Chase && !book.isSelected) // Only escape while chasing and not held.
        {
            Vector3 offset = new Vector3(Mathf.Sin(elapsed * 1.8f) * .5f, .12f + Mathf.Abs(Mathf.Sin(elapsed * 2f)) * .13f, Mathf.Sin(elapsed * .9f) * .22f);
            bookBody.MovePosition(Vector3.MoveTowards(bookBody.position, home + offset, Time.deltaTime * .65f));
        }
        if (CurrentStage == Stage.Pack && InsideBag(book, bagVolume) && !book.isSelected) // Both correct placement and release are required.
        {
            settled += Time.deltaTime;
            if (settled >= .4f) // Require a released book to remain inside for 0.4 seconds.
            {
                SetStage(Stage.Won);
                task.CompleteAdventure();
                int stars = remaining > 90f ? 3 : remaining > 30f ? 2 : 1;
                TaskFeedback.For(this).Clear();
                if (task.task2Panel != null) task.task2Panel.SetActive(false);
                ((RectTransform)hud.transform.parent).sizeDelta = new Vector2(1000, 600);
                hud.fontSize = 44;
                hud.text = "任务完成！\nMission complete!\n" + new string('★', stars) + "\n书包装好了！\nSelect RESTART to play again.";
                CreateExitButton();
                return;
            }
        }
        else settled = 0f;
        ExplainBagAttempt();
        if (!book.isSelected && (book.transform.position.y < home.y - .8f || Vector3.Distance(book.transform.position, home) > 2.5f))
        {
            bookBody.position = home;
            if (!bookBody.isKinematic) { bookBody.linearVelocity = Vector3.zero; bookBody.angularVelocity = Vector3.zero; }
        }
    }

    void CreateExitButton() // Create an exit action usable by controller rays or the editor mouse.
    {
        var panel = (RectTransform)hud.transform.parent;
        panel.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
        panel.gameObject.AddComponent<GraphicRaycaster>();
        ((RectTransform)hud.transform).offsetMin = new Vector2(16, 140);
        var buttonObject = new GameObject("Exit Button", typeof(RectTransform), typeof(Image), typeof(Button));
        var rect = buttonObject.GetComponent<RectTransform>();
        rect.SetParent(panel, false);
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, 0);
        rect.anchoredPosition = new Vector2(0, 70);
        rect.sizeDelta = new Vector2(400, 90);
        var image = buttonObject.GetComponent<Image>();
        image.color = new Color(.12f, .35f, .55f);
        var button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(ExitGame);
        var labelObject = new GameObject("Exit Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(rect, false);
        var labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero; labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;
        var label = labelObject.GetComponent<TextMeshProUGUI>();
        label.font = hud.font; label.fontSize = 36;
        label.text = "退出游戏 / Exit";
        label.alignment = TextAlignmentOptions.Center; label.raycastTarget = false;
    }

    public void ExitGame() // Stop Play mode in the editor; quit the installed app.
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ExplainBagAttempt() // Explain held-book and wrong-book attempts without spamming hints.
    {
        if (Time.unscaledTime < nextHintTime) return;
        string message = null;
        if (InsideBag(book, bagVolume) && book.isSelected)
            message = "位置对了！请松手。\nCorrect position! Release the book.";
        else
        {
            foreach (var word in FindObjectsByType<LookWord>(FindObjectsSortMode.None))
            {
                if (word.english != "green book") continue;
                var green = word.GetComponent<XRGrabInteractable>();
                if (green != null && InsideBag(green, bagVolume)) message = "这是绿书，请把红书放进书包。\nThat is the green book. Use the RED book.";
            }
        }
        if (message == null) return;
        nextHintTime = Time.unscaledTime + 3f;
        TaskFeedback.For(this).Show(false, message, task.feedbackText);
    }

    public static Bounds ItemBounds(XRGrabInteractable item) // Combine enabled non-trigger colliders to locate the book centre.
    {
        Bounds bounds = new Bounds(item.transform.position, Vector3.zero);
        bool found = false;
        foreach (var collider in item.GetComponentsInChildren<Collider>())
        {
            if (collider.isTrigger || !collider.enabled || !collider.gameObject.activeInHierarchy) continue;
            if (!found) { bounds = collider.bounds; found = true; }
            else bounds.Encapsulate(collider.bounds);
        }
        return bounds;
    }

    public static bool InsideBag(XRGrabInteractable item, BoxCollider interior) // Check the book centre in the bag target’s local coordinates.
    {
        if (item == null || interior == null || !item.gameObject.activeInHierarchy) return false;
        Vector3 center = interior.transform.InverseTransformPoint(ItemBounds(item).center) - interior.center;
        Vector3 half = interior.size * .5f;
        return Mathf.Abs(center.x) <= half.x && Mathf.Abs(center.y) <= half.y && Mathf.Abs(center.z) <= half.z;
    }

    void RestoreBook() // Cancel selection and restore the original pose and physics.
    {
        if (book == null) return;
        if (book.isSelected && book.interactionManager != null) book.interactionManager.CancelInteractableSelection((IXRSelectInteractable)book);
        book.transform.SetParent(bookParent, true);
        book.transform.localScale = bookScale;
        bookBody.position = home;
        bookBody.rotation = bookRotation;
        bookBody.isKinematic = originalKinematic;
        bookBody.useGravity = originalGravity;
        book.throwOnDetach = originalThrow;
        if (!bookBody.isKinematic) { bookBody.linearVelocity = Vector3.zero; bookBody.angularVelocity = Vector3.zero; }
    }

    public void StopRound() // Remove listeners, round props and temporary materials.
    {
        CurrentStage = Stage.Idle;
        if (book != null) book.selectExited.RemoveListener(BookReleased);
        RestoreBook();
        if (props != null) { props.SetActive(false); Destroy(props); }
        foreach (var material in frontMaterials.Values) Destroy(material);
        frontMaterials.Clear();
        hud = null;
        labels.Clear();
        props = null;
        book = null;
    }

    void OnDestroy() { StopRound(); }
}
