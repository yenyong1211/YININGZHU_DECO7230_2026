using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using UnityEngine.XR.Interaction.Toolkit; // 导入 UnityEngine.XR.Interaction.Toolkit 命名空间 / Import the UnityEngine.XR.Interaction.Toolkit namespace.
using UnityEngine.XR.Interaction.Toolkit.Interactables; // 导入 UnityEngine.XR.Interaction.Toolkit.Interactables 命名空间 / Import the UnityEngine.XR.Interaction.Toolkit.Interactables namespace.

public class WordCardTrigger : MonoBehaviour // 声明 WordCardTrigger 组件或辅助类型 / Declare the WordCardTrigger component or helper type.
{
    public Task1Manager task1Manager; // 把当前物品交给第一关判断 / Send this object to Task1 for checking.
    private Task2Manager task2Manager; // 把当前物品交给第二关判断 / Send this object to Task2 for checking.
    private XRBaseInteractable interactable; // 接收当前物品的 XR 激活事件 / Receive XR activation events from this object.
    private float lastInteraction = -1f; // 抑制重复选择事件的最近操作时刻 / Last interaction time used to suppress duplicate events.

    void Awake() // 初始化场景引用与启动状态 / Initialize scene references and startup state.
    {
        if (task1Manager == null) task1Manager = FindFirstObjectByType<Task1Manager>(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        task2Manager = FindFirstObjectByType<Task2Manager>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        interactable = GetComponent<XRBaseInteractable>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
    }

    void OnEnable() // 启用时初始化并连接交互事件 / Initialize and connect interaction events when enabled.
    {
        if (interactable != null) interactable.activated.AddListener(OnActivated); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    }

    void OnDisable() // 停用时停止播放或解除事件监听 / Stop playback or remove listeners when disabled.
    {
        if (interactable != null) interactable.activated.RemoveListener(OnActivated); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    }

    void OnActivated(ActivateEventArgs args) { ShowCard(); } // 把控制器激活操作转为物品选择 / Forward controller activation to object selection.

    public void ShowCard() // 保留原事件方法名，只检查任务，不再弹出词卡 / Keep the existing event name and check tasks without showing word cards.
    {
        if (Time.unscaledTime - lastInteraction < 0.25f) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        lastInteraction = Time.unscaledTime; // 抑制重复选择事件的最近操作时刻 / Last interaction time used to suppress duplicate events.
        if (task1Manager != null) task1Manager.CheckObject(gameObject); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (task2Manager != null) task2Manager.CheckObject(gameObject); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    }
}
