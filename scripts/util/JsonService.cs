using System.IO;
using Godot;
using GDict = Godot.Collections.Dictionary;
using GArray = Godot.Collections.Array;
using Tetris.scripts.dto;


namespace Tetris.scripts.util;

/// <summary>
/// BlockJsonService 提供对用户数据 UserData.json 的操作服务。
/// 包括初始化、重置、加载和保存方块数据。
/// </summary>
public static class JsonService
{
    /**
     *用户数据路径：每个用户 OS.GetUserDataDir() 下的 UserData.json
     */ 
    private static readonly string UserDataPath = Path.Combine(OS.GetUserDataDir(), "UserData.json");
    /**
     * 打包资源路径：项目 res:// 根目录下的 DefaultData.json
     */
    private const string DefaultDataPath = "res://DefaultData.json";


    /**
     * 初始化：如果不存在 UserData.json，则创建并把 DefaultData.json 的 DefaultBlocks 覆盖到 Blocks。
     */
    public static void InitializeUserData()
    {
        if (Godot.FileAccess.FileExists(UserDataPath))
        {
            GD.PushWarning("UserData.json 已存在，地址：" + UserDataPath);
            return;
        }

        // 读取打包的 DefaultData.json
        string defaultJson = Godot.FileAccess
            .Open(DefaultDataPath, Godot.FileAccess.ModeFlags.Read)
            .GetAsText();

        // 解析
        var parser = new Json();
        Error err = parser.Parse(defaultJson);
        if (err != Error.Ok)
        {
            GD.PushError($"解析 DefaultData.json 失败：{parser.GetErrorMessage()} （行 {parser.GetErrorLine()}）");
            return;
        }

        // 拿到根字典并取出 DefaultBlocks
        var defaultRoot = (GDict)parser.Data;
        var defaultBlocks = defaultRoot.ContainsKey("DefaultBlocks")
            ? (GArray)defaultRoot["DefaultBlocks"]
            : new GArray();

        // 构造用户数据，只包含 Blocks
        var userRoot = new GDict
        {
            ["Blocks"] = defaultBlocks
        };

        // 写入用户目录
        WriteJsonToFile(UserDataPath, userRoot);
    }

    /**
     * 重置：强制删除/覆盖 UserData.json，把 DefaultData.json 的 DefaultBlocks 写入 Blocks。
     */
    public static void ResetUserDataToDefault()
    {
        // 读取 DefaultData.json
        string defaultJson = Godot.FileAccess
            .Open(DefaultDataPath, Godot.FileAccess.ModeFlags.Read)
            .GetAsText();

        // 解析
        var parser = new Json();
        Error err = parser.Parse(defaultJson);
        if (err != Error.Ok)
        {
            GD.PushError($"解析 DefaultData.json 失败：{parser.GetErrorMessage()} （行 {parser.GetErrorLine()}）");
            return;
        }

        // 取 DefaultBlocks
        var defaultRoot = (GDict)parser.Data;
        var defaultBlocks = defaultRoot.ContainsKey("DefaultBlocks")
            ? (GArray)defaultRoot["DefaultBlocks"]
            : new GArray();

        // 构造并覆盖写入
        var userRoot = new GDict
        {
            ["Blocks"] = defaultBlocks
        };
        WriteJsonToFile(UserDataPath, userRoot);
    }

    /**
     * 读取：把 UserData.json 的 Blocks 字段解析成 BlockData，填充到全局 BlockDictionary。
     */
    public static void LoadBlocksToDictionary()
    {
        if (!Godot.FileAccess.FileExists(UserDataPath))
        {
            GD.PushWarning("UserData.json 不存在，正在创建默认用户数据…");
            InitializeUserData();
        }

        // 读取文本
        string jsonText = Godot.FileAccess
            .Open(UserDataPath, Godot.FileAccess.ModeFlags.Read)
            .GetAsText();

        // 解析
        var parser = new Json();
        Error err = parser.Parse(jsonText);
        if (err != Error.Ok)
        {
            GD.PushError($"解析 UserData.json 失败：{parser.GetErrorMessage()} （行 {parser.GetErrorLine()}）");
            return;
        }

        // 根字典
        var root = (GDict)parser.Data;

        // 取 Blocks
        var blocks = root.ContainsKey("Blocks")
            ? (GArray)root["Blocks"]
            : new GArray();

        // 清空现有字典
        BlockDictionary.Clear();

        // 反序列化到 BlockDictionary
        foreach (var obj in blocks)
        {
            var item = (GDict)obj;
            string name = item["name"].ToString();
            string texturePath = item["texturePath"].ToString();
            string shaderCode = item["shaderCode"].ToString();
            var shapeArr = (GArray)item["shape"];

            int rows = shapeArr.Count;
            int cols = ((GArray)shapeArr[0]).Count;
            var shape = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                var row = (GArray)shapeArr[i];
                for (int j = 0; j < cols; j++)
                    shape[i, j] = (int)(long)row[j];
            }

            var data = new BlockData(name, shape, texturePath, shaderCode);
            BlockDictionary.Add(name, data);
        }
    }
    
    /**
     * 保存：将全局 BlockDictionary 序列化后写回 UserData.json 的 Blocks 字段。
     */
    public static void SaveDictionaryToUserData()
    {
        var blocksArray = new GArray();

        // 把字典里的每个 BlockData 转成 Dictionary
        foreach (var data in BlockDictionary.GetAll())
        {
            var shape = data.GetShape();
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);
            var shapeArr = new GArray();
            for (int i = 0; i < rows; i++)
            {
                var row = new GArray();
                for (int j = 0; j < cols; j++)
                    row.Add(shape[i, j]);
                shapeArr.Add(row);
            }

            var dict = new GDict
            {
                ["name"] = data.GetName(),
                ["texturePath"] = data.GetTexturePath(),
                ["shaderCode"] = data.GetShaderCode(),
                ["shape"] = shapeArr
            };
            blocksArray.Add(dict);
        }

        var userRoot = new GDict { ["Blocks"] = blocksArray };
        WriteJsonToFile(UserDataPath, userRoot);
    }

    /**
     * 内部方法：把给定的 Godot.Collections.Dictionary 序列化并写到指定 path。
     */
    private static void WriteJsonToFile(string path, GDict root)
    {
        // 使用 JSON.Stringify 而不是 JSON.Print
        // 这里演示美化输出，使用一个 tab 缩进
        string jsonText = Json.Stringify(root, indent: "\t");

        // 确保目录存在
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, jsonText);
    }
}

