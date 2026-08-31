using UnityEngine;
using TMPro;

public class VocabularyUI : MonoBehaviour
{
    public GameObject vocabularyPanel;

    public TextMeshProUGUI chineseText;
    public TextMeshProUGUI pinyinText;
    public TextMeshProUGUI englishText;

    public AudioSource audioSource;

    void Start()
    {
        vocabularyPanel.SetActive(false);
    }

    public void ShowWord(
        string chinese,
        string pinyin,
        string english,
        AudioClip pronunciation)
    {
        vocabularyPanel.SetActive(true);

        chineseText.text = chinese;
        pinyinText.text = pinyin;
        englishText.text = english;


        if (pronunciation != null)
        {
            audioSource.Stop();
            audioSource.clip = pronunciation;

            Debug.Log(
                "AUDIO CHANGED TO: " + pronunciation.name
            );
        }
        else
        {
            audioSource.clip = null;

            Debug.LogError(
                "NO AUDIO FOR: " + english
            );
        }
    }

    public void PlayPronunciation()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is NULL!");
            return;
        }

        if (audioSource.clip == null)
        {
            Debug.LogError("AudioSource clip is NULL!");
            return;
        }

        Debug.Log(
            "PLAYING: " + audioSource.clip.name
        );

        audioSource.Stop();
        audioSource.Play();
    }
}