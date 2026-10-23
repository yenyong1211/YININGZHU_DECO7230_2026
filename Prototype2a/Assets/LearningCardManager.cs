using UnityEngine;
using UnityEngine.UI;
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

    private sealed class Word
    {
        public readonly string Chinese, Pinyin, English, Category;
        public readonly int AudioIndex;
        public Word(string chinese, string pinyin, string english, string category, int audioIndex = -1)
        {
            Chinese=chinese; Pinyin=pinyin; English=english; Category=category; AudioIndex=audioIndex;
        }
    }

    private static readonly Word[] words = // Chinese, pinyin, meaning, category and recording index; audio starts at zero.
    {
        new Word("书", "shū", "book", "物品", 0),
        new Word("电脑", "diàn nǎo", "computer", "物品", 2),
        new Word("书包", "shū bāo", "school bag", "物品", 5),
        new Word("找到", "zhǎo dào", "find", "动作", 3),
        new Word("拿起", "ná qǐ", "pick up", "动作", 4),
        new Word("放进", "fàng jìn", "put into", "动作", 6),
    };

    private int currentIndex;
    private Button audioButton, previousButton, nextButton;

    void Start() { ShowCard(); }

    public void ResetCards() // Return to the first vocabulary card.
    {
        currentIndex=0;
        ShowCard();
    }

    void OnDisable() { if(audioSource!=null) audioSource.Stop(); }

    public void NextCard() // Advance to the next word, or open the ready screen.
    {
        if(currentIndex < words.Length-1) { currentIndex++; ShowCard(); }
        else
        {
            if(audioSource!=null) audioSource.Stop();
            if(uiFlowManager!=null) uiFlowManager.ShowReady();
        }
    }

    public void PreviousCard()
    {
        if(currentIndex>0) {currentIndex--;ShowCard();}
    }

    Button FindButton(string name)
    {
        var child=transform.Find(name);
        return child!=null ? child.GetComponent<Button>() : null;
    }

    AudioClip CurrentAudio() // Match the current word to its recording index.
    {
        int index=words[currentIndex].AudioIndex;
        return wordAudioClips!=null && index>=0 && index<wordAudioClips.Length ? wordAudioClips[index] : null;
    }

    void ShowCard() // Update the existing text fields and navigation controls.
    {
        if(audioSource!=null) audioSource.Stop();
        var word=words[currentIndex];
        if(chineseText!=null) chineseText.text=word.Chinese;
        if(pinyinText!=null) pinyinText.text=word.Pinyin;
        if(englishText!=null) englishText.text=word.English;
        if(progressText!=null) progressText.text=word.Category+" · "+(currentIndex+1)+" / "+words.Length;
        if(audioButton==null) audioButton=FindButton("AudioButton");
        if(previousButton==null) previousButton=FindButton("PreviousButton");
        if(nextButton==null) nextButton=FindButton("NextButton");
        if(audioButton!=null) audioButton.interactable=audioSource!=null && CurrentAudio()!=null;
        if(previousButton!=null) previousButton.interactable=currentIndex>0;
        if(nextButton!=null)
        {
            var label=nextButton.GetComponentInChildren<TMP_Text>(true);
            if(label!=null) label.text=currentIndex==words.Length-1 ? "Ready" : "Next";
        }
    }

    public void PlayWordAudio() // Stop the previous recording before playing this word.
    {
        var clip=CurrentAudio();
        if(audioSource==null || clip==null) return;
        audioSource.Stop();audioSource.PlayOneShot(clip);
    }
}
