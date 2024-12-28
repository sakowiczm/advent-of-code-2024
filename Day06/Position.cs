namespace Day06;

class Position
{
    public Position(int x, int y, int dx, int dy)
    {
        this.x = x;
        this.y = y;
        this.dx = dx;
        this.dy = dy;
    }

    public int x { get; set; }
    public int y { get; set; }
    public int dx { get; set; }
    public int dy { get; set; }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, dx, dy);
        
        // unchecked
        // {
        //     var result = 0;
        //     result = (result * 397) ^ x;
        //     result = (result * 397) ^ y;
        //     result = (result * 397) ^ dx;
        //     result = (result * 397) ^ dy;
        //     return result;
        // }        
    }

    public override bool Equals(object? obj)
    {
        Position? o = obj as Position;

        if (o == null)
            return false;

        return o.x == this.x && o.y == this.y && o.dx == this.dx && o.dy == this.dy;
    }
}