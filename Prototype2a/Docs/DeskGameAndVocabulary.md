# 当前场景和词语卡片

场景统一根节点为 DeskGame，内部保留模型与文具两个嵌套 Prefab。保留原有红书、绿书、笔、开口笔袋、书包、多儿和重玩按钮。已移除额外的抽屉柜、钥匙、绿色区域和书包锁装饰。原桌子模型自带的抽屉装饰不属于任务机关。

Task1：找到电脑。

Task2：抓住逃跑的红书，放进书包，然后松手。

开始游戏前的 6 个词语：书、电脑、书包、找到、拿起、放进。每张含中文、拼音和英文。保留原五段录音索引；其他词没有录音时，播放按钮禁用。

字体文件：Assets/Fonts/CharacterSets/TMP_AllCharacters.txt，包含当前文字和拼音声调。全部中文文案在 Chinese_GameText.txt。

现有中文字体为 ChineseNotoSansSC-VariableFont_wght SDF.asset，另有 Resources/ChineseRuntimeFallback.asset 提供已预填充的动态补充字符。不要恢复已删除的 ChineseVocabulary_SDF 字体引用。

详细测试结果见 Task2FlowValidation.md。

当前流程：介绍 → 六张词语卡片 → 准备界面 → Task1 → Task2 → 完成。仅取消房间物品弹出的词卡；中文、拼音和英文学习页面保留。
