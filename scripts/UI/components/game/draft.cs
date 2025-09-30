using Godot;
using System;
using Tetris.scripts.dto;

namespace Tetris.scripts.UI.components.game;

public partial class Draft : Node2D
{
    private int _pixelWidth;
    private int _pixelHeight;
    private Panel _mainPanel;
    private VBoxContainer _vBoxContainer;
    private BlockRenderDto _blockRenderDto;

    public override void _Ready()
    {
        _mainPanel = new Panel();
        AddChild(_mainPanel);

        _vBoxContainer = new VBoxContainer();
        _vBoxContainer.AddThemeConstantOverride("separation", 0);
        _mainPanel.AddChild(_vBoxContainer);
    }

    public void SetPixelSize(int pixelWidth, int pixelHeight)
    {
        if (pixelWidth <= 0 || pixelHeight <= 0)
            return;
        _pixelWidth = pixelWidth;
        _pixelHeight = pixelHeight;

        _mainPanel.CustomMinimumSize = new Vector2(_pixelWidth, _pixelHeight);
        _mainPanel.SizeFlagsHorizontal = 0;
        _mainPanel.SizeFlagsVertical = 0;

        _vBoxContainer.CustomMinimumSize = new Vector2(_pixelWidth, _pixelHeight);
        _vBoxContainer.SizeFlagsHorizontal = 0;
        _vBoxContainer.SizeFlagsVertical = 0;
    }

    public void SetBlockRenderDto(BlockRenderDto blockRenderDto)
    {
        if (blockRenderDto == null)
            return;
        _blockRenderDto = blockRenderDto;
    }

    private Panel GetFullTextPanel(int width, int height, string text)
    {
        Panel cell = new Panel()
        {
            CustomMinimumSize = new Vector2(width, height)
        };
        cell.SizeFlagsHorizontal = 0;
        cell.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
        var simpleStyle = new StyleBoxFlat()
        {
            BgColor = new Color(0, 0, 0, 0), // 背景透明
            BorderColor = new Color(0, 0, 0), // 边框颜色（黑色）
            BorderWidthTop = 1, // 边框
            BorderWidthLeft = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            DrawCenter = false
        };
        cell.AddThemeStyleboxOverride("panel", simpleStyle);

        Label textLabel = new Label()
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AnchorLeft = 0,
            AnchorTop = 0,
            AnchorRight = 1,
            AnchorBottom = 1,
            SizeFlagsHorizontal = Control.SizeFlags.Fill,
            SizeFlagsVertical = Control.SizeFlags.Fill
        };
        cell.AddChild(textLabel);

        return cell;
    }

    public void PaintSideBar()
    {
        if (_blockRenderDto == null)
            return;
        ClearContainerChildren();

        _vBoxContainer.AddChild(GetFullTextPanel(_pixelWidth, _pixelHeight / 10, "score"));
        _vBoxContainer.AddChild(GetFullTextPanel(_pixelWidth, _pixelHeight / 10, _blockRenderDto.GetScore().ToString()));
        _vBoxContainer.AddChild(GetFullTextPanel(_pixelWidth, _pixelHeight / 10, "handled blocks"));
        _vBoxContainer.AddChild(GetFullTextPanel(_pixelWidth, _pixelHeight / 10, _blockRenderDto.GetHandledCount().ToString()));
        _vBoxContainer.AddChild(GetFullTextPanel(_pixelWidth, _pixelHeight / 10, "next block"));
        _vBoxContainer.AddChild(GetFullTextPanel(_pixelWidth, _pixelHeight / 10, _blockRenderDto.GetNextBlock()));
        _vBoxContainer.AddChild(GetFullTextPanel(_pixelWidth, _pixelHeight / 10, "next gravity"));
        _vBoxContainer.AddChild(GetFullTextPanel(_pixelWidth, _pixelHeight / 10, _blockRenderDto.GetNextGravityDirection().ToString()));
    }
    
    private void ClearContainerChildren()
    {
        foreach (Node node in _vBoxContainer.GetChildren())
        {
            _vBoxContainer.RemoveChild(node);
            node.QueueFree();
        }
    }
}
