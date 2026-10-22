using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using TMPro; // 导入 TMPro 命名空间 / Import the TMPro namespace.

public class Task2Manager : MonoBehaviour // 声明 Task2Manager 组件或辅助类型 / Declare the Task2Manager component or helper type.
{
    public GameObject task2Panel; // 第二关的任务面板 / The Task2 panel.
    public TMP_Text feedbackText; // 成功或错误反馈文字引用 / Success or retry feedback text reference.
    public TMP_Text instructionText; // 原任务面板中的当前中文指令 / Current Chinese objective in the original task panel.
    public AudioClip taskInstructionAudio; // 进入任务二时播放的中文指令录音 / Recorded Chinese instruction played on entry to Task2.
    private AudioSource instructionSpeaker; // 独立播放任务语音，避免被反馈音效打断 / Separate voice source so feedback sounds do not interrupt narration.
    private bool running; // 控制当前任务是否接受物品输入 / Whether the current task accepts object input.
    private DeskAdventure adventure; // 负责红书移动和装包判定的玩法组件 / Gameplay component for book motion and packing checks.

    void Awake() // 初始化场景引用与启动状态 / Initialize scene references and startup state.
    {
        instructionSpeaker = gameObject.AddComponent<AudioSource>(); // 创建任务指令播放器 / Create the task instruction audio source.
        instructionSpeaker.playOnAwake = false; // 不在场景加载时自动播放 / Do not play narration when the scene loads.
        instructionSpeaker.spatialBlend = 0f; // 使用二维声音确保玩家始终听清 / Use non-spatial audio so the player can hear clearly.
        var preview = GameObject.Find("DeskAdventureModels"); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (preview != null) preview.SetActive(false); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        ResetTask(); // 清理任务状态并隐藏任务面板 / Clear task state and hide its panel.
    }

    public void ResetTask() // 清理任务状态并隐藏任务面板 / Clear task state and hide its panel.
    {
        if (instructionSpeaker != null) instructionSpeaker.Stop(); // 复位任务时停止尚未播完的指令 / Stop unfinished instructions when resetting the task.
        running = false; // 控制当前任务是否接受物品输入 / Whether the current task accepts object input.
        if (adventure != null) adventure.StopRound(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (task2Panel != null) task2Panel.SetActive(false); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    }

    public void StartTask2() // 启动抓住逃跑红书并装包的任务 / Start the escaping-red-book packing task.
    {
        if (running) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        running = true; // 控制当前任务是否接受物品输入 / Whether the current task accepts object input.
        if (task2Panel != null) task2Panel.SetActive(true); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (adventure == null) adventure = gameObject.GetComponent<DeskAdventure>(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (adventure == null) adventure = gameObject.AddComponent<DeskAdventure>(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        adventure.Begin(this); // 重置并开始一局抓红书装包游戏 / Reset and begin a catch-and-pack round.
        if (taskInstructionAudio != null && adventure.CurrentStage != DeskAdventure.Stage.Idle) // 仅在任务成功启动并且已有录音时播放 / Play only when the task starts successfully and a clip exists.
        {
            instructionSpeaker.Stop(); // 避免重玩时叠加上一段指令 / Avoid overlapping narration when restarting.
            instructionSpeaker.PlayOneShot(taskInstructionAudio); // 播放用户录制的任务二中文指令 / Play the user-recorded Chinese Task2 instruction.
        }
    }

    public void RestartAdventure() // 先复位再重新启动任务二 / Reset and restart Task2.
    {
        ResetTask(); // 清理任务状态并隐藏任务面板 / Clear task state and hide its panel.
        StartTask2(); // 启动抓住逃跑红书并装包的任务 / Start the escaping-red-book packing task.
    }

    public void CompleteAdventure() { running = false; if (instructionSpeaker != null) instructionSpeaker.Stop(); } // 标记任务二完成并关闭输入判定 / Mark Task2 complete and stop objective input checks.

    public void CheckObject(GameObject selectedObject) // 根据当前任务检查玩家选择的物品 / Check the selected object against the active objective.
    {
        if (!running || selectedObject == null || adventure == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        var word = selectedObject.GetComponentInParent<LookWord>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (word == null || word.english != "book") adventure.WrongObject(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    }

    public void BookPickedUp() // 把原书的抓取事件转交给游戏流程 / Forward the original book grab event to the game flow.
    {
        if (running && adventure != null) adventure.BookGrabbed(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    }
}
