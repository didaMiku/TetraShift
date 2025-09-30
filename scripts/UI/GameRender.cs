using System;
using System.Collections.Generic;
using Tetris.scripts.service;
using Tetris.scripts.util;
using Tetris.scripts.UI.components.game;
using Godot;
using Tetris.scripts.dto;

namespace Tetris.scripts.UI;


/// <summary>
/// 场景渲染节点，用于显示网格、方块、侧边栏辅助信息
/// </summary>
public partial class GameRender : Node2D
{
    /**
     * 游戏场景的像素最大宽高
     */
    private int _pixelWidth;
    private int _pixelHeight;
    /**
     * 游戏的宽高
     */
    private int _width;
    private int _height;
    /**
     * 游戏类型
     */
    private GlobalConstant.GameType _gameType;
    /**
     * 游戏配置DTO
     */
    private GameConfigDto _gameConfigDto;
    /**
     * 游戏底层逻辑对象
     */
    private Grid _grid;
    /**
     * 二维数组坐标到像素坐标的映射
     */
    Dictionary<IntVector2, IntVector2> _posToPixel = new Dictionary<IntVector2, IntVector2>();
    /**
     * 每个基本方块的边长
     */
    private int _blockSize;
    /**
     * 网格绘制节点
     */
    private GridPainter _gridPainterNode;
    /**
     * 方块绘制节点
     */
    private BlockPainter _blockPainterNode;
    /**
     * 侧边栏绘制节点
     */
    private SideBarPainter _sideBarPainterNode;

    private ButtonPainter _buttonPainterNode;

    /**
     * 构造方法
     */
    public GameRender(int width, int height, GlobalConstant.GameType gameType)
    {
        _width = width;
        _height = height;
        _gameType = gameType;
        _pixelWidth = 0;
        _pixelHeight = 0;
        _blockSize = Math.Min(_pixelWidth / _width, _pixelHeight / _height);
        _gridPainterNode = new GridPainter();
        _blockPainterNode = new BlockPainter();
        _sideBarPainterNode = new SideBarPainter();
        _grid = new Grid(_gameType, _width, _height);
    }
    public GameRender()
    {
        _width = 10;
        _height = 15;
        _gameType = GlobalConstant.GameType.TypeClassic;
        _pixelWidth = 400;
        _pixelHeight = 800;
        _blockSize = Math.Min(_pixelWidth / _width, _pixelHeight / _height);
        _gridPainterNode = new GridPainter(_blockSize, _width, _height);
        _blockPainterNode = new BlockPainter(_blockSize);
        _sideBarPainterNode = new SideBarPainter();
        _grid = new Grid(_gameType, _width, _height);
    }

    public override void _Ready()
    {
        JsonService.InitializeUserData();
        _gameConfigDto = JsonService.GetGameConfig();

        GD.Print(_gameConfigDto.ToString());

        _width = _gameConfigDto.GetWidth();
        _height = _gameConfigDto.GetHeight();
        _gameType = _gameConfigDto.GetGameType();

        AddChild(_gridPainterNode);
        AddChild(_blockPainterNode);
        AddChild(_sideBarPainterNode);

        _gridPainterNode.SetGridWidth(_width);
        _gridPainterNode.SetGridHeight(_height);
        _gridPainterNode.SetGridPainter(_blockSize);
        _gridPainterNode.Position = new Vector2(0, 0);

        _grid = new Grid(_gameType, _width, _height);

        var blockRenderDto = _grid.HandleOperation(GlobalConstant.BlockOperations.BlockDown);
        var blockRenderArray = blockRenderDto.GetBlockRenderArray();

        _blockPainterNode.SetBlockPainter(_blockSize, blockRenderArray);
        _blockPainterNode.Position = new Vector2(0, 0);

        _sideBarPainterNode.SetBlockRenderDto(blockRenderDto);
        _sideBarPainterNode.SetPixelSize(150, 800);
        _sideBarPainterNode.Position = new Vector2(400, 0);

        _buttonPainterNode = new ButtonPainter();
        AddChild(_buttonPainterNode);
        _buttonPainterNode.Position = new Vector2(0, 600);

        GD.Print("block size: " + _blockSize);

        GD.Print(blockRenderDto.ToString());
    }


    private float _tickTimer = 0f;
    private float _tickInterval = 1.0f; // 每 0.1 秒一次（即 10 tick 每秒）

    public override void _Process(double delta)
    {
        _tickTimer += (float)delta;
        while (_tickTimer >= _tickInterval)
        {
            _tickTimer -= _tickInterval;
            var blockRenderDto = _grid.HandleOperation(GlobalConstant.BlockOperations.BlockTick); // 调用你的 Tick 逻辑
            _sideBarPainterNode.SetBlockRenderDto(blockRenderDto);
            _blockPainterNode.SetBlockPainter(_blockSize, blockRenderDto.GetBlockRenderArray());

            GD.Print($"输入: Tick => 输出: \n{blockRenderDto}");
        }
    }

    public override void _Input(InputEvent @event)
    {
        Simulate(@event);
    }

    private void Simulate(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            string input = MapEventKeyToString(eventKey);
            var operation = MapStringToOperation(input);
            if (operation == null)
            {
                GD.Print("无效指令: " + input);
                return;
            }
            var blockRenderDto = _grid.HandleOperation(operation.Value);
            GD.Print($"输入: {input} => 输出: \n{blockRenderDto}");

            _sideBarPainterNode.SetBlockRenderDto(blockRenderDto);
            _blockPainterNode.SetBlockPainter(_blockSize, blockRenderDto.GetBlockRenderArray());
        }
    }

    private GlobalConstant.BlockOperations? MapStringToOperation(string input)
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

    private string MapEventKeyToString(InputEventKey eventKey)
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


    /**
     * 计算和更新二维数组坐标到相对像素坐标的映射
     */
    private Dictionary<IntVector2, IntVector2> CalculatePosToPixelMap()
    {
        for (int i = 0; i < _width; i++)
            for (int j = 0; j < _height; j++)
                _posToPixel.Add(new IntVector2(i, j), new IntVector2(i * _blockSize, j * _blockSize));
        return _posToPixel;
    }
    private void UpdatePosToPixelMap() => _posToPixel = CalculatePosToPixelMap();


    /**
     * 计算和更新基本方块边长
     */
    private int CalculateBlockSize()
    {
        int widthSize = _pixelWidth / _width;
        int heightSize = _pixelHeight / _height;
        return Math.Min(widthSize, heightSize);
    }
    private void UpdateBlockSize() => _blockSize = CalculateBlockSize();


    /**
     * 仅更改游戏窗口尺寸时更新场景
     */
    public bool RenderGameScene(int pixelWidth, int pixelHeight)
    {
        if (_pixelWidth != pixelWidth || _pixelHeight != pixelHeight)
        {
            _pixelWidth = pixelWidth;
            _pixelHeight = pixelHeight;
            UpdateBlockSize();
            UpdatePosToPixelMap();
        }

        int blockPixelWidth = _pixelWidth / _width;
        int blockPixelHeight = _pixelHeight / _height;
        _blockSize = Math.Min(blockPixelWidth, blockPixelHeight);



        var blockRenderArray = _grid.GetBlockRenderDto();

        return true;
    }


    /**
     * 游戏有tick或玩家输入时更新场景
     */
    public bool RenderGameScene(int pixelWidth, int pixelHeight, GlobalConstant.BlockOperations operation)
    {
        _pixelWidth = pixelWidth;
        _pixelHeight = pixelHeight;
        return true;
    }



    /**
     * 在指定坐标创建一个格子
     */
    private void CreateGridAtPos(IntVector2 position)
    {
        var sprite = new Sprite2D();
        var texture = GD.Load<Texture2D>(GlobalConstant.DEFAULT_GRID_PATH);
        sprite.Texture = texture;
        sprite.Position = new Vector2(position.X, position.Y);
        AddChild(sprite);
    }


    /**
     * 在指定位置创建一个Sprite2D
     */
    private void CreateSpriteAtPos(IntVector2 posiiton)
    {
        var sprite = new Sprite2D();
        var texture = GD.Load<Texture2D>(GlobalConstant.DEFAULT_TEXTURE_PATH);
        sprite.Texture = texture;

        sprite.Position = new Vector2(posiiton.X, posiiton.Y);
        AddChild(sprite);
    }
}
