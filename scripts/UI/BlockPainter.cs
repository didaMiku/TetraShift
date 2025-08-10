using System;
using System.Linq;
using Godot;
using Tetris.scripts.dto;
using Tetris.scripts.util;

namespace Tetris.scripts.UI;


/// <summary>
/// 专门用来绘制方块的节点
/// </summary>
public partial class BlockPainter : Node2D
{
    private int _blockSize;
    private BlockRenderData[,] _blockRenderArray;

    public BlockPainter()
    {
        _blockSize = 0;
        _blockRenderArray = null;
    }
    public BlockPainter(int blockSize)
    {
        _blockSize = blockSize;
        _blockRenderArray = null;
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
     * 根据现有数组渲染所有方块
     */
    private void PaintBlocks()
    {
        ClearAllChildren();
        for (int y = 0; y < _blockRenderArray.GetLength(0); y++)
            for (int x = 0; x < _blockRenderArray.GetLength(1); x++)
                if (_blockRenderArray[y, x] != null)
                    CreateSpriteAtPos(new IntVector2(x, y), _blockRenderArray[y, x]);
    }

    /**
     * 在指定位置创建一个Sprite2D
     */
    private void CreateSpriteAtPos(IntVector2 position, BlockRenderData blockRenderData)
    {
        var sprite = new Sprite2D();
        var texture = GD.Load<Texture2D>(blockRenderData.GetTexturePath());
        sprite.Texture = texture;
        sprite.Position = new Vector2(position.X * _blockSize, position.Y * _blockSize);
        sprite.Scale = new Vector2(
            _blockSize / texture.GetSize().X,
            _blockSize / texture.GetSize().Y
        );
        AddChild(sprite);
    }

    /**
     * 有变动时设置方块绘制器并绘制
     */
    public void SetBlockPainter(int blockSize, BlockRenderData[,] blockRenderArray)
    {
        _blockSize = blockSize;
        _blockRenderArray = blockRenderArray;
        PaintBlocks();
    }

    public void SetBlockSize(int blockSize) => _blockSize = blockSize;
    public void SetBlockRenderArray(BlockRenderData[,] blockRenderArray) => _blockRenderArray = blockRenderArray;
}
