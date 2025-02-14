// https://adventofcode.com/2024/day/16

var map = File.ReadAllLines("test1.txt")
    .Select(o => o.ToCharArray())
    .ToArray();

var maze = new Maze(map);

var (cost, steps) = maze.GetPath();

Console.WriteLine($"Cost: {cost}");
Console.WriteLine($"Seats: {steps.Count}");

//maze.PrintPath(steps);

public record Point(int Row, int Col)
{
    public static Point operator +(Point a, Direction b)
    {
        return new Point(a.Row + b.Row, a.Col + b.Col);
    }
}

public record Direction(int Row, int Col) : Point(Row, Col)
{
    public static readonly Direction North = new(-1, 0);
    public static readonly Direction South = new(1, 0);
    public static readonly Direction West = new(0, -1);
    public static readonly Direction East = new(0, 1);
    
    public static readonly Direction[] All = [North, South, West, East];
}

public enum Rotation
{
    Right,
    Left
}

public static class DirectionExtensions
{
    public static Direction Rotate(this Direction direction, Rotation rotation) 
        => rotation switch
        {
            Rotation.Right => new Direction(-direction.Col, direction.Row),
            Rotation.Left => new Direction(direction.Col, -direction.Row),
            _ => throw new InvalidOperationException("Invalid rotation value.")
        };
}

public record Position(Point Location, Direction Direction);
public record Path(int Cost, List<Point> Steps);

public class Maze(char[][] map)
{
    public int Rows => Map.Length - 1;
    public int Cols => Map[0].Length - 1;
    public char[][] Map { get; } = map;
    public char GetValue(Point pos) => Map[pos.Row][pos.Col];
    public bool IsWall(Point pos) => GetValue(pos) == '#';
    public bool IsMaze(Point pos) => pos.Row >= 0 && pos.Row <= Rows && pos.Col >= 0 && pos.Col <= Cols;
    public Point GetStart() => GetPoint('S');
    public Point GetFinish() => GetPoint('E');
    public bool IsFinish(Point p) => p == GetFinish();

    private Point GetPoint(char target)
    {
        for (int row = 0; row <= Rows; row++)
        {
            for (int col = 0; col <= Cols; col++)
            {
                var point = new Point(row, col);
                
                if (GetValue(point) == target)
                    return point;
            }
        }

        throw new InvalidOperationException($"No point '{target}' found in the maze.");
    }

    public (int Cost, List<Point> Steps) GetPath()
    {
        var start = new Position(GetStart(), Direction.East);
        var visited = new HashSet<Position>();
        var positions = new PriorityQueue<Position, int>([(start, 0)]);
        var paths = new Dictionary<Position, Path> { [start] = new(0, []) };

        while (positions.Count > 0)
        {
            var current = positions.Dequeue();
            
            if(IsFinish(current.Location))
            {
                var (cost, steps) = paths[current];
                return (cost, steps.Distinct().ToList());
            }
            
            if(!visited.Add(current))
                continue;

            var path = paths[current];
            
            var nextMoves = new[]
            {
                (current with { Location = current.Location + current.Direction }, path.Cost + 1),
                (current with { Direction = current.Direction.Rotate(Rotation.Right) }, path.Cost + 1000),
                (current with { Direction = current.Direction.Rotate(Rotation.Left) }, path.Cost + 1000)
            };

            foreach (var (nextPos, nextCost) in nextMoves)
            {
                if (!IsMaze(nextPos.Location) || IsWall(nextPos.Location))
                    continue;

                // cost higher move on
                if (paths.TryGetValue(nextPos, out var posPath) && nextCost > posPath.Cost)
                    continue;
                
                // no cost or less
                if (posPath == null || nextCost < posPath.Cost)
                {
                    paths[nextPos] = new Path(nextCost, [..path.Steps, nextPos.Location]);
                }
                else
                {
                    // cost is equal
                    paths[nextPos] = new Path(nextCost, [..path.Steps, ..posPath.Steps, nextPos.Location]);;
                }

                positions.Enqueue(nextPos, nextCost);
            }
        }

        return (-1, []);
    }

    public void PrintPath(IEnumerable<Point> path)
    {
        var copy = Map.Select(row => row.ToArray()).ToArray();

        foreach (var point in path)
        {
            copy[point.Row][point.Col] = '0';
        }
        
        Console.WriteLine();
        
        foreach (var row in copy)
        {
            Console.WriteLine(row);
        }        
    } 
}