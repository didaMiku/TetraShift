using System;
using Godot;
using Tetris.scripts.util;

namespace Tetris.scripts.UI;

public partial class PauseScene : Control
{
    public override void _Ready()
    {
        GetNode<Button>("ContinueButton").Pressed += _OnContinueButtonPressed;
        GetNode<Button>("QuitButton").Pressed += _OnQuitButtonPressed;
    }

    public override void _Input(InputEvent @event)
    {
        GameScene parrent = GetParent<GameScene>();
        MainNode mainParrent = parrent.GetParent<MainNode>();
        if (Input.IsActionJustPressed("Pause"))
        {
            SetProcessInput(false);
            Visible = false;
            parrent.SetProcessInput(true);
        }
        else if (Input.IsActionJustPressed("Esc"))
            mainParrent?.SwitchTo(GlobalConstant.UISceneState.SceneMenu);
    }

    private void _OnContinueButtonPressed()
    {
        GameScene parrent = GetParent<GameScene>();
        SetProcessInput(false);
        Visible = false;
        parrent.SetProcessInput(true);
    }

    private void _OnQuitButtonPressed()
    {
        GameScene parrent = GetParent<GameScene>();
        MainNode mainParrent = parrent.GetParent<MainNode>();
        mainParrent?.SwitchTo(GlobalConstant.UISceneState.SceneMenu);
    }
}
