using System.Text;
using Tetris.scripts.util;

namespace Tetris.scripts.dto;


/// <summary>
/// 方块渲染Dto。
/// 传给前端供其每帧渲染一次，同时传递其它参数。
/// </summary>
public class BlockRenderDto
{
    private BlockRenderData[,] _blockRenderArray;
    private GlobalConstant.GameState _gameState;
    private int _score;
    private int _handledCount;
    private string _currentBlock;
    private string _nextBlock;
    private GlobalConstant.GravityDirection _gravityDirection;
    private GlobalConstant.GravityDirection _nextGravityDirection;

    public BlockRenderDto(BlockRenderData[,] blockRenderArray, GlobalConstant.GameState gameState, int score, int handledCount,
        string currentBlock, string nextBlock, GlobalConstant.GravityDirection gravityDirection, GlobalConstant.GravityDirection nextGravityDirection)
    {
        _blockRenderArray = blockRenderArray;
        _gameState = gameState;
        _score = score;
        _handledCount = handledCount;
        _currentBlock = currentBlock;
        _nextBlock = nextBlock;
        _gravityDirection = gravityDirection;
        _nextGravityDirection = nextGravityDirection;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"GameState: {_gameState}");
        sb.AppendLine($"Score: {_score}");
        sb.AppendLine($"Handled Count: {_handledCount}");
        sb.AppendLine($"Current Block: {_currentBlock}");
        sb.AppendLine($"Next Block: {_nextBlock}");
        sb.AppendLine($"Gravity Direction: {_gravityDirection}");
        sb.AppendLine($"Next Gravity Direction: {_nextGravityDirection}");
        sb.AppendLine("All Blocks:");

        int rows = _blockRenderArray.GetLength(0);
        int cols = _blockRenderArray.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (_blockRenderArray[y, x] == null)
                    sb.Append("· ");
                else
                    sb.Append("# ");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
    
    public BlockRenderData[,] GetBlockRenderArray() => _blockRenderArray;
    public void SetBlockRenderArray(BlockRenderData[,] blockRenderArray) => _blockRenderArray = blockRenderArray;

    public GlobalConstant.GameState GetGameState() => _gameState;
    public void SetGameState(GlobalConstant.GameState gameState) => _gameState = gameState;
    
    public int GetScore() => _score;
    public void SetScore(int score) => _score = score;
    
    public string GetCurrentBlock() => _currentBlock;
    public void SetCurrentBlock(string currentBlock) => _currentBlock = currentBlock;
    
    public string GetNextBlock() => _nextBlock;
    public void SetNextBlock(string nextBlock) => _nextBlock = nextBlock;

    public string GetGravityDirection() => _gravityDirection.ToString();
    public void SetGravityDirection(GlobalConstant.GravityDirection gravityDirection) => _gravityDirection = gravityDirection;

    public string GetNextGravityDirection() => _nextGravityDirection.ToString();
    public void SetNextGravityDirection(GlobalConstant.GravityDirection nextGravityDirection) => _nextGravityDirection = nextGravityDirection;
    
    public int GetHandledCount() => _handledCount;
    public void SetHandledCount(int handledCount) => _handledCount = handledCount;
}