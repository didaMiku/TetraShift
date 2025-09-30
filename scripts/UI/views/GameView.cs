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
    private GameContainer _gameContainer;
    private ButtonContainer _buttonContainer;
    private SideBarContainer _sideBarContainer;
    private PausePainter _pausePainter;
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
     * 各个区域的像素长宽和边界宽度
     */
    private int _gameAreaPixelWidth;
    private int _gameAreaPixelHeight;
    private int _sideBarAreaPixelWidth;
    private int _sideBarAreaPixelHeight;
    private int _buttonAreaPixelWidth;
    private int _buttonAreaPixelHeight;
    private int _border;
    /**
     * 时间钟表
     */
    private Timer _clock;
    /**
     * 场景锚点
     */
    private Vector2 _anchorPoint;

    public GameView()
    {
        _gameContainer = new GameContainer();
        _buttonContainer = new ButtonContainer();
        _sideBarContainer = new SideBarContainer();
        _pausePainter = new PausePainter();
        _gameAreaPixelWidth = 450;
        _gameAreaPixelHeight = 600;
        _sideBarAreaPixelWidth = 150;
        _sideBarAreaPixelHeight = 600;
        _buttonAreaPixelWidth = 600;
        _buttonAreaPixelHeight = 300;
        _border = 1;
        _clock = new Timer();
        _anchorPoint = new Vector2(0, 0);
    }

    public override void _Ready()
    {
        // 将四个子节点和时钟添加到当前节点，指定位置
        AddChild(_gameContainer);
        AddChild(_buttonContainer);
        AddChild(_sideBarContainer);
        AddChild(_clock);
        
        // 读取配置文件，获得方块字典
        JsonService.InitializeUserData();

        // 读取配置文件，获得长宽信息和游戏类型
        var _gameConfigDto = JsonService.GetGameConfig();
        _width = _gameConfigDto.GetWidth();
        _height = _gameConfigDto.GetHeight();
        _gameType = _gameConfigDto.GetGameType();

        // 实例化并初始化游戏逻辑
        _grid = new Grid(_gameType, _width, _height);
        var blockRenderDto = _grid.HandleOperation(GlobalConstant.BlockOperations.BlockDown);

        // 各个绘制节点初始化
        _gameContainer.SetGameContainer(_gameAreaPixelWidth, _gameAreaPixelHeight, 2, _width, _height);
        _buttonContainer.SetButtonContainer(_buttonAreaPixelWidth, _buttonAreaPixelHeight, 2);
        _sideBarContainer.SetSideBarContainer(_sideBarAreaPixelWidth, _sideBarAreaPixelHeight, 2);

        // 绘制初始化后界面
        RenderGameScene(blockRenderDto);
        _buttonContainer.PaintButton();

        // 设置各节点位置
        _gameContainer.Position = new Vector2(0, 0);
        _buttonContainer.Position = new Vector2(0, _gameAreaPixelHeight);
        _sideBarContainer.Position = new Vector2(_gameAreaPixelWidth, 0);

        // 初始化 Timer
        _clock.WaitTime = 1.0f;   // 默认 1 秒
        _clock.OneShot = false;   // 循环
        _clock.Timeout += OnTick; // 触发时执行
        StartClock();
    }


    /**
     * 处理游戏时钟
     */
    public void HandleTick()
    {
        var blockRenderDto = _grid.HandleOperation(GlobalConstant.BlockOperations.BlockTick);
        RenderGameScene(blockRenderDto);
    }
    /**
     * 处理输入
     */
    public void HandleInput(GlobalConstant.BlockOperations blockOperations)
    {
        var blockRenderDto = _grid.HandleOperation(blockOperations);
        RenderGameScene(blockRenderDto);
    }

    /**
     * 渲染游戏，也就是重绘一遍方块和侧边栏
     */
    private void RenderGameScene(BlockRenderDto blockRenderDto)
    {
        _gameContainer.PaintBlocks(blockRenderDto.GetBlockRenderArray());
        _sideBarContainer.PaintSideBar(blockRenderDto);
    }


    /**
     * 时钟控制方法
     */
    public void GamePause()
    {
        if (_pausePainter.GetParent() == this) return;
        _clock.Stop();
        AddChild(_pausePainter);
    }
    public void GameContinue()
    {
        _clock.Start();
        RemoveChild(_pausePainter);
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
