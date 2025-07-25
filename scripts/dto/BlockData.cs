using System.Text.Json.Serialization;

namespace Tetris.scripts.dto;


/// <summary>
/// 封装方块基本信息。
/// 包括了名称，形状，贴图路径，着色器源码等内容
/// </summary>
public class BlockData
{
    [JsonPropertyName("name")]
    private readonly string _name;
    [JsonPropertyName("shape")]
    private readonly int[,] _shape;
    [JsonPropertyName("texturePath")]
    private readonly string _texturePath;
    [JsonPropertyName("shaderCode")]
    private readonly string _shaderCode;

    [JsonConstructor]
    public BlockData(string name, int[,] shape, string texturePath, string shaderCode)
    {
        _name = name;
        _shape = shape;
        _texturePath = texturePath;
        _shaderCode = shaderCode;
    }

    public string GetName() => _name;
    public int[,] GetShape() => _shape;
    public string GetTexturePath() => _texturePath;
    public string GetShaderCode() => _shaderCode;
}
