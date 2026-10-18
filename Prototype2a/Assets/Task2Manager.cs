using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using TMPro; // 导入 TMPro 命名空间 / Import the TMPro namespace.

public class Task2Manager : MonoBehaviour // 声明 Task2Manager 组件或辅助类型 / Declare the Task2Manager component or helper type.
{ // 开始代码块 / Begin the block.
    public GameObject task2Panel; // 执行 Task2Manager 的当前操作 / Perform the current Task2Manager operation.
    public TMP_Text feedbackText; // 成功或错误反馈文字引用 / Success or retry feedback text reference.
    public TMP_Text instructionText; // 原任务面板中的当前中文指令 / Current Chinese objective in the original task panel.
    private bool running; // 控制当前任务是否接受物品输入 / Whether the current task accepts object input.
    private DeskAdventure adventure; // 执行 Task2Manager 的当前操作 / Perform the current Task2Manager operation.

    void Awake() // 初始化场景引用与启动状态 / Initialize scene references and startup state.
    { // 开始代码块 / Begin the block.
        var preview = GameObject.Find("DeskAdventureModels"); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (preview != null) preview.SetActive(false); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        ResetTask(); // 清理任务状态并隐藏任务面板 / Clear task state and hide its panel.
    } // 结束代码块 / End the block.

    public void ResetTask() // 清理任务状态并隐藏任务面板 / Clear task state and hide its panel.
    { // 开始代码块 / Begin the block.
        running = false; // 控制当前任务是否接受物品输入 / Whether the current task accepts object input.
        if (adventure != null) adventure.StopRound(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (task2Panel != null) task2Panel.SetActive(false); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    } // 结束代码块 / End the block.

    public void StartTask2() // 启动抓住逃跑红书并装包的任务 / Start the escaping-red-book packing task.
    { // 开始代码块 / Begin the block.
        if (running) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        running = true; // 控制当前任务是否接受物品输入 / Whether the current task accepts object input.
        if (task2Panel != null) task2Panel.SetActive(true); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (adventure == null) adventure = gameObject.GetComponent<DeskAdventure>(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (adventure == null) adventure = gameObject.AddComponent<DeskAdventure>(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        adventure.Begin(this); // 重置并开始一局抓红书装包游戏 / Reset and begin a catch-and-pack round.
    } // 结束代码块 / End the block.

    public void RestartAdventure() // 先复位再重新启动任务二 / Reset and restart Task2.
    { // 开始代码块 / Begin the block.
        ResetTask(); // 清理任务状态并隐藏任务面板 / Clear task state and hide its panel.
        StartTask2(); // 启动抓住逃跑红书并装包的任务 / Start the escaping-red-book packing task.
    } // 结束代码块 / End the block.

    public void CompleteAdventure() { running = false; } // 标记任务二完成并关闭输入判定 / Mark Task2 complete and stop objective input checks.

    public void CheckObject(GameObject selectedObject) // 根据当前任务检查玩家选择的物品 / Check the selected object against the active objective.
    { // 开始代码块 / Begin the block.
        if (!running || selectedObject == null || adventure == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        var word = selectedObject.GetComponentInParent<LookWord>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (word == null || word.english != "book") adventure.WrongObject(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    } // 结束代码块 / End the block.

    public void BookPickedUp() // 把原书的抓取事件转交给游戏流程 / Forward the original book grab event to the game flow.
    { // 开始代码块 / Begin the block.
        if (running && adventure != null) adventure.BookGrabbed(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    } // 结束代码块 / End the block.
} // 结束代码块 / End the block.
