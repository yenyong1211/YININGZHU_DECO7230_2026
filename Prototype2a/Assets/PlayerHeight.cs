using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;

public class PlayerHeight : MonoBehaviour
{
    private XROrigin origin;
    private Transform heightOffset;
    private float baseHeight;
    private bool ready;

    IEnumerator Start() // Wait for tracking, then align eye height with the learning panel.
    {
        origin = FindFirstObjectByType<XROrigin>();
        var flow = GetComponent<UIFlowManager>();
        if (origin == null || origin.Camera == null || origin.CameraFloorOffsetObject == null || flow == null) yield break;
        heightOffset = origin.CameraFloorOffsetObject.transform; // Adjust the shared head-and-hand offset rather than fighting locomotion gravity.
        float deadline = Time.realtimeSinceStartup + 10f;
        while (Time.realtimeSinceStartup < deadline)
        {
            var head = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (head.TryGetFeatureValue(CommonUsages.isTracked, out bool tracked) && tracked) break;
            yield return null;
        }
        if (flow.learningPanel != null)
        {
            var card = flow.learningPanel.GetComponent<RectTransform>();
            float target = card != null ? card.TransformPoint(card.rect.center).y : flow.learningPanel.transform.position.y;
            heightOffset.position += Vector3.up * (target - origin.Camera.transform.position.y);
        }
        baseHeight = heightOffset.localPosition.y;
        ready = true;
    }

    void Update() // Hold left X to lower the view or Y to raise it within limits.
    {
        if (!ready || heightOffset == null) return;
        var left = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        left.TryGetFeatureValue(CommonUsages.primaryButton, out bool lower);
        left.TryGetFeatureValue(CommonUsages.secondaryButton, out bool raise);
        var position = heightOffset.localPosition;
        position.y = Mathf.Clamp(position.y + ((raise ? 1 : 0) - (lower ? 1 : 0)) * .35f * Time.unscaledDeltaTime, baseHeight - .6f, baseHeight + .6f);
        heightOffset.localPosition = position;
    }
}
