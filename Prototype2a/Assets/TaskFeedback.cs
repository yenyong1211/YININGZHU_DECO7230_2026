using System.Collections; // 导入 System.Collections 命名空间 / Import the System.Collections namespace.
using TMPro; // 导入 TMPro 命名空间 / Import the TMPro namespace.
using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using UnityEngine.UI; // 导入 UnityEngine.UI 命名空间 / Import the UnityEngine.UI namespace.

    // 说明：TaskFeedback 的实现说明 / Implementation note: Shared feedback stays outside task panels so transitions cannot hide it.
public class TaskFeedback : MonoBehaviour // 声明 TaskFeedback 组件或辅助类型 / Declare the TaskFeedback component or helper type.
{ // 开始代码块 / Begin the block.
    private CanvasGroup group; // 执行 TaskFeedback 的当前操作 / Perform the current TaskFeedback operation.
    private RectTransform panel; // 执行 TaskFeedback 的当前操作 / Perform the current TaskFeedback operation.
    private TMP_Text label; // 执行 TaskFeedback 的当前操作 / Perform the current TaskFeedback operation.
    private AudioSource speaker; // 执行 TaskFeedback 的当前操作 / Perform the current TaskFeedback operation.
    private Coroutine dismiss; // 执行 TaskFeedback 的当前操作 / Perform the current TaskFeedback operation.
    private Camera view; // 玩家当前使用的主摄像机 / Main camera currently used by the player.

    public static TaskFeedback For(MonoBehaviour owner) // 取得或创建共用反馈组件 / Get or create the shared feedback component.
    { // 开始代码块 / Begin the block.
        var feedback = FindFirstObjectByType<TaskFeedback>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        return feedback != null ? feedback : owner.gameObject.AddComponent<TaskFeedback>(); // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
    } // 结束代码块 / End the block.

    void Awake() // 初始化场景引用与启动状态 / Initialize scene references and startup state.
    { // 开始代码块 / Begin the block.
        speaker = gameObject.AddComponent<AudioSource>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
        speaker.playOnAwake = false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        speaker.spatialBlend = 0f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        speaker.volume = 0.65f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        var root = new GameObject("Player Task Feedback", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup)); // 创建所需的场景对象或组件 / Create the required scene object or component.
        panel = root.GetComponent<RectTransform>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        panel.SetParent(transform, false); // 执行 TaskFeedback 的当前操作 / Perform the current TaskFeedback operation.
        panel.sizeDelta = new Vector2(720, 200); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        panel.localScale = Vector3.one * 0.001f; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        var canvas = root.GetComponent<Canvas>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        canvas.renderMode = RenderMode.WorldSpace; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        canvas.sortingOrder = 100; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        group = root.GetComponent<CanvasGroup>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        group.alpha = 0; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        group.interactable = group.blocksRaycasts = false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        var background = root.AddComponent<Image>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
        background.color = new Color(0.025f, 0.04f, 0.07f, 0.95f); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        background.raycastTarget = false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        var textObject = new GameObject("Result", typeof(RectTransform)); // 创建所需的场景对象或组件 / Create the required scene object or component.
        textObject.transform.SetParent(panel, false); // 执行 TaskFeedback 的当前操作 / Perform the current TaskFeedback operation.
        var rect = textObject.GetComponent<RectTransform>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        rect.anchorMin = Vector2.zero; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        rect.anchorMax = Vector2.one; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        rect.offsetMin = new Vector2(24, 16); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        rect.offsetMax = new Vector2(-24, -16); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        label = textObject.AddComponent<TextMeshProUGUI>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
        label.fontSize = 38; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        label.alignment = TextAlignmentOptions.Center; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        label.raycastTarget = false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    } // 结束代码块 / End the block.

    public void Show(bool success, string message, TMP_Text fontSource) // 显示双语反馈并播放对应音效 / Display bilingual feedback and play its sound.
    { // 开始代码块 / Begin the block.
        if (fontSource != null && fontSource.font != null) label.font = fontSource.font; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        label.text = message; // 更新玩家可见的界面文字 / Update text visible to the player.
        label.color = success ? new Color(0.45f, 1f, 0.65f) : new Color(1f, 0.75f, 0.35f); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        group.alpha = 1; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        LateUpdate(); // 在帧末更新面向玩家的界面位置 / Update player-facing UI positions at the end of the frame.
        Play(success ? "success" : "retry"); // 加载并播放指定的反馈音效 / Load and play the specified feedback sound.
        if (dismiss != null) StopCoroutine(dismiss); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        dismiss = StartCoroutine(Dismiss()); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    } // 结束代码块 / End the block.

    public void Play(string sound) // 加载并播放指定的反馈音效 / Load and play the specified feedback sound.
    { // 开始代码块 / Begin the block.
        var clip = Resources.Load<AudioClip>("Feedback/" + sound); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if (clip != null) { speaker.Stop(); speaker.PlayOneShot(clip); } // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    } // 结束代码块 / End the block.

    public void Clear() // 停止提示动画与音效并隐藏反馈 / Stop feedback animation and audio and hide the panel.
    { // 开始代码块 / Begin the block.
        if (dismiss != null) StopCoroutine(dismiss); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        dismiss = null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        group.alpha = 0; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        speaker.Stop(); // 执行 TaskFeedback 的当前操作 / Perform the current TaskFeedback operation.
    } // 结束代码块 / End the block.

    void LateUpdate() // 在帧末更新面向玩家的界面位置 / Update player-facing UI positions at the end of the frame.
    { // 开始代码块 / Begin the block.
        if (group == null || group.alpha == 0) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (view == null) view = Camera.main; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (view == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    // 说明：TaskFeedback 的实现说明 / Implementation note: Slightly below eye level; remains readable as the player turns.
        panel.position = view.transform.position + view.transform.forward * 1.4f - view.transform.up * 0.12f; // 玩家当前使用的主摄像机 / Main camera currently used by the player.
        panel.rotation = view.transform.rotation; // 玩家当前使用的主摄像机 / Main camera currently used by the player.
    } // 结束代码块 / End the block.

    IEnumerator Dismiss() // 等待后淡出反馈提示 / Wait and fade out the feedback message.
    { // 开始代码块 / Begin the block.
        yield return new WaitForSecondsRealtime(2f); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        for (float t = 0; t < 0.25f; t += Time.unscaledDeltaTime) // 逐帧或按索引遍历当前操作 / Iterate this operation by frame or index.
        { // 开始代码块 / Begin the block.
            group.alpha = 1f - t / 0.25f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            yield return null; // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        } // 结束代码块 / End the block.
        group.alpha = 0; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        dismiss = null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    } // 结束代码块 / End the block.
} // 结束代码块 / End the block.
