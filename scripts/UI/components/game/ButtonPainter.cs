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
    private Button _pauseButton;
    private Button _spinACWButton;
    private Button _spinCWButton;
    private Button _moveUpButton;
    private Button _moveDownButton;
    private Button _moveLeftButton;
    private Button _moveRightButton;
    private Button _dropButton;
    private GameView _gameView;

    public override void _Ready()
    {
        // 暂停
        _pauseButton = new Button();
        _pauseButton.Position = new Vector2(33, 200);
        _pauseButton.Size = new Vector2(66, 66);
        _pauseButton.Text = "暂停";
        // 逆时针旋转
        _spinACWButton = new Button();
        _spinACWButton.Position = new Vector2(33, 50);
        _spinACWButton.Size = new Vector2(66, 66);
        _spinACWButton.Text = "逆时针旋转";
        // 顺时针旋转
        _spinCWButton = new Button();
        _spinCWButton.Position = new Vector2(166, 50);
        _spinCWButton.Size = new Vector2(66, 66);
        _spinCWButton.Text = "顺时针旋转";
        // 上移
        _moveUpButton = new Button();
        _moveUpButton.Position = new Vector2(400, 33);
        _moveUpButton.Size = new Vector2(66, 66);
        _moveUpButton.Text = "上移";
        // 下移
        _moveDownButton = new Button();
        _moveDownButton.Position = new Vector2(400, 200);
        _moveDownButton.Size = new Vector2(66, 66);
        _moveDownButton.Text = "下移";
        // 左移
        _moveLeftButton = new Button();
        _moveLeftButton.Position = new Vector2(300, 116);
        _moveLeftButton.Size = new Vector2(66, 66);
        _moveLeftButton.Text = "左移";
        // 右移
        _moveRightButton = new Button();
        _moveRightButton.Position = new Vector2(500, 116);
        _moveRightButton.Size = new Vector2(66, 66);
        _moveRightButton.Text = "右移";
        // 下落
        _dropButton = new Button();
        _dropButton.Position = new Vector2(400, 116);
        _dropButton.Size = new Vector2(66, 66);
        _dropButton.Text = "下落";

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

        _gameView = GetParent<GameView>();
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