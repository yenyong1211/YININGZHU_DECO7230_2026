using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using TMPro; // 导入 TMPro 命名空间 / Import the TMPro namespace.

public class ObjectWordCard : MonoBehaviour // 声明 ObjectWordCard 组件或辅助类型 / Declare the ObjectWordCard component or helper type.
{ // 开始代码块 / Begin the block.
    public GameObject wordHintPanel; // 执行 ObjectWordCard 的当前操作 / Perform the current ObjectWordCard operation.
    public TMP_Text chineseText; // 词卡的中文文字引用 / Chinese word-card text reference.
    public TMP_Text pinyinText; // 词卡的声调拼音文字引用 / Tone-marked pinyin text reference.
    public Camera playerCamera; // 执行 ObjectWordCard 的当前操作 / Perform the current ObjectWordCard operation.
    public float heightOffset = 0.25f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    public float forwardOffset = 0.05f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    private LookWord target; // 执行 ObjectWordCard 的当前操作 / Perform the current ObjectWordCard operation.

    void Start() { HideWordCard(); } // 在场景加载完成后执行初始化 / Initialize after the scene has loaded.

    public void ShowWordCard(GameObject selectedObject) // 读取物品词语并显示跟随词卡 / Read the object vocabulary and show its following word card.
    { // 开始代码块 / Begin the block.
        if (selectedObject == null || wordHintPanel == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        target = selectedObject.GetComponentInParent<LookWord>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (target == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (chineseText != null) chineseText.text = target.chinese; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (pinyinText != null) pinyinText.text = target.pinyin + // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            (string.IsNullOrEmpty(target.english) ? "" : "\n" + target.english); // 执行 ObjectWordCard 的当前操作 / Perform the current ObjectWordCard operation.
        wordHintPanel.SetActive(true); // 设置对应场景物体的显示状态 / Set the visibility of the corresponding scene object.
        LateUpdate(); // 在帧末更新面向玩家的界面位置 / Update player-facing UI positions at the end of the frame.
    } // 结束代码块 / End the block.

    void LateUpdate() // 在帧末更新面向玩家的界面位置 / Update player-facing UI positions at the end of the frame.
    { // 开始代码块 / Begin the block.
        if (wordHintPanel == null || !wordHintPanel.activeSelf) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (target == null || !target.gameObject.activeInHierarchy) { HideWordCard(); return; } // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (playerCamera == null) playerCamera = Camera.main; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (playerCamera == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        var col = target.GetComponentInChildren<Collider>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        Vector3 top = col != null ? new Vector3(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z) : target.transform.position; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        Vector3 position = top + Vector3.up * heightOffset; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        position += (playerCamera.transform.position - position).normalized * forwardOffset; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        wordHintPanel.transform.position = position; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        Vector3 direction = position - playerCamera.transform.position; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        if (direction.sqrMagnitude > 0.0001f) wordHintPanel.transform.rotation = Quaternion.LookRotation(direction); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    } // 结束代码块 / End the block.

    public void HideWordCard() // 清除目标并隐藏物品词卡 / Clear the target and hide the object word card.
    { // 开始代码块 / Begin the block.
        target = null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if (wordHintPanel != null) wordHintPanel.SetActive(false); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
    } // 结束代码块 / End the block.
} // 结束代码块 / End the block.
