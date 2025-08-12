using System.Text;
using Tetris.scripts.util;

namespace Tetris.scripts.dto;


/// <summary>
/// 游戏状态Dto。
/// 传递给前端渲染场景需要的所有信息。
/// </summary>
public class GameStateDto
{
    private int[,] _allBlocks;
    private GlobalConstant.GameState _gameState;
    private int _score;
    private string _currentBlock;
    private string _nextBlock;
    private GlobalConstant.GravityDirection _currentGravity;
    private GlobalConstant.GravityDirection _nextGravity;
    private int _handledCount;

    public GameStateDto(int[,] allBlocks, GlobalConstant.GameState gameState, int score, string currentBlock, string nextBlock,
        GlobalConstant.GravityDirection currentGravity, GlobalConstant.GravityDirection nextGravity, int handledCount)
    {
        _allBlocks = allBlocks;
        _gameState = gameState;
        _score = score;
        _currentBlock = currentBlock;
        _nextBlock = nextBlock;
        _currentGravity = currentGravity;
        _nextGravity = nextGravity;
        _handledCount = handledCount;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Score: {_score}");
        sb.AppendLine($"GameState: {_gameState}");
        sb.AppendLine($"Current Block: {_currentBlock}");
        sb.AppendLine($"Next Block: {_nextBlock}");
        sb.AppendLine($"Current Gravity: {_currentGravity}");
        sb.AppendLine($"Next Gravity: {_nextGravity}");
        sb.AppendLine($"Handled Count: {_handledCount}");
        sb.AppendLine("All Blocks:");

        int rows = _allBlocks.GetLength(0);
        int cols = _allBlocks.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (_allBlocks[y, x] == 0)
                    sb.Append("· ");
                else
                    sb.Append("# ");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public int[,] GetAllBlocks() => _allBlocks;
    public void SetAllBlocks(int[,] allBlocks) => _allBlocks = allBlocks;

    public GlobalConstant.GameState GetGameState() => _gameState;
    public void SetGameState(GlobalConstant.GameState gameState) => _gameState = gameState;

    public int GetScore() => _score;
    public void SetScore(int score) => _score = score;

    public string GetCurrentBlock() => _currentBlock;
    public void SetCurrentBlock(string currentBlock) => _currentBlock = currentBlock;

    public string GetNextBlock() => _nextBlock;
    public void SetNextBlock(string nextBlock) => _nextBlock = nextBlock;

    public GlobalConstant.GravityDirection GetCurrentGravity() => _currentGravity;
    public void SetCurrentGravity(GlobalConstant.GravityDirection currentGravity) => _currentGravity = currentGravity;

    public GlobalConstant.GravityDirection GetNextGravity() => _nextGravity;
    public void SetNextGravity(GlobalConstant.GravityDirection nextGravity) => _nextGravity = nextGravity;

    public int GetHandledCount() => _handledCount;
    public void SetHandledCount(int handledCount) => _handledCount = handledCount;
}
