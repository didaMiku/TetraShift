using System;
using System.Collections.Generic;
using Tetris.scripts.service;
using Tetris.scripts.util;
using Tetris.scripts.UI.components.game;
using Godot;

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
        _grid = new Grid(_gameType, _width, _height);
    }

    public override void _Ready()
    {
        JsonService.InitializeUserData();

        AddChild(_gridPainterNode);
        AddChild(_blockPainterNode);

        _gridPainterNode.SetGridPainter(_blockSize);
        _gridPainterNode.Position = new Vector2(0, 0);

        _grid = new Grid(GlobalConstant.GameType.TypeClassic, 10, 15);

        var blockRenderDto = _grid.HandleOperation(GlobalConstant.BlockOperations.BlockDown);
        var blockRenderArray = blockRenderDto.GetBlockRenderArray();

        _blockPainterNode.SetBlockPainter(_blockSize, blockRenderArray);
        _blockPainterNode.Position = new Vector2(0, 0);

        GD.Print("block size: " + _blockSize);

        GD.Print(blockRenderDto.ToString());
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
