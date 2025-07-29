using System;
using Godot;
using Tetris.scripts.util;

namespace Tetris.scripts.UI;

public partial class GameSaveScene : Control
{
    public override void _Ready()
    {
        GetNode<Button>("QuitButton").Pressed += _OnQuitButtonPressed;
    }

    bool disableInput = false;
    public void SetDisableInput(bool disableInput) => this.disableInput = disableInput;
    public bool GetDisableInput() => disableInput;

    public override void _Input(InputEvent @event)
    {
        if (disableInput == true)
            return;

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
