using System.Collections; // 导入 System.Collections 命名空间 / Import the System.Collections namespace.
using TMPro; // 导入 TMPro 命名空间 / Import the TMPro namespace.
using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using UnityEngine.UI; // 导入 UnityEngine.UI 命名空间 / Import the UnityEngine.UI namespace.

    // 说明：TaskFeedback 的实现说明 / Implementation note: Shared feedback stays outside task panels so transitions cannot hide it.
public class TaskFeedback : MonoBehaviour // 声明 TaskFeedback 组件或辅助类型 / Declare the TaskFeedback component or helper type.
{
    private CanvasGroup group; // 控制整块提示的透明度 / Control feedback panel opacity.
    private RectTransform panel; // 提示面板的位置和尺寸 / Feedback panel position and size.
    private TMP_Text label; // 显示提示内容的文字组件 / Text component displaying feedback.
    private AudioSource speaker; // 播放成功、重试和过渡音效 / Audio player for success, retry and transition sounds.
    private Coroutine dismiss; // 记住正在运行的淡出流程，方便取消 / Store the fade coroutine so it can be cancelled.
    private Camera view; // 玩家当前使用的主摄像机 / Main camera currently used by the player.

    public static TaskFeedback For(MonoBehaviour owner) // 取得或创建共用反馈组件 / Get or create the shared feedback component.
    {
        var feedback = FindFirstObjectByType<TaskFeedback>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        return feedback != null ? feedback : owner.gameObject.AddComponent<TaskFeedback>(); // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
    }

    void Awake() // 初始化场景引用与启动状态 / Initialize scene references and startup state.
    {
        speaker = gameObject.AddComponent<AudioSource>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
        speaker.playOnAwake = false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        speaker.spatialBlend = 0f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        speaker.volume = 0.65f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        label = CreatePanel(transform, "Player Task Feedback", new Vector2(720, 200), new Vector2(24, 16), 38, 100, new Color(.025f, .04f, .07f, .95f)); // 创建原尺寸和颜色的反馈界面 / Create feedback UI with its existing size and colour.
        panel = (RectTransform)label.transform.parent; // 保存反馈面板位置引用 / Keep the feedback panel transform.
        group = panel.gameObject.AddComponent<CanvasGroup>(); // 控制整块反馈的淡入淡出 / Control fading of the whole feedback panel.
        group.alpha = 0; // 初始隐藏反馈 / Hide feedback initially.
        group.interactable = group.blocksRaycasts = false; // 文字反馈不拦截操作 / Feedback must not intercept interaction.
    }

    public static TMP_Text CreatePanel(Transform parent, string name, Vector2 size, Vector2 padding, float fontSize, int order, Color colour) // 共用任务与反馈的界面创建代码 / Share UI construction between tasks and feedback.
    {
        var root = new GameObject(name, typeof(RectTransform), typeof(Canvas)); // 创建面板和画布 / Create the panel and canvas.
        var rect = root.GetComponent<RectTransform>(); // 取得面板布局 / Get the panel layout.
        rect.SetParent(parent, false); // 放到指定父物体下 / Attach to the requested parent.
        rect.sizeDelta = size; // 保留调用者指定的面板尺寸 / Use the requested panel size.
        rect.localScale = Vector3.one * .001f; // 把界面像素尺寸转换为世界尺寸 / Convert UI pixel dimensions to world dimensions.
        var canvas = root.GetComponent<Canvas>(); // 取得画布设置 / Get the canvas settings.
        canvas.renderMode = RenderMode.WorldSpace; // 在 VR 世界中显示 / Display inside the VR world.
        canvas.sortingOrder = order; // 保留任务与反馈的显示顺序 / Preserve task and feedback draw order.
        var background = root.AddComponent<Image>(); // 创建背景 / Create the background.
        background.color = colour; // 使用原来的背景颜色 / Use the original background colour.
        background.raycastTarget = false; // 背景不拦截控制器射线 / Do not block controller rays.
        var textObject = new GameObject("Message", typeof(RectTransform), typeof(TextMeshProUGUI)); // 创建文字物体 / Create the text object.
        textObject.transform.SetParent(rect, false); // 把文字放在面板内 / Place text inside the panel.
        var textRect = textObject.GetComponent<RectTransform>(); // 取得文字布局 / Get the text layout.
        textRect.anchorMin = Vector2.zero; // 锚定左下角 / Anchor the bottom-left corner.
        textRect.anchorMax = Vector2.one; // 锚定右上角 / Anchor the top-right corner.
        textRect.offsetMin = padding; // 保留左下留白 / Preserve bottom-left padding.
        textRect.offsetMax = -padding; // 保留右上留白 / Preserve top-right padding.
        var text = textObject.GetComponent<TextMeshProUGUI>(); // 取得 TMP 文字 / Get the TMP text.
        text.fontSize = fontSize; // 保留原字号 / Preserve the original font size.
        text.alignment = TextAlignmentOptions.Center; // 保持居中 / Keep text centred.
        text.raycastTarget = false; // 文字不拦截射线 / Do not intercept rays with text.
        return text; // 返回可填写内容的文字组件 / Return the text component for messages.
    }

    public void Show(bool success, string message, TMP_Text fontSource) // 显示双语反馈并播放对应音效 / Display bilingual feedback and play its sound.
    {
        if (fontSource != null && fontSource.font != null) label.font = fontSource.font; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        label.text = message; // 更新玩家可见的界面文字 / Update text visible to the player.
        label.color = success ? new Color(0.45f, 1f, 0.65f) : new Color(1f, 0.75f, 0.35f); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        group.alpha = 1; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        LateUpdate(); // 在帧末更新面向玩家的界面位置 / Update player-facing UI positions at the end of the frame.
        Play(success ? "success" : "retry"); // 加载并播放指定的反馈音效 / Load and play the specified feedback sound.
        if (dismiss != null) StopCoroutine(dismiss); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        dismiss = StartCoroutine(Dismiss()); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    }

    public void Play(string sound) // 加载并播放指定的反馈音效 / Load and play the specified feedback sound.
    {
        var clip = Resources.Load<AudioClip>("Feedback/" + sound); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if (clip != null) { speaker.Stop(); speaker.PlayOneShot(clip); } // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    }

    public void Clear() // 停止提示动画与音效并隐藏反馈 / Stop feedback animation and audio and hide the panel.
    {
        if (dismiss != null) StopCoroutine(dismiss); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        dismiss = null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        group.alpha = 0; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        speaker.Stop(); // 停止当前反馈音效 / Stop the current feedback sound.
    }

    void LateUpdate() // 在帧末更新面向玩家的界面位置 / Update player-facing UI positions at the end of the frame.
    {
        if (group == null || group.alpha == 0) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (view == null) view = Camera.main; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (view == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    // 说明：TaskFeedback 的实现说明 / Implementation note: Slightly below eye level; remains readable as the player turns.
        panel.position = view.transform.position + view.transform.forward * 1.4f - view.transform.up * 0.12f; // 玩家当前使用的主摄像机 / Main camera currently used by the player.
        panel.rotation = view.transform.rotation; // 玩家当前使用的主摄像机 / Main camera currently used by the player.
    }

    IEnumerator Dismiss() // 等待后淡出反馈提示 / Wait and fade out the feedback message.
    {
        yield return new WaitForSecondsRealtime(2f); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        for (float t = 0; t < 0.25f; t += Time.unscaledDeltaTime) // 逐帧或按索引遍历当前操作 / Iterate this operation by frame or index.
        {
            group.alpha = 1f - t / 0.25f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            yield return null; // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        }
        group.alpha = 0; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        dismiss = null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    }
}
