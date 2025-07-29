using System;
using System.Collections.Generic;
using Godot;
using Tetris.scripts.util;

namespace Tetris.scripts.UI;

/// <summary>
/// UI部分最上层的UI路由，负责UI切换
/// </summary>
public partial class MainNode : Control
{
    /**
     * 当前界面
     */
    private GlobalConstant.UISceneState currentState;
    /**
     * 界面字典，简化切换逻辑
     */
    private Dictionary<GlobalConstant.UISceneState, Control> sceneMap;

    /**
     * 初始化，绑定UI，切换到菜单
     */
    public override void _Ready()
    {
        sceneMap = new Dictionary<GlobalConstant.UISceneState, Control>
        {
            { GlobalConstant.UISceneState.SceneMenu, GetNode<Control>("MenuScene") },
            { GlobalConstant.UISceneState.SceneGame, GetNode<Control>("GameScene") },
            { GlobalConstant.UISceneState.ScenePause, GetNode<Control>("GameScene/PauseScene") },
            { GlobalConstant.UISceneState.SceneGameConfig, GetNode<Control>("GameConfigScene") },
            { GlobalConstant.UISceneState.SceneBlockConfig, GetNode<Control>("BlockConfigScene") },
            { GlobalConstant.UISceneState.SceneGameSave, GetNode<Control>("GameSaveScene") },
        };

        SwitchTo(GlobalConstant.UISceneState.SceneMenu);
    }

    /**
     * 切换到另一界面
     */
    public void SwitchTo(GlobalConstant.UISceneState newState)
    {
        // 先隐藏所有
        foreach (var scene in sceneMap.Values)
        {
            scene.Visible = false;
            scene.SetProcessInput(false);
        }
        currentState = newState;
        // 再显示目标
        sceneMap[newState].Visible = true;
        sceneMap[newState].SetProcessInput(true);
    }
}

