using UnityEngine;

public class LearningObject : MonoBehaviour
{
    public string chinese;
    public string pinyin;
    public string english;

    public AudioClip pronunciation;

    private VocabularyUI vocabularyUI;

    void Start()
    {
        vocabularyUI =
            FindFirstObjectByType<VocabularyUI>();
    }

    public void OpenLearningCard()
    {
        Debug.Log(
            "OPEN OBJECT: " + gameObject.name
        );

        if (pronunciation == null)
        {
            Debug.LogError(
                "❌ THIS OBJECT HAS NO AUDIO: "
                + gameObject.name
            );
        }
        else
        {
            Debug.Log(
                "✅ OBJECT AUDIO = "
                + pronunciation.name
            );
        }

        vocabularyUI.ShowWord(
            chinese,
            pinyin,
            english,
            pronunciation
        );
    }
}