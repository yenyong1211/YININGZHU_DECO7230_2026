# Task2 简化流程与验证

当前唯一目标：**抓住逃跑的红书，放进书包，然后松手。**

书包始终可用，不需要抽屉、钥匙、绿色区域或解锁步骤。只保留 Idle、Chase、Pack、Won、TimedOut 状态。原来的场景书本继续作为红书目标，绿书为干扰物。倒计时、音效、星级、重玩、掉落找回和视线前方的任务提示继续保留。

清理内容：移除新增抽屉柜、钥匙、绿色目标区、书包锁装饰和相关 Prefab 覆盖；删除未使用的 GazeWordDetector 脚本；清除 Task2 中旧的备用成功分支和未使用的音频字段。学习卡片删去“钥匙、抽屉、打开”，现有 13 张词语卡。

所有 11 个自定义 C# 游戏/编辑器验证脚本的非空代码行均有中英文注释；未修改 Unity/XR 插件和示例脚本。字体字符文本已按现用字符串重新导出。

## 实际验证

在 Unity 6000.3.21f1 中，以正式场景运行无图形界面的 Play Mode 测试，并通过 XRInteractionManager 发送 SelectEnter/SelectExit 事件。完整简化流程、错误绿书、未松手、书包外放置、重玩、超时和字体检查全部通过。

此验证没有连接头显，不等于已检查真实控制器映射、双眼渲染或抓取手感。

可在 Unity 菜单 Tools > Desk Game > Verify task flow 复测；测试文件 DeskFlowVerification.cs 仅在编辑器编译，不进入发布版本。

```text
PASS: Computer and both books are connected
PASS: Task1 rejects wrong object
PASS: Task1 transitions to escaping red book
PASS: Task2 HUD has a valid font
PASS: Original Task2 panel shows the current objective
PASS: HUD glyph 抓
PASS: HUD glyph 住
PASS: HUD glyph 逃
PASS: HUD glyph 跑
PASS: HUD glyph 的
PASS: HUD glyph 红
PASS: HUD glyph 书
PASS: HUD glyph ，
PASS: HUD glyph 放
PASS: HUD glyph 进
PASS: HUD glyph 书
PASS: HUD glyph 包
PASS: HUD glyph ，
PASS: HUD glyph 然
PASS: HUD glyph 后
PASS: HUD glyph 松
PASS: HUD glyph 手
PASS: HUD glyph 。
PASS: HUD remains in camera view
PASS: No puzzle object: Playful Desk Adventure
PASS: No puzzle object: Bag
PASS: No puzzle object: Bag bottom
PASS: No puzzle object: Bag back
PASS: No puzzle object: Bag front
PASS: No puzzle object: Bag side
PASS: No puzzle object: Bag side
PASS: No puzzle object: Front pocket
PASS: No puzzle object: Pocket zipper
PASS: No puzzle object: Shoulder strap
PASS: No puzzle object: Handle side
PASS: No puzzle object: Shoulder strap
PASS: No puzzle object: Handle side
PASS: No puzzle object: Carry handle
PASS: No puzzle object: Bag interior target
PASS: No puzzle object: Label
PASS: No puzzle object: TMP SubMesh [NotoSansSC-VariableFont_wght SDF Material + Noto Sans SC - Thin Atlas]
PASS: No puzzle object: 多儿
PASS: No puzzle object: Duo Er body
PASS: No puzzle object: Eye
PASS: No puzzle object: Pupil
PASS: No puzzle object: Eye
PASS: No puzzle object: Pupil
PASS: No puzzle object: Smile
PASS: No puzzle object: Label
PASS: No puzzle object: TMP SubMesh [NotoSansSC-VariableFont_wght SDF Material + Noto Sans SC - Thin Atlas]
PASS: No puzzle object: Restart
PASS: No puzzle object: Restart button
PASS: No puzzle object: Label
PASS: No puzzle object: TMP SubMesh [NotoSansSC-VariableFont_wght SDF Material + Noto Sans SC - Thin Atlas]
PASS: No puzzle object: Task2 Instructions
PASS: No puzzle object: Current Mission
PASS: No puzzle object: TMP SubMeshUI [NotoSansSC-VariableFont_wght SDF Material + Noto Sans SC - Thin Atlas]
PASS: Green book cannot complete Task2
PASS: XR selected Book
PASS: Catching red book goes directly to packing without puzzle steps
PASS: Held red book reaches bag
PASS: Must release the book before completion
PASS: Book outside bag fails the interior check
PASS: Released red book inside bag wins directly
PASS: Restart leaves one adventure copy
PASS: XR selected Book
PASS: Grabbing works after win and restart
PASS: Timeout works
PASS: Restart works after timeout
```
