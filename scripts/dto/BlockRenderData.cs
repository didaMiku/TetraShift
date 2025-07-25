using Tetris.scripts.util;

namespace Tetris.scripts.dto;


/// <summary>
/// 渲染一个方块所需要的基本信息。
/// renderType决定渲染类型，访问texture路径配置贴图，在根据shader渲染最终结果。
/// </summary>
public class BlockRenderData
{
    private GlobalConstant.BlockRenderType _renderType;
    private string _texturePath;
    private string _shaderCode;

    public BlockRenderData(GlobalConstant.BlockRenderType renderType, string texturePath, string shaderCode)
    {
        _renderType = renderType;
        _texturePath = texturePath;
        _shaderCode = shaderCode;
    }
    public BlockRenderData()
    {
        _renderType = GlobalConstant.BlockRenderType.RenderHide;
        _texturePath = null;
        _shaderCode = null;
    }

    public string GetRenderTypeString()
    {
        if (_renderType == GlobalConstant.BlockRenderType.RenderHide) return "RenderHide";
        if (_renderType == GlobalConstant.BlockRenderType.RenderShow) return "RenderShow";
        return "RenderHide";
    }
    
    public GlobalConstant.BlockRenderType GetRenderType() => _renderType;
    public void SetRenderType(GlobalConstant.BlockRenderType renderType) => _renderType = renderType;
    
    public string GetTexturePath() => _texturePath;
    public void SetTexturePath(string texturePath) => _texturePath = texturePath;
    
    public string GetShaderCode() => _shaderCode;
    public void SetShaderCode(string shaderCode) => _shaderCode = shaderCode;
}