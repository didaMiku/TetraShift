using System;
using Tetris.scripts.util;

namespace Tetris.scripts.dto;

public class GameConfigDto
{
    private int _width;
    private int _height;
    private GlobalConstant.GameType _gameType;

    public GameConfigDto(int width, int height, GlobalConstant.GameType gameType)
    {
        _width = width;
        _height = height;
        _gameType = gameType;
    }

    public GameConfigDto()
    {
        _width = 10;
        _height = 15;
        _gameType = GlobalConstant.GameType.TypeClassic;
    }

    public int GetWidth() => _width;
    public void SetWidth(int width) => _width = width;
    public int GetHeight() => _height;
    public void SetHeight(int height) => _height = height;
    public GlobalConstant.GameType GetGameType() => _gameType;
    public void SetGameType(GlobalConstant.GameType gameType) => _gameType = gameType;

    public override String ToString()
    {
        return "GameConfigDto{" +
                "width=" + _width +
                ", height=" + _height +
                ", gameType=" + _gameType +
                '}';
    }
}
