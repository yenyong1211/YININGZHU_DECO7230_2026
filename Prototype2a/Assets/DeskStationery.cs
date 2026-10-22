using System.Collections.Generic; // 导入 System.Collections.Generic 命名空间 / Import the System.Collections.Generic namespace.
using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using UnityEngine.XR.Interaction.Toolkit; // 导入 UnityEngine.XR.Interaction.Toolkit 命名空间 / Import the UnityEngine.XR.Interaction.Toolkit namespace.
using UnityEngine.XR.Interaction.Toolkit.Interactables; // 导入 UnityEngine.XR.Interaction.Toolkit.Interactables 命名空间 / Import the UnityEngine.XR.Interaction.Toolkit.Interactables namespace.

[ExecuteAlways] // 声明组件或编辑器入口的元数据 / Declare component or editor-entry metadata.
[DefaultExecutionOrder(-100)] // 声明组件或编辑器入口的元数据 / Declare component or editor-entry metadata.
public class DeskStationery : MonoBehaviour // 声明 DeskStationery 组件或辅助类型 / Declare the DeskStationery component or helper type.
{
    private readonly List<XRGrabInteractable> items = new List<XRGrabInteractable>(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    private readonly List<Vector3> homes = new List<Vector3>(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    private readonly List<Quaternion> rotations = new List<Quaternion>(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    private LookWord original; // 原场景中的红书，用来安排桌面位置 / Original red book used for desktop placement.
    private float surfaceY; // 桌面高度，用来对齐物品底部 / Tabletop height used to align item bottoms.
    private Bounds desktop; // 桌面的范围，用来限制物品摆放位置 / Desktop bounds used to limit placement.
    private bool hasDesktop; // 记录是否成功取得桌面范围 / Track whether desktop bounds were found.
    private bool runtimeReady; // 避免重复创建文具交互组件 / Prevent duplicate stationery interaction setup.

    void OnEnable() { InitializeStationery(); } // 启用时初始化并连接交互事件 / Initialize and connect interaction events when enabled.
    void Start() { InitializeStationery(); } // 在场景加载完成后执行初始化 / Initialize after the scene has loaded.

    void InitializeStationery() // 等待场景就绪后配置文具 / Configure stationery once the scene is ready.
    {
        if (Application.isPlaying && runtimeReady) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (!gameObject.scene.IsValid() || !gameObject.scene.isLoaded) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        foreach (var word in FindObjectsByType<LookWord>(FindObjectsSortMode.None)) // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
            if (word.english == "book") { original = word; break; } // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        var green = transform.Find("GreenBook"); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        var pen = transform.Find("Pen"); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        var pencilCase = transform.Find("PencilCase"); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (green == null || pen == null || pencilCase == null) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        surfaceY = .77f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        foreach (var word in FindObjectsByType<LookWord>(FindObjectsSortMode.None)) // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
        {
            if (word.english != "table" && word.chinese != "桌子") continue; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            var col = word.GetComponent<Collider>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
            if (col != null) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            {
                desktop = col.bounds; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
                hasDesktop = true; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
                surfaceY = desktop.max.y; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            }
            break; // 已找到桌子，不再继续寻找 / Stop searching after finding the table.
        }
        PlaceOnDesk(green, .30f, .3f); // 按桌面边界和物品底部对齐位置 / Align the item to the tabletop bounds and its bottom.
        PlaceOnDesk(pen, .45f, .10f); // 按桌面边界和物品底部对齐位置 / Align the item to the tabletop bounds and its bottom.
        PlaceOnDesk(pencilCase, .70f, .7f); // 按桌面边界和物品底部对齐位置 / Align the item to the tabletop bounds and its bottom.
        if (original != null) PlaceOnDesk(original.transform, .18f, .25f); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        if (!Application.isPlaying) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        items.Clear(); homes.Clear(); rotations.Clear(); // 停止提示动画与音效并隐藏反馈 / Stop feedback animation and audio and hide the panel.
        Setup(green.gameObject,new Vector3(.174f,.038f,.234f),"绿书","lǜ shū","green book", false); // 配置文具的词语、碰撞体、刚体和抓取 / Configure stationery vocabulary, colliders, physics and grabbing.
        Setup(pen.gameObject,new Vector3(.18f,.026f,.03f),"笔","bǐ","pen", false); // 配置文具的词语、碰撞体、刚体和抓取 / Configure stationery vocabulary, colliders, physics and grabbing.
        Setup(pencilCase.gameObject,new Vector3(.26f,.062f,.12f),"笔袋","bǐ dài","pencil case", true); // 配置文具的词语、碰撞体、刚体和抓取 / Configure stationery vocabulary, colliders, physics and grabbing.
        runtimeReady = true; // 避免重复创建文具交互组件 / Prevent duplicate stationery interaction setup.
    }

    void PlaceOnDesk(Transform target, float x, float z) // 按桌面边界和物品底部对齐位置 / Align the item to the tabletop bounds and its bottom.
    {
        var renderers = target.GetComponentsInChildren<Renderer>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (renderers.Length == 0) return; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        Bounds bounds = renderers[0].bounds; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds); // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
        Vector3 offset = Vector3.up*(surfaceY+.004f-bounds.min.y); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if (hasDesktop) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        {
            float targetX = Mathf.Lerp(desktop.min.x,desktop.max.x,x); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            float targetZ = Mathf.Lerp(desktop.min.z,desktop.max.z,z); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            targetX = Mathf.Clamp(targetX,desktop.min.x+bounds.extents.x+.015f,desktop.max.x-bounds.extents.x-.015f); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            targetZ = Mathf.Clamp(targetZ,desktop.min.z+bounds.extents.z+.015f,desktop.max.z-bounds.extents.z-.015f); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            foreach (var obstacle in FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None)) // 检查键盘占用的桌面区域 / Inspect the keyboard footprint on the desk.
            {
                if (obstacle.name.IndexOf("keyboard", System.StringComparison.OrdinalIgnoreCase) < 0) continue; // 只避让键盘 / Only avoid keyboard meshes.
                Bounds occupied = obstacle.bounds; // 读取键盘世界边界 / Read the keyboard world bounds.
                bool overlaps = Mathf.Abs(targetX - occupied.center.x) < bounds.extents.x + occupied.extents.x + .025f && Mathf.Abs(targetZ - occupied.center.z) < bounds.extents.z + occupied.extents.z + .025f; // 检查俯视投影是否重叠 / Check overlap in the horizontal plane.
                if (!overlaps) continue; // 已经避开时保留位置 / Keep positions that are already clear.
                float left = occupied.min.x - bounds.extents.x - .025f; // 计算键盘左侧安全位置 / Compute a safe position left of the keyboard.
                float right = occupied.max.x + bounds.extents.x + .025f; // 计算键盘右侧安全位置 / Compute a safe position right of the keyboard.
                if (left >= desktop.min.x + bounds.extents.x + .015f) targetX = left; // 优先使用桌面左侧空位 / Prefer free space on the left of the desk.
                else if (right <= desktop.max.x - bounds.extents.x - .015f) targetX = right; // 左侧不足时使用右侧空位 / Use right-side space when the left is insufficient.
            }
            offset.x=targetX-bounds.center.x; offset.z=targetZ-bounds.center.z; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        }
        target.position += offset; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
    }

    void Setup(GameObject go, Vector3 size, string chinese, string pinyin, string english, bool openContainer) // 配置文具的词语、碰撞体、刚体和抓取 / Configure stationery vocabulary, colliders, physics and grabbing.
    {
        var word = go.GetComponent<LookWord>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if (word == null) word = go.AddComponent<LookWord>(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        word.chinese=chinese; word.pinyin=pinyin; word.english=english; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if (openContainer) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        {
            foreach (var renderer in go.GetComponentsInChildren<MeshRenderer>()) // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
                if (renderer.name.StartsWith("Case ") && renderer.GetComponent<BoxCollider>() == null) // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
                    renderer.gameObject.AddComponent<BoxCollider>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
        }
        else if(go.GetComponent<BoxCollider>()==null) go.AddComponent<BoxCollider>().size=size; // 处理前一条件不满足的分支 / Handle the branch where the previous condition is false.
        var body=go.GetComponent<Rigidbody>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if(body==null) body=go.AddComponent<Rigidbody>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
        body.isKinematic = true; // 放置后保持稳定，抓取时仍由 XR 控制 / Stay stable when placed while XR still controls grabbing.
        body.useGravity = false; // 防止文具在学习和任务一期间掉落 / Prevent stationery from falling during learning and Task1.
        body.mass = openContainer ? .25f : .08f; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        body.interpolation=RigidbodyInterpolation.Interpolate; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        body.collisionDetectionMode=CollisionDetectionMode.ContinuousSpeculative; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        var grab=go.GetComponent<XRGrabInteractable>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if(grab==null) grab=go.AddComponent<XRGrabInteractable>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
        grab.throwOnDetach=false; // 松手时不施加抛掷速度 / Do not apply throwing velocity on release.
        grab.useDynamicAttach=true; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        grab.movementType=XRBaseInteractable.MovementType.Kinematic; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        grab.selectEntered.AddListener(OnSelected); // 连接交互事件监听器 / Connect the interaction event listener.
        var trigger=go.GetComponent<WordCardTrigger>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if(trigger==null) trigger=go.AddComponent<WordCardTrigger>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
        items.Add(grab);homes.Add(go.transform.position);rotations.Add(go.transform.rotation); // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
    }

    void OnSelected(SelectEnterEventArgs args) // 选中文具时只检查当前任务 / Check only the current task when stationery is selected.
    {
        var trigger=args.interactableObject.transform.GetComponent<WordCardTrigger>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if(trigger!=null) trigger.ShowCard(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    }

    public void ResetItems() // 把文具逐个复位到初始位置 / Restore each stationery item to its starting pose.
    {
        for(int i=0;i<items.Count;i++) Restore(i); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    }

    void Restore(int index) // 安全释放并复位指定文具 / Safely release and reset the specified stationery item.
    {
        var item=items[index]; if(item==null) return; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if(item.isSelected && item.interactionManager!=null) // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            item.interactionManager.CancelInteractableSelection((IXRSelectInteractable)item); // 先取消抓取，再把文具放回原位 / Cancel grabbing before restoring the item.
        var body=item.GetComponent<Rigidbody>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        body.position=homes[index]; body.rotation=rotations[index]; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        if (!body.isKinematic) { body.linearVelocity=Vector3.zero;body.angularVelocity=Vector3.zero; } // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    }

    void Update() // 每帧推进计时、移动和任务判定 / Advance timing, motion and objective checks each frame.
    {
        if(!Application.isPlaying) return; // 没有运行游戏时，不检查文具掉落 / Skip dropped-item checks outside Play mode.
        for(int i=0;i<items.Count;i++) // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            if(items[i]!=null && !items[i].isSelected && // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
                (items[i].transform.position.y<surfaceY-.6f || Vector3.Distance(items[i].transform.position,homes[i])>2.5f)) Restore(i); // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
    }

    void OnDisable() // 停用时停止播放或解除事件监听 / Stop playback or remove listeners when disabled.
    {
        foreach(var item in items) if(item!=null) item.selectEntered.RemoveListener(OnSelected); // 解除交互事件监听器 / Remove the interaction event listener.
        items.Clear();homes.Clear();rotations.Clear(); // 停止提示动画与音效并隐藏反馈 / Stop feedback animation and audio and hide the panel.
        runtimeReady=false; // 避免重复创建文具交互组件 / Prevent duplicate stationery interaction setup.
    }
}
