using System;
using Godot;
using Tetris.scripts.UI.views;

namespace Tetris.scripts.UI.components.game;

/// <summary>
/// 用来绘制暂停菜单的节点
/// </summary>
public partial class PausePainter : Node2D
{
    private Button _continueButton;
    private GameView _gameView;
    public override void _Ready()
    {
        _continueButton = new Button();
        _continueButton.Position = new Vector2(50, 50);
        _continueButton.Size = new Vector2(20, 100);
        _continueButton.Text = "Continue";

        AddChild(_continueButton);

        _gameView = GetParent<GameView>();

        _continueButton.Pressed += OnContinueButtonPressed;
    }

    private void OnContinueButtonPressed() { _gameView?.GameContinue(); }
}