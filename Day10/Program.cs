// https://adventofcode.com/2024/day/10

using Point = (int x, int y, int h);

//Console.WriteLine($"Part 1. Score: {GetPart2ScoreRecursion(new Point(2, 0, 0), File.ReadAllLines("test.txt"))}");

 var map = File.ReadAllLines("input.txt"); // P1 786 P2 1722
var startingPoints = FindStartingPoint(map);
 int p1score = startingPoints.Select(o => GetPart1Score(o, map)).Sum();
 int p2score = startingPoints.Select(o => GetPart2ScoreRecursion(o, map)).Sum();

 Console.WriteLine($"Part 1. Score: {p1score}");
 Console.WriteLine($"Part 2. Score: {p2score}");

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