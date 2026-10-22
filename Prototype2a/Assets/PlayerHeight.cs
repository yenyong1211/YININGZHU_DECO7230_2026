using System.Collections; // 使用等待头显定位的协程 / Use a coroutine to wait for headset tracking.
using UnityEngine; // 使用位置和时间功能 / Use transforms and time.
using UnityEngine.XR; // 读取左手柄 X 和 Y / Read left controller X and Y.
using Unity.XR.CoreUtils; // 获取 XR 玩家根物体 / Access the XR player origin.

public class PlayerHeight : MonoBehaviour // 校准初始高度并允许手动升降 / Calibrate initial height and allow manual adjustment.
{
    private XROrigin origin; // 移动整个玩家，保留头手相对位置 / Move the whole player while preserving head and hand offsets.
    private Transform heightOffset; // 使用 XR 高度偏移层，避免被移动重力抵消 / Use the XR offset layer so locomotion gravity cannot undo height changes.
    private float baseHeight; // 限制手动调整范围的基准 / Reference for manual adjustment limits.
    private bool ready; // 定位完成后才接受输入 / Accept input after calibration.

    IEnumerator Start() // 等待真实头显建立定位 / Wait for headset tracking.
    {
        origin = FindFirstObjectByType<XROrigin>(); // 获取玩家 / Find the player.
        var flow = GetComponent<UIFlowManager>(); // 获取学习卡 / Get the learning card reference.
        if (origin == null || origin.Camera == null || origin.CameraFloorOffsetObject == null || flow == null) yield break; // 缺少引用时保留原视角 / Keep the existing view if references are missing.
        heightOffset = origin.CameraFloorOffsetObject.transform; // 同时调整头显和手柄的父层 / Adjust the shared headset and controller parent.
        float deadline = Time.realtimeSinceStartup + 10f; // 最多等待十秒，不阻塞界面 / Wait up to ten seconds without blocking UI.
        while (Time.realtimeSinceStartup < deadline) // 等待定位就绪 / Wait for tracking readiness.
        {
            var head = InputDevices.GetDeviceAtXRNode(XRNode.Head); // 获取头显设备 / Get the headset device.
            if (head.TryGetFeatureValue(CommonUsages.isTracked, out bool tracked) && tracked) break; // 定位就绪后校准 / Calibrate once tracking is ready.
            yield return null; // 下一帧再次检查 / Check again next frame.
        }
        if (flow.learningPanel != null) // 使用学习面板中心作为眼睛高度 / Use the learning panel centre as eye height.
        {
            var card = flow.learningPanel.GetComponent<RectTransform>(); // 取得面板几何中心 / Get the panel geometry.
            float target = card != null ? card.TransformPoint(card.rect.center).y : flow.learningPanel.transform.position.y; // 计算世界高度 / Calculate world-space height.
            heightOffset.position += Vector3.up * (target - origin.Camera.transform.position.y); // 对齐视线，不覆盖头显旋转 / Align eye height without overriding headset rotation.
        }
        baseHeight = heightOffset.localPosition.y; // 保存调整基准 / Store the adjustment reference.
        ready = true; // 启用 X/Y 升降 / Enable X/Y height adjustment.
    }

    void Update() // 按住左手柄按键连续调整 / Hold left-controller buttons to adjust continuously.
    {
        if (!ready || heightOffset == null) return; // 校准前不移动 / Do not move before calibration.
        var left = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand); // 左手柄 X/Y，不占用摇杆 / Read left X/Y without using the stick.
        left.TryGetFeatureValue(CommonUsages.primaryButton, out bool lower); // X 降低视角 / X lowers the viewpoint.
        left.TryGetFeatureValue(CommonUsages.secondaryButton, out bool raise); // Y 升高视角 / Y raises the viewpoint.
        var position = heightOffset.localPosition; // 保留水平移动 / Preserve horizontal movement.
        position.y = Mathf.Clamp(position.y + ((raise ? 1 : 0) - (lower ? 1 : 0)) * .35f * Time.unscaledDeltaTime, baseHeight - .6f, baseHeight + .6f); // 每秒升降 0.35 米，限制范围 / Adjust at 0.35 m/s within a bounded range.
        heightOffset.localPosition = position; // 同时移动头显和双手 / Move headset and hands together.
    }
}
