// https://adventofcode.com/2024/day/16

var map = File.ReadAllLines("input.txt")
    .Select(o => o.ToCharArray())
    .ToArray();

var maze = new Maze(map);
//maze.Print();

Console.WriteLine(maze.FindCheapestPath());

// test1.txt -> 7036
// test2.txt -> 11048

public record Point(int Row, int Col)
{
    public static Point operator +(Point a, Direction b)
    {
        return new Point(a.Row + b.Row, a.Col + b.Col);
    }
    
    public static Point operator -(Point a, Direction b)
    {
        return new Point(a.Row - b.Row, a.Col - b.Col);
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
            Rotation.Right => new Direction(direction.Col, -direction.Row),
            Rotation.Left => new Direction(-direction.Col, direction.Row),
            _ => throw new InvalidOperationException("Invalid rotation value.")
        };
}

public record Position(Point Location, Direction Direction);

public class Maze(char[][] map)
{
    public int Rows => Map.Length - 1;
    public int Cols => Map[0].Length - 1;
    public char[][] Map { get; } = map;
    public char GetValue(Point pos) => Map[pos.Row][pos.Col];
    public bool IsWall(Point pos) => GetValue(pos) == '#';
    public bool IsBound(Point pos) => pos.Row >= 0 && pos.Row <= Rows && pos.Col >= 0 && pos.Col <= Cols;
    public Point GetStart() => GetPoint('S');
    public Point GetFinish() => GetPoint('E');

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

    public int FindCheapestPath()
    {
        var startPosition = new Position(GetStart(), Direction.East);

        var visitedPositions = new HashSet<Position>();
        var positions = new PriorityQueue<Position, int>([(startPosition, 0)]);
        var minPositionCost = new Dictionary<Position, int> { [startPosition] = 0 };

        while (positions.Count > 0)
        {
            var currentPos = positions.Dequeue();
            var currentCost = minPositionCost[currentPos];
            visitedPositions.Add(currentPos);
            
            var moves = new[]
            {
                (currentPos with { Location = currentPos.Location + currentPos.Direction }, currentCost + 1),
                (currentPos with { Direction = currentPos.Direction.Rotate(Rotation.Right) }, currentCost + 1000),
                (currentPos with { Direction = currentPos.Direction.Rotate(Rotation.Left) }, currentCost + 1000)
            };

            foreach (var (pos, cost) in moves)
            {
                if (!IsBound(pos.Location) || IsWall(pos.Location) || visitedPositions.Contains(pos) || (minPositionCost.TryGetValue(pos, out var positionCost) && positionCost <= cost))
                    continue;

                minPositionCost[pos] = cost;
                positions.Enqueue(pos, cost);
            }
        }

        return Direction.All.Select(d => new Position(GetFinish(), d))
            .Where(p => minPositionCost.ContainsKey(p))
            .Select(p => minPositionCost[p])
            .Min();
    }

    public void Print()
    {
        foreach (var row in Map)
        {
            Console.WriteLine(row);
        }
    }
}