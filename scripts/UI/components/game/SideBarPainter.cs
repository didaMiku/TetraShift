using System;
using Godot;
using Tetris.scripts.dto;
using Tetris.scripts.util;

namespace Tetris.scripts.UI.components.game;

public partial class SideBarPainter : Node
{
    private int _pixelWidth;
    private int _pixelHeight;
    private BlockRenderDto _blockRenderDto;
    private Panel _mainPanel;

    public SideBarPainter()
    {
        _pixelWidth = _pixelHeight = 0;
        _blockRenderDto = null;
        _mainPanel = null;
        AddChild(_mainPanel);
    }
    public SideBarPainter(int pixelWidth, int pixelHeight, BlockRenderDto blockRenderDto)
    {
        _pixelWidth = pixelWidth;
        _pixelHeight = pixelHeight;
        _blockRenderDto = blockRenderDto;
        _mainPanel = new Panel();
        AddChild(_mainPanel);
    }

    public void SetPixelSize(int pixelWidth, int pixelHeight)
    {
        _pixelWidth = pixelWidth;
        _pixelHeight = pixelHeight;
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
            Size = new Vector2(sideLength, sideLength),
        };
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

        Panel scoreTitlePanel = GetFullTextPanel(_pixelWidth, 20, "score");
        Panel scoreContentPanel = GetFullTextPanel(_pixelWidth, 20, score.ToString());
        Panel handledCountTitlePanel = GetFullTextPanel(_pixelWidth, 20, "handled blocks");
        Panel handledCountContentPanel = GetFullTextPanel(_pixelWidth, 20, handledCount.ToString());
        Panel nextBlockTitlePanel = GetFullTextPanel(_pixelWidth, 20, "next block");
        Panel nextBlockPanel = GetNextBlockContentPanel(_pixelWidth, nextBlock);
        Panel nextGravityDirectionTitlePanel = GetFullTextPanel(_pixelWidth, 20, "next gravity");
        Panel nextGravityDirectionPanel = GetFullTextPanel(_pixelWidth, 20, nextGravityDirection.ToString());

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