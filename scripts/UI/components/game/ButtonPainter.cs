using System;
using Godot;
using Tetris.scripts.UI.views;
using Tetris.scripts.util;

namespace Tetris.scripts.UI.components.game;

/// <summary>
/// 专门用来绘制按钮的节点
/// </summary>
public partial class ButtonPainter : Node2D
{
    private int _pixelWidth;
    private int _pixelHeight;
    private int _buttonSize;
    private Button _pauseButton;
    private Button _spinACWButton;
    private Button _spinCWButton;
    private Button _moveUpButton;
    private Button _moveDownButton;
    private Button _moveLeftButton;
    private Button _moveRightButton;
    private Button _dropButton;
    private GameView _gameView;

    public ButtonPainter()
    {
        _pauseButton = new Button();
        _spinACWButton = new Button();
        _spinCWButton = new Button();
        _moveUpButton = new Button();
        _moveDownButton = new Button();
        _moveLeftButton = new Button();
        _moveRightButton = new Button();
        _dropButton = new Button();
    }

    public override void _Ready()
    {
        AddChild(_pauseButton);
        AddChild(_spinACWButton);
        AddChild(_spinCWButton);
        AddChild(_moveUpButton);
        AddChild(_moveDownButton);
        AddChild(_moveLeftButton);
        AddChild(_moveRightButton);
        AddChild(_dropButton);

        _pauseButton.Pressed += OnPauseButtonPressed;
        _spinACWButton.Pressed += OnSpinACWButtonPressed;
        _spinCWButton.Pressed += OnSpinCWButtonPressed;
        _moveUpButton.Pressed += OnMoveUpButtonPressed;
        _moveDownButton.Pressed += OnMoveDownButtonPressed;
        _moveLeftButton.Pressed += OnMoveLeftButtonPressed;
        _moveRightButton.Pressed += OnMoveRightButtonPressed;
        _dropButton.Pressed += OnDropButtonPressed;

        _gameView = GetParent<ButtonContainer>().GetParent<GameView>();
    }

    public void PaintButton()
    {
        // 暂停
        _pauseButton.Size = new Vector2(_buttonSize, _buttonSize);
        _pauseButton.Position = new Vector2(0.125f * _pixelWidth - _pauseButton.Size.X / 2, 0.666f * _pixelHeight - _pauseButton.Size.Y / 2);
        _pauseButton.Text = "Pause";
        // 逆时针旋转
        _spinACWButton.Size = new Vector2(_buttonSize, _buttonSize);
        _spinACWButton.Position = new Vector2(0.125f * _pixelWidth - _spinACWButton.Size.X / 2, 0.25f * _pixelHeight - _spinACWButton.Size.Y / 2);
        _spinACWButton.Text = "Spin ACW";
        // 顺时针旋转
        _spinCWButton.Size = new Vector2(_buttonSize, _buttonSize);
        _spinCWButton.Position = new Vector2(0.333f * _pixelWidth - _spinCWButton.Size.X / 2, 0.25f * _pixelHeight - _spinCWButton.Size.Y / 2);
        _spinCWButton.Text = "Spin CW";
        // 上移
        _moveUpButton.Size = new Vector2(_buttonSize, _buttonSize);
        _moveUpButton.Position = new Vector2(0.75f * _pixelWidth - _moveUpButton.Size.X / 2, 0.166f * _pixelHeight - _moveUpButton.Size.Y / 2);
        _moveUpButton.Text = "Move Up";
        // 下移
        _moveDownButton.Size = new Vector2(_buttonSize, _buttonSize);
        _moveDownButton.Position = new Vector2(0.75f * _pixelWidth - _moveDownButton.Size.X / 2, 0.833f * _pixelHeight - _moveDownButton.Size.Y / 2);
        _moveDownButton.Text = "Move Down";
        // 左移
        _moveLeftButton.Size = new Vector2(_buttonSize, _buttonSize);
        _moveLeftButton.Position = new Vector2(0.583f * _pixelWidth - _moveLeftButton.Size.X / 2, 0.5f * _pixelHeight - _moveLeftButton.Size.Y / 2);
        _moveLeftButton.Text = "Move Left";
        // 右移
        _moveRightButton.Size = new Vector2(_buttonSize, _buttonSize);
        _moveRightButton.Position = new Vector2(0.916f * _pixelWidth - _moveRightButton.Size.X / 2, 0.5f * _pixelHeight - _moveRightButton.Size.Y / 2);
        _moveRightButton.Text = "Move Right";
        // 下落
        _dropButton.Size = new Vector2(_buttonSize, _buttonSize);
        _dropButton.Position = new Vector2(0.75f * _pixelWidth - _dropButton.Size.X / 2, 0.5f * _pixelHeight - _dropButton.Size.Y / 2);
        _dropButton.Text = "Drop";
    }

    public void SetButtonPainter(int pixelWidth, int pixelHeight, int buttonSize)
    {
        _pixelWidth = pixelWidth;
        _pixelHeight = pixelHeight;
        _buttonSize = buttonSize;
        GD.Print(_pixelWidth + " " + _pixelHeight + " " + _buttonSize);
    }

    private void OnPauseButtonPressed() { _gameView?.GamePause(); }
    private void OnSpinACWButtonPressed() { _gameView?.HandleInput(GlobalConstant.BlockOperations.BlockSpinLeft); }
    private void OnSpinCWButtonPressed() { _gameView?.HandleInput(GlobalConstant.BlockOperations.BlockSpinRight); }
    private void OnMoveUpButtonPressed() { _gameView?.HandleInput(GlobalConstant.BlockOperations.BlockUp); }
    private void OnMoveDownButtonPressed() { _gameView?.HandleInput(GlobalConstant.BlockOperations.BlockDown); }
    private void OnMoveLeftButtonPressed() { _gameView?.HandleInput(GlobalConstant.BlockOperations.BlockLeft); }
    private void OnMoveRightButtonPressed() { _gameView?.HandleInput(GlobalConstant.BlockOperations.BlockRight); }
    private void OnDropButtonPressed() { _gameView?.HandleInput(GlobalConstant.BlockOperations.BlockFall); }
}