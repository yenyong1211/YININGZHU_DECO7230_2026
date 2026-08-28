using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectClickRaycast : MonoBehaviour
{
    public Camera playerCamera;

    public float maxDistance = 100f;
    public float doubleClickTime = 0.5f;

    private float lastClickTime = -10f;
    private LearningObject lastClickedObject;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckClick();
        }
    }

    void CheckClick()
    {
        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Ray ray =
            playerCamera.ScreenPointToRay(mousePosition);

        Debug.DrawRay(
            ray.origin,
            ray.direction * maxDistance,
            Color.red,
            1f
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            maxDistance))
        {
            Debug.Log(
                "YOU CLICKED: " +
                hit.collider.gameObject.name
            );

            LearningObject learningObject =
                hit.collider.GetComponent<LearningObject>();

            if (learningObject == null)
            {
                learningObject =
                    hit.collider.GetComponentInParent<LearningObject>();
            }

            if (learningObject != null)
            {
                float timeSinceLastClick =
                    Time.time - lastClickTime;

                bool sameObject =
                    learningObject == lastClickedObject;

                if (
                    sameObject &&
                    timeSinceLastClick <= doubleClickTime
                )
                {
                    Debug.Log(
                        "DOUBLE CLICK SUCCESS!"
                    );

                    learningObject.OpenLearningCard();

                    lastClickTime = -10f;
                    lastClickedObject = null;
                }
                else
                {
                    Debug.Log(
                        "FIRST CLICK: " +
                        learningObject.english
                    );

                    lastClickTime = Time.time;
                    lastClickedObject = learningObject;
                }
            }
            else
            {
                Debug.Log(
                    "This object has no LearningObject script."
                );
            }
        }
        else
        {
            Debug.Log("CLICK HIT NOTHING");
        }
    }
}