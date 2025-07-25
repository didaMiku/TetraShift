using System.Collections.Generic;
using System.Linq;
using Tetris.scripts.dto;

namespace Tetris.scripts.util;

/// <summary>
/// 方块信息字典。
/// 包括了添加，获取，清空，删除，查询等方法
/// </summary>
public static class BlockDictionary
{
    private static Dictionary<string, BlockData> _data = new();

    public static bool Add(string name, BlockData data)
    {
        if (!_data.ContainsKey(name))
        {
            _data[name] = data;
            return true;
        }
        return false;
    }

    public static BlockData Get(string name)
    {
        return _data.ContainsKey(name) ? _data[name] : null;
    }

    public static void Clear()
    {
        _data.Clear();
    }

    public static bool Remove(string name)
    {
        return _data.Remove(name);
    }

    public static IEnumerable<BlockData> GetAll()
    {
        return _data.Values;
    }

    public static List<string> GetAllKeys()
    {
        return _data.Keys.ToList();
    }

    public static bool ContainsKey(string name)
    {
        return _data.ContainsKey(name);
    }
}