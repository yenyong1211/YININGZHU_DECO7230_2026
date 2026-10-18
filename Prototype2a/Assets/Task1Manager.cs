using System.Collections; // 导入 System.Collections 命名空间 / Import the System.Collections namespace.
using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using TMPro; // 导入 TMPro 命名空间 / Import the TMPro namespace.

public class Task1Manager : MonoBehaviour // 声明 Task1Manager 组件或辅助类型 / Declare the Task1Manager component or helper type.
{ // 开始代码块 / Begin the block.
    public GameObject task1Panel; // 执行 Task1Manager 的当前操作 / Perform the current Task1Manager operation.
    public TMP_Text feedbackText; // 成功或错误反馈文字引用 / Success or retry feedback text reference.
    public GameObject startTaskButton; // 执行 Task1Manager 的当前操作 / Perform the current Task1Manager operation.
    public Task2Manager task2Manager; // 执行 Task1Manager 的当前操作 / Perform the current Task1Manager operation.
    public float successPause = 2.1f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    public float fadeDuration = 0.25f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    private bool running; // 控制当前任务是否接受物品输入 / Whether the current task accepts object input.
    private bool taskCompleted; // 执行 Task1Manager 的当前操作 / Perform the current Task1Manager operation.

    void Awake() { if (task1Panel != null) task1Panel.SetActive(false); } // 初始化场景引用与启动状态 / Initialize scene references and startup state.

    public void ResetTask() // 清理任务状态并隐藏任务面板 / Clear task state and hide its panel.
    { // 开始代码块 / Begin the block.
        StopAllCoroutines(); // 执行 Task1Manager 的当前操作 / Perform the current Task1Manager operation.
        TaskFeedback.For(this).Clear(); // 取得或创建共用反馈组件 / Get or create the shared feedback component.
        running = false; // 控制当前任务是否接受物品输入 / Whether the current task accepts object input.
        taskCompleted = false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if (task1Panel != null) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        { // 开始代码块 / Begin the block.
            task1Panel.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
            var group = task1Panel.GetComponent<CanvasGroup>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
            if (group != null) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            { // 开始代码块 / Begin the block.
                group.alpha = 1f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
                group.interactable = group.blocksRaycasts = true; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            } // 结束代码块 / End the block.
        } // 结束代码块 / End the block.
    } // 结束代码块 / End the block.

    public void StartTask1() // 显示并激活寻找电脑任务 / Display and activate the find-computer task.
    { // 开始代码块 / Begin the block.
        if (running) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        ResetTask(); // 清理任务状态并隐藏任务面板 / Clear task state and hide its panel.
        if (task2Manager != null) task2Manager.ResetTask(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        running = true; // 控制当前任务是否接受物品输入 / Whether the current task accepts object input.
        if (startTaskButton != null) startTaskButton.SetActive(false); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (task1Panel != null) task1Panel.SetActive(true); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (feedbackText != null) feedbackText.text = ""; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    } // 结束代码块 / End the block.

    public void CheckObject(GameObject selectedObject) // 根据当前任务检查玩家选择的物品 / Check the selected object against the active objective.
    { // 开始代码块 / Begin the block.
        if (!running || taskCompleted || selectedObject == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        var word = selectedObject.GetComponentInParent<LookWord>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (selectedObject.tag == "Computer" || (word != null && word.english == "computer")) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        { // 开始代码块 / Begin the block.
            taskCompleted = true; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            if (feedbackText != null) feedbackText.text = "太棒了！\nGreat job!"; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            TaskFeedback.For(this).Show(true, "太棒了！找到电脑了\nCorrect! You found the computer.", feedbackText); // 成功或错误反馈文字引用 / Success or retry feedback text reference.
            StartCoroutine(Advance()); // 执行 Task1Manager 的当前操作 / Perform the current Task1Manager operation.
        } // 结束代码块 / End the block.
        else // 处理前一条件不满足的分支 / Handle the branch where the previous condition is false.
        { // 开始代码块 / Begin the block.
            if (feedbackText != null) feedbackText.text = "再试一次\nTry again"; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            TaskFeedback.For(this).Show(false, "再试一次：请找到电脑\nTry again: find the computer.", feedbackText); // 成功或错误反馈文字引用 / Success or retry feedback text reference.
        } // 结束代码块 / End the block.
    } // 结束代码块 / End the block.

    IEnumerator Advance() // 等待成功反馈后平滑切换到任务二 / Wait for success feedback and smoothly transition to Task2.
    { // 开始代码块 / Begin the block.
        yield return new WaitForSecondsRealtime(successPause); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        yield return FadePanel(task1Panel, 1f, 0f, fadeDuration); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        if (task1Panel != null) task1Panel.SetActive(false); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        running = false; // 控制当前任务是否接受物品输入 / Whether the current task accepts object input.
        if (task2Manager != null) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        { // 开始代码块 / Begin the block.
            TaskFeedback.For(this).Play("transition"); // 取得或创建共用反馈组件 / Get or create the shared feedback component.
            task2Manager.StartTask2(); // 启动抓住逃跑红书并装包的任务 / Start the escaping-red-book packing task.
            yield return FadePanel(task2Manager.task2Panel, 0f, 1f, fadeDuration); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        } // 结束代码块 / End the block.
    } // 结束代码块 / End the block.

    public static IEnumerator FadePanel(GameObject panel, float from, float to, float duration) // 逐帧改变面板透明度与交互状态 / Animate panel opacity and interaction state.
    { // 开始代码块 / Begin the block.
        if (panel == null) yield break; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        var group = panel.GetComponent<CanvasGroup>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (group == null) group = panel.AddComponent<CanvasGroup>(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        group.alpha = from; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        group.interactable = group.blocksRaycasts = false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime) // 逐帧或按索引遍历当前操作 / Iterate this operation by frame or index.
        { // 开始代码块 / Begin the block.
            group.alpha = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, elapsed / duration)); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            yield return null; // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        } // 结束代码块 / End the block.
        group.alpha = to; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        group.interactable = group.blocksRaycasts = to > 0f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    } // 结束代码块 / End the block.
} // 结束代码块 / End the block.
