using Tetris.scripts.util;

namespace Tetris.scripts.dto;

/// <summary>
/// 传递下落方块信息的Dto，用于上级备份和回滚Tetromino
/// </summary>
public class TetrominoDto
{
    private string _currentBlock;
    private IntVector2 _blockPosition;
    private int[,] _blockStatus;

    public TetrominoDto(string currentBlock, IntVector2 blockPosition, int[,] blockStatus)
    {
        _currentBlock = currentBlock;
        _blockPosition = blockPosition;
        _blockStatus = DeepCopyBlockStatus(blockStatus);    // 此处使用深拷贝
    }
    
    // 深拷贝方法，因为是备份，数组类型的数据不能使用直接赋值的浅拷贝
    private int[,] DeepCopyBlockStatus(int[,] source)
    {
        int rows = source.GetLength(0);
        int cols = source.GetLength(1);
        int[,] copy = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            copy[i, j] = source[i, j];
        return copy;
    }

    public string GetCurrentBlock() => _currentBlock;
    public void SetCurrentBlock(string currentBlock) => _currentBlock = currentBlock;

    public IntVector2 GetBlockPosition() => _blockPosition;
    public void SetBlockPosition(IntVector2 blockPosition) => _blockPosition = blockPosition;

    public int[,]GetBlockStatus() => DeepCopyBlockStatus(_blockStatus);
    public void SetBlockStatus(int[,] blockStatus) => _blockStatus = DeepCopyBlockStatus(blockStatus);
}