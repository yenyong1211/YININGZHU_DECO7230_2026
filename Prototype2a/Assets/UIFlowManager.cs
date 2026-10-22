using TMPro; // 显示手柄操作说明 / Display controller instructions.
using UnityEngine.UI; // 连接继续按钮 / Connect the continue button.
using UnityEngine; // 导入 Unity 基础功能 / Import Unity functionality.

public class UIFlowManager : MonoBehaviour // 管理介绍、学习、准备和开始任务的界面 / Manage introduction, learning, ready and task entry screens.
{
    public GameObject introPanel, learningPanel, readyPanel, startTaskButton; // 保留现有 Inspector 引用名称 / Preserve existing Inspector reference names.

    private GameObject controlsPanel; // 独立的手柄说明页面 / Separate controller help page.

    void Start() { gameObject.AddComponent<PlayerHeight>(); ShowIntro(); } // 添加玩家高度调整功能,调用 ShowIntro()，先显示介绍/ Show the introduction when entering the room.

    void ShowPanel(GameObject panel) // 统一处理互斥面板和开始按钮 / Handle mutually exclusive panels and the start button in one place.
    {
        if (controlsPanel != null) controlsPanel.SetActive(panel == controlsPanel); // 同步说明页显示状态 / Synchronize the help page.
        introPanel.SetActive(panel == introPanel); // 仅在需要介绍时显示介绍 / Show the introduction only when requested.
        learningPanel.SetActive(panel == learningPanel); // 仅在学习时显示词语卡 / Show vocabulary cards only during learning.
        readyPanel.SetActive(panel == readyPanel); // 仅在准备阶段显示准备页 / Show the ready screen only in the ready stage.
        startTaskButton.SetActive(panel == null); // 关闭引导后显示原来的任务开始按钮 / Show the existing task-start button after onboarding.
    }

    public void ShowIntro() // 返回介绍并复位任务 / Return to the introduction and reset tasks.
    {
        var task1 = FindFirstObjectByType<Task1Manager>(); // Find Task1.
        var task2 = FindFirstObjectByType<Task2Manager>(); // Find Task2.
        if (task1 != null) task1.ResetTask(); // 清理任务一 / Reset Task1.
        if (task2 != null) task2.ResetTask(); // 清理任务二 / Reset Task2.
        ShowPanel(introPanel); // 显示介绍并隐藏其他引导 / Show the introduction and hide other onboarding screens.
    }

    public void StartLearning() // 介绍之后先显示操作说明 / Show controls after the introduction.
    {
        if (controlsPanel == null) // 首次使用时复用原介绍页外观 / Reuse introduction styling on first use.
        {
            controlsPanel = Instantiate(introPanel, introPanel.transform.parent); // 保留原来的 VR 射线交互布局 / Preserve the existing VR UI layout.
            controlsPanel.name = "Controller Instructions"; // 清楚标识说明页 / Name the help page clearly.
            foreach (var text in controlsPanel.GetComponentsInChildren<TMP_Text>(true)) // 更新页面文字 / Update page text.
            {
                bool title = text.text == "Chinese Learning Room"; // 识别原介绍页标题 / Identify the original introduction heading.
                bool button = text.GetComponentInParent<Button>() != null; // 检查按钮标签 / Identify button labels.
                text.text = button ? "Continue" : title ? "Controller Guide" : "MOVE: Push the left thumbstick.\nHEIGHT: Hold X to lower your view, Y to raise it.\nSELECT: Point the ray and press the index trigger\nto click buttons or activate the computer.\nGRAB: Hold the side grip button with your middle finger.\nRELEASE: Let go of the grip button to put the book down.\nTASK 2: Put the red book inside the bag, then release it.\nLOOK: Turn your head to look around."; // 显示与项目绑定一致的英文说明 / Show English instructions matching project bindings.
                if (!button) { text.rectTransform.anchoredPosition = new Vector2(0, title ? 220 : 0); text.rectTransform.sizeDelta = new Vector2(850, title ? 70 : 340); text.enableAutoSizing = true; text.fontSizeMin = 22; text.fontSizeMax = title ? 40 : 28; text.raycastTarget = false; } // 在原面板中完整显示说明 / Fit instructions inside the existing panel.
            }
            foreach (var button in controlsPanel.GetComponentsInChildren<Button>(true)) // 替换克隆按钮的旧事件 / Replace cloned button callbacks.
            {
                ((RectTransform)button.transform).anchoredPosition = new Vector2(0, -230); // 放在说明正文下方 / Place below the instructions.
                button.onClick = new Button.ButtonClickedEvent(); // 清除原来的持久事件 / Clear original persistent callbacks.
                button.onClick.AddListener(BeginVocabulary); // 继续后显示六张词卡 / Continue to the six vocabulary cards.
            }
        }
        ShowPanel(controlsPanel); // 显示说明并隐藏介绍和任务 / Show help and hide introduction and tasks.
    }

    public void BeginVocabulary() // 从第一张词卡开始学习 / Begin learning from the first word card.
    {
        ShowPanel(learningPanel); // 显示学习页面 / Show the learning screen.
        var cards = learningPanel.GetComponentInChildren<LearningCardManager>(); // 取得词卡组件 / Get the vocabulary component.
        if (cards != null) cards.ResetCards(); // 重置六个词语的进度 / Reset progress through the six words.
    }

    public void ShowReady() { ShowPanel(readyPanel); } // 学完后进入准备页 / Open the ready screen after learning.
    public void StartMission() { ShowPanel(null); } // 保留准备页原来的开始事件 / Preserve the ready screen's existing start event.
}
