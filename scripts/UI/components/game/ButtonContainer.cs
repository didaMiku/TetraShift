using Godot;
using System;
using Tetris.scripts.util;

namespace Tetris.scripts.UI.components.game;

public partial class ButtonContainer : Node2D
{
    private int _pixelWidth;
    private int _pixelHeight;
    private int _borderWidth;
    private ButtonPainter _buttonPainter;

    public override void _Ready()
    {
        _buttonPainter = new ButtonPainter();
        AddChild(_buttonPainter);
    }

    public void SetButtonContainer(int pixelWidth, int pixelHeight, int borderWidth)
    {
        _pixelWidth = pixelWidth;
        _pixelHeight = pixelHeight;
        _borderWidth = borderWidth;
        PaintContainerGrid();
    }

    public void PaintButton()
    {
        _buttonPainter.SetButtonPainter(_pixelWidth - 2 * _borderWidth, _pixelHeight - 2 * _borderWidth, 66);
        _buttonPainter.PaintButton();
        _buttonPainter.Position = new Vector2(_borderWidth, _borderWidth);
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