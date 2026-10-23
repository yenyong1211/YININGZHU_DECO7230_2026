#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DeskFlowVerification : MonoBehaviour
{
    private const string Pending="DeskGame.VerifyFlow";
    private readonly List<string> results=new List<string>();
    private DeskVerificationInteractor hand;
    private XRInteractionManager manager;
    private DeskAdventure adventure;

    [InitializeOnLoadMethod]
    static void Register()
    {
        EditorApplication.playModeStateChanged-=OnPlayMode;
        EditorApplication.playModeStateChanged+=OnPlayMode;
    }

    static void OnPlayMode(PlayModeStateChange state)
    {
        if(state==PlayModeStateChange.EnteredEditMode && SessionState.GetBool("DeskGame.BatchComplete",false))
        {
            EditorApplication.Exit(SessionState.GetInt("DeskGame.BatchCode",1));
            return;
        }
        if(state!=PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(Pending,false)) return;
        SessionState.SetBool(Pending,false);
        new GameObject("Automated Desk Flow Verification").AddComponent<DeskFlowVerification>();
    }

    public static void RunBatch() // Run editor-only regression checks against the real scene.
    {
        PrepareFont();
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/FirstXRScene.unity");
        SessionState.SetBool("DeskGame.Batch",true);
        SessionState.SetBool("DeskGame.BatchComplete",false);
        SessionState.SetBool(Pending,true);
        EditorApplication.EnterPlaymode();
    }

    [MenuItem("Tools/Desk Game/Verify task flow")]
    static void Verify()
    {
        if(EditorApplication.isPlaying)
        {
            new GameObject("Automated Desk Flow Verification").AddComponent<DeskFlowVerification>();
            return;
        }
        SessionState.SetBool(Pending,true);
        EditorApplication.EnterPlaymode();
    }

    [MenuItem("Tools/Desk Game/Prepare Chinese fallback")]
    static void PrepareFont() // Prepare Chinese fallback glyphs for editor validation.
    {
        string path="Assets/Resources/ChineseRuntimeFallback.asset";
        var fallback=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
        if(fallback==null)
        {
            var source=AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/NotoSansSC-VariableFont_wght.ttf");
            fallback=TMP_FontAsset.CreateFontAsset(source,64,8,GlyphRenderMode.SDFAA,2048,2048,AtlasPopulationMode.Dynamic,true);
            fallback.name="ChineseRuntimeFallback";
            AssetDatabase.CreateAsset(fallback,path);
            AssetDatabase.AddObjectToAsset(fallback.material,fallback);
            foreach(var atlas in fallback.atlasTextures) AssetDatabase.AddObjectToAsset(atlas,fallback);
        }
        string text=File.ReadAllText("Assets/Fonts/CharacterSets/TMP_AllCharacters.txt");
        fallback.TryAddCharacters(text,out string missing);
        foreach(var atlas in fallback.atlasTextures)
            if(!AssetDatabase.Contains(atlas)) AssetDatabase.AddObjectToAsset(atlas,fallback);
        foreach(string asset in new[]{"Assets/Fonts/ChineseNotoSansSC-VariableFont_wght SDF.asset","Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset"})
        {
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(asset);
            if(font==null) continue;
            if(font.fallbackFontAssetTable==null) font.fallbackFontAssetTable=new List<TMP_FontAsset>();
            font.fallbackFontAssetTable.RemoveAll(f=>f==null);
            if(!font.fallbackFontAssetTable.Contains(fallback)) font.fallbackFontAssetTable.Add(fallback);
            EditorUtility.SetDirty(font);
        }
        EditorUtility.SetDirty(fallback);AssetDatabase.SaveAssets();
        File.WriteAllText("/tmp/Prototype2a-font-check.txt","Prepared Chinese fallback. Missing characters: "+missing);
        Debug.Log("DESK FONT PREPARED; missing: "+missing);
    }

    IEnumerator Start()
    {
        var run=Run();
        Exception failure=null;
        while(true)
        {
            bool more=false;object next=null;
            try {more=run.MoveNext();if(more) next=run.Current;}
            catch(Exception e) {failure=e;}
            if(failure!=null || !more) break;
            yield return next;
        }
        if(failure!=null)results.Add("FAIL: "+failure);
        string report=string.Join("\n",results);
        File.WriteAllText("/tmp/Prototype2a-flow-results.txt",report);
        Debug.Log("DESK FLOW "+(failure==null?"PASSED":"FAILED")+"\n"+report);
        if(hand!=null)Destroy(hand.gameObject);
        if(SessionState.GetBool("DeskGame.Batch",false))
        {
            SessionState.SetBool("DeskGame.BatchComplete",true);
            SessionState.SetInt("DeskGame.BatchCode",failure==null?0:1);
        }
        EditorApplication.delayCall+=()=>EditorApplication.ExitPlaymode();
    }

    void Check(bool pass,string name)
    {
        if(!pass)throw new Exception(name+"; stage="+(adventure!=null?adventure.CurrentStage.ToString():"not started"));
        results.Add("PASS: "+name);
    }

    void Select(XRGrabInteractable item) // Send a real XR select event using the test interactor.
    {
        hand.target=item;hand.held=true;
        hand.transform.position=item.transform.position;
        manager.SelectEnter((IXRSelectInteractor)hand,(IXRSelectInteractable)item);
        Check(item.isSelected,"XR selected "+item.name);
    }

    void Release(XRGrabInteractable item) // Send a real XR release event using the test interactor.
    {
        hand.held=false;
        if(item.isSelected)manager.SelectExit((IXRSelectInteractor)hand,(IXRSelectInteractable)item);
        hand.target=null;
    }

    void Place(XRGrabInteractable item,Vector3 center)
    {
        var body=item.GetComponent<Rigidbody>();
        item.transform.position+=center-DeskAdventure.ItemBounds(item).center;
        body.position=item.transform.position;
        if(!body.isKinematic){body.linearVelocity=Vector3.zero;body.angularVelocity=Vector3.zero;}
        Physics.SyncTransforms();
    }

    IEnumerator Run() // Check onboarding, task transitions, packing, results and restart.
    {
        yield return new WaitForSeconds(1f);
        var first = FindFirstObjectByType<Task1Manager>();
        var entryFlow = FindFirstObjectByType<UIFlowManager>();
        Check(entryFlow.introPanel.activeSelf && !first.task1Panel.activeSelf && !entryFlow.learningPanel.activeSelf && !entryFlow.readyPanel.activeSelf, "Room starts with introduction, not Task1");
        entryFlow.StartLearning();
        Check(GameObject.Find("Controller Instructions") != null && !entryFlow.learningPanel.activeSelf, "Controller instructions precede vocabulary");
        GameObject.Find("Controller Instructions").GetComponentInChildren<UnityEngine.UI.Button>().onClick.Invoke();
        Check(entryFlow.learningPanel.activeSelf && !first.task1Panel.activeSelf, "Vocabulary learning precedes Task1");
        var cards = entryFlow.learningPanel.GetComponent<LearningCardManager>();
        for (int index = 0; index < 5; index++) cards.NextCard();
        Check(entryFlow.learningPanel.activeSelf && cards.progressText.text.Contains("6 / 6"), "All six vocabulary cards remain available");
        cards.NextCard();
        Check(entryFlow.readyPanel.activeSelf && !entryFlow.learningPanel.activeSelf && !first.task1Panel.activeSelf, "Ready screen follows vocabulary learning");
        foreach (var text in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (text.name == "WordHintPanel") Check(!text.gameObject.activeSelf, "Object word-card panel stays hidden");
        var second = FindFirstObjectByType<Task2Manager>();
        FindFirstObjectByType<UIFlowManager>().StartMission();
        first.StartTask1();
        GameObject computer = null, red = null;
        XRGrabInteractable green = null;
        foreach (var word in FindObjectsByType<LookWord>(FindObjectsSortMode.None))
        {
            if (word.english == "computer") computer = word.gameObject;
            if (word.english == "book") red = word.gameObject;
            if (word.english == "green book") green = word.GetComponent<XRGrabInteractable>();
        }
        Check(computer != null && red != null && green != null, "Computer and both books are connected");
        first.CheckObject(red);
        Check(first.feedbackText.text.Contains("Try again"), "Task1 rejects wrong object");
        first.CheckObject(computer);
        yield return new WaitForSeconds(3f);
        adventure = FindFirstObjectByType<DeskAdventure>();
        Check(adventure != null && adventure.CurrentStage == DeskAdventure.Stage.Chase, "Task1 transitions to escaping red book");
        Check(adventure.MissionText != null && adventure.MissionText.font != null, "Task2 HUD has a valid font");
        Check(adventure.MissionText.transform.parent.GetComponent<UnityEngine.UI.Image>().color.a == 1f, "Task2 card has an opaque background");
        Check(Resources.Load<Shader>("DeskUI/PanelOverlay") != null && Resources.Load<Shader>("DeskUI/TextOverlay") != null, "Foreground shaders included in Resources");
        Check(second.instructionText.text.Contains("红书"), "Original Task2 panel shows the current objective");
        foreach (char c in adventure.Mission())
            if (c > 127 && !char.IsWhiteSpace(c)) Check(adventure.MissionText.font.HasCharacter(c, true, true), "HUD glyph " + c);
        Vector3 point = Camera.main.WorldToViewportPoint(adventure.MissionText.transform.parent.position);
        Check(point.z > 0 && point.x > 0 && point.x < 1 && point.y > 0 && point.y < 1, "HUD remains in camera view");
        foreach (var t in GameObject.Find("Playful Desk Adventure").GetComponentsInChildren<Transform>(true))
            Check(t.name != "Key" && t.name != "Drawer" && t.name != "Cabinet" && t.name != "Pad", "No puzzle object: " + t.name);
        hand = new GameObject("Verification XR hand").AddComponent<DeskVerificationInteractor>();
        manager = FindFirstObjectByType<XRInteractionManager>();
        Place(green, adventure.BagVolume.bounds.center);
        yield return new WaitForSeconds(.6f);
        Check(adventure.CurrentStage == DeskAdventure.Stage.Chase, "Green book cannot complete Task2");
        FindFirstObjectByType<DeskStationery>().ResetItems();
        var book = adventure.TargetBook;
        Select(book);
        Check(adventure.CurrentStage == DeskAdventure.Stage.Pack, "Catching red book goes directly to packing without puzzle steps");
        hand.transform.position += adventure.BagVolume.bounds.center - DeskAdventure.ItemBounds(book).center;
        yield return new WaitForSeconds(.6f);
        Check(book.isSelected && DeskAdventure.InsideBag(book, adventure.BagVolume), "Held red book reaches bag");
        Check(adventure.CurrentStage == DeskAdventure.Stage.Pack, "Must release the book before completion");
        Release(book);
        Place(book, adventure.BagVolume.bounds.center + Vector3.right * .8f);
        Check(!DeskAdventure.InsideBag(book, adventure.BagVolume), "Book outside bag fails the interior check");
        Place(book, adventure.BagVolume.bounds.center);
        yield return new WaitForSeconds(1f);
        Check(adventure.CurrentStage == DeskAdventure.Stage.Won, "Released red book inside bag wins directly");
        Check(adventure.MissionText.text.Contains("Mission complete!") && ((RectTransform)adventure.MissionText.transform.parent).sizeDelta.y == 600 && !second.task2Panel.activeSelf, "Persistent completion screen replaces task panel");
        var exit = adventure.MissionText.transform.parent.Find("Exit Button");
        Check(exit != null && exit.GetComponent<UnityEngine.UI.Button>().interactable && exit.GetComponentInChildren<TMP_Text>().text.Contains("Exit"), "Result screen provides an interactive Exit button");
        second.RestartAdventure();
        yield return null;
        int copies = 0;
        foreach (var t in FindObjectsByType<Transform>(FindObjectsSortMode.None)) if (t.name == "Playful Desk Adventure") copies++;
        Check(copies == 1, "Restart leaves one adventure copy");
        Select(adventure.TargetBook);
        Check(adventure.CurrentStage == DeskAdventure.Stage.Pack, "Grabbing works after win and restart");
        Release(adventure.TargetBook);
        adventure.roundSeconds = .2f;
        second.RestartAdventure();
        yield return new WaitForSeconds(.4f);
        Check(adventure.CurrentStage == DeskAdventure.Stage.TimedOut, "Timeout works");
        adventure.roundSeconds = 180f;
        second.RestartAdventure();
        yield return null;
        Check(adventure.CurrentStage == DeskAdventure.Stage.Chase, "Restart works after timeout");
    }

}

public class DeskVerificationInteractor : XRBaseInteractor
{
    public XRGrabInteractable target;
    public bool held;
    public override bool isSelectActive=>held;
    public override bool isHoverActive=>false;
    public override void GetValidTargets(List<IXRInteractable> targets)
    {
        targets.Clear();if(target!=null)targets.Add(target);
    }
}
#endif
