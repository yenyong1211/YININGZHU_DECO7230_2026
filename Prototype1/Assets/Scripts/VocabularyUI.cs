using UnityEngine;
using TMPro;

public class VocabularyUI : MonoBehaviour
{
    public GameObject vocabularyPanel;

    public GameObject frontPanel;
    public GameObject backPanel;

    public TextMeshProUGUI chineseText;
    public TextMeshProUGUI pinyinText;
    public TextMeshProUGUI englishText;

    public AudioSource audioSource;

    private bool pinyinVisible = false;

    void Start()
    {
        vocabularyPanel.SetActive(false);

        frontPanel.SetActive(true);
        backPanel.SetActive(false);

        pinyinText.gameObject.SetActive(false);
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

        audioSource.clip = pronunciation;

        frontPanel.SetActive(true);
        backPanel.SetActive(false);

        pinyinVisible = false;
        pinyinText.gameObject.SetActive(false);
    }

    public void TogglePinyin()
    {
        pinyinVisible = !pinyinVisible;

        pinyinText.gameObject.SetActive(pinyinVisible);
    }

    public void FlipToBack()
    {
        frontPanel.SetActive(false);
        backPanel.SetActive(true);
    }

    public void FlipToFront()
    {
        backPanel.SetActive(false);
        frontPanel.SetActive(true);
    }

    public void PlayPronunciation()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is missing!");
            return;
        }

        if (audioSource.clip == null)
        {
            Debug.LogError("No pronunciation audio!");
            return;
        }

        audioSource.Stop();
        audioSource.Play();
    }
}