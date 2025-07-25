using System;
using System.Text;
using Godot;
using Tetris.scripts.dto;

namespace Tetris.scripts.util;


/// <summary>
/// 打印方块渲染信息数组的辅助类
/// </summary>
public static class PrintUtils
{
    public static void PrintBlockRenderDataArray(BlockRenderData[,] blockRenderArray, int height, int width)
    {
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (blockRenderArray[y, x] != null)
                {
                    sb.Append("[" + x + "," + y + "]:");
                    sb.Append(blockRenderArray[y, x].GetRenderTypeString());
                }
                else
                {
                    sb.Append("[" + x + "," + y + "]:null");
                }
            }
            sb.AppendLine();
        }
        GD.Print(sb.ToString());
    }
}
