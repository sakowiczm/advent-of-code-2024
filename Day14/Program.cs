// https://adventofcode.com/2024/day/14

using System.Text;
using System.Text.RegularExpressions;

// col = x, row = y

using Position = (int col, int row);
using Velocity = (int dcol, int drow);

var robots = File.ReadAllLines("input.txt").Select(l => Robot.Create(l)).ToList();
var map = new Map(101, 103, robots);

map.Move(100);

Console.WriteLine($"Part 1. Safety factor: {map.GetSafetyFactor()}");

// Part 2
map.PrintMaps(100 + 10000);

// search for ******************


class Map(int width, int height, IEnumerable<Robot> robots)
{
    public int Width { get; } = width;
    public int Height { get; } = height;
    public IEnumerable<Robot> Robots { get; init; } = robots;

    public void Move(int seconds)
    {
        foreach (var robot in Robots)
        {
            //PrintPosition(robot, 0);
            
            for (int i = 1; i <= seconds; i++)
            {
                robot.Move(this);
                //PrintPosition(robot, i);
            }
        }
    }

    public int GetSafetyFactor()
    {
        var mCol = (Width-1) / 2;
        var mRow = (Height-1) / 2;

        var q1 = Robots.Select(r => r.Position).Count(p => p.col >= 0 && p.col < mCol && p.row >= 0 && p.row < mRow);
        var q2 = Robots.Select(r => r.Position).Count(p => p.col > mCol && p.col <= Width && p.row >= 0 && p.row < mRow);
        var q3 = Robots.Select(r => r.Position).Count(p => p.col >= 0 && p.col < mCol && p.row > mRow && p.row <= Height);
        var q4 = Robots.Select(r => r.Position).Count(p => p.col > mCol && p.col <= Width && p.row > mRow && p.row <= Height);

        return q1 * q2 * q3 * q4;
    }

    public void PrintPosition(Robot robot, int iteration)
    {
        Console.WriteLine($"Second: {iteration}, Position: {robot.Position}");
        
        for (int r = 0; r < Height; r++)
        {
            if (robot.Position.row != r)
            {
                Console.WriteLine(new string('.', Width));
            }
            else
            {
                Console.Write(new string('.', robot.Position.col));
                Console.Write(new string('#', 1));
                Console.Write(new string('.', Width - robot.Position.col - 1) + Environment.NewLine);
            }
        }
    }

    public void PrintMap(IEnumerable<Robot> robots)
    {
        var positions = robots.GroupBy(r => r.Position)
            .Select(o => new { Position = o.Key, Count = o.Count() })
            .ToList();

        var emptyRow = new string(' ', Width);
        
        for (int r = 0; r < Height; r++)
        {
            if(positions.Count(o => o.Position.row != r) == 0)
            {
                Console.WriteLine(emptyRow);
            }
            else
            {
                var sb = new StringBuilder(emptyRow);

                foreach (var p in positions.Where(o => o.Position.row == r).OrderBy(o => o.Position.col))
                {
                    sb[p.Position.col] = '*';
                }
                
                Console.WriteLine(sb.ToString());
            }
        }        
    }
    
    public void PrintMaps(int seconds)
    {
        for (int i = 0; i <= seconds; i++)
        {
            Console.WriteLine($"Seconds: {i}");
            PrintMap(Robots);
            Move(1);
        }
    }
}

class Robot(Position position, Velocity velocity)
{
    public Position Position { get; private set; } = position;
    public Velocity Velocity { get; init; } = velocity;

    private static readonly Regex CompiledRegex = new("p=(?<col>[-0-9]+),(?<row>[-0-9]+) v=(?<dcol>[-0-9]+),(?<drow>[-0-9]+)",
        RegexOptions.Compiled | RegexOptions.Singleline);
    
    public override string ToString() => $"Robot Velocity: ({Velocity.dcol, 3}, {Velocity.drow, 3}), Position: ({Position.col, 3},{Position.row, 3})";
    
    public static Robot Create(string input)
    {
        var match = CompiledRegex.Match(input);

        int x = int.Parse(match.Groups["col"].ValueSpan);
        int y = int.Parse(match.Groups["row"].ValueSpan);
        int dx = int.Parse(match.Groups["dcol"].ValueSpan);
        int dy = int.Parse(match.Groups["drow"].ValueSpan);
        
        return new Robot(new Position(x,y), new Velocity(dx,dy));
    }

    public void Move(Map map)
    {
        var nCol = Position.col + Velocity.dcol;
        var nRow = Position.row + Velocity.drow;
        
        if(nCol >= map.Width)
            nCol -= map.Width;
        
        if(nCol < 0)
            nCol = map.Width + nCol;
        
        if(nRow >= map.Height)
            nRow -= map.Height;
        
        if(nRow < 0)
            nRow = map.Height + nRow;
        
        Position = new Position(nCol, nRow);
    }
}

