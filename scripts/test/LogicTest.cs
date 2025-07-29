using Godot;
using System;
using System.Linq;
using Tetris.scripts.util;
using Tetris.scripts.service;

public partial class LogicTest : Node
{
    private Grid _grid = new Grid(GlobalConstant.GameType.TypeFourWay, 15, 15);

    public override void _Ready()
    {
        JsonService.InitializeUserData();
        // BlockJsonService.ResetUserDataToDefault();
        JsonService.LoadBlocksToDictionary();
        _grid.HandleOperation(GlobalConstant.BlockOperations.BlockDown);
        GD.Print(BlockDictionary.GetAll().Count());
        GD.Print("逻辑测试已启动，等待键盘输入...");
        GD.Print("支持指令：Left, Right, Up, Down, SpinLeft, SpinRight, Fall");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            string input = GetInputCommand(eventKey);
            if (!string.IsNullOrEmpty(input))
            {
                Simulate(input);
            }
        }
    }

    private float _tickTimer = 0f;
    private float _tickInterval = 1.0f; // 每 0.1 秒一次（即 10 tick 每秒）

    public override void _Process(double delta)
    {
        _tickTimer += (float)delta;
        while (_tickTimer >= _tickInterval)
        {
            _tickTimer -= _tickInterval;
            var result = _grid.HandleOperation(GlobalConstant.BlockOperations.BlockTick); // 调用你的 Tick 逻辑
            GD.Print($"输入: Tick => 输出: \n{result}");
        }
    }

    private void Simulate(string input)
    {
        var operation = MapInputToOperation(input);
        if (operation == null)
        {
            GD.Print("无效指令: " + input);
            return;
        }

        var result = _grid.HandleOperation(operation.Value);
        GD.Print($"输入: {input} => 输出: \n{result}");
    }

    private GlobalConstant.BlockOperations? MapInputToOperation(string input)
    {
        switch (input.ToLower())
        {
            case "left": return GlobalConstant.BlockOperations.BlockLeft;
            case "right": return GlobalConstant.BlockOperations.BlockRight;
            case "up": return GlobalConstant.BlockOperations.BlockUp;
            case "down": return GlobalConstant.BlockOperations.BlockDown;
            case "spinright": return GlobalConstant.BlockOperations.BlockSpinRight;
            case "spinleft": return GlobalConstant.BlockOperations.BlockSpinLeft;
            case "fall": return GlobalConstant.BlockOperations.BlockFall;
            default: return null;
        }
    }

    private string GetInputCommand(InputEventKey eventKey)
    {
        switch (eventKey.Keycode)
        {
            case Key.Left:
                return "Left";
            case Key.Right:
                return "Right";
            case Key.Up:
                return "Up";
            case Key.Down:
                return "Down";
            case Key.Q:
                return "SpinLeft";
            case Key.E:
                return "SpinRight";
            case Key.Space:
                return "Fall";
            default:
                return null;
        }
    }
}