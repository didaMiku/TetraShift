using System;
using Godot;

namespace Tetris.scripts.UI.components.game;

/// <summary>
/// 专门用来绘制按钮的节点
/// </summary>
public partial class ButtonPainter : Node2D
{
    public override void _Ready()
    {
        // 暂停
        Button pauseButton = new Button();
        pauseButton.Position = new Vector2(33, 200);
        pauseButton.Size = new Vector2(66, 66);
        pauseButton.Text = "暂停";
        // 逆时针旋转
        Button spinACWButton = new Button();
        spinACWButton.Position = new Vector2(33, 50);
        spinACWButton.Size = new Vector2(66, 66);
        spinACWButton.Text = "逆时针旋转";
        // 顺时针旋转
        Button spinCWButton = new Button();
        spinCWButton.Position = new Vector2(166, 50);
        spinCWButton.Size = new Vector2(66, 66);
        spinCWButton.Text = "顺时针旋转";
        // 上移
        Button moveUpButton = new Button();
        moveUpButton.Position = new Vector2(400, 33);
        moveUpButton.Size = new Vector2(66, 66);
        moveUpButton.Text = "上移";
        // 下移
        Button moveDownButton = new Button();
        moveDownButton.Position = new Vector2(400, 200);
        moveDownButton.Size = new Vector2(66, 66);
        moveDownButton.Text = "下移";
        // 左移
        Button moveLeftButton = new Button();
        moveLeftButton.Position = new Vector2(300, 116);
        moveLeftButton.Size = new Vector2(66, 66);
        moveLeftButton.Text = "左移";
        // 右移
        Button moveRightButton = new Button();
        moveRightButton.Position = new Vector2(500, 116);
        moveRightButton.Size = new Vector2(66, 66);
        moveRightButton.Text = "右移";
        // 下落
        Button dropButton = new Button();
        dropButton.Position = new Vector2(400, 116);
        dropButton.Size = new Vector2(66, 66);
        dropButton.Text = "下落";

        AddChild(pauseButton);
        AddChild(spinACWButton);
        AddChild(spinCWButton);
        AddChild(moveUpButton);
        AddChild(moveDownButton);
        AddChild(moveLeftButton);
        AddChild(moveRightButton);
        AddChild(dropButton);
    }
}