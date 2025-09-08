using System;
using System.Runtime.Versioning;
using Godot;
using Tetris.scripts.dto;
using Tetris.scripts.service;
using Tetris.scripts.UI.components.game;
using Tetris.scripts.util;

namespace Tetris.scripts.UI.views;

/// <summary>
/// 显示游戏界面的UI顶层，
/// 界面的比例固定
/// </summary>
public partial class GameView : Node2D
{
    /**
     * 游戏各个部分的UI绘制节点
     */
    private BlockPainter _blockPainter;
    private GridPainter _gridPainter;
    private SideBarPainter _sideBarPainter;
    private ButtonPainter _buttonPainter;
    /**
     * 底层逻辑
     */
    private Grid _grid;
    /**
     * 游戏的长宽以及类型，游戏的初始化配置
     */
    private int _width;
    private int _height;
    private GlobalConstant.GameType _gameType;
    /**
     * 各个区域的像素长宽
     */
    private int _gameAreaPixelWidth;
    private int _gameAreaPixelHeight;
    private int _sideBarAreaPixelWidth;
    private int _sideBarAreaPixelHeight;
    private int _buttonAreaPixelWidth;
    private int _buttonAreaPixelHeight;
    /**
     * 时间钟表
     */
    private Timer _clock;

    public GameView()
    {
        _blockPainter = new BlockPainter();
        _gridPainter = new GridPainter();
        _sideBarPainter = new SideBarPainter();
        _buttonPainter = new ButtonPainter();
        _clock = new Timer();
        _gameAreaPixelWidth = 400;
        _gameAreaPixelHeight = 600;
        _sideBarAreaPixelWidth = 200;
        _sideBarAreaPixelHeight = 600;
        _buttonAreaPixelWidth = 600;
        _buttonAreaPixelHeight = 300;
    }

    public override void _Ready()
    {
        // 读取配置文件，获得方块字典
        JsonService.InitializeUserData();

        // 读取配置文件，获得长宽信息和游戏类型
        var _gameConfigDto = JsonService.GetGameConfig();
        _width = _gameConfigDto.GetWidth();
        _height = _gameConfigDto.GetHeight();
        _gameType = _gameConfigDto.GetGameType();

        // 根据长宽信息求得方块尺寸
        var blockSize = Math.Min(_gameAreaPixelWidth / _width, _gameAreaPixelHeight / _height);

        // 绘制网格
        _gridPainter.SetGridPainter(_width, _height, blockSize);

        // 实例化游戏逻辑
        _grid = new Grid(_gameType, _width, _height);
        var blockRenderDto = _grid.HandleOperation(GlobalConstant.BlockOperations.BlockDown);

        // 绘制方块
        _blockPainter.SetBlockPainter(blockSize, blockRenderDto.GetBlockRenderArray());

        // 初始化 Timer
        _clock.WaitTime = 1.0f;   // 默认 1 秒
        _clock.OneShot = false;   // 循环
        _clock.Timeout += OnTick; // 触发时执行

        // 将四个子节点和时钟添加到当前节点，指定位置
        AddChild(_gridPainter);
        AddChild(_blockPainter);
        AddChild(_sideBarPainter);
        AddChild(_buttonPainter);
        AddChild(_clock);

        _gridPainter.Position = new Vector2(0, 0);
        _blockPainter.Position = new Vector2(0, 0);
        _sideBarPainter.Position = new Vector2(_gameAreaPixelWidth, 0);
        _buttonPainter.Position = new Vector2(0, _gameAreaPixelHeight);

        StartClock();
    }

    public void HandleTick()
    {
        var blockRenderDto = _grid.HandleOperation(GlobalConstant.BlockOperations.BlockTick);
        RenderGameScene(blockRenderDto);
    }
    public void HandleInput(GlobalConstant.BlockOperations blockOperations)
    {
        var blockRenderDto = _grid.HandleOperation(blockOperations);
        RenderGameScene(blockRenderDto);
    }



    private void RenderGameScene(BlockRenderDto blockRenderDto)
    {
        _blockPainter.SetBlockPainter(blockRenderDto.GetBlockRenderArray());
        _sideBarPainter.SetBlockRenderDto(blockRenderDto);
    }


    public void GamePause()
    {
        _clock.Stop();
        // 添加暂停UI绘制节点
    }
    public void GameContinue()
    {
        _clock.Start();
        // 删除暂停UI绘制节点
    }
    private void OnTick()
    {
        GD.Print("Clock Tick");
        HandleTick();
    }

    private void StartClock()
    {
        _clock.Start();
    }

    private void PauseClock()
    {
        _clock.Stop();
    }

    private void SetClockInterval(float seconds)
    {
        _clock.WaitTime = seconds;
        if (!_clock.IsStopped())
            _clock.Start(); // 保证新间隔立即生效
    }
}
