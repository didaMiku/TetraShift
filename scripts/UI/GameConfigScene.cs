using System;
using Godot;
using Tetris.scripts.util;

namespace Tetris.scripts.UI;

public partial class GameConfigScene : Control
{
    public override void _Ready()
    {
        GetNode<Button>("QuitButton").Pressed += _OnQuitButtonPressed;
    }

    public override void _Input(InputEvent @event)
    {
        MainNode parrent = GetParent<MainNode>();
        if (Input.IsActionJustPressed("Esc"))
            parrent?.SwitchTo(GlobalConstant.UISceneState.SceneMenu);
    }

    private void _OnQuitButtonPressed()
    {
        MainNode parrent = GetParent<MainNode>();
        parrent?.SwitchTo(GlobalConstant.UISceneState.SceneMenu);
    }
}
