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
        BlockJsonService.InitializeUserData();
        // BlockJsonService.ResetUserDataToDefault();
        BlockJsonService.LoadBlocksToDictionary();
        GD.Print(BlockDictionary.GetAll().Count());
        GD.Print("逻辑测试已启动，等待键盘输入...");
        GD.Print("支持指令：Left, Right, Up, Down, SpinLeft, SpinRight, Fall");
    }
}
