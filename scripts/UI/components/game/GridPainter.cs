using System;
using System.Linq;
using Godot;
using Tetris.scripts.util;

namespace Tetris.scripts.UI.components.game;


/// <summary>
/// 专门用来绘制网格的节点
/// </summary>
public partial class GridPainter : Node2D
{
    private int _cellSize;        // 每个格子的像素大小
    private int _gridWidth;       // 横向格子数
    private int _gridHeight;      // 纵向格子数

    public GridPainter()
    {
        _cellSize = 0;
        _gridWidth = 0;
        _gridHeight = 0;
    }
    public GridPainter(int cellSize, int width, int height)
    {
        _cellSize = cellSize;
        _gridWidth = width;
        _gridHeight = height;
    }

    /**
     * 清空所有子节点
     */
    private void ClearAllChildren()
    {
        foreach (Node node in GetChildren().ToArray())
        {
            RemoveChild(node);
            node.QueueFree();
        }
    }

    /**
     * 渲染所有网格
     */
    private void PaintGrid()
    {
        ClearAllChildren();
        for (int y = 0; y < _gridHeight; y++)
            for (int x = 0; x < _gridWidth; x++)
                CreateGridAtPos(new IntVector2(x, y));
    }

    /**
     * 在指定位置生成一个Sprite2D
     */
    private void CreateGridAtPos(IntVector2 position)
    {
        var sprite = new Sprite2D();
        var texture = GD.Load<Texture2D>(GlobalConstant.DEFAULT_GRID_PATH);
        sprite.Centered = false;
        sprite.Texture = texture;
        sprite.Position = new Vector2(position.X * _cellSize, position.Y * _cellSize);
        sprite.Scale = new Vector2(
            _cellSize / texture.GetSize().X,
            _cellSize / texture.GetSize().Y
        );
        AddChild(sprite);
    }

    /**
     * 有变动时重新设置参数并更新绘制
     */
    public void SetGridPainter(int gridWidth, int gridHeight, int cellSize)
    {
        _gridWidth = gridWidth;
        _gridHeight = gridHeight;
        _cellSize = cellSize;
        PaintGrid();
    }
    public void SetGridPainter(int cellSize)
    {
        _cellSize = cellSize;
        PaintGrid();
    }

    public void SetCellSize(int cellSize) => _cellSize = cellSize;
    public void SetGridWidth(int width) => _gridWidth = width;
    public void SetGridHeight(int height) => _gridHeight = height;
}

