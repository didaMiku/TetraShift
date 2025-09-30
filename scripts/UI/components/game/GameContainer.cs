using Godot;
using System;
using Tetris.scripts.dto;
using Tetris.scripts.util;

namespace Tetris.scripts.UI.components.game;

public partial class GameContainer : Node2D
{
    private int _pixelWidth;
    private int _pixelHeight;
    private int _borderWidth;
    private int _width;
    private int _height;
    private BlockPainter _blockPainter;
    private GridPainter _gridPainter;

    public override void _Ready()
    {
        _blockPainter = new BlockPainter();
        _gridPainter = new GridPainter();
        AddChild(_blockPainter);
        AddChild(_gridPainter);
    }

    public void SetGameContainer(int pixelWidth, int pixelHeight, int borderWidth, int width, int height)
    {
        _pixelWidth = pixelWidth;
        _pixelHeight = pixelHeight;
        _borderWidth = borderWidth;
        _width = width;
        _height = height;
        InitializePainters();
        PaintContainerGrid();
    }

    private void InitializePainters()
    {
        var blockSize = Math.Min(_pixelWidth / _width - _borderWidth * 2, _pixelHeight / _height - _borderWidth * 2);
        _blockPainter.SetBlockSize(blockSize);
        _gridPainter.SetGridPainter(_width, _height, blockSize);
        var blockGridPainterPositionOffset = new Vector2((_pixelWidth - blockSize * _width) / 2, (_pixelHeight - blockSize * _height) / 2);
        _blockPainter.Position = blockGridPainterPositionOffset;
        _gridPainter.Position = blockGridPainterPositionOffset;
    }

    public void PaintBlocks(BlockRenderData[,] blockRenterArray)
    {
        _blockPainter.SetBlockPainter(blockRenterArray);
    }
    
    private void PaintContainerGrid()
    {
        var gridPath = GlobalConstant.DEFAULT_GRID_PATH;
        var gridWidth = _pixelWidth - 2 * _borderWidth;
        var gridHeight = _pixelHeight - 2 * _borderWidth;
        Sprite2D sprite = new Sprite2D();
        sprite.Texture = GD.Load<Texture2D>(gridPath);
        sprite.Centered = false;
        sprite.Position = new Vector2(_borderWidth, _borderWidth);
        sprite.Scale = new Vector2(gridWidth / sprite.Texture.GetSize().X, gridHeight / sprite.Texture.GetSize().Y);
        AddChild(sprite);
    }
}