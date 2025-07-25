using System;
using System.Collections.Generic;
using Godot;
using Tetris.scripts.dto;
using Tetris.scripts.util;


namespace Tetris.scripts.service;

/// <summary>
/// 控制下落方块行为，包括生成、移动、旋转
/// 不在乎任何行为合法性，行为合不合法交给上级判断，本类只负责执行
/// </summary>
public class Tetromino
{
    /**
     * 属性：上层网格的宽度和高度，用于生成方块
     */
    private readonly int _width;  // X、横、宽
    private readonly int _height; // Y、纵、高
    
    /**
     * 属性：当前正在下落的方块的名称
     */
    private string _currentBlock;
    
    /**
     * 属性：方块容器的原点在上层网格中的坐标
     */
    private IntVector2 _blockPosition;
    
    /**
     * 属性：方块容器，用来承载方块的现在的状态，旋转在此容器中进行
     */
    private int[,] _blockStatus;
    
    /**
     * 属性：通过前两个属性得到现在正在下落的方块的每个基本方块在上层网格相对坐标系中的绝对坐标
     */
    private List<IntVector2> _fallingBlockPositions;

    /**
     * 属性：游戏类型
     */
    private GlobalConstant.GameType _gameType;


    /**
     * 构造方法
     */
    public Tetromino(GlobalConstant.GameType gameType, int width, int height)
    {
        _gameType = gameType;
        _width = width;
        _height = height;
        _currentBlock = "";
        _blockPosition = new IntVector2(0, 0);
        _blockStatus = new int[0,0];
        _fallingBlockPositions = new List<IntVector2>();
        CreateNewBlock(_currentBlock);
    }
    
    /**
     * 方法：计算fallingBlockPositions
     */
    private List<IntVector2> CalculateFallingBlockPositions()
    {
        List<IntVector2> positions = new List<IntVector2>();
        for (int i = 0; i < _blockStatus.GetLength(0); i++)
        {
            for (int j = 0; j < _blockStatus.GetLength(1); j++)
            {
                if (_blockStatus[i, j] == 1)
                {
                    int x = _blockPosition.X + j;
                    int y = _blockPosition.Y + i;
                    positions.Add(new IntVector2(x, y));
                }
            }
        }
        return positions;
    }

    /**
     * 方法：更新fallingBlockPositions
     */
    private void UpdateFallingBlockPositions()
    {
        _fallingBlockPositions = CalculateFallingBlockPositions();
    }

    /**
     * 方法：方块顺时针旋转
     */
    private void SpinRight(int turn = 1)
    {
        for (int i = 0; i < turn; i++)
        {
            int rows = _blockStatus.GetLength(0); // 原始行数
            int cols = _blockStatus.GetLength(1); // 原始列数
            // 创建新的旋转后的矩阵，行列互换
            int[,] rotated = new int[cols, rows];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int newR = c;
                    int newC = rows - 1 - r;
                    rotated[newR, newC] = _blockStatus[r, c];
                }
            }
            _blockStatus = rotated;
        }
    }

    /**
     * 方法：方块逆时针旋转
     */
    private void SpinLeft(int turn = 1)
    {
        for (int i = 0; i < turn; i++)
        {
            int rows = _blockStatus.GetLength(0); // 原始行数
            int cols = _blockStatus.GetLength(1); // 原始列数
            // 创建新的旋转后的矩阵，行列互换
            int[,] rotated = new int[cols, rows];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int newR = cols - 1 - c;
                    int newC = r;
                    rotated[newR, newC] = _blockStatus[r, c];
                }
            }
            _blockStatus = rotated;
        }
    }
    
    /**
     *  方法/接口：通过偏移量直接操纵blockPosition，也被用于本类的基础移动
     */
    public void MoveBlockPosition(IntVector2 offset, int turn = 1)
    {
        for (int i = 0; i < turn; i++)
            _blockPosition += offset;
        
        UpdateFallingBlockPositions();
    }
    
    
    
    /**
     * 接口：上级获取fallingBlockPositions
     */
    public List<IntVector2> GetFallingBlockPositions()
    {
        UpdateFallingBlockPositions();
        return _fallingBlockPositions;
    }
    
    /**
     * 接口：生成新的掉落方块
     */
    public void CreateNewBlock(string newBlock)
    {
        if (!BlockDictionary.ContainsKey(newBlock))
            return;
        _currentBlock = newBlock;
        _blockStatus = BlockDictionary.Get(_currentBlock).GetShape();
        int offsetY = _blockStatus.GetLength(0) /2;
        int offsetX = _blockStatus.GetLength(1) /2;
        if (_gameType == GlobalConstant.GameType.TypeFourWay)
            _blockPosition = new IntVector2(_width / 2 - offsetX, _height / 2 - offsetY);
        else
        {
            // 因为方块顶部可能会有空行，所以设置偏移量使方块生成时不会有空
            int emptyOffsetY = 0;
            bool isEmpty = false;
            for (int i = 0; i < _blockStatus.GetLength(0); i++)
            {
                isEmpty = true;
                for (int j = 0; j < _blockStatus.GetLength(1); j++)
                    if (_blockStatus[i, j] == 1)
                    {
                        isEmpty = false;
                        break;
                    }
            }
            if (isEmpty)
                emptyOffsetY--;
            _blockPosition = new IntVector2(_width / 2 - offsetX, emptyOffsetY);
        }
                
        UpdateFallingBlockPositions();
    }

    /**
     * 接口：上级获取下落方块备份，包含容器位置，方块状态，方块名称
     */
    public TetrominoDto GetTetrominoBackups()
    {
        UpdateFallingBlockPositions();
        return new TetrominoDto(_currentBlock, _blockPosition, _blockStatus);
    }

    /**
     * 接口：上级回滚下落方块信息
     */
    public void RollbackTetromino(TetrominoDto tetromino)
    {
        _currentBlock = tetromino.GetCurrentBlock();
        _blockPosition = tetromino.GetBlockPosition();
        _blockStatus = tetromino.GetBlockStatus();
        UpdateFallingBlockPositions();
    }
    
    /**
     * 接口：操作预览
     */
    public List<IntVector2> MovePreview(GlobalConstant.BlockOperations operation, int turn = 1)
    {
        // 备份状态
        var backupPos = _blockPosition;
        var backupStatus = _blockStatus;
        // 应用操作（调用 moveLeft/moveRight/spin 等）
        switch (operation)
        {
            case GlobalConstant.BlockOperations.BlockLeft: MoveBlockPosition(new IntVector2(-1, 0), turn); break;
            case GlobalConstant.BlockOperations.BlockRight: MoveBlockPosition(new IntVector2(1, 0), turn); break;
            case GlobalConstant.BlockOperations.BlockUp: MoveBlockPosition(new IntVector2(0, -1), turn); break;
            case GlobalConstant.BlockOperations.BlockDown: MoveBlockPosition(new IntVector2(0, 1), turn); break;
            case GlobalConstant.BlockOperations.BlockSpinRight: SpinRight(turn); break;
            case GlobalConstant.BlockOperations.BlockSpinLeft: SpinLeft(turn); break;
            // fall 不做预览
        }
        // 生成预览位置
        var preview = CalculateFallingBlockPositions();
        // 恢复状态
        _blockPosition = backupPos;
        _blockStatus = backupStatus;
        return preview;
    }

    /**
     * 接口：坐标偏移预览
     */
    public List<IntVector2> MovePositionPreview(IntVector2 offset, int turn = 1)
    {
        var backupPos = _blockPosition;
        var backupStatus = _blockStatus;
        MoveBlockPosition(offset, turn);
        var preview = CalculateFallingBlockPositions();
        _blockPosition = backupPos;
        _blockStatus = backupStatus;
        return preview;
    }
    
    /**
     * 接口：上级根据操作预览确认合法性等因素后，执行指令，turn表示执行次数
     */
    public void Execute(GlobalConstant.BlockOperations operation, int turn = 1)
    {
        switch (operation)
        {
            case GlobalConstant.BlockOperations.BlockLeft: MoveBlockPosition(new IntVector2(-1, 0), turn); break;
            case GlobalConstant.BlockOperations.BlockRight: MoveBlockPosition(new IntVector2(1, 0), turn); break;
            case GlobalConstant.BlockOperations.BlockUp: MoveBlockPosition(new IntVector2(0, -1), turn); break;
            case GlobalConstant.BlockOperations.BlockDown: MoveBlockPosition(new IntVector2(0, 1), turn); break;
            case GlobalConstant.BlockOperations.BlockSpinRight: SpinRight(turn); break;
            case GlobalConstant.BlockOperations.BlockSpinLeft: SpinLeft(turn); break;
            case GlobalConstant.BlockOperations.BlockFall: MoveBlockPosition(new IntVector2(0, 1), turn); break;
        }
        UpdateFallingBlockPositions();
    }
}