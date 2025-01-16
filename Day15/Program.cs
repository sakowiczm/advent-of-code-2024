// https://adventofcode.com/2024/day/15

using Size = (int Rows, int Cols);
using Direction = (int DRow, int DCol);

var input = File.ReadAllLines("input.txt");

var map = input.Where(o => o.Length > 0 && o[0] == '#').Select(o => o.ToCharArray()).ToArray();
var instructions = string.Join("", input.Where(o => o.Length > 0 && o[0] != '#').Select(o => o));

var robot = Robot.Create(map, instructions);
var warehouse = Warehouse.Create(map, robot);

warehouse.Print();

robot.Execute(warehouse);

warehouse.Print();

Console.WriteLine($"Part 1: GPS coordinate sum: {warehouse.GetGpsCoordinates().Sum()}");


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

class Warehouse
{
    public Size Size { get; init; }

    public Robot Robot { get; init; }

    private char[][] Map { get; init; }

    private char GetValue(Point pos) => Map[pos.Row][pos.Col];

    public void Move(Direction direction)
    {
        var nextPos = Robot.Position + direction;
        var currentPos = nextPos;

        // check for any crates in the direction of move
        while (GetValue(currentPos) == 'O')
        {
            currentPos += direction;
        }

        // ignore move
        if (GetValue(currentPos) == '#')
            return;

        // we reached empty space - swap backwards
        while (currentPos != Robot.Position)
        {
            var tmp = currentPos - direction;
            Swap(currentPos, tmp);
            currentPos = tmp;
        }

        Robot.Position = nextPos;
    }

    private void Swap(Point p1, Point p2)
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

    public IEnumerable<int> GetGpsCoordinates()
    {
        for (var row = 0; row < Size.Rows; row++)
        {
            for (var col = 0; col < Size.Cols; col++)
            {
                if (Map[row][col] == 'O')
                {
                    yield return 100 * row + col;
                }
            }
        }
    }
    
    public static Warehouse Create(char[][] map, Robot robot)
    {
        return new Warehouse
        {
            Map = map,
            Size = new Size(map.Length, map.Length),
            Robot = robot
        };
    }
}

class Robot(Point position, string instructions) 
{
    public Point Position { get; set; } = position;
    
    private static readonly Direction MoveUp = new(-1, 0);
    private static readonly Direction MoveDown = new(1, 0);
    private static readonly Direction MoveLeft = new(0, -1);
    private static readonly Direction MoveRight = new(0, 1);

    public void Execute(Warehouse warehouse)
    {
        foreach (var instruction in instructions)
        {
            var direction = GetDirection(instruction);
            warehouse.Move(direction);
        }
    }

    private Direction GetDirection(char instruction) => instruction switch
        {
            '^' => MoveUp,
            '>' => MoveRight,
            '<' => MoveLeft,
            _ => MoveDown
        };
    
    private static Point GetRobotPosition(char[][] map)
    {
        for (var row = 0; row < map.Length; row++)
        {
            for (var col = 0; col < map.Length; col++)
            {
                if (map[row][col] == '@')
                {
                    return new Point(row, col);
                }
            }
        }
       
        throw new ArgumentException("Missing robot position");
    }

    public static Robot Create(char[][] map, string input) => new(GetRobotPosition(map), input);
}