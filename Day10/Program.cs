// https://adventofcode.com/2024/day/10

using Point = (int x, int y, int h);

//Console.WriteLine($"Test. Score: {GetPart1ScoreUsingDFS(new Point(2, 0, 0), File.ReadAllLines("test.txt"))}");

var map = File.ReadAllLines("input.txt"); // P1 786 P2 1722
var startingPoints = FindStartingPoint(map);
int p1score = startingPoints.Select(o => GetPart1Score(o, map)).Sum();
int p1dfsscore = startingPoints.Select(o => GetPart1ScoreUsingDFS(o, map)).Sum();
int p2score = startingPoints.Select(o => GetPart2ScoreRecursion(o, map)).Sum();
int p2dfsscore = startingPoints.Select(o => GetPart2ScoreUsingDFS(o, map)).Sum();

Console.WriteLine($"Part 1. Score: {p1score}");
Console.WriteLine($"Part 1 DFS. Score: {p1dfsscore}");
Console.WriteLine($"Part 2. Score: {p2score}");
Console.WriteLine($"Part 2 DFS. Score: {p2dfsscore}");

// Get all unique 9 we reach from given start point 
int GetPart1Score(Point start, string[] map)
{
    IEnumerable<Point> next = new[] { start };

    int h = 1;
    while (next.Any() && h <= 9)
    {
        next = next.SelectMany(o => GetNextSteps(o, map)).Where(o => o.h == h).Distinct().ToList();
        //PrintPoints(next);
        h++;
    }

    return next.Count();
}

// Count all unique 9 we can reach from starting point
int GetPart1ScoreUsingDFS(Point start, string[] map)
{
    int result = 0;
    HashSet<Point> visited = new HashSet<Point>();
    Queue<Point> q = new Queue<Point>();
    
    q.Enqueue(start);

    while (q.Count > 0)
    {
        var current = q.Dequeue();

        if (current.h == 9)
        {
            // check if the 9 we reach is unique
            if (!visited.Contains(current))
            {
                result += 1;
                visited.Add(current);
            }
            
            continue;
        }

        foreach (var neighbor in GetNextSteps(current, map))
        {
            if(neighbor.h == current.h + 1)
                q.Enqueue(neighbor);
        }
    }

    return result;
}

// Sum every unique step leading to any accessible 9
int GetPart2ScoreUsingDFS(Point start, string[] map)
{
    int result = 0;
    Queue<Point> q = new Queue<Point>();
    
    q.Enqueue(start);

    while (q.Count > 0)
    {
        var current = q.Dequeue();

        if (current.h == 9)
        {
            result += 1;
            continue;
        }

        foreach (var neighbor in GetNextSteps(current, map))
        {
            if(neighbor.h == current.h + 1)
                q.Enqueue(neighbor);
        }
    }

    return result;
}

// Sum all unique points encountered getting to 9 from given start point
int GetPart2ScoreRecursion(Point start, string[] map)
{
    if (start.h == 9)
        return 1;

    int ans = 0;
    foreach (var step in GetNextSteps(start, map))
    {
        if (step.h == start.h + 1)
            ans += GetPart2ScoreRecursion(step, map);
    }

    return ans;
}

IEnumerable<Point> GetNextSteps(Point point, string[] map)
{
    var directions = new Point[] { (0, +1, 0), (+1, 0, 0), (0, -1, 0), (-1, 0, 0) };

    return directions.Select(o => new Point(point.x + o.x, point.y + o.y, 0))
    .Where(o => IsInside(o, map.Length))
    .Select(o => new Point(o.x, o.y, map[o.y][o.x] - '0'));
}

bool IsInside(Point p, int length)
{
    return p.x >= 0 && p.x < length && p.y >= 0 && p.y < length;
}

IEnumerable<Point> FindStartingPoint(string[] map)
{
    for (var i = 0; i < map.Length; i++)
    {
        for (var j = 0; j < map.Length; j++)
        {
            if (map[i][j] == '0')
                yield return new Point(j, i, 0);
        }
    }
}









void PrintPoints(IEnumerable<Point> points)
{
    Console.WriteLine(string.Join("", points));
}

void PrintMap(string[] map)
{
    foreach (var m in map)
    {
        Console.WriteLine(m);
    }
}