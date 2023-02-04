using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Grid
{
    public int x;
    public int y;

    public static Grid Create(int _x, int _y)
    {
        data.x = _x;
        data.y = _y;
        return data;
    }

    private Grid(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
	
    private static Grid data;
    private static Grid Up = new Grid(0, -1);
    private static Grid Down = new Grid(0, 1);
    private static Grid Right = new Grid(1, 0);
    private static Grid Left = new Grid(-1, 0);
    private static Grid UpRight = new Grid(1, -1);
    private static Grid DownRight = new Grid(1, 1);
    private static Grid DownLeft = new Grid(-1, 1);
    private static Grid UpLeft = new Grid(-1, -1);

    public static Grid up { get { return Up; } }
    public static Grid down { get { return Down; } }
    public static Grid right { get { return Right; } }
    public static Grid left { get { return Left; } }
    public static Grid upright { get { return UpRight; } }
    public static Grid downright { get { return DownRight; } }
    public static Grid downleft { get { return DownLeft; } }
    public static Grid upleft { get { return UpLeft; } }


    public static Grid operator +(Grid a, Grid b)
    {
        return Create(a.x + b.x, a.y + b.y);
    }
    public static Grid operator -(Grid a, Grid b)
    {
        return Create(a.x - b.x, a.y - b.y);
    }
    public static bool operator ==(Grid a, Grid b)
    {
        return (a.x == b.x && a.y == b.y);
    }
    public static bool operator !=(Grid a, Grid b)
    {
        return (a.x != b.x || a.y != b.y);
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
	public override string ToString()
	{
		return string.Format("x:{0}, y:{1}", x, y);
	}
}
