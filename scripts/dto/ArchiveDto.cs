namespace Tetris.scripts.dto;


/// <summary>
/// 保存游戏Dto
/// </summary>
public class GameSaveDto
{
    private BlockRenderData[,] _deadBlocks;
    private int _score;
    private int _handledCount;
    private string _currentBlock;
    private string _nextBlock;

    public GameSaveDto(BlockRenderData[,] deadBlocks, int score, int handledCount, string currentBlock,
        string nextBlock)
    {
        _deadBlocks = deadBlocks;
        _score = score;
        _handledCount = handledCount;
        _currentBlock = currentBlock;
        _nextBlock = nextBlock; 
    }
    
    public BlockRenderData[,] GetDeadBlocks() => _deadBlocks;
    public void SetDeadBlocks(BlockRenderData[,] deadBlocks) => _deadBlocks = deadBlocks;
    
    public int GetScore() => _score;
    public void SetScore(int score) => _score = score;
    
    public int GetHandledCount() => _handledCount;
    public void SetHandledCount(int handledCount) => _handledCount = handledCount;
    
    public string GetCurrentBlock() => _currentBlock;
    public void SetCurrentBlock(string currentBlock) => _currentBlock = currentBlock;
    
    public string GetNextBlock() => _nextBlock;
    public void SetNextBlock(string nextBlock) => _nextBlock = nextBlock;
}