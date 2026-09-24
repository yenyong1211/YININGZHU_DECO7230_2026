using UnityEngine;
using TMPro;

public class ObjectWordCard : MonoBehaviour
{
    public GameObject wordHintPanel;
    public TMP_Text chineseText;
    public TMP_Text pinyinText;

    public Camera playerCamera;

    public float heightOffset = 0.25f;
    public float forwardOffset = 0.05f;

    public void ShowWordCard(GameObject selectedObject)
    {
        LookWord word = selectedObject.GetComponent<LookWord>();

        if (word == null)
        {
            return;
        }

        chineseText.text = word.chinese;
        pinyinText.text = word.pinyin;

        wordHintPanel.SetActive(true);

        Collider col = selectedObject.GetComponent<Collider>();

        Vector3 objectTop;

        if (col != null)
        {
            objectTop = new Vector3(
                col.bounds.center.x,
                col.bounds.max.y,
                col.bounds.center.z
            );
        }
        else
        {
            objectTop = selectedObject.transform.position;
        }

        // Move the card above the object
        Vector3 cardPosition = objectTop + Vector3.up * heightOffset;

        // Move the card slightly toward the player
        Vector3 directionToPlayer =
            (playerCamera.transform.position - cardPosition).normalized;

        cardPosition += directionToPlayer * forwardOffset;

        wordHintPanel.transform.position = cardPosition;

        // Make the card face the player's eyes
        Vector3 lookDirection =
            wordHintPanel.transform.position - playerCamera.transform.position;

        wordHintPanel.transform.rotation =
            Quaternion.LookRotation(lookDirection);
    }

    public void HideWordCard()
    {
        wordHintPanel.SetActive(false);
    }
}