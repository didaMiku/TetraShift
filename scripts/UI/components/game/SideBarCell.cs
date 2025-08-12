using System;
using Godot;
using System.Linq;

namespace Tetris.scripts.UI.components.game;

public partial class SideBarCell : Node2D
{
    private int _labelHeight;
    private int _objectHeight;
    private int _width;
    private Node2D _labelNode;
    private Node2D _objectNode;

    public SideBarCell()
    {
        _labelHeight = _objectHeight = _width = 0;
        _labelNode = new Node2D();
        _objectNode = new Node2D();
    }
    public SideBarCell(int labelHeight, int objectHeight, int width, Node2D labelNode, Node2D objectNode)
    {
        _labelHeight = labelHeight;
        _objectHeight = objectHeight;
        _width = width;
        _labelNode = labelNode;
        _objectNode = objectNode;
        AddChild(_labelNode);
        AddChild(_objectNode);
    }

    /**
     * 清空所有子节点
     */
    private void ClearAllChildren()
    {
        foreach (Node node in GetChildren().ToArray())
        {
            RemoveChild(node);
            node.QueueFree();
        }
    }
}