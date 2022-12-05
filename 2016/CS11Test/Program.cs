using System.Numerics;

public record Translation<T>(T XOffset, T YOffset) : IAdditiveIdentity<Translation<T>, Translation<T>>
    where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T> {
    public static Translation<T> AdditiveIdentity => new Translation<T>(T.AdditiveIdentity, T.AdditiveIdentity);
}

public record Point<T>(T X, T Y) : IAdditionOperators<Point<T>, Translation<T>, Point<T>>
    where T: IAdditionOperators<T, T, T>, IAdditiveIdentity<T,T> {
    public static Point<T> operator +(Point<T> left, Translation<T> right) =>
        left with { X = left.X + right.XOffset, Y = left.Y + right.YOffset };
}

class GenericAttribute<T> : Attribute {}
struct Coords {
    public double x, y;
}

class Program {
    public static void Main(string[] args)
    {
        Coords p;
        p.y = 0;
        p.x = 0;
    }

    [Generic<ValueTuple<int>>]
    string f() => string.Empty;
}
