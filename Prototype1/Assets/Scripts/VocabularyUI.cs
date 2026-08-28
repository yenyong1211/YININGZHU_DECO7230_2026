using UnityEngine;
using TMPro;

public class VocabularyUI : MonoBehaviour
{
    public GameObject vocabularyPanel;

    public TextMeshProUGUI chineseText;
    public TextMeshProUGUI pinyinText;
    public TextMeshProUGUI englishText;

    public AudioSource audioSource;

    private AudioClip currentPronunciation;

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

        currentPronunciation = pronunciation;

        if (currentPronunciation == null)
        {
            Debug.LogError(
                "ShowWord received NO AUDIO for: " + english
            );
        }
        else
        {
            Debug.Log(
                "ShowWord received audio: " +
                currentPronunciation.name
            );
        }
    }

    public void PlayPronunciation()
    {
        Debug.Log("LISTEN BUTTON CLICKED");

        if (currentPronunciation == null)
        {
            Debug.LogError(
                "Pronunciation AudioClip is NULL!"
            );
            return;
        }

        if (audioSource == null)
        {
            Debug.LogError(
                "AudioSource is NULL!"
            );
            return;
        }

        Debug.Log(
            "PLAYING: " + currentPronunciation.name
        );

        audioSource.PlayOneShot(currentPronunciation);
    }
}