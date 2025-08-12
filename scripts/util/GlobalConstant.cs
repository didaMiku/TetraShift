namespace Tetris.scripts.util;


/// <summary>
/// 全局常量，封装方块字典，操作等基本信息，保证信息传递安全性
/// </summary>
public static class GlobalConstant
{
    /**
     * 方块渲染类型
     */
    public enum BlockRenderType
    {
        RenderShow,   // 显示方块
        RenderHide,   // 不显示方块
        RenderShadow  // 显示虚化
    }

    /**
     * 方块操作
     */
    public enum BlockOperations
    {
        BlockLeft,
        BlockRight,
        BlockUp,
        BlockDown,
        BlockFall,
        BlockSpinLeft,
        BlockSpinRight,
        BlockTick
    }

    /**
     * 所有输入类型，包括游戏控制
     */
    public enum InputOperations
    {
        InputTick,  // 每帧输入
        InputLeft,
        InputRight,
        InputUp,
        InputDown,
        InputFall,
        InputSpinLeft,
        InputSpinRight,
        InputPause,
        InputStart,
        InputSave
    }

    public enum GravityDirection
    {
        Down,   // 向下
        Up,     // 向上
        Left,   // 向左
        Right   // 向右
    }

    /**
     * 游戏运行状态
     */
    public enum GameState
    {
        GameRunning,    // 游戏正在运行
        GameOver,       // 游戏结束
        GamePause,      // 游戏暂停
        GameWaiting     // 初始化完成游戏等待
    }

    /**
     * 游戏运行类型
     */
    public enum GameType
    {
        TypeClassic,
        TypeFourWay,
        TypeDebug
    }

    /**
     * UI状态枚举
     */
    public enum UISceneState
    {
        SceneMenu,          // 首页
        SceneGame,          // 游戏界面
        ScenePause,         // 暂停界面
        SceneGameConfig,    // 游戏设置界面
        SceneBlockConfig,   // 方块设置界面
        SceneGameSave       // 游戏存档界面
    }

    public static string GetGravityTexturePath(GravityDirection gravityDirection)
    {
        switch (gravityDirection)
        {
            case GravityDirection.Down:
                return "res://assets/textures/down.png";
            case GravityDirection.Up:
                return "res://assets/textures/up.png";
            case GravityDirection.Left:
                return "res://assets/textures/left.png";
            case GravityDirection.Right:
                return "res://assets/textures/right.png";
            default:
                return "res://assets/textures/down.png";
        }
    }

    public const string DEFAULT_TEXTURE_PATH = "res://assets/textures/Tetris_black_block.png"; 

    public const string DEFAULT_GRID_PATH = "res://assets/grid/Tetris_black_grid.png";
}