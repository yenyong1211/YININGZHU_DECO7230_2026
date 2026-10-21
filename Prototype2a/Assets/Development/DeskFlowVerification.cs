#if UNITY_EDITOR // 限定此段代码的编译条件 / Set the compilation condition for this code.
using System; // 导入 System 命名空间 / Import the System namespace.
using System.Collections; // 导入 System.Collections 命名空间 / Import the System.Collections namespace.
using System.Collections.Generic; // 导入 System.Collections.Generic 命名空间 / Import the System.Collections.Generic namespace.
using System.IO; // 导入 System.IO 命名空间 / Import the System.IO namespace.
using TMPro; // 导入 TMPro 命名空间 / Import the TMPro namespace.
using UnityEditor; // 导入 UnityEditor 命名空间 / Import the UnityEditor namespace.
using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using UnityEngine.TextCore.LowLevel; // 导入 UnityEngine.TextCore.LowLevel 命名空间 / Import the UnityEngine.TextCore.LowLevel namespace.
using UnityEngine.XR.Interaction.Toolkit; // 导入 UnityEngine.XR.Interaction.Toolkit 命名空间 / Import the UnityEngine.XR.Interaction.Toolkit namespace.
using UnityEngine.XR.Interaction.Toolkit.Interactors; // 导入 UnityEngine.XR.Interaction.Toolkit.Interactors 命名空间 / Import the UnityEngine.XR.Interaction.Toolkit.Interactors namespace.
using UnityEngine.XR.Interaction.Toolkit.Interactables; // 导入 UnityEngine.XR.Interaction.Toolkit.Interactables 命名空间 / Import the UnityEngine.XR.Interaction.Toolkit.Interactables namespace.

    // 说明：DeskFlowVerification 的实现说明 / Implementation note: Runs against the open scene and real XR select/release events. No headset input is synthesized.
public class DeskFlowVerification : MonoBehaviour // 声明 DeskFlowVerification 组件或辅助类型 / Declare the DeskFlowVerification component or helper type.
{
    private const string Pending="DeskGame.VerifyFlow"; // 跨脚本重载保存测试启动请求的键 / Key that preserves the test request across script reloads.
    private readonly List<string> results=new List<string>(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    private DeskVerificationInteractor hand; // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
    private XRInteractionManager manager; // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
    private DeskAdventure adventure; // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.

    [InitializeOnLoadMethod] // 声明组件或编辑器入口的元数据 / Declare component or editor-entry metadata.
    static void Register() // 仅在编辑器中注册自动测试入口 / Register automated test hooks in the editor only.
    {
        EditorApplication.playModeStateChanged-=OnPlayMode; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        EditorApplication.playModeStateChanged+=OnPlayMode; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    }

    static void OnPlayMode(PlayModeStateChange state) // 按编辑器播放状态启动或结束测试 / Start or finish tests when editor play mode changes.
    {
        if(state==PlayModeStateChange.EnteredEditMode && SessionState.GetBool("DeskGame.BatchComplete",false)) // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        {
            EditorApplication.Exit(SessionState.GetInt("DeskGame.BatchCode",1)); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
            return; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
        }
        if(state!=PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(Pending,false)) return; // 跨脚本重载保存测试启动请求的键 / Key that preserves the test request across script reloads.
        SessionState.SetBool(Pending,false); // 跨脚本重载保存测试启动请求的键 / Key that preserves the test request across script reloads.
        new GameObject("Automated Desk Flow Verification").AddComponent<DeskFlowVerification>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
    }

    public static void RunBatch() // 加载正式场景并运行批处理验证 / Load the actual scene and run batch verification.
    { 
        PrepareFont(); // 生成并预填充可读的中文备用字库 / Generate and prefill a readable Chinese fallback atlas.
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/FirstXRScene.unity"); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        SessionState.SetBool("DeskGame.Batch",true); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        SessionState.SetBool("DeskGame.BatchComplete",false); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        SessionState.SetBool(Pending,true); // 跨脚本重载保存测试启动请求的键 / Key that preserves the test request across script reloads.
        EditorApplication.EnterPlaymode(); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
    }

    [MenuItem("Tools/Desk Game/Verify task flow")] // 声明组件或编辑器入口的元数据 / Declare component or editor-entry metadata.
    static void Verify() // 从编辑器菜单启动流程测试 / Start flow verification from the editor menu.
    {
        if(EditorApplication.isPlaying) // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        {
            new GameObject("Automated Desk Flow Verification").AddComponent<DeskFlowVerification>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
            return; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
        }
        SessionState.SetBool(Pending,true); // 跨脚本重载保存测试启动请求的键 / Key that preserves the test request across script reloads.
        EditorApplication.EnterPlaymode(); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
    }

    [MenuItem("Tools/Desk Game/Prepare Chinese fallback")] // 声明组件或编辑器入口的元数据 / Declare component or editor-entry metadata.
    static void PrepareFont() // 生成并预填充可读的中文备用字库 / Generate and prefill a readable Chinese fallback atlas.
    {
        string path="Assets/Resources/ChineseRuntimeFallback.asset"; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        var fallback=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if(fallback==null) // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        {
            var source=AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/NotoSansSC-VariableFont_wght.ttf"); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            fallback=TMP_FontAsset.CreateFontAsset(source,64,8,GlyphRenderMode.SDFAA,2048,2048,AtlasPopulationMode.Dynamic,true); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            fallback.name="ChineseRuntimeFallback"; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            AssetDatabase.CreateAsset(fallback,path); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
            AssetDatabase.AddObjectToAsset(fallback.material,fallback); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
            foreach(var atlas in fallback.atlasTextures) AssetDatabase.AddObjectToAsset(atlas,fallback); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        }
        string text=File.ReadAllText("Assets/Fonts/CharacterSets/TMP_AllCharacters.txt"); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        fallback.TryAddCharacters(text,out string missing); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        foreach(var atlas in fallback.atlasTextures) // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
            if(!AssetDatabase.Contains(atlas)) AssetDatabase.AddObjectToAsset(atlas,fallback); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        foreach(string asset in new[]{"Assets/Fonts/ChineseNotoSansSC-VariableFont_wght SDF.asset","Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset"}) // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        {
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(asset); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            if(font==null) continue; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            if(font.fallbackFontAssetTable==null) font.fallbackFontAssetTable=new List<TMP_FontAsset>(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            font.fallbackFontAssetTable.RemoveAll(f=>f==null); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            if(!font.fallbackFontAssetTable.Contains(fallback)) font.fallbackFontAssetTable.Add(fallback); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
            EditorUtility.SetDirty(font); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        }
        EditorUtility.SetDirty(fallback);AssetDatabase.SaveAssets(); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        File.WriteAllText("/tmp/Prototype2a-font-check.txt","Prepared Chinese fallback. Missing characters: "+missing); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        Debug.Log("DESK FONT PREPARED; missing: "+missing); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
    }

    IEnumerator Start() // 在场景加载完成后执行初始化 / Initialize after the scene has loaded.
    {
        var run=Run(); // 验证简化任务的真实播放模式流程 / Verify the simplified objective in actual Play Mode.
        Exception failure=null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        while(true) // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        {
            bool more=false;object next=null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            try {more=run.MoveNext();if(more) next=run.Current;} // 尝试执行可报告失败的验证步骤 / Attempt a verification step whose failure can be reported.
            catch(Exception e) {failure=e;} // 记录测试异常并安全结束验证 / Record the test exception and finish safely.
            if(failure!=null || !more) break; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            yield return next; // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        }
        if(failure!=null)results.Add("FAIL: "+failure); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        string report=string.Join("\n",results); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        File.WriteAllText("/tmp/Prototype2a-flow-results.txt",report); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        Debug.Log("DESK FLOW "+(failure==null?"PASSED":"FAILED")+"\n"+report); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if(hand!=null)Destroy(hand.gameObject); // 释放当前组件创建的临时对象 / Release the temporary object created by this component.
        if(SessionState.GetBool("DeskGame.Batch",false)) // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        {
            SessionState.SetBool("DeskGame.BatchComplete",true); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
            SessionState.SetInt("DeskGame.BatchCode",failure==null?0:1); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        }
        EditorApplication.delayCall+=()=>EditorApplication.ExitPlaymode(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    }

    void Check(bool pass,string name) // 记录通过项或在断言失败时终止测试 / Record a pass or fail the test on a false assertion.
    {
        if(!pass)throw new Exception(name+"; stage="+(adventure!=null?adventure.CurrentStage.ToString():"not started")); // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
        results.Add("PASS: "+name); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
    }

    void Select(XRGrabInteractable item) // 通过实际XR管理器发送抓取事件 / Send a grab event through the actual XR manager.
    {
        hand.target=item;hand.held=true; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        hand.transform.position=item.transform.position; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        manager.SelectEnter((IXRSelectInteractor)hand,(IXRSelectInteractable)item); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        Check(item.isSelected,"XR selected "+item.name); // 记录通过项或在断言失败时终止测试 / Record a pass or fail the test on a false assertion.
    }

    void Release(XRGrabInteractable item) // 通过实际XR管理器发送松手事件 / Send a release event through the actual XR manager.
    {
        hand.held=false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if(item.isSelected)manager.SelectExit((IXRSelectInteractor)hand,(IXRSelectInteractable)item); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
        hand.target=null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    }

    void Place(XRGrabInteractable item,Vector3 center) // 按物品边界中心设置测试位置 / Position a test item using its bounds center.
    {
        var body=item.GetComponent<Rigidbody>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        item.transform.position+=center-DeskAdventure.ItemBounds(item).center; // 合并物品有效碰撞体的世界边界 / Combine world bounds of the active item colliders.
        body.position=item.transform.position; // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        if(!body.isKinematic){body.linearVelocity=Vector3.zero;body.angularVelocity=Vector3.zero;} // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        Physics.SyncTransforms(); // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
    }

    IEnumerator Run() // 验证简化任务的真实播放模式流程 / Verify the simplified objective in actual Play Mode.
    {
        yield return new WaitForSeconds(1f); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        var first = FindFirstObjectByType<Task1Manager>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        var entryFlow = FindFirstObjectByType<UIFlowManager>(); // 取得介绍、学习和准备流程组件 / Resolve the introduction, learning and ready flow.
        Check(entryFlow.introPanel.activeSelf && !first.task1Panel.activeSelf && !entryFlow.learningPanel.activeSelf && !entryFlow.readyPanel.activeSelf, "Room starts with introduction, not Task1"); // 验证入房时先显示介绍 / Verify that room entry shows the introduction first.
        entryFlow.StartLearning();
        Check(GameObject.Find("Controller Instructions") != null && !entryFlow.learningPanel.activeSelf, "Controller instructions precede vocabulary"); // 验证说明页先于词卡 / Verify controls precede vocabulary.
        GameObject.Find("Controller Instructions").GetComponentInChildren<UnityEngine.UI.Button>().onClick.Invoke(); // 从介绍进入词语学习 / Move from the introduction to vocabulary learning.
        Check(entryFlow.learningPanel.activeSelf && !first.task1Panel.activeSelf, "Vocabulary learning precedes Task1"); // 验证学习阶段不会提前开始测试 / Verify learning does not start the test early.
        var cards = entryFlow.learningPanel.GetComponent<LearningCardManager>(); // 取得六张词语卡的管理器 / Resolve the six-card vocabulary manager.
        for (int index = 0; index < 5; index++) cards.NextCard(); // 浏览至第六张词语卡 / Advance to the sixth vocabulary card.
        Check(entryFlow.learningPanel.activeSelf && cards.progressText.text.Contains("6 / 6"), "All six vocabulary cards remain available"); // 验证第六张仍在学习页面 / Verify the sixth card remains in the learning page.
        cards.NextCard(); // 学完六个词后进入准备界面 / Enter the ready screen after all six words.
        Check(entryFlow.readyPanel.activeSelf && !entryFlow.learningPanel.activeSelf && !first.task1Panel.activeSelf, "Ready screen follows vocabulary learning"); // 验证准备界面出现在学习之后 / Verify the ready screen follows learning.
        foreach (var text in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None)) // 检查所有场景物体的词卡显示状态 / Inspect scene objects for word-card visibility.
            if (text.name == "WordHintPanel") Check(!text.gameObject.activeSelf, "Object word-card panel stays hidden"); // 验证物品词卡不会显示 / Verify object word cards remain hidden.
        var second = FindFirstObjectByType<Task2Manager>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        FindFirstObjectByType<UIFlowManager>().StartMission(); // 关闭引导并显示开始任务按钮 / Close onboarding and show the task-start button.
        first.StartTask1(); // 显示并激活寻找电脑任务 / Display and activate the find-computer task.
        GameObject computer = null, red = null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        XRGrabInteractable green = null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        foreach (var word in FindObjectsByType<LookWord>(FindObjectsSortMode.None)) // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
        {
            if (word.english == "computer") computer = word.gameObject; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            if (word.english == "book") red = word.gameObject; // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
            if (word.english == "green book") green = word.GetComponent<XRGrabInteractable>(); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        }
        Check(computer != null && red != null && green != null, "Computer and both books are connected"); // 记录通过项或在断言失败时终止测试 / Record a pass or fail the test on a false assertion.
        first.CheckObject(red); // 根据当前任务检查玩家选择的物品 / Check the selected object against the active objective.
        Check(first.feedbackText.text.Contains("Try again"), "Task1 rejects wrong object"); // 成功或错误反馈文字引用 / Success or retry feedback text reference.
        first.CheckObject(computer); // 根据当前任务检查玩家选择的物品 / Check the selected object against the active objective.
        yield return new WaitForSeconds(3f); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        adventure = FindFirstObjectByType<DeskAdventure>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        Check(adventure != null && adventure.CurrentStage == DeskAdventure.Stage.Chase, "Task1 transitions to escaping red book"); // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
        Check(adventure.MissionText != null && adventure.MissionText.font != null, "Task2 HUD has a valid font");
        Check(adventure.MissionText.transform.parent.GetComponent<UnityEngine.UI.Image>().color.a == 1f && adventure.MissionText.GetComponentInParent<FrontPanel>() != null, "Task2 card is opaque and uses foreground rendering"); // 验证不透明前景任务卡 / Verify opaque foreground instructions.
        Check(Resources.Load<Shader>("DeskUI/PanelOverlay") != null && Resources.Load<Shader>("DeskUI/TextOverlay") != null, "Foreground shaders included in Resources"); // 验证头显构建包含所需着色器 / Verify shader inclusion for headset builds. // 记录通过项或在断言失败时终止测试 / Record a pass or fail the test on a false assertion.
        Check(second.instructionText.text.Contains("红书"), "Original Task2 panel shows the current objective"); // 原任务面板中的当前中文指令 / Current Chinese objective in the original task panel.
        foreach (char c in adventure.Mission()) // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
            if (c > 127 && !char.IsWhiteSpace(c)) Check(adventure.MissionText.font.HasCharacter(c, true, true), "HUD glyph " + c); // 检查条件，仅在满足时执行后续操作 / Execute the following action only when its condition holds.
        Vector3 point = Camera.main.WorldToViewportPoint(adventure.MissionText.transform.parent.position); // 设置物体的位置、旋转或尺寸 / Set the object position, rotation or size.
        Check(point.z > 0 && point.x > 0 && point.x < 1 && point.y > 0 && point.y < 1, "HUD remains in camera view"); // 玩家当前使用的主摄像机 / Main camera currently used by the player.
        foreach (var t in GameObject.Find("Playful Desk Adventure").GetComponentsInChildren<Transform>(true)) // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
            Check(t.name != "Key" && t.name != "Drawer" && t.name != "Cabinet" && t.name != "Pad", "No puzzle object: " + t.name); // 记录通过项或在断言失败时终止测试 / Record a pass or fail the test on a false assertion.
        hand = new GameObject("Verification XR hand").AddComponent<DeskVerificationInteractor>(); // 创建所需的场景对象或组件 / Create the required scene object or component.
        manager = FindFirstObjectByType<XRInteractionManager>(); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        Place(green, adventure.BagVolume.bounds.center); // 按物品边界中心设置测试位置 / Position a test item using its bounds center.
        yield return new WaitForSeconds(.6f); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        Check(adventure.CurrentStage == DeskAdventure.Stage.Chase, "Green book cannot complete Task2"); // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
        FindFirstObjectByType<DeskStationery>().ResetItems(); // 把文具逐个复位到初始位置 / Restore each stationery item to its starting pose.
        var book = adventure.TargetBook; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        Select(book); // 通过实际XR管理器发送抓取事件 / Send a grab event through the actual XR manager.
        Check(adventure.CurrentStage == DeskAdventure.Stage.Pack, "Catching red book goes directly to packing without puzzle steps"); // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
        hand.transform.position += adventure.BagVolume.bounds.center - DeskAdventure.ItemBounds(book).center; // 合并物品有效碰撞体的世界边界 / Combine world bounds of the active item colliders.
        yield return new WaitForSeconds(.6f); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        Check(book.isSelected && DeskAdventure.InsideBag(book, adventure.BagVolume), "Held red book reaches bag"); // 记录通过项或在断言失败时终止测试 / Record a pass or fail the test on a false assertion.
        Check(adventure.CurrentStage == DeskAdventure.Stage.Pack, "Must release the book before completion"); // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
        Release(book); // 通过实际XR管理器发送松手事件 / Send a release event through the actual XR manager.
        Place(book, adventure.BagVolume.bounds.center + Vector3.right * .8f); // 按物品边界中心设置测试位置 / Position a test item using its bounds center.
        Check(!DeskAdventure.InsideBag(book, adventure.BagVolume), "Book outside bag fails the interior check"); // 记录通过项或在断言失败时终止测试 / Record a pass or fail the test on a false assertion.
        Place(book, adventure.BagVolume.bounds.center); // 按物品边界中心设置测试位置 / Position a test item using its bounds center.
        yield return new WaitForSeconds(1f); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        Check(adventure.CurrentStage == DeskAdventure.Stage.Won, "Released red book inside bag wins directly");
        Check(adventure.MissionText.text.Contains("Mission complete!") && ((RectTransform)adventure.MissionText.transform.parent).sizeDelta.y == 600 && !second.task2Panel.activeSelf, "Persistent completion screen replaces task panel");
        var exit = adventure.MissionText.transform.parent.Find("Exit Button"); // 查找退出按钮 / Find the exit button.
        Check(exit != null && exit.GetComponent<UnityEngine.UI.Button>().interactable && exit.GetComponentInChildren<TMP_Text>().text.Contains("Exit"), "Result screen provides an interactive Exit button"); // 验证可交互的退出入口 / Verify the interactive exit action. // 验证完成画面持续显示 / Verify the persistent completion screen. // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
        second.RestartAdventure(); // 先复位再重新启动任务二 / Reset and restart Task2.
        yield return null; // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        int copies = 0; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        foreach (var t in FindObjectsByType<Transform>(FindObjectsSortMode.None)) if (t.name == "Playful Desk Adventure") copies++; // 逐一处理集合中的物品或组件 / Process each item or component in the collection.
        Check(copies == 1, "Restart leaves one adventure copy"); // 记录通过项或在断言失败时终止测试 / Record a pass or fail the test on a false assertion.
        Select(adventure.TargetBook); // 通过实际XR管理器发送抓取事件 / Send a grab event through the actual XR manager.
        Check(adventure.CurrentStage == DeskAdventure.Stage.Pack, "Grabbing works after win and restart"); // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
        Release(adventure.TargetBook); // 通过实际XR管理器发送松手事件 / Send a release event through the actual XR manager.
        adventure.roundSeconds = .2f; // 每局时间上限，单位秒 / Round time limit in seconds.
        second.RestartAdventure(); // 先复位再重新启动任务二 / Reset and restart Task2.
        yield return new WaitForSeconds(.4f); // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        Check(adventure.CurrentStage == DeskAdventure.Stage.TimedOut, "Timeout works"); // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
        adventure.roundSeconds = 180f; // 每局时间上限，单位秒 / Round time limit in seconds.
        second.RestartAdventure(); // 先复位再重新启动任务二 / Reset and restart Task2.
        yield return null; // 等待指定时间或下一帧后继续 / Resume after the requested wait or next frame.
        Check(adventure.CurrentStage == DeskAdventure.Stage.Chase, "Restart works after timeout"); // 当前回合阶段，只允许组件内部修改 / Current round stage, writable only inside this component.
    }

}

public class DeskVerificationInteractor : XRBaseInteractor // 声明 DeskFlowVerification 组件或辅助类型 / Declare the DeskFlowVerification component or helper type.
{
    public XRGrabInteractable target; // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
    public bool held; // 执行 DeskFlowVerification 的当前操作 / Perform the current DeskFlowVerification operation.
    public override bool isSelectActive=>held; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    public override bool isHoverActive=>false; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
    public override void GetValidTargets(List<IXRInteractable> targets) // 向测试交互器提供有效目标 / Provide valid targets to the test interactor.
    {
        targets.Clear();if(target!=null)targets.Add(target); // 停止提示动画与音效并隐藏反馈 / Stop feedback animation and audio and hide the panel.
    }
}
#endif // 限定此段代码的编译条件 / Set the compilation condition for this code.
