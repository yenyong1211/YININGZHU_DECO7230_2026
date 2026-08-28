using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ObjectClickRaycast : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 100f;

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

            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("UI CLICK - ignore scene raycast");
                return;
            }

            CheckClick();
        }
    }

    void CheckClick()
    {
        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Ray ray =
            playerCamera.ScreenPointToRay(mousePosition);

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
                learningObject.OpenLearningCard();
            }
        }
    }
}