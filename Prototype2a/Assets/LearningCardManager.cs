using UnityEngine;
using TMPro;

public class LearningCardManager : MonoBehaviour
{
    public TMP_Text chineseText;
    public TMP_Text pinyinText;
    public TMP_Text englishText;
    public TMP_Text progressText;

    public UIFlowManager uiFlowManager;

    public AudioSource audioSource;
    public AudioClip[] wordAudioClips;

    private int currentIndex = 0;

    private string[] chineseWords =
    {
        "书",
        "桌子",
        "电脑",
        "找到",
        "拿起"
    };

    private string[] pinyinWords =
    {
        "shū",
        "zhuō zi",
        "diàn nǎo",
        "zhǎo dào",
        "ná qǐ"
    };

    private string[] englishWords =
    {
        "book",
        "table",
        "computer",
        "find",
        "pick up"
    };

    void Start()
    {
        ShowCard();
    }

    public void NextCard()
    {
        if (currentIndex < chineseWords.Length - 1)
        {
            currentIndex++;
            ShowCard();
        }
        else
        {
            uiFlowManager.ShowReady();
        }
    }

    public void PreviousCard()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowCard();
        }
    }

    void ShowCard()
    {
        chineseText.text = chineseWords[currentIndex];
        pinyinText.text = pinyinWords[currentIndex];
        englishText.text = englishWords[currentIndex];
        progressText.text =
            (currentIndex + 1) + " / " + chineseWords.Length;
    }

    public void PlayWordAudio()
    {
        Debug.Log("Play audio: " + currentIndex);
        
        if (audioSource != null &&
            wordAudioClips != null &&
            currentIndex < wordAudioClips.Length &&
            wordAudioClips[currentIndex] != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(wordAudioClips[currentIndex]);
        }
    }
}