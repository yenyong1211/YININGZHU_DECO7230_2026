using UnityEngine;

public class LearningObject : MonoBehaviour
{
    public string chinese;
    public string pinyin;
    public string english;
    public AudioClip pronunciation;

    public void OpenLearningCard()
    {
        VocabularyUI ui = FindFirstObjectByType<VocabularyUI>();

        if (ui == null)
        {
            Debug.LogError("VocabularyUI not found.");
            return;
        }

        Debug.Log(
            "OPEN: " + gameObject.name +
            " | audio = " +
            (pronunciation != null ? pronunciation.name : "NULL")
        );

        ui.ShowWord(
            chinese,
            pinyin,
            english,
            pronunciation
        );
    }
}