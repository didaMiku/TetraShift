using System;
using Godot;
using Tetris.scripts.util;

namespace Tetris.scripts.UI;

public partial class GameScene : Control
{
    public override void _Ready()
    {
        GetNode<Button>("PauseButton").Pressed += _OnPauseButtonPressed;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("Pause"))
        {
            SetProcessInput(false);
            GetNode<Control>("PauseScene").Visible = true;
            GetNode<Control>("PauseScene").SetProcessInput(true);
        }
    }

    private void _OnPauseButtonPressed()
    {
        SetProcessInput(false);
        GetNode<Control>("PauseScene").Visible = true;
        GetNode<Control>("PauseScene").SetProcessInput(true);
    }
}
