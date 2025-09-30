using Godot;
using System;
using Tetris.scripts.dto;
using Tetris.scripts.util;

namespace Tetris.scripts.UI.components.game;

public partial class SideBarContainer : Node2D
{
    private int _pixelWidth;
    private int _pixelHeight;
    private int _borderWidth;
    private SideBarPainter _sideBarPainter;

    public override void _Ready()
    {
        _sideBarPainter = new SideBarPainter();
        AddChild(_sideBarPainter);
    }

    public void SetSideBarContainer(int pixelWidth, int pixelHeight, int borderWidth)
    {
        _pixelWidth = pixelWidth;
        _pixelHeight = pixelHeight;
        _borderWidth = borderWidth;
        _sideBarPainter.SetPixelSize(_pixelWidth - 2 * _borderWidth, _pixelHeight - 2 * _borderWidth);
        _sideBarPainter.Position = new Vector2(_borderWidth, _borderWidth);
        PaintContainerGrid();
    }

    public void PaintSideBar(BlockRenderDto blockRenderDto)
    {
        _sideBarPainter.SetBlockRenderDto(blockRenderDto);
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