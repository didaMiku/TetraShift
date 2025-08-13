using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Tetris.scripts.dto;
using Tetris.scripts.util;

namespace Tetris.scripts.service;


/// <summary>
/// 游戏核心逻辑部分
///	碰撞检测，方块冻结，消行判定，处理tick和用户输入等功能
/// 维护一个场景数组，可由上级获取并渲染
/// </summary>
public class Grid
{
    /**
     * 属性：网格宽度和网格高度
     */
    private readonly int _width;  // X、横、宽
    private readonly int _height; // Y、纵、高

    /**
     * 属性：存储已经固定的底层方块
     */
    private BlockRenderData[,] _deadBlocks;

    /**
     * 属性：存储格式化后的下落方块
     */
    private BlockRenderData[,] _fallingBlocks;

    /**
     * 属性：存储所有场景中的方块
     */
    private BlockRenderData[,] _allBlocks;

    /**
     * 属性：上一个tick是否碰撞，辅助碰撞逻辑
     */
    private bool _isCollided;

    /**
     * 属性：游戏状态
     */
    private GlobalConstant.GameState _gameState;

    /**
     * 属性：当前正在下落的方块
     */
    private string _currentBlock;

    /**
     * 属性：下一个下落方块
     */
    private string _nextBlock;

    /**
     * 属性：Tetromino实例
     */
    private readonly Tetromino _tetromino;

    /**
     * 属性：随机数
     */
    private readonly Random _random;

    /**
     * 属性：分数
     */
    private int _score;

    /**
     * 属性：已经处理过的方块
     */
    private int _handledCount;

    /**
     * 属性：当前重力方向
     */
    private GlobalConstant.GravityDirection _gravityDirection;

    /**
     * 属性：下一个重力方向
     */
    private GlobalConstant.GravityDirection _nextGravityDirection;

    /**
     * 属性：游戏类型
     */
    private GlobalConstant.GameType _gameType;

    /**
     * 构造方法
     */
    public Grid(GlobalConstant.GameType gameType, int width, int height)
    {
        _gameType = gameType;
        _width = width;
        _height = height;
        _deadBlocks = new BlockRenderData[_height, _width];
        _fallingBlocks = new BlockRenderData[_height, _width];
        _allBlocks = new BlockRenderData[_height, _width];
        _isCollided = false;
        _gameState = GlobalConstant.GameState.GameWaiting;
        _currentBlock = _nextBlock = "";
        _tetromino = new Tetromino(_gameType, _width, _height);
        _random = new Random();
        _score = 0;
        _handledCount = 0;
        _gravityDirection = _nextGravityDirection = GlobalConstant.GravityDirection.Down;
    }



    /**
     * 方法：计算下级存储的下落方块
     */
    private BlockRenderData[,] CalculateFallingBlocks()
    {
        var fallingBlocks = new BlockRenderData[_height, _width];
        List<IntVector2> positions = _tetromino.GetFallingBlockPositions();
        foreach (IntVector2 pos in positions)
        {
            int x = pos.X;
            int y = pos.Y;
            if (x >= 0 && x < _width && y >= 0 && y < _height)
            {
                fallingBlocks[y, x] = new BlockRenderData();
                fallingBlocks[y, x].SetRenderType(GlobalConstant.BlockRenderType.RenderShow);
                fallingBlocks[y, x].SetTexturePath(BlockDictionary.Get(_currentBlock).GetTexturePath());
                fallingBlocks[y, x].SetShaderCode(BlockDictionary.Get(_currentBlock).GetShaderCode());
            }
            // 越界的不能插入
        }
        return fallingBlocks;
    }

    /**
     * 方法：更新下落方块
     */
    private void UpdateFallingBlocks()
    {
        _fallingBlocks = CalculateFallingBlocks();
    }

    /**
     * 方法：计算所有方块
     */
    private BlockRenderData[,] CalculateAllBlocks()
    {
        UpdateFallingBlocks();
        var allBlocks = new BlockRenderData[_height, _width];
        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                allBlocks[y, x] = _fallingBlocks[y, x] ?? _deadBlocks[y, x];
        return allBlocks;
    }

    /**
     * 方法：更新所有方块
     */
    private void UpdateAllBlocks()
    {
        _allBlocks = CalculateAllBlocks();
    }

    /**
     * 方法：越界检测
     */
    private bool IsOutOfBounds(List<IntVector2> fallingBlockPositions)
    {
        foreach (IntVector2 pos in fallingBlockPositions)
        {
            if (pos.Y >= _height || pos.Y < 0)
                return true;
            if (pos.X >= _width || pos.X < 0)
                return true;
        }
        return false;
    }

    /**
     * 方法：越界预测
     */
    private bool WillOutOfBounds(GlobalConstant.BlockOperations operation, int turn = 1)
    {
        return IsOutOfBounds(_tetromino.MovePreview(operation, turn));
    }

    /**
     * 方法：重叠检测
     */
    private bool IsOverlap(List<IntVector2> fallingBlockPositions)
    {
        foreach (IntVector2 pos in fallingBlockPositions)
            if (pos.X >= 0 && pos.X < _width && pos.Y >= 0 && pos.Y < _height)
                if (_deadBlocks[pos.Y, pos.X] != null)
                    return true;
        return false;
    }

    /**
     * 方法：重叠预测
     */
    private bool WillOverlap(GlobalConstant.BlockOperations operation, int turn = 1)
    {
        return IsOverlap(_tetromino.MovePreview(operation, turn));
    }

    /**
     * 方法：方块冻结
     */
    private void FreezeFallingBlock()
    {
        for (int y = 0; y < _deadBlocks.GetLength(0); y++)
            for (int x = 0; x < _deadBlocks.GetLength(1); x++)
                _deadBlocks[y, x] = _fallingBlocks[y, x] ?? _deadBlocks[y, x];
        // 重置fallingBlocks
        _fallingBlocks = new BlockRenderData[_height, _width];
    }

    /**
     * 方法：消除满行，通过根据重力方向翻转矩阵实现
     */
    private void ClearFullLinesByGravity()
    {
        var temp = new BlockRenderData[_height, _width];
        int lineCount, cellCount, startLine, lineStep;
        // writeCount用于记录写入的索引
        int writeCount = 0;
        // srcMap控制是否沿对角线翻转，结合物理的上下遍历的方向映射逻辑的四个遍历方向
        // dstMap映射物理行到新行，辅助插入未满行逻辑
        Func<int, int, (int y, int x)> srcMap, dstMap;

        switch (_gravityDirection)
        {
            // 物理从下到上遍历，不翻转，逻辑从下到上
            case GlobalConstant.GravityDirection.Down:
                lineCount = _height; cellCount = _width;
                startLine = _height - 1; lineStep = -1;
                srcMap = (l, c) => (l, c);
                dstMap = (l, c) => (startLine - writeCount, c);
                break;
            // 物理从上到下遍历，不翻转，逻辑从上到下
            case GlobalConstant.GravityDirection.Up:
                lineCount = _height; cellCount = _width;
                startLine = 0; lineStep = +1;
                srcMap = (l, c) => (l, c);
                dstMap = (l, c) => (startLine + writeCount, c);
                break;
            // 物理从上到下遍历，翻转，逻辑从左到右
            case GlobalConstant.GravityDirection.Left:
                lineCount = _width; cellCount = _height;
                startLine = 0; lineStep = +1;
                srcMap = (l, c) => (c, l);
                dstMap = (l, c) => (c, startLine + writeCount);
                break;
            // 物理从下到上遍历，翻转，逻辑从右到左
            case GlobalConstant.GravityDirection.Right:
                lineCount = _width; cellCount = _height;
                startLine = _width - 1; lineStep = -1;
                srcMap = (l, c) => (c, l);
                dstMap = (l, c) => (c, startLine - writeCount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // 通用逻辑
        int fullCount = 0;  // 已经消掉的行数
        int lineIndex = startLine;  // 开始索引
        int halfCount;  // 消行终止位置
        // 终止位置在四向模式中表现为行数一半
        if (_gameType == GlobalConstant.GameType.TypeFourWay) halfCount = lineCount / 2;
        else halfCount = lineCount;
        for (int i = 0; i < lineCount; i++, lineIndex += lineStep)
        {
            bool isFull = true;
            for (int j = 0; j < cellCount; j++)
            {
                var (sy, sx) = srcMap(lineIndex, j);
                if (_deadBlocks[sy, sx] == null || _deadBlocks[sy, sx].GetRenderType() != GlobalConstant.BlockRenderType.RenderShow)
                {
                    isFull = false;
                    break;
                }
            }

            // 已满就触发分数统计
            if (isFull)
            {
                fullCount++;
                _score += fullCount * cellCount;
            }
            // 未满就复制
            else
            {
                // 非满行：分两种情况
                if (i < halfCount)
                {
                    // 在“中点”以内，按原来的下移逻辑
                    for (int j = 0; j < cellCount; j++)
                    {
                        var (sy, sx) = srcMap(lineIndex, j);
                        var (dy, dx) = dstMap(lineIndex, j);  // 基于 writeCount
                        temp[dy, dx] = _deadBlocks[sy, sx];
                    }
                    writeCount++;
                }
                else
                {
                    // 超过“中点”以外，直接原位复制
                    for (int j = 0; j < cellCount; j++)
                    {
                        var (sy, sx) = srcMap(lineIndex, j);
                        temp[sy, sx] = _deadBlocks[sy, sx];
                    }
                }
            }
        }
        _deadBlocks = temp;
    }

    /**
     * 方法：获得下一个方块，随机，不能和上一个相同
     */
    private void GetNewBlock()
    {
        _currentBlock = _nextBlock;
        var dict = BlockDictionary.GetAllKeys();
        List<string> candidates = new List<string>();
        foreach (var key in dict)
            if (!key.Equals(_currentBlock))
                candidates.Add(key);
        if (candidates.Count > 0)
        {
            int index = _random.Next(candidates.Count);
            _nextBlock = candidates[index];
        }
    }

    /**
     * 方法：获得下一个重力方向，随机，不能和上一个相同
     */
    private void GetNewGravityDirection()
    {
        _gravityDirection = _nextGravityDirection;
        if (_gameType == GlobalConstant.GameType.TypeClassic)
        {
            _nextGravityDirection = GlobalConstant.GravityDirection.Down;
            return;
        }
        var directions = Enum.GetValues(typeof(GlobalConstant.GravityDirection));
        List<GlobalConstant.GravityDirection> candidates = new List<GlobalConstant.GravityDirection>();
        foreach (GlobalConstant.GravityDirection direction in directions)
            if (!direction.Equals(_gravityDirection))
                candidates.Add(direction);
        if (candidates.Count > 0)
        {
            int index = _random.Next(candidates.Count);
            _nextGravityDirection = candidates[index];
        }
        else
        {
            _nextGravityDirection = GlobalConstant.GravityDirection.Down;
        }
    }

    /**
     * 方法：创建新方块
     */
    private void SpawnNewBlock()
    {
        GetNewBlock();
        _tetromino.CreateNewBlock(_currentBlock);
        _gameState = GlobalConstant.GameState.GameRunning;
    }

    /**
     * 方法：获取重力方向向量，原点左上角，横x纵y
     */
    private IntVector2 GravityDir()
    {
        switch (_gravityDirection)
        {
            case GlobalConstant.GravityDirection.Down:
                return new IntVector2(0, 1);
            case GlobalConstant.GravityDirection.Up:
                return new IntVector2(0, -1);
            case GlobalConstant.GravityDirection.Left:
                return new IntVector2(-1, 0);
            case GlobalConstant.GravityDirection.Right:
                return new IntVector2(1, 0);
            default:
                return new IntVector2(0, 1);
        }
    }

    /**
     * 方法：获取墙踢偏移量
     */
    private List<IntVector2> GetWallKickOffsetList(bool isClockwise)
    {
        List<IntVector2> offsets = new List<IntVector2>();
        int maxOffset = BlockDictionary.Get(_currentBlock).GetShape().GetLength(1) / 2;

        offsets.Add(new IntVector2(0, 0));  // 原地尝试

        // 定义相对方向
        IntVector2 down = GravityDir();
        IntVector2 right = GravityDir().Rotate90CCW();  // “右” = 重力逆时针
        IntVector2 left = GravityDir().Rotate90CW();    // “左” = 重力顺时针

        for (int i = 1; i <= maxOffset; i++)
        {
            if (isClockwise)
            {
                offsets.Add(left * i);               // 左移
                offsets.Add(right * i);              // 右移
                offsets.Add(left * i + down * i);    // 左下
                offsets.Add(down * i);               // 下移
                offsets.Add(right * i + down * i);   // 右下
            }
            else
            {
                offsets.Add(right * i);              // 右移
                offsets.Add(left * i);               // 左移
                offsets.Add(right * i + down * i);   // 右下
                offsets.Add(down * i);               // 下移
                offsets.Add(left * i + down * i);    // 左下
            }
        }
        return offsets;
    }

    /**
     * 方法：返回当前重力对应的方块下落操作方向
     */
    private GlobalConstant.BlockOperations GetFallingOperation()
    {
        return _gravityDirection switch
        {
            GlobalConstant.GravityDirection.Down => GlobalConstant.BlockOperations.BlockDown,
            GlobalConstant.GravityDirection.Up => GlobalConstant.BlockOperations.BlockUp,
            GlobalConstant.GravityDirection.Left => GlobalConstant.BlockOperations.BlockLeft,
            GlobalConstant.GravityDirection.Right => GlobalConstant.BlockOperations.BlockRight,
            _ => GlobalConstant.BlockOperations.BlockDown,
        };
    }

    /**
     * 方法；返回当前重力对应的方块的反向操作方向
     */
    private GlobalConstant.BlockOperations GetReverseFallingOperation()
    {
        return _gravityDirection switch
        {
            GlobalConstant.GravityDirection.Down => GlobalConstant.BlockOperations.BlockUp,
            GlobalConstant.GravityDirection.Up => GlobalConstant.BlockOperations.BlockDown,
            GlobalConstant.GravityDirection.Left => GlobalConstant.BlockOperations.BlockRight,
            GlobalConstant.GravityDirection.Right => GlobalConstant.BlockOperations.BlockLeft,
            _ => GlobalConstant.BlockOperations.BlockDown,
        };
    }

    /**
     * 方法：封装冻结消行新方块的逻辑
     */
    private void CreateNewBlock()
    {
        // 冻结
        FreezeFallingBlock();
        // 消行
        ClearFullLinesByGravity();
        // 获得新重力
        GetNewGravityDirection();
        // 生成新方块
        SpawnNewBlock();
        var backups = _tetromino.GetTetrominoBackups();
        var offsets = GetWallKickOffsetList(true);
        // 遍历墙踢列表，找到能创建新方块的位置，否则游戏结束
        foreach (IntVector2 offset in offsets)
        {
            var preview = _tetromino.MovePositionPreview(offset);
            if (!IsOverlap(preview) && !IsOutOfBounds(preview))
            {
                _tetromino.MoveBlockPosition(offset);
                _handledCount++;
                return;
            }
        }
        _gameState = GlobalConstant.GameState.GameOver;
    }

    /**
     * 方法/接口：返回一个当前状态的BlockRenderDto
     */
    public BlockRenderDto GetBlockRenderDto() {
        return new BlockRenderDto(_allBlocks, _gameState, _score, _handledCount, _currentBlock, _nextBlock, _gravityDirection, _nextGravityDirection);
    }




    /**
     * 接口：tick和用户输入都能处理的通用接口，返回dto
     */
    public BlockRenderDto HandleOperation(GlobalConstant.BlockOperations operation)
    {
        // 预处理新开始的游戏
        if (_gameState == GlobalConstant.GameState.GameWaiting)
        {
            GetNewBlock();
            GetNewGravityDirection();
            SpawnNewBlock();
            _gameState = GlobalConstant.GameState.GameRunning;
            UpdateAllBlocks();
            return GetBlockRenderDto();
        }
        // 游戏结束和游戏暂停
        if (_gameState == GlobalConstant.GameState.GameOver || _gameState == GlobalConstant.GameState.GamePause)
            return GetBlockRenderDto();

        UpdateAllBlocks();
        GlobalConstant.BlockOperations fallingOperation = GetFallingOperation();
        GlobalConstant.BlockOperations reverseFallingOperation = GetReverseFallingOperation();
        // 是移动操作
        if (operation != GlobalConstant.BlockOperations.BlockSpinRight && operation != GlobalConstant.BlockOperations.BlockSpinLeft)
        {
            // 向重力方向移动
            if (operation == fallingOperation || operation == GlobalConstant.BlockOperations.BlockTick)
            {
                // 发生了碰撞
                if (WillOverlap(fallingOperation) || WillOutOfBounds(fallingOperation))
                    // 上一tick已经碰撞
                    if (_isCollided)
                        CreateNewBlock();
                    else
                        _isCollided = true;
                else
                    _tetromino.Execute(fallingOperation);
            }
            // 向重力方向掉落
            else if (operation == GlobalConstant.BlockOperations.BlockFall)
            {
                while (!WillOverlap(fallingOperation) && !WillOutOfBounds(fallingOperation))
                    _tetromino.Execute(fallingOperation);
                UpdateFallingBlocks();
                CreateNewBlock();
            }
            // 左右移动
            else if (operation != reverseFallingOperation)
            {
                if (!WillOverlap(operation) && !WillOutOfBounds(operation))
                    _tetromino.Execute(operation);
            }
        }
        // 向反重力方向移动非法，不予处理
        else if (operation == reverseFallingOperation) { }
        // 是旋转操作
        else
        {
            bool isClockwise = operation == GlobalConstant.BlockOperations.BlockSpinRight;
            var offsetList = GetWallKickOffsetList(isClockwise);
            var backups = _tetromino.GetTetrominoBackups();
            _tetromino.Execute(operation);
            bool isAble = false;
            foreach (IntVector2 offset in offsetList)
            {
                var preview = _tetromino.MovePositionPreview(offset);
                if (!IsOverlap(preview) && !IsOutOfBounds(preview))
                {
                    _tetromino.MoveBlockPosition(offset);
                    isAble = true;
                    break;
                }
            }
            if (!isAble)
                _tetromino.RollbackTetromino(backups);
        }
        UpdateAllBlocks();
        _isCollided = WillOutOfBounds(fallingOperation) || WillOverlap(fallingOperation);
        return GetBlockRenderDto();
    }

    /**
     * 接口：处理方块字典的增删改查
     */
    public void UpdateBlockDictionary() {}

    /**
     * 接口：存档系统
     */
    public void ArchiveSystem() {}
}
