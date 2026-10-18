using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.

public class UIFlowManager : MonoBehaviour // 声明 UIFlowManager 组件或辅助类型 / Declare the UIFlowManager component or helper type.
{ // 开始代码块 / Begin the block.
    public GameObject introPanel; // 执行 UIFlowManager 的当前操作 / Perform the current UIFlowManager operation.
    public GameObject learningPanel; // 执行 UIFlowManager 的当前操作 / Perform the current UIFlowManager operation.
    public GameObject readyPanel; // 执行 UIFlowManager 的当前操作 / Perform the current UIFlowManager operation.

    public GameObject startTaskButton; // 执行 UIFlowManager 的当前操作 / Perform the current UIFlowManager operation.

    void Start() // 在场景加载完成后执行初始化 / Initialize after the scene has loaded.
    { // 开始代码块 / Begin the block.
        startTaskButton.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
        ShowIntro(); // 入房后先显示介绍，再学习词语和准备测试 / Show the introduction before vocabulary learning and test preparation.
    } // 结束代码块 / End the block.

    public void ShowIntro() // 显示介绍并复位两个任务 / Show the introduction and reset both tasks.
    { // 开始代码块 / Begin the block.
        var task1 = FindFirstObjectByType<Task1Manager>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (task1 != null) task1.ResetTask(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        var task2 = FindFirstObjectByType<Task2Manager>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (task2 != null) task2.ResetTask(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        introPanel.SetActive(true); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
        learningPanel.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
        readyPanel.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.

        startTaskButton.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
    } // 结束代码块 / End the block.

    public void StartLearning() // 打开学习卡片并从第一张开始 / Open vocabulary learning from the first card.
    { // 开始代码块 / Begin the block.
        introPanel.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
        learningPanel.SetActive(true); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
        var cards = learningPanel.GetComponentInChildren<LearningCardManager>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (cards != null) cards.ResetCards(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        readyPanel.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.

        startTaskButton.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
    } // 结束代码块 / End the block.

    public void ShowReady() // 学习结束后显示准备界面 / Show the ready screen after vocabulary learning.
    { // 开始代码块 / Begin the block.
        introPanel.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
        learningPanel.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
        readyPanel.SetActive(true); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.

        startTaskButton.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
    } // 结束代码块 / End the block.

    public void StartMission() // 关闭引导并显示开始任务按钮 / Close onboarding and show the task-start button.
    { // 开始代码块 / Begin the block.
        introPanel.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
        learningPanel.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
        readyPanel.SetActive(false); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.

        startTaskButton.SetActive(true); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
    } // 结束代码块 / End the block.
} // 结束代码块 / End the block.
