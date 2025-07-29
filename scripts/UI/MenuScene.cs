using Godot;
using Godot.Collections;
using System;
using Tetris.scripts.UI;
using Tetris.scripts.util;

public partial class MenuScene : Control
{
    public override void _Ready()
    {
        GetNode<Button>("GameButton").Pressed += _OnGameButtonPressed;
        GetNode<Button>("GameConfigButton").Pressed += _OnGameConfigButtonPressed;
        GetNode<Button>("BlockConfigButton").Pressed += _OnBlockConfigButtonPressed;
        GetNode<Button>("GameSaveButton").Pressed += _OnGameSaveButtonPressed;
    }


    public override void _Input(InputEvent @event)
    {
        MainNode parent = GetParent<MainNode>();
        if (Input.IsActionJustPressed("Game"))
            parent?.SwitchTo(GlobalConstant.UISceneState.SceneGame);
        else if (Input.IsActionJustPressed("GameConfig"))
            parent?.SwitchTo(GlobalConstant.UISceneState.SceneGameConfig);
        else if (Input.IsActionJustPressed("BlockConfig"))
            parent?.SwitchTo(GlobalConstant.UISceneState.SceneBlockConfig);
        else if (Input.IsActionJustPressed("GameSave"))
            parent?.SwitchTo(GlobalConstant.UISceneState.SceneGameSave);

    }

    private void _OnGameButtonPressed() => GetParent<MainNode>()?.SwitchTo(GlobalConstant.UISceneState.SceneGame);

    private void _OnGameConfigButtonPressed() => GetParent<MainNode>()?.SwitchTo(GlobalConstant.UISceneState.SceneGameConfig);

    private void _OnBlockConfigButtonPressed() => GetParent<MainNode>()?.SwitchTo(GlobalConstant.UISceneState.SceneBlockConfig);

    private void _OnGameSaveButtonPressed() => GetParent<MainNode>()?.SwitchTo(GlobalConstant.UISceneState.SceneGameSave);
}
