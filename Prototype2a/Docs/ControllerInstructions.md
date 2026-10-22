# 手柄说明与 Task2 提示卡

流程：介绍 → 手柄说明 → 继续 → 六张词卡 → 准备 → Task1 → Task2 → 完成和退出。

已核对 XRI Default Input Actions：左右手 Select 绑定 GripButton；Activate 和 UI Press 绑定 TriggerButton。因此侧面握持键用于抓取，食指扳机用于界面点击与电脑激活。左摇杆移动，左手柄 X 降低、Y 升高，头显转动控制观看方向。

Task2 提示卡位于摄像机前方 0.9 米、上移 0.22 米，以较小尺寸保留桌面操作区域。背景完全不透明；DeskAdventure 内的 DrawInFront 对面板及 TMP 中文后备字体使用独立前景材质，避免被电脑或场景标签遮挡。结束画面居中并保留退出按钮。Resources/DeskUI 中的着色器随构建打包。

新增说明文字已加入 Fonts/CharacterSets 下的字符表。若使用静态字体图集，请用更新后的 TMP_AllCharacters.txt 重新生成，确保新字显示。

验证：Unity 编辑器回归测试覆盖说明页继续按钮、六张词卡、Task1/Task2、成功与失败路径、结束画面、退出按钮存在和重玩；真实头显中的前景渲染、字号和按键舒适性仍需实机确认。
