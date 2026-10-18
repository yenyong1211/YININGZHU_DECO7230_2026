using UnityEngine; // 导入 UnityEngine 命名空间 / Import the UnityEngine namespace.
using UnityEngine.UI; // 导入 UnityEngine.UI 命名空间 / Import the UnityEngine.UI namespace.
using TMPro; // 导入 TMPro 命名空间 / Import the TMPro namespace.

public class LearningCardManager : MonoBehaviour // 声明 LearningCardManager 组件或辅助类型 / Declare the LearningCardManager component or helper type.
{ // 开始代码块 / Begin the block.
    public TMP_Text chineseText; // 词卡的中文文字引用 / Chinese word-card text reference.
    public TMP_Text pinyinText; // 词卡的声调拼音文字引用 / Tone-marked pinyin text reference.
    public TMP_Text englishText; // 词卡的英文释义文字引用 / English word-meaning text reference.
    public TMP_Text progressText; // 类别和学习进度文字引用 / Category and learning progress text reference.
    public UIFlowManager uiFlowManager; // 执行 LearningCardManager 的当前操作 / Perform the current LearningCardManager operation.
    public AudioSource audioSource; // 执行 LearningCardManager 的当前操作 / Perform the current LearningCardManager operation.
    // 说明：LearningCardManager 的实现说明 / Implementation note: Existing scene recordings: book, table, computer, find, pick up.
    public AudioClip[] wordAudioClips; // 原场景的五段词语录音，保留原索引 / Five original word recordings with their original indices.

    private sealed class Word // 声明 LearningCardManager 组件或辅助类型 / Declare the LearningCardManager component or helper type.
    { // 开始代码块 / Begin the block.
        public readonly string Chinese, Pinyin, English, Category; // 执行 LearningCardManager 的当前操作 / Perform the current LearningCardManager operation.
        public readonly int AudioIndex; // 执行 LearningCardManager 的当前操作 / Perform the current LearningCardManager operation.
        public Word(string chinese, string pinyin, string english, string category, int audioIndex = -1) // 将词语、拼音、释义、类别与音频索引保存在一起 / Keep word, pinyin, meaning, category and audio index together.
        { // 开始代码块 / Begin the block.
            Chinese=chinese; Pinyin=pinyin; English=english; Category=category; AudioIndex=audioIndex; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        } // 结束代码块 / End the block.
    } // 结束代码块 / End the block.

    private static readonly Word[] words = // 六张核心物品与动作词语卡片 / Six core object and action vocabulary cards.
    { // 开始代码块 / Begin the block.
        new Word("书", "shū", "book", "物品", 0), // 学习词语“书”：拼音与英文释义对应 / Vocabulary 书: shū / book.
        new Word("电脑", "diàn nǎo", "computer", "物品", 2), // 学习词语“电脑”：拼音与英文释义对应 / Vocabulary 电脑: diàn nǎo / computer.
        new Word("书包", "shū bāo", "school bag", "物品"), // 学习词语“书包”：拼音与英文释义对应 / Vocabulary 书包: shū bāo / school bag.
        new Word("找到", "zhǎo dào", "find", "动作", 3), // 学习词语“找到”：拼音与英文释义对应 / Vocabulary 找到: zhǎo dào / find.
        new Word("拿起", "ná qǐ", "pick up", "动作", 4), // 学习词语“拿起”：拼音与英文释义对应 / Vocabulary 拿起: ná qǐ / pick up.
        new Word("放进", "fàng jìn", "put into", "动作"), // 学习词语“放进”：拼音与英文释义对应 / Vocabulary 放进: fàng jìn / put into.
    }; // 结束代码块 / End the block.

    private int currentIndex; // 当前显示的词语索引 / Index of the currently displayed word.
    private Button audioButton, previousButton, nextButton; // 执行 LearningCardManager 的当前操作 / Perform the current LearningCardManager operation.

    void Start() { ShowCard(); } // 在场景加载完成后执行初始化 / Initialize after the scene has loaded.

    public void ResetCards() // 返回第一张词语卡并刷新内容 / Return to the first vocabulary card and refresh it.
    { // 开始代码块 / Begin the block.
        currentIndex=0; // 当前显示的词语索引 / Index of the currently displayed word.
        ShowCard(); // 显示当前词卡并更新按钮与进度 / Display the current word card and update controls and progress.
    } // 结束代码块 / End the block.

    void OnDisable() { if(audioSource!=null) audioSource.Stop(); } // 停用时停止播放或解除事件监听 / Stop playback or remove listeners when disabled.

    public void NextCard() // 翻到下一词或进入准备界面 / Advance to the next word or the ready screen.
    { // 开始代码块 / Begin the block.
        if(currentIndex < words.Length-1) { currentIndex++; ShowCard(); } // 六张核心物品与动作词语卡片 / Six core object and action vocabulary cards.
        else // 处理前一条件不满足的分支 / Handle the branch where the previous condition is false.
        { // 开始代码块 / Begin the block.
            if(audioSource!=null) audioSource.Stop(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
            if(uiFlowManager!=null) uiFlowManager.ShowReady(); // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        } // 结束代码块 / End the block.
    } // 结束代码块 / End the block.

    public void PreviousCard() // 返回上一张词语卡 / Return to the previous vocabulary card.
    { // 开始代码块 / Begin the block.
        if(currentIndex>0) {currentIndex--;ShowCard();} // 当前显示的词语索引 / Index of the currently displayed word.
    } // 结束代码块 / End the block.

    Button FindButton(string name) // 查找学习面板中的指定按钮 / Find a named button in the learning panel.
    { // 开始代码块 / Begin the block.
        var child=transform.Find(name); // 取得所需的场景组件引用 / Resolve the required scene component reference.
        return child!=null ? child.GetComponent<Button>() : null; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
    } // 结束代码块 / End the block.

    AudioClip CurrentAudio() // 通过原录音索引取得当前词语发音 / Resolve current word audio using its original recording index.
    { // 开始代码块 / Begin the block.
        int index=words[currentIndex].AudioIndex; // 六张核心物品与动作词语卡片 / Six core object and action vocabulary cards.
        return wordAudioClips!=null && index>=0 && index<wordAudioClips.Length ? wordAudioClips[index] : null; // 返回结果或提前结束当前操作 / Return the result or exit this operation early.
    } // 结束代码块 / End the block.

    void ShowCard() // 显示当前词卡并更新按钮与进度 / Display the current word card and update controls and progress.
    { // 开始代码块 / Begin the block.
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
        { // 开始代码块 / Begin the block.
            var label=nextButton.GetComponentInChildren<TMP_Text>(true); // 取得所需的场景组件引用 / Resolve the required scene component reference.
            if(label!=null) label.text=currentIndex==words.Length-1 ? "Ready" : "Next"; // 六张核心物品与动作词语卡片 / Six core object and action vocabulary cards.
        } // 结束代码块 / End the block.
    } // 结束代码块 / End the block.

    public void PlayWordAudio() // 播放当前词语对应的已有录音 / Play the available recording for the current word.
    { // 开始代码块 / Begin the block.
        var clip=CurrentAudio(); // 通过原录音索引取得当前词语发音 / Resolve current word audio using its original recording index.
        if(audioSource==null || clip==null) return; // 保存或更新当前操作所需的数据 / Store or update data needed by the current operation.
        audioSource.Stop();audioSource.PlayOneShot(clip); // 执行 LearningCardManager 的当前操作 / Perform the current LearningCardManager operation.
    } // 结束代码块 / End the block.
} // 结束代码块 / End the block.
