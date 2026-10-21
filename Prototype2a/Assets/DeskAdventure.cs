using System.Collections.Generic; // 导入 System.Collections.Generic 命名空间 / Import the System.Collections.Generic namespace.
using UnityEngine.UI; // 创建可点击退出按钮 / Create a clickable exit button.
using UnityEngine.XR.Interaction.Toolkit.UI; // 让控制器射线操作界面 / Enable controller rays on UI.
using TMPro; // 导入 TMPro 命名空间 / Import the TMPro namespace.
using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using UnityEngine.XR.Interaction.Toolkit; // 导入 UnityEngine.XR.Interaction.Toolkit 命名空间 / Import the UnityEngine.XR.Interaction.Toolkit namespace.
using UnityEngine.XR.Interaction.Toolkit.Interactables; // 导入 UnityEngine.XR.Interaction.Toolkit.Interactables 命名空间 / Import the UnityEngine.XR.Interaction.Toolkit.Interactables namespace.

public class DeskAdventure : MonoBehaviour // 声明 DeskAdventure 组件或辅助类型 / Declare the DeskAdventure component or helper type.
{
    public enum Stage { Idle, Chase, Pack, Won, TimedOut } // 仅保留未开始、追书、装包、成功和超时五种状态 / Keep only idle, chase, pack, won and timeout states.
    public Stage CurrentStage { get; private set; } // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
    public float roundSeconds = 180f; // 每局时间上限，单位秒 / Round time limit in seconds.
    public XRGrabInteractable TargetBook => book; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    public BoxCollider BagVolume => bagVolume; // 随书包变换的内部检测体积 / Interior detection volume that follows the bag transform.
    public TMP_Text MissionText => hud; // 持续显示的任务文字 / Persistent objective text.
    private XRGrabInteractable book; // 执行 DeskAdventure 的当前操作 / Perform the current DeskAdventure operation.
    private Rigidbody bookBody; // 目标红书的刚体引用 / Target red book rigidbody reference.
    private Transform bookParent, bag, mascot; // 红书最初的父物体 / Original parent of the red book.
    private Vector3 home, bookScale; // 物品初始位置，用于移动和复位 / Original item position for movement and reset.
    private Quaternion bookRotation; // 原书旋转，用于复位 / Original book rotation for restoration.
    private bool originalKinematic, originalGravity, originalThrow; // 执行 DeskAdventure 的当前操作 / Perform the current DeskAdventure operation.
    private GameObject props; // 当前回合生成的模型副本 / Model instance generated for the current round.
    private BoxCollider bagVolume; // 随书包变换的内部检测体积 / Interior detection volume that follows the bag transform.
    private TMP_Text hud; // 持续显示的任务文字 / Persistent objective text.
    private Task2Manager task; // 执行 DeskAdventure 的当前操作 / Perform the current DeskAdventure operation.
    private Camera view; // 玩家当前使用的主摄像机 / Main camera currently used by the player.
    private float remaining, elapsed, settled, nextHintTime; // 下一次允许显示纠错提示的时刻 / Next allowed time for a correction hint.
    private readonly List<Transform> labels = new List<Transform>(); // 需要面向玩家的物品标签集合 / Object labels that must face the player.

    public void Begin(Task2Manager owner) // 重置并开始一局抓红书装包游戏 / Reset and begin a catch-and-pack round.
    {
        StopRound(); // 停止当前回合并清理生成的物品和监听器 / Stop the round and clean up spawned objects and listeners.
        task = owner; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        var stationery = FindFirstObjectByType<DeskStationery>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (stationery != null) stationery.ResetItems(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        foreach (var word in FindObjectsByType<LookWord>(FindObjectsSortMode.None)) // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
        {
            if (word.english != "book") continue; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            book = word.GetComponent<XRGrabInteractable>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
            break; // 执行 DeskAdventure 的当前操作 / Perform the current DeskAdventure operation.
        }
        if (book == null) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        {
            TaskFeedback.For(this).Show(false, "找不到可抓取的红书\nMissing grabbable red book.", task.feedbackText); // 成功或错误反馈文字引用 / Success or retry feedback text reference.
            return; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
        }
        bookBody = book.GetComponent<Rigidbody>(); // 目标红书的刚体引用 / Target red book rigidbody reference.
        home = book.transform.position; // 物品初始位置，用于移动和复位 / Original item position for movement and reset.
        bookRotation = book.transform.rotation; // 原书旋转，用于复位 / Original book rotation for restoration.
        bookScale = book.transform.localScale; // 原书局部缩放，用于复位 / Original book local scale for restoration.
        bookParent = book.transform.parent; // 红书最初的父物体 / Original parent of the red book.
        originalKinematic = bookBody.isKinematic; // 目标红书的刚体引用 / Target red book rigidbody reference.
        originalGravity = bookBody.useGravity; // 目标红书的刚体引用 / Target red book rigidbody reference.
        originalThrow = book.throwOnDetach; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        book.throwOnDetach = false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        Transform template = null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        foreach (var candidate in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None)) // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
        {
            if (candidate.name == "DeskAdventureModels") { template = candidate; break; } // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        }
        if (template == null) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        {
            book.throwOnDetach = originalThrow; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            TaskFeedback.For(this).Show(false, "找不到书包模型\nMissing bag models.", task.feedbackText); // 成功或错误反馈文字引用 / Success or retry feedback text reference.
            return; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
        }
        props = Instantiate(template.gameObject, template.position, template.rotation, template.parent); // 当前回合生成的模型副本 / Model instance generated for the current round.
        props.name = "Playful Desk Adventure"; // 当前回合生成的模型副本 / Model instance generated for the current round.
        props.SetActive(true); // 当前回合生成的模型副本 / Model instance generated for the current round.
        bag = props.transform.Find("Bag"); // 当前回合生成的模型副本 / Model instance generated for the current round.
        mascot = props.transform.Find("多儿"); // 当前回合生成的模型副本 / Model instance generated for the current round.
        FitBag(); // 按红书尺寸调整书包并建立内部检测体积 / Fit the bag to the red book and create its interior volume.
        CreateHUD(); // 创建持续显示的当前任务界面 / Create the persistent objective display.
        Label(bag, "书包\nBAG", .36f); // 为场景物品建立双语标签 / Create a bilingual label for a scene object.
        if (mascot != null) Label(mascot, "多儿\nDUO ER", .13f); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        var restart = props.transform.Find("Restart").gameObject; // 当前回合生成的模型副本 / Model instance generated for the current round.
        Label(restart.transform, "重新开始\nRESTART", .12f); // 为场景物品建立双语标签 / Create a bilingual label for a scene object.
        restart.AddComponent<XRSimpleInteractable>().selectEntered.AddListener(RestartSelected); // 连接交互事件监听器 / Connect the interaction event listener.
        book.selectExited.AddListener(BookReleased); // 连接交互事件监听器 / Connect the interaction event listener.
        remaining = roundSeconds; // 每局时间上限，单位秒 / Round time limit in seconds.
        elapsed = settled = nextHintTime = 0f; // 下一次允许显示纠错提示的时刻 / Next allowed time for a correction hint.
        SetStage(Stage.Chase); // 切换阶段并同步任务文字和反馈 / Change stage and synchronize instructions and feedback.
        if (book.isSelected) BookGrabbed(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        else { bookBody.isKinematic = true; bookBody.useGravity = false; } // 处理前一条件不满足的分支 / Handle the branch where the previous condition is false.
    }

    void FitBag() // 按红书尺寸调整书包并建立内部检测体积 / Fit the bag to the red book and create its interior volume.
    {
        var bounds = ItemBounds(book); // 合并物品有效碰撞体的世界边界 / Combine world bounds of the active item colliders.
        var scale = bag.localScale; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        scale.x *= Mathf.Max(1f, (bounds.size.x + .04f) / (.26f * Mathf.Abs(bag.lossyScale.x))); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        scale.z *= Mathf.Max(1f, (bounds.size.z + .04f) / (.20f * Mathf.Abs(bag.lossyScale.z))); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        bag.localScale = scale; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        var target = new GameObject("Bag interior target"); // 创建所需的场景对象或组件 / Create the required scene object or component.
        target.transform.SetParent(bag, false); // 执行 DeskAdventure 的当前操作 / Perform the current DeskAdventure operation.
        target.transform.localPosition = new Vector3(0f, .08f, 0f); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        bagVolume = target.AddComponent<BoxCollider>(); // 随书包变换的内部检测体积 / Interior detection volume that follows the bag transform.
        bagVolume.isTrigger = true; // 随书包变换的内部检测体积 / Interior detection volume that follows the bag transform.
        bagVolume.size = new Vector3(.26f, .38f, .20f); // 随书包变换的内部检测体积 / Interior detection volume that follows the bag transform.
    }

    void CreateHUD() // 使用共用方法创建任务提示 / Create task instructions through the shared UI method.
    { // 开始创建任务提示 / Begin task UI setup.
        hud = TaskFeedback.CreatePanel(props.transform, "Task2 Instructions", new Vector2(900, 230), new Vector2(16, 8), 32, 110, new Color(.02f, .04f, .08f, 1f)); // 保留任务面板原来的尺寸、留白和外观 / Preserve the existing task panel size, padding and appearance.
        hud.transform.parent.gameObject.AddComponent<FrontPanel>(); // 在模型前显示不透明任务卡 / Show the opaque task card in front of models.
        if (task.feedbackText != null) hud.font = task.feedbackText.font; // 继续使用原中文字体 / Keep the existing Chinese font.
    } // 完成任务提示创建 / Finish task UI setup.

    void Label(Transform target, string text, float height) // 为场景物品建立双语标签 / Create a bilingual label for a scene object.
    {
        var root = new GameObject("Label", typeof(TextMeshPro)); // 创建所需的场景对象或组件 / Create the required scene object or component.
        root.transform.SetParent(target, false); // 执行 DeskAdventure 的当前操作 / Perform the current DeskAdventure operation.
        root.transform.localScale = new Vector3(1f / target.lossyScale.x, 1f / target.lossyScale.y, 1f / target.lossyScale.z); // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        root.transform.position = target.position + Vector3.up * height; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        var label = root.GetComponent<TextMeshPro>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (task.feedbackText != null) label.font = task.feedbackText.font; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        label.text = text; // 更新玩家可见的界面文字 / Update text visible to the player.
        label.fontSize = .7f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        label.alignment = TextAlignmentOptions.Center; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        label.rectTransform.sizeDelta = new Vector2(.4f, .12f); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        labels.Add(root.transform); // 需要面向玩家的物品标签集合 / Object labels that must face the player.
    }

    void RestartSelected(SelectEnterEventArgs args) { task.RestartAdventure(); } // 点击重玩按钮时重新开始任务二 / Restart Task2 when its restart control is selected.

    public void BookGrabbed() // 抓住红书后直接进入装包阶段 / Go directly to packing when the red book is grabbed.
    {
        if (CurrentStage != Stage.Chase) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        bookBody.useGravity = originalGravity; // 目标红书的刚体引用 / Target red book rigidbody reference.
        SetStage(Stage.Pack); // 切换阶段并同步任务文字和反馈 / Change stage and synchronize instructions and feedback.
    }

    void BookReleased(SelectExitEventArgs args) // 松手后恢复书本的原始物理设置 / Restore the original book physics after release.
    {
        if (CurrentStage != Stage.Pack) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        bookBody.isKinematic = originalKinematic; // 目标红书的刚体引用 / Target red book rigidbody reference.
        bookBody.useGravity = originalGravity; // 目标红书的刚体引用 / Target red book rigidbody reference.
    }

    public void WrongObject() // 提示玩家改用目标红书 / Ask the player to use the target red book.
    {
        if (CurrentStage != Stage.Chase && CurrentStage != Stage.Pack) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        TaskFeedback.For(this).Show(false, "请拿红书，放进书包。\nUse the RED book and put it in the bag.", task.feedbackText); // 成功或错误反馈文字引用 / Success or retry feedback text reference.
    }

    public string Mission() // 返回当前阶段的中英文指令 / Return the current bilingual objective.
    {
        switch (CurrentStage) // 按照当前任务阶段选择对应提示 / Choose the objective for the current task stage.
        {
            case Stage.Chase: return "抓住逃跑的红书，放进书包，然后松手。\nCatch the RED book, put it in the bag and release."; // 返回该任务阶段的双语提示 / Return the bilingual objective for this stage.
            case Stage.Pack: return "把红书放进书包，然后松手。\nPut the RED book inside the bag and release."; // 返回该任务阶段的双语提示 / Return the bilingual objective for this stage.
            case Stage.Won: return "任务完成！\nMission complete!"; // 返回该任务阶段的双语提示 / Return the bilingual objective for this stage.
            case Stage.TimedOut: return "时间到了，请重新开始。\nTime's up. Restart to try again."; // 返回该任务阶段的双语提示 / Return the bilingual objective for this stage.
            default: return "准备开始任务。\nReady to start."; // 处理尚未开始任务的默认提示 / Handle the default objective before a round starts.
        }
    }

    void SetStage(Stage stage) // 切换阶段并同步任务文字和反馈 / Change stage and synchronize instructions and feedback.
    {
        CurrentStage = stage; // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
        settled = 0f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        string message = Mission(); // 返回当前阶段的中英文指令 / Return the current bilingual objective.
        if (task.instructionText != null) task.instructionText.text = message.Split('\n')[0]; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (task.feedbackText != null) task.feedbackText.text = message; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        RefreshMission(); // 刷新任务文字与剩余时间 / Refresh the objective and remaining time.
        TaskFeedback.For(this).Show(stage != Stage.TimedOut, message, task.feedbackText); // 成功或错误反馈文字引用 / Success or retry feedback text reference.
    }

    void RefreshMission() // 刷新任务文字与剩余时间 / Refresh the objective and remaining time.
    {
        if (hud != null) hud.text = "TASK 2 · " + Mathf.CeilToInt(remaining) + "s\n" + Mission(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    }

    void LateUpdate() // 在帧末更新面向玩家的界面位置 / Update player-facing UI positions at the end of the frame.
    {
        if (props == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (view == null) view = Camera.main; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (view == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        var panel = hud.transform.parent; // 持续显示的任务文字 / Persistent objective text.
        panel.position = view.transform.position + view.transform.forward * .9f + view.transform.up * (CurrentStage == Stage.Won ? 0f : .22f); // 玩家当前使用的主摄像机 / Main camera currently used by the player.
        panel.localScale = Vector3.one * .0007f; // 缩小近距离卡片，保留操作视野 / Scale the closer card to preserve the play area.
        panel.rotation = view.transform.rotation; // 玩家当前使用的主摄像机 / Main camera currently used by the player.
        foreach (var label in labels) if (label != null) label.rotation = view.transform.rotation; // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
    }

    void Update() // 每帧推进计时、移动和任务判定 / Advance timing, motion and objective checks each frame.
    {
        if (props == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (mascot != null) mascot.position = home + new Vector3(Mathf.Sin(Time.time) * .25f, .3f, .2f); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (CurrentStage != Stage.Chase && CurrentStage != Stage.Pack) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        remaining = Mathf.Max(0f, remaining - Time.deltaTime); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        elapsed += Time.deltaTime; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        RefreshMission(); // 刷新任务文字与剩余时间 / Refresh the objective and remaining time.
        if (remaining <= 0f) { SetStage(Stage.TimedOut); RestoreBook(); return; } // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (CurrentStage == Stage.Chase && !book.isSelected) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        {
            Vector3 offset = new Vector3(Mathf.Sin(elapsed * 1.8f) * .5f, .12f + Mathf.Abs(Mathf.Sin(elapsed * 2f)) * .13f, Mathf.Sin(elapsed * .9f) * .22f); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            bookBody.MovePosition(Vector3.MoveTowards(bookBody.position, home + offset, Time.deltaTime * .65f)); // 目标红书的刚体引用 / Target red book rigidbody reference.
        }
        if (CurrentStage == Stage.Pack && InsideBag(book, bagVolume) && !book.isSelected) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        {
            settled += Time.deltaTime; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            if (settled >= .4f) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            {
                SetStage(Stage.Won); // 切换阶段并同步任务文字和反馈 / Change stage and synchronize instructions and feedback.
                task.CompleteAdventure(); // 标记任务二完成并关闭输入判定 / Mark Task2 complete and stop objective input checks.
                int stars = remaining > 90f ? 3 : remaining > 30f ? 2 : 1; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
                TaskFeedback.For(this).Clear(); // 清除短提示，让结束画面保持清晰 / Clear transient feedback for the persistent result screen.
                if (task.task2Panel != null) task.task2Panel.SetActive(false); // 结束后隐藏旧任务面板 / Hide the old task panel on completion.
                ((RectTransform)hud.transform.parent).sizeDelta = new Vector2(1000, 600); // 扩展为独立的结束画面 / Expand into a dedicated completion screen.
                hud.fontSize = 44; // 放大结束画面文字 / Enlarge result text.
                hud.text = "任务完成！\nMission complete!\n" + new string('★', stars) + "\n书包装好了！\nSelect RESTART to play again."; // 持续显示的任务文字 / Persistent objective text.
                CreateExitButton(); // 在结束画面提供退出入口 / Provide an exit action on the result screen.
                return; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
            }
        }
        else settled = 0f; // 处理前一条件不满足的分支 / Handle the branch where the previous condition is false.
        ExplainBagAttempt(); // 解释未松手或放错绿书的情况 / Explain held-book and wrong-book attempts.
        if (!book.isSelected && (book.transform.position.y < home.y - .8f || Vector3.Distance(book.transform.position, home) > 2.5f)) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        {
            bookBody.position = home; // 目标红书的刚体引用 / Target red book rigidbody reference.
            if (!bookBody.isKinematic) { bookBody.linearVelocity = Vector3.zero; bookBody.angularVelocity = Vector3.zero; } // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        }
    }

    void CreateExitButton() // 创建手柄可点击的退出按钮 / Create a controller-clickable exit button.
    { // 开始创建退出界面 / Begin exit UI setup.
        var panel = (RectTransform)hud.transform.parent; // 使用现有结束画面 / Use the existing result panel.
        panel.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>(); // 接受 XR 控制器射线 / Accept XR controller rays.
        panel.gameObject.AddComponent<GraphicRaycaster>(); // 同时支持编辑器鼠标 / Also support the editor mouse.
        ((RectTransform)hud.transform).offsetMin = new Vector2(16, 140); // 给底部按钮预留空间 / Reserve bottom space for the button.
        var buttonObject = new GameObject("Exit Button", typeof(RectTransform), typeof(Image), typeof(Button)); // 创建按钮 / Create the button.
        var rect = buttonObject.GetComponent<RectTransform>(); // 获取布局 / Get the layout.
        rect.SetParent(panel, false); // 放在结束画面内 / Parent to the result panel.
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, 0); // 锚定底部中央 / Anchor at bottom centre.
        rect.anchoredPosition = new Vector2(0, 70); // 设置按钮位置 / Set button position.
        rect.sizeDelta = new Vector2(400, 90); // 提供较大的点击区域 / Provide a large click target.
        var image = buttonObject.GetComponent<Image>(); // 获取按钮背景 / Get the button background.
        image.color = new Color(.12f, .35f, .55f); // 设置清晰的背景颜色 / Use a distinct background colour.
        var button = buttonObject.GetComponent<Button>(); // 获取点击组件 / Get the button component.
        button.targetGraphic = image; // 启用悬停及按下反馈 / Enable hover and press feedback.
        button.onClick.AddListener(ExitGame); // 连接退出动作 / Connect the exit action.
        var labelObject = new GameObject("Exit Label", typeof(RectTransform), typeof(TextMeshProUGUI)); // 创建双语文字 / Create bilingual text.
        labelObject.transform.SetParent(rect, false); // 文字放在按钮内 / Parent text to the button.
        var labelRect = labelObject.GetComponent<RectTransform>(); // 获取文字布局 / Get text layout.
        labelRect.anchorMin = Vector2.zero; labelRect.anchorMax = Vector2.one; // 填满按钮 / Fill the button.
        labelRect.offsetMin = labelRect.offsetMax = Vector2.zero; // 去除默认偏移 / Remove default offsets.
        var label = labelObject.GetComponent<TextMeshProUGUI>(); // 获取文字组件 / Get the text component.
        label.font = hud.font; label.fontSize = 36; // 复用中文字体 / Reuse the Chinese font.
        label.text = "退出游戏 / Exit"; // 显示退出标签 / Show the exit label.
        label.alignment = TextAlignmentOptions.Center; label.raycastTarget = false; // 文字居中且不拦截按钮 / Centre text without intercepting button input.
    } // 完成退出按钮 / Finish the exit button.

    public void ExitGame() // 退出应用，编辑器中停止测试 / Quit the app or stop the editor test.
    { // 开始退出 / Begin exit.
#if UNITY_EDITOR // 编辑器专用路径 / Editor-only path.
        UnityEditor.EditorApplication.isPlaying = false; // 停止 Play 模式 / Stop Play mode.
#else // 头显安装版路径 / Installed headset build path.
        Application.Quit(); // 关闭当前应用 / Close the application.
#endif // 结束平台选择 / End platform selection.
    } // 完成退出 / End exit.

    void ExplainBagAttempt() // 解释未松手或放错绿书的情况 / Explain held-book and wrong-book attempts.
    {
        if (Time.unscaledTime < nextHintTime) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        string message = null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if (InsideBag(book, bagVolume) && book.isSelected) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            message = "位置对了！请松手。\nCorrect position! Release the book."; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        else // 处理前一条件不满足的分支 / Handle the branch where the previous condition is false.
        {
            foreach (var word in FindObjectsByType<LookWord>(FindObjectsSortMode.None)) // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
            {
                if (word.english != "green book") continue; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
                var green = word.GetComponent<XRGrabInteractable>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
                if (green != null && InsideBag(green, bagVolume)) message = "这是绿书，请把红书放进书包。\nThat is the green book. Use the RED book."; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            }
        }
        if (message == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        nextHintTime = Time.unscaledTime + 3f; // 下一次允许显示纠错提示的时刻 / Next allowed time for a correction hint.
        TaskFeedback.For(this).Show(false, message, task.feedbackText); // 成功或错误反馈文字引用 / Success or retry feedback text reference.
    }

    public static Bounds ItemBounds(XRGrabInteractable item) // 合并物品有效碰撞体的世界边界 / Combine world bounds of the active item colliders.
    {
        Bounds bounds = new Bounds(item.transform.position, Vector3.zero); // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        bool found = false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        foreach (var collider in item.GetComponentsInChildren<Collider>()) // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
        {
            if (collider.isTrigger || !collider.enabled || !collider.gameObject.activeInHierarchy) continue; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            if (!found) { bounds = collider.bounds; found = true; } // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            else bounds.Encapsulate(collider.bounds); // 处理前一条件不满足的分支 / Handle the branch where the previous condition is false.
        }
        return bounds; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
    }

    public static bool InsideBag(XRGrabInteractable item, BoxCollider interior) // 在书包局部坐标中检查物品中心 / Check the item center in the bag local coordinates.
    {
        if (item == null || interior == null || !item.gameObject.activeInHierarchy) return false; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        Vector3 center = interior.transform.InverseTransformPoint(ItemBounds(item).center) - interior.center; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        Vector3 half = interior.size * .5f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        return Mathf.Abs(center.x) <= half.x && Mathf.Abs(center.y) <= half.y && Mathf.Abs(center.z) <= half.z; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
    }

    void RestoreBook() // 取消抓取并复位原书的姿态与物理状态 / Cancel selection and restore the book pose and physics.
    {
        if (book == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (book.isSelected && book.interactionManager != null) book.interactionManager.CancelInteractableSelection((IXRSelectInteractable)book); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        book.transform.SetParent(bookParent, true); // 红书最初的父物体 / Original parent of the red book.
        book.transform.localScale = bookScale; // 原书局部缩放，用于复位 / Original book local scale for restoration.
        bookBody.position = home; // 目标红书的刚体引用 / Target red book rigidbody reference.
        bookBody.rotation = bookRotation; // 目标红书的刚体引用 / Target red book rigidbody reference.
        bookBody.isKinematic = originalKinematic; // 目标红书的刚体引用 / Target red book rigidbody reference.
        bookBody.useGravity = originalGravity; // 目标红书的刚体引用 / Target red book rigidbody reference.
        book.throwOnDetach = originalThrow; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if (!bookBody.isKinematic) { bookBody.linearVelocity = Vector3.zero; bookBody.angularVelocity = Vector3.zero; } // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    }

    public void StopRound() // 停止当前回合并清理生成的物品和监听器 / Stop the round and clean up spawned objects and listeners.
    {
        CurrentStage = Stage.Idle; // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
        if (book != null) book.selectExited.RemoveListener(BookReleased); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        RestoreBook(); // 取消抓取并复位原书的姿态与物理状态 / Cancel selection and restore the book pose and physics.
        if (props != null) { props.SetActive(false); Destroy(props); } // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        labels.Clear(); // 需要面向玩家的物品标签集合 / Object labels that must face the player.
        props = null; // 当前回合生成的模型副本 / Model instance generated for the current round.
        book = null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    }

    void OnDestroy() { StopRound(); } // 销毁时清理本组件创建的状态 / Clean up this component state when destroyed.
}
