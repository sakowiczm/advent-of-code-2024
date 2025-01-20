// https://adventofcode.com/2024/day/15

var input = File.ReadAllLines("input.txt");

var map = input.Where(o => o.Length > 0 && o[0] == '#').Select(o => o.ToCharArray()).ToArray();
var instructions = string.Join("", input.Where(o => o.Length > 0 && o[0] != '#').Select(o => o));

var smallWarehouse = SmallWarehouse.Create(map);
var bigWarehouse = BigWarehouse.Create(map);

smallWarehouse.Print();
smallWarehouse.Execute(instructions);
smallWarehouse.Print();

Console.WriteLine($"Part 1: GPS coordinate sum: {smallWarehouse.GetGpsCoordinates().Sum()}");

bigWarehouse.Print();
bigWarehouse.Execute(instructions);
bigWarehouse.Print();

Console.WriteLine($"Part 2: GPS coordinate sum: {bigWarehouse.GetGpsCoordinates().Sum()}");



record Size(int Rows, int Cols);

record Point(int Row, int Col)
{
    public static Point operator +(Point a, Direction b)
    {
        return new Point(a.Row + b.DRow, a.Col + b.DCol);
    }
    
    public static Point operator -(Point a, Direction b)
    {
        return new Point(a.Row - b.DRow, a.Col - b.DCol);
    }    
}

record Direction(int DRow, int DCol)
{
    public static readonly Direction Up = new(-1, 0);
    public static readonly Direction Down = new(1, 0);
    public static readonly Direction Left = new(0, -1);
    public static readonly Direction Right = new(0, 1);

    public static Direction Create(char instruction) => instruction switch
    {
        '^' => Up,
        '>' => Right,
        '<' => Left,
        _ => Down
    };
}

record Robot(Point Position) 
{
    public Point Position { get; set; } = Position;
    
    private static Point GetRobotPosition(char[][] map)
    {
        for (var row = 0; row < map.Length; row++)
        {
            for (var col = 0; col < map[0].Length; col++)
            {
                if (map[row][col] == '@')
                {
                    return new Point(row, col);
                }
            }
        }
       
        throw new ArgumentException("Missing robot position");
    }

    public static Robot Create(char[][] map) => new(GetRobotPosition(map));
}