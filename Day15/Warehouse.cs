abstract class Warehouse
{
    protected Size Size { get; init; }
    protected Robot Robot { get; init; }
    protected char[][] Map { get; init; }
    
    protected char GetValue(Point pos) => Map[pos.Row][pos.Col];
    
    protected abstract void Move(Direction direction);
    
    public void Execute(string instructions)
    {
        foreach (var instruction in instructions)
        {
            Move(Direction.Create(instruction));
        }
    } 

    protected void Swap(Point p1, Point p2)
    {
        (Map[p1.Row][p1.Col], Map[p2.Row][p2.Col]) = (Map[p2.Row][p2.Col], Map[p1.Row][p1.Col]);
    }
    
    public void Print()
    {
        Console.WriteLine($"\r\nRobot Position: ({Robot.Position.Row}, {Robot.Position.Col})");

        for (int i = 0; i < Size.Rows; i++)
        {
            Console.WriteLine(new string(Map[i]));
        }
    }
    
    protected IEnumerable<int> GetGpsCoordinates(char crate)
    {
        for (var row = 0; row < Size.Rows; row++)
        {
            for (var col = 0; col < Size.Cols; col++)
            {
                if (Map[row][col] == crate)
                {
                    yield return 100 * row + col;
                }
            }
        }
    }    

}

