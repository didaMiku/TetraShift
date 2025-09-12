using System;
using System.Linq;
using Godot;
using Tetris.scripts.dto;
using Tetris.scripts.util;

namespace Tetris.scripts.UI.components.game;


/// <summary>
/// 专门用来绘制侧边栏的节点
/// </summary>
public partial class SideBarPainter : Node2D
{
    private int _pixelWidth;
    private int _pixelHeight;
    private BlockRenderDto _blockRenderDto;
    private Panel _mainPanel;

    public SideBarPainter()
    {
        _pixelWidth = _pixelHeight = 0;
        _blockRenderDto = null;
    }
    public SideBarPainter(int pixelWidth, int pixelHeight, BlockRenderDto blockRenderDto)
    {
        _pixelWidth = pixelWidth;
        _pixelHeight = pixelHeight;
        _blockRenderDto = blockRenderDto;
    }

    public override void _Ready()
    {
        _mainPanel = new Panel();
        _mainPanel.CustomMinimumSize = new Vector2(_pixelWidth, _pixelHeight);
        _mainPanel.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        _mainPanel.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
        AddChild(_mainPanel);
    }

    public void SetPixelSize(int pixelWidth, int pixelHeight)
    {
        _pixelWidth = pixelWidth;
        _pixelHeight = pixelHeight;
    }

    public void SetBlockRenderDto(BlockRenderDto blockRenderDto)
    {
        ClearAllChildren();
        _blockRenderDto = blockRenderDto;
        PaintSideBar();
    }


    /**
     * 清空所有子节点
     */
    private void ClearAllChildren()
    {
        foreach (Node node in _mainPanel.GetChildren().ToArray())
        {
            _mainPanel.RemoveChild(node);
            node.QueueFree();
        }
    }



    /**
     * 获取使用Label包装的显示文本的Panel
     */
    private Panel GetFullTextPanel(int width, int height, string text)
    {
        Panel resultPanel = new Panel()
        {
            CustomMinimumSize = new Vector2(width, height),
        };
        var style = new StyleBoxFlat
        {
            BgColor = new Color(0, 0, 0, 0), // 背景透明
            BorderColor = new Color(0, 0, 0), // 边框颜色（黑色）
            BorderWidthTop = 1, // 边框
            BorderWidthLeft = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            DrawCenter = false
        };
        resultPanel.AddThemeStyleboxOverride("panel", style);
        Label resultTextLabel = new Label()
        {
            Text = text,
            AnchorLeft = 0,
            AnchorTop = 0,
            AnchorRight = 1,
            AnchorBottom = 1,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            
        };
        resultPanel.AddChild(resultTextLabel);
        return resultPanel;
    }

    /**
     * 获取装载方块形状的正方形Panel，结果填充全部Panel空间
     */
    private Panel GetNextBlockContentPanel(int sideLength, string blockName)
    {
        Panel nextBlockContentPanel = new Panel()
        {
            CustomMinimumSize = new Vector2(sideLength, sideLength),
        };
        var style = new StyleBoxFlat
        {
            BgColor = new Color(0, 0, 0, 0), // 背景透明
            BorderColor = new Color(0, 0, 0), // 边框颜色（黑色）
            BorderWidthTop = 1, // 边框
            BorderWidthLeft = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            DrawCenter = false
        };
        nextBlockContentPanel.AddThemeStyleboxOverride("panel", style);
        // 获取方块信息，并生成用来渲染和定位方块的Node
        BlockData blockData = BlockDictionary.Get(blockName);
        int[,] blockArray = blockData.GetShape();
        Node2D blcokNode = new Node2D()
        {
            Position = new Vector2(sideLength / 2, sideLength / 2),
        };
        // 计算方块的尺寸和偏移量
        int arrayYCount = blockArray.GetLength(0);
        int arrayXCount = blockArray.GetLength(1);
        int blockSize = Math.Min(sideLength / arrayYCount, sideLength / arrayXCount);
        int offset = sideLength / 2;
        for (int y = 0; y < arrayYCount; y++)
        {
            for (int x = 0; x < arrayXCount; x++)
            {
                if (blockArray[y, x] == 1)
                {
                    Sprite2D sprite = new Sprite2D();
                    var texture = GD.Load<Texture2D>(blockData.GetTexturePath());
                    sprite.Texture = texture;
                    sprite.Scale = new Vector2(
                        blockSize / texture.GetSize().X,
                        blockSize / texture.GetSize().Y
                    );
                    sprite.Position = new Vector2(x * blockSize - offset, y * blockSize - offset);
                    sprite.Centered = false;
                    blcokNode.AddChild(sprite);
                }
            }
        }
        nextBlockContentPanel.AddChild(blcokNode);
        return nextBlockContentPanel;
    }


    /**
     * 绘制侧边栏
     */
    public void PaintSideBar()
    {
        int score = _blockRenderDto.GetScore();
        int handledCount = _blockRenderDto.GetHandledCount();
        string nextBlock = _blockRenderDto.GetNextBlock();
        var nextGravityDirection = _blockRenderDto.GetNextGravityDirection();
        int cellHeight = _pixelHeight / 10;

        Panel scoreTitlePanel = GetFullTextPanel(_pixelWidth, cellHeight, "score");
        Panel scoreContentPanel = GetFullTextPanel(_pixelWidth, cellHeight, score.ToString());
        Panel handledCountTitlePanel = GetFullTextPanel(_pixelWidth, cellHeight, "handled blocks");
        Panel handledCountContentPanel = GetFullTextPanel(_pixelWidth, cellHeight, handledCount.ToString());
        Panel nextBlockTitlePanel = GetFullTextPanel(_pixelWidth, cellHeight, "next block");
        Panel nextBlockPanel = GetNextBlockContentPanel(cellHeight * 3, nextBlock);
        Panel nextGravityDirectionTitlePanel = GetFullTextPanel(_pixelWidth, cellHeight, "next gravity");
        Panel nextGravityDirectionPanel = GetFullTextPanel(_pixelWidth, cellHeight, nextGravityDirection.ToString());

        VBoxContainer vBoxContainer = new VBoxContainer();
        vBoxContainer.AddChild(scoreTitlePanel);
        vBoxContainer.AddChild(scoreContentPanel);
        vBoxContainer.AddChild(handledCountTitlePanel);
        vBoxContainer.AddChild(handledCountContentPanel);
        vBoxContainer.AddChild(nextBlockTitlePanel);
        vBoxContainer.AddChild(nextBlockPanel);
        vBoxContainer.AddChild(nextGravityDirectionTitlePanel);
        vBoxContainer.AddChild(nextGravityDirectionPanel);

        _mainPanel.AddChild(vBoxContainer);
    }
}