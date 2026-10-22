using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using UnityEngine.UI; // 导入 UnityEngine.UI 命名空间 / Import the UnityEngine.UI namespace.
using TMPro; // 导入 TMPro 命名空间 / Import the TMPro namespace.

public class LearningCardManager : MonoBehaviour // 声明 LearningCardManager 组件或辅助类型 / Declare the LearningCardManager component or helper type.
{
    public TMP_Text chineseText; // 词卡的中文文字引用 / Chinese word-card text reference.
    public TMP_Text pinyinText; // 词卡的声调拼音文字引用 / Tone-marked pinyin text reference.
    public TMP_Text englishText; // 词卡的英文释义文字引用 / English word-meaning text reference.
    public TMP_Text progressText; // 类别和学习进度文字引用 / Category and learning progress text reference.
    public UIFlowManager uiFlowManager; // 联系页面管理器，学完后显示准备页 / Access the screen manager to show the ready screen.
    public AudioSource audioSource; // 播放词语发音的播放器 / Audio player for vocabulary recordings.
    public AudioClip[] wordAudioClips; // 保留原五段录音，并追加书包与放进 / Keep the original five clips and append school bag and put into.

    private sealed class Word // 声明 LearningCardManager 组件或辅助类型 / Declare the LearningCardManager component or helper type.
    {
        public readonly string Chinese, Pinyin, English, Category; // 每个词语的中文、拼音、英文和类别 / Each word’s Chinese, pinyin, English and category.
        public readonly int AudioIndex; // 这个词语对应的录音编号，从零开始 / Matching recording index, starting at zero.
        public Word(string chinese, string pinyin, string english, string category, int audioIndex = -1) // 将词语、拼音、释义、类别与音频索引保存在一起 / Keep word, pinyin, meaning, category and audio index together.
        {
            Chinese=chinese; Pinyin=pinyin; English=english; Category=category; AudioIndex=audioIndex; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        }
    }

    private static readonly Word[] words = // 六张核心物品与动作词语卡片 / Six core object and action vocabulary cards.
    {
        new Word("书", "shū", "book", "物品", 0),
        new Word("电脑", "diàn nǎo", "computer", "物品", 2),
        new Word("书包", "shū bāo", "school bag", "物品", 5),
        new Word("找到", "zhǎo dào", "find", "动作", 3),
        new Word("拿起", "ná qǐ", "pick up", "动作", 4),
        new Word("放进", "fàng jìn", "put into", "动作", 6),
    };

    private int currentIndex; // 当前显示的词语索引 / Index of the currently displayed word.
    private Button audioButton, previousButton, nextButton; // 播放发音、上一张和下一张按钮 / Audio, previous-card and next-card buttons.

    void Start() { ShowCard(); } // 在场景加载完成后执行初始化 / Initialize after the scene has loaded.

    public void ResetCards() // 返回第一张词语卡并刷新内容 / Return to the first vocabulary card and refresh it.
    {
        currentIndex=0; // 当前显示的词语索引 / Index of the currently displayed word.
        ShowCard(); // 显示当前词卡并更新按钮与进度 / Display the current word card and update controls and progress.
    }

    void OnDisable() { if(audioSource!=null) audioSource.Stop(); } // 停用时停止播放或解除事件监听 / Stop playback or remove listeners when disabled.

    public void NextCard() // 翻到下一词或进入准备界面 / Advance to the next word or the ready screen.
    {
        if(currentIndex < words.Length-1) { currentIndex++; ShowCard(); } // 六张核心物品与动作词语卡片 / Six core object and action vocabulary cards.
        else
        {
            if(audioSource!=null) audioSource.Stop(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            if(uiFlowManager!=null) uiFlowManager.ShowReady(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        }
    }

    public void PreviousCard() // 返回上一张词语卡 / Return to the previous vocabulary card.
    {
        if(currentIndex>0) {currentIndex--;ShowCard();} // 当前显示的词语索引 / Index of the currently displayed word.
    }

    Button FindButton(string name) // 查找学习面板中的指定按钮 / Find a named button in the learning panel.
    {
        var child=transform.Find(name); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        return child!=null ? child.GetComponent<Button>() : null; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
    }

    AudioClip CurrentAudio() // 通过原录音索引取得当前词语发音 / Resolve current word audio using its original recording index.
    {
        int index=words[currentIndex].AudioIndex; // 六张核心物品与动作词语卡片 / Six core object and action vocabulary cards.
        return wordAudioClips!=null && index>=0 && index<wordAudioClips.Length ? wordAudioClips[index] : null; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
    }

    void ShowCard() // 显示当前词卡并更新按钮与进度 / Display the current word card and update controls and progress.
    {
        if(audioSource!=null) audioSource.Stop(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        var word=words[currentIndex]; // 六张核心物品与动作词语卡片 / Six core object and action vocabulary cards.
        if(chineseText!=null) chineseText.text=word.Chinese; // 词卡的中文文字引用 / Chinese word-card text reference.
        if(pinyinText!=null) pinyinText.text=word.Pinyin; // 词卡的声调拼音文字引用 / Tone-marked pinyin text reference.
        if(englishText!=null) englishText.text=word.English; // 词卡的英文释义文字引用 / English word-meaning text reference.
        if(progressText!=null) progressText.text=word.Category+" · "+(currentIndex+1)+" / "+words.Length; // 六张核心物品与动作词语卡片 / Six core object and action vocabulary cards.
        if(audioButton==null) audioButton=FindButton("AudioButton"); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if(previousButton==null) previousButton=FindButton("PreviousButton"); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if(nextButton==null) nextButton=FindButton("NextButton"); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        if(audioButton!=null) audioButton.interactable=audioSource!=null && CurrentAudio()!=null; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        if(previousButton!=null) previousButton.interactable=currentIndex>0; // 当前显示的词语索引 / Index of the currently displayed word.
        if(nextButton!=null) // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        {
            var label=nextButton.GetComponentInChildren<TMP_Text>(true); // 取得所需的场景组件引用 / Resolve the required scene component reference.
            if(label!=null) label.text=currentIndex==words.Length-1 ? "Ready" : "Next"; // 六张核心物品与动作词语卡片 / Six core object and action vocabulary cards.
        }
    }

    public void PlayWordAudio() // 播放当前词语对应的已有录音 / Play the available recording for the current word.
    {
        var clip=CurrentAudio(); // 通过原录音索引取得当前词语发音 / Resolve current word audio using its original recording index.
        if(audioSource==null || clip==null) return; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        audioSource.Stop();audioSource.PlayOneShot(clip); // 停止上一段录音，再播放当前词语发音 / Stop the previous recording and play the current word.
    }
}
