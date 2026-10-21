using System.Collections.Generic; // 缓存专用材质 / Cache private materials.
using TMPro; // 识别中文文字材质 / Identify text materials.
using UnityEngine; // 使用画布与材质 / Use canvases and materials.
using UnityEngine.UI; // 取得背景和按钮 / Access backgrounds and buttons.

public class FrontPanel : MonoBehaviour // 使提示显示在场景物体前面 / Render a panel in front of scene objects.
{ // 开始组件 / Begin component.
    private readonly Dictionary<Material, Material> copies = new Dictionary<Material, Material>(); // 不修改共享字体资源 / Do not modify shared font assets.
    void OnEnable() { Canvas.willRenderCanvases += DrawInFront; } // 在界面重建后设置显示材质 / Apply materials after UI rebuilds.
    void OnDisable() { Canvas.willRenderCanvases -= DrawInFront; } // 停用时解除监听 / Unsubscribe when disabled.
    void DrawInFront() // 同时处理主字体和中文后备字体 / Handle both main and fallback font materials.
    { // 开始设置渲染 / Begin rendering setup.
        foreach (var graphic in GetComponentsInChildren<Graphic>()) // 遍历本面板界面元素 / Visit this panel's UI elements.
        { // 处理每个元素 / Process each element.
            bool text = graphic is TMP_Text || graphic is TMP_SubMeshUI; // 区分文字与背景 / Distinguish text from backgrounds.
            for (int i = 0; i < graphic.canvasRenderer.materialCount; i++) // 包含中文后备字体材质 / Include fallback font materials.
            { // 处理每个材质 / Process each material.
                var source = graphic.canvasRenderer.GetMaterial(i); // 获取实际渲染材质 / Get the rendered material.
                if (source == null || source.shader.name.StartsWith("DeskUI/")) continue; // 已处理则保留 / Keep materials already processed.
                if (!copies.TryGetValue(source, out var copy)) // 每个来源只创建一次 / Create each material copy once.
                { // 创建本面板专用材质 / Create a private panel material.
                    copy = new Material(source); // 保留字体图集与参数 / Preserve font atlas and properties.
                    copy.shader = Resources.Load<Shader>(text ? "DeskUI/TextOverlay" : "DeskUI/PanelOverlay"); // 忽略场景深度遮挡 / Ignore scene depth occlusion.
                    copy.renderQueue = 4000; // 在普通场景之后绘制 / Draw after the normal scene.
                    copies.Add(source, copy); // 缓存复用 / Cache for reuse.
                } // 完成材质创建 / Finish material creation.
                graphic.canvasRenderer.SetMaterial(copy, i); // 应用到当前画布元素 / Apply to this canvas element.
            } // 完成材质遍历 / Finish material iteration.
        } // 完成元素遍历 / Finish element iteration.
    } // 完成渲染设置 / Finish rendering setup.
    void OnDestroy() { foreach (var material in copies.Values) Destroy(material); } // 释放专用材质 / Release private materials.
} // 结束组件 / End component.
