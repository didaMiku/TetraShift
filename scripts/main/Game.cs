using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Tetris.scripts.util;


namespace Tetris.scripts.main;

public partial class Game : Node
{
    public override void _Ready()
    {
        // 初始化方块信息
        // 更新全局方块字典
        // 显示初始游戏界面
    }

    public void UIManager()
    {
        // 首先获取现在的窗口大小
        // 根据这个窗口大小计算各个部分的像素长宽
        // 以游戏开始后的游戏界面为例，主题上分为上下两个部分，下半部分是按钮操作，应当是高度固定有最小宽度
        // 上半部分分为左右两个部分，右半部分是分数和方块、重力信息，应当是宽度固定有最小高度
        // 左半部分是网格区域，根据玩家预设的网格长宽，在确定了上述的两个部分的位置后，最大限度地利用剩余空间渲染网格
    }
}
