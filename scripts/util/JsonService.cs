using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Tetris.scripts.dto;
using Tetris.scripts.util;


/// <summary>
/// JSON 读写服务
/// </summary>
public static class JsonService
{
    private static readonly string DefaultDataPath = "res://scripts/DefaultData.json";
    private static readonly string UserDataPath = System.IO.Path.Combine(OS.GetUserDataDir(), "UserData.json");
    private static Godot.Collections.Dictionary _cachedRoot; // GDict 缓存

    /**
     * 初始化用户数据
     */
    public static void InitializeUserData()
    {
        if (FileAccess.FileExists(UserDataPath))
        {
            GD.Print("UserData 已存在，跳过初始化。地址：" + UserDataPath);
            UpdateRoot();
            UpdateBlockDictionary();
            return;
        }
        GD.Print("UserData 不存在，复制默认数据。地址：" + UserDataPath);

        // 复制 DefaultData.json 内容到 UserData.json
        using var defaultFile = FileAccess.Open(DefaultDataPath, FileAccess.ModeFlags.Read);
        var content = defaultFile.GetAsText();

        using var userFile = FileAccess.Open(UserDataPath, FileAccess.ModeFlags.Write);
        userFile.StoreString(content);

        UpdateRoot();
        UpdateBlockDictionary();
    }

    /**
     * 更新全局的方块信息字典
     */
    public static void UpdateBlockDictionary()
    {
        var blockData = GetField("blockData");
        if (blockData.VariantType != Variant.Type.Array)
        {
            return;
        }
        Godot.Collections.Array blockDataArray = blockData.AsGodotArray();
        BlockDictionary.Clear();
        foreach (var data in blockDataArray)
        {
            if (data.VariantType != Variant.Type.Dictionary)
            {
                GD.PushWarning("blockData 中存在非字典元素，已跳过");
                continue;
            }
            var blockDict = (Godot.Collections.Dictionary)data;
            string name = (string)blockDict["name"];
            string texturePath = (string)blockDict["texturePath"];
            string shaderCode = (string)blockDict["shaderCode"];
            // 二维数组较为特殊，需要单独处理
            var shapeArrayVariant = blockDict["shape"];
            var shapeRows = shapeArrayVariant.AsGodotArray();
            int rowCount = shapeRows.Count;
            int colCount = ((Godot.Collections.Array)shapeRows[0]).Count;
            int[,] shape = new int[rowCount, colCount];
            for (int i = 0; i < rowCount; i++)
            {
                var row = (Godot.Collections.Array)shapeRows[i];
                for (int j = 0; j < colCount; j++)
                {
                    shape[i, j] = (int)(long)row[j]; // 注意 Variant → long → int
                }
            }
            // 添加到字典
            BlockDictionary.Add(name, new BlockData(name, (int[,])shape, texturePath, shaderCode));
        }
    }

    /**
     * 同步JSON中的方块信息
     */
    public static void SyncBlockData()
    {
        System.Collections.Generic.Dictionary<string, BlockData> blockData = BlockDictionary.GetDictionary();
        var blockDataArray = new Godot.Collections.Array();
        foreach (var data in blockData.Values)
        {
            var blockDict = new Godot.Collections.Dictionary();
            blockDict["name"] = data.GetName();
            blockDict["texturePath"] = data.GetTexturePath();
            blockDict["shaderCode"] = data.GetShaderCode();
            var shapeArray = data.GetShape();
            var shapeRows = new Godot.Collections.Array();
            for (int i = 0; i < shapeArray.GetLength(0); i++)
            {
                var shapeRow = new Godot.Collections.Array();
                for (int j = 0; j < shapeArray.GetLength(1); j++)
                {
                    shapeRow.Add(shapeArray[i, j]);
                }
                shapeRows.Add(shapeRow);
            }
            blockDict["shape"] = shapeRows;
            blockDataArray.Add(blockDict);
        }
        SetField("blockData", blockDataArray);
    }

    /**
     * 获取游戏配置
     */
    public static GameConfigDto GetGameConfig()
    {
        var config = GetField("gameConfig");
        if (config.VariantType != Variant.Type.Dictionary)
        {
            GD.Print("无法获取游戏配置");
            return new GameConfigDto();
        }
        Godot.Collections.Dictionary configDict = config.AsGodotDictionary();
        int width = (int)configDict["width"];
        int height = (int)configDict["height"];
        string gameTypeStr = (string)configDict["gameType"];
        switch (gameTypeStr)
        {
            case "TypeClassic":
                return new GameConfigDto(width, height, GlobalConstant.GameType.TypeClassic);
            case "TypeTetris":
                return new GameConfigDto(width, height, GlobalConstant.GameType.TypeFourWay);
            default:
                GD.Print("无法获取游戏类型");
                return new GameConfigDto();
        }
    }

    /**
     * 设置游戏配置
     */
    public static void SetGameConfig(GameConfigDto gameConfig)
    {
        var config = GetField("gameConfig");
        if (config.VariantType != Variant.Type.Dictionary)
        {
            GD.Print("无法设置游戏配置");
            return;
        }
        Godot.Collections.Dictionary configDict = config.AsGodotDictionary();
        configDict["width"] = gameConfig.GetWidth();
        configDict["height"] = gameConfig.GetHeight();
        configDict["gameType"] = gameConfig.GetGameType().ToString();
        SetField("gameConfig", configDict);
    }


    /**
     * 其它读写方法、、、
     */


    /**
     * 读取并缓存整个 JSON 到本类的 cache
     */
    private static Godot.Collections.Dictionary UpdateRoot()
    {
        var file = FileAccess.Open(UserDataPath, FileAccess.ModeFlags.Read);
        string jsonStr = file.GetAsText();
    
        var json = new Json();
        Error err = json.Parse(jsonStr);
        if (err != Error.Ok)
        {
            GD.PushError($"UserData JSON 解析失败：{json.GetErrorMessage()}（行 {json.GetErrorLine()}）");
            return new Godot.Collections.Dictionary();
        }
    
        Variant result = json.GetData();
        if (result.VariantType != Variant.Type.Dictionary)
        {
            GD.PushError("UserData 根节点不是字典，解析失败。");
            return new Godot.Collections.Dictionary();
        }
    
        _cachedRoot = result.AsGodotDictionary();
        return _cachedRoot;
    }

    /**
     * 清空缓存
     */
    private static void ClearCache() => _cachedRoot = null;

    /**
     * 保存当前 root 到文件
     */
    public static void Save()
    {
        var jsonStr = Json.Stringify(UpdateRoot(), indent: "  ");
        using var file = FileAccess.Open(UserDataPath, FileAccess.ModeFlags.Write);
        file.StoreString(jsonStr);
        GD.Print("UserData 保存成功！");
    }

    /** 
     *获取某个大字段
     */
    public static Variant GetField(string fieldName)
    {
        var root = UpdateRoot();
        return root.ContainsKey(fieldName) ? root[fieldName] : new Variant();
    }

    /**
     * 精确设置某个大字段（如 gameConfig / blockDatas / records）
     */
    public static void SetField(string fieldName, Variant value)
    {
        var root = UpdateRoot();
        root[fieldName] = value;
    }

    /**
     * 将 Godot Dictionary 转换为 C# Dictionary
     */
    public static System.Collections.Generic.Dictionary<string, Variant> ToCSharpDictionary(Godot.Collections.Dictionary<string, Variant> dict)
    {
        var result = new System.Collections.Generic.Dictionary<string, Variant>();
        foreach (var kvp in dict)
            result[kvp.Key] = kvp.Value;
        return result;
    }

    /**
     * 将 C# Dictionary 转换为 Godot Dictionary
     */
    public static Godot.Collections.Dictionary<string, Variant> ToGodotDictionary(System.Collections.Generic.Dictionary<string, Variant> dict)
    {
        var result = new Godot.Collections.Dictionary<string, Variant>();
        foreach (var kvp in dict)
            result[kvp.Key] = Variant.From(kvp.Value);
        return result;
    }
}
