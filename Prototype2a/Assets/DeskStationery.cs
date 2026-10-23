using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[ExecuteAlways]
[DefaultExecutionOrder(-100)]
public class DeskStationery : MonoBehaviour
{
    private readonly List<XRGrabInteractable> items = new List<XRGrabInteractable>();
    private readonly List<Vector3> homes = new List<Vector3>();
    private readonly List<Quaternion> rotations = new List<Quaternion>();
    private LookWord original;
    private float surfaceY;
    private Bounds desktop;
    private bool hasDesktop;
    private bool runtimeReady;

    void OnEnable() { InitializeStationery(); }
    void Start() { InitializeStationery(); }

    void InitializeStationery() // Place books on the desk and configure grabbable stationery once.
    {
        if (Application.isPlaying && runtimeReady) return;
        if (!gameObject.scene.IsValid() || !gameObject.scene.isLoaded) return;
        foreach (var word in FindObjectsByType<LookWord>(FindObjectsSortMode.None))
            if (word.english == "book") { original = word; break; }
        var green = transform.Find("GreenBook");
        var pen = transform.Find("Pen");
        var pencilCase = transform.Find("PencilCase");
        if (green == null || pen == null || pencilCase == null) return;
        surfaceY = .77f;
        foreach (var word in FindObjectsByType<LookWord>(FindObjectsSortMode.None))
        {
            if (word.english != "table" && word.chinese != "桌子") continue; // Find the tabletop by vocabulary data.
            var col = word.GetComponent<Collider>();
            if (col != null)
            {
                desktop = col.bounds;
                hasDesktop = true;
                surfaceY = desktop.max.y; // Use the collider top as the tabletop height.
            }
            break;
        }
        PlaceOnDesk(green, .30f, .3f);
        PlaceOnDesk(pen, .45f, .10f);
        PlaceOnDesk(pencilCase, .70f, .7f);
        if (original != null) PlaceOnDesk(original.transform, .18f, .25f);
        if (!Application.isPlaying) return;
        items.Clear(); homes.Clear(); rotations.Clear();
        Setup(green.gameObject,new Vector3(.174f,.038f,.234f),"绿书","lǜ shū","green book", false);
        Setup(pen.gameObject,new Vector3(.18f,.026f,.03f),"笔","bǐ","pen", false);
        Setup(pencilCase.gameObject,new Vector3(.26f,.062f,.12f),"笔袋","bǐ dài","pencil case", true);
        runtimeReady = true;
    }

    void PlaceOnDesk(Transform target, float x, float z) // Align the item bottom with the desk and avoid the keyboard.
    {
        var renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;
        Bounds bounds = renderers[0].bounds;
        foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
        Vector3 offset = Vector3.up*(surfaceY+.004f-bounds.min.y);
        if (hasDesktop)
        {
            float targetX = Mathf.Lerp(desktop.min.x,desktop.max.x,x);
            float targetZ = Mathf.Lerp(desktop.min.z,desktop.max.z,z);
            targetX = Mathf.Clamp(targetX,desktop.min.x+bounds.extents.x+.015f,desktop.max.x-bounds.extents.x-.015f);
            targetZ = Mathf.Clamp(targetZ,desktop.min.z+bounds.extents.z+.015f,desktop.max.z-bounds.extents.z-.015f);
            foreach (var obstacle in FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
            {
                if (obstacle.name.IndexOf("keyboard", System.StringComparison.OrdinalIgnoreCase) < 0) continue;
                Bounds occupied = obstacle.bounds;
                bool overlaps = Mathf.Abs(targetX - occupied.center.x) < bounds.extents.x + occupied.extents.x + .025f && Mathf.Abs(targetZ - occupied.center.z) < bounds.extents.z + occupied.extents.z + .025f;
                if (!overlaps) continue;
                float left = occupied.min.x - bounds.extents.x - .025f;
                float right = occupied.max.x + bounds.extents.x + .025f;
                if (left >= desktop.min.x + bounds.extents.x + .015f) targetX = left;
                else if (right <= desktop.max.x - bounds.extents.x - .015f) targetX = right;
            }
            offset.x=targetX-bounds.center.x; offset.z=targetZ-bounds.center.z;
        }
        target.position += offset;
    }

    void Setup(GameObject go, Vector3 size, string chinese, string pinyin, string english, bool openContainer) // Configure vocabulary, colliders, physics and XR grabbing.
    {
        var word = go.GetComponent<LookWord>();
        if (word == null) word = go.AddComponent<LookWord>();
        word.chinese=chinese; word.pinyin=pinyin; word.english=english;
        if (openContainer)
        {
            foreach (var renderer in go.GetComponentsInChildren<MeshRenderer>())
                if (renderer.name.StartsWith("Case ") && renderer.GetComponent<BoxCollider>() == null)
                    renderer.gameObject.AddComponent<BoxCollider>();
        }
        else if(go.GetComponent<BoxCollider>()==null) go.AddComponent<BoxCollider>().size=size;
        var body=go.GetComponent<Rigidbody>();
        if(body==null) body=go.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        body.mass = openContainer ? .25f : .08f;
        body.interpolation=RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode=CollisionDetectionMode.ContinuousSpeculative;
        var grab=go.GetComponent<XRGrabInteractable>();
        if(grab==null) grab=go.AddComponent<XRGrabInteractable>();
        grab.throwOnDetach=false;
        grab.useDynamicAttach=true;
        grab.movementType=XRBaseInteractable.MovementType.Kinematic;
        grab.selectEntered.AddListener(OnSelected);
        var trigger=go.GetComponent<WordCardTrigger>();
        if(trigger==null) trigger=go.AddComponent<WordCardTrigger>();
        items.Add(grab);homes.Add(go.transform.position);rotations.Add(go.transform.rotation);
    }

    void OnSelected(SelectEnterEventArgs args)
    {
        var trigger=args.interactableObject.transform.GetComponent<WordCardTrigger>();
        if(trigger!=null) trigger.ShowCard();
    }

    public void ResetItems() // Return stationery to the saved starting poses.
    {
        for(int i=0;i<items.Count;i++) Restore(i);
    }

    void Restore(int index) // Cancel selection before restoring an item.
    {
        var item=items[index]; if(item==null) return;
        if(item.isSelected && item.interactionManager!=null)
            item.interactionManager.CancelInteractableSelection((IXRSelectInteractable)item);
        var body=item.GetComponent<Rigidbody>();
        body.position=homes[index]; body.rotation=rotations[index];
        if (!body.isKinematic) { body.linearVelocity=Vector3.zero;body.angularVelocity=Vector3.zero; }
    }

    void Update() // Recover unheld items that fall or move too far away.
    {
        if(!Application.isPlaying) return;
        for(int i=0;i<items.Count;i++)
            if(items[i]!=null && !items[i].isSelected &&
                (items[i].transform.position.y<surfaceY-.6f || Vector3.Distance(items[i].transform.position,homes[i])>2.5f)) Restore(i);
    }

    void OnDisable() // Remove grab listeners and clear saved item data.
    {
        foreach(var item in items) if(item!=null) item.selectEntered.RemoveListener(OnSelected);
        items.Clear();homes.Clear();rotations.Clear();
        runtimeReady=false;
    }
}
