using UnityEngine;
using TMPro;

public class GazeWordDetector : MonoBehaviour
{
    public Camera playerCamera;

    public GameObject wordHintPanel;

    public TMP_Text chineseText;
    public TMP_Text pinyinText;
    public TMP_Text englishText;

    public float lookDistance = 10f;

    public Vector3 offset = new Vector3(0.25f, 0.3f, 0f);

    void Start()
    {
        wordHintPanel.SetActive(false);
    }

    void Update()
    {
        CheckLookObject();
    }

    void CheckLookObject()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lookDistance))
        {
            LookWord word = hit.collider.GetComponent<LookWord>();

            if (word != null)
            {
                chineseText.text = word.chinese;
                pinyinText.text = word.pinyin;
                englishText.text = word.english;

                wordHintPanel.SetActive(true);

                // Place the word card next to the object
                wordHintPanel.transform.position =
                    hit.collider.bounds.center + offset;

                // Make the word card face the player
                wordHintPanel.transform.LookAt(playerCamera.transform);

                // Flip the panel so the front side faces the camera
                wordHintPanel.transform.Rotate(0, 180, 0);

                return;
            }
        }

        // Hide the card when the player is not looking at a learning object
        wordHintPanel.SetActive(false);
    }
}