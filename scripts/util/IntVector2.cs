namespace Tetris.scripts.util;

public class IntVector2
{
    public readonly int X;
    public readonly int Y;

    public IntVector2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b)
    {
        return new IntVector2(a.X + b.X, a.Y + b.Y);
    }

    public static IntVector2 operator *(IntVector2 v, int scalar)
    {
        return new IntVector2(v.X * scalar, v.Y * scalar);
    }
    
    /**
     * 方法：逆时针旋转90度
     */
    public IntVector2 Rotate90CCW()
    {
        return new IntVector2(Y, -X);
    }

    /**
     * 方法：顺时针旋转90度
     */
    public IntVector2 Rotate90CW()
    {
        return new IntVector2(-Y, X);
    }
}