// https://adventofcode.com/2024/day/8

using Point = (int x, int y);
using Boundary = (int xb, int yb);

class Program
{
    static void Main()
    {
        // frequency (char) has multiple antennas (Point) 
        //var (frequencies, boundary) = GetInput(File.ReadAllLines("testA.txt"));
        //var (frequencies, boundary) = GetInput(File.ReadAllLines("testB.txt"));
        var (frequencies, boundary) = GetInput(File.ReadAllLines("input.txt"));

        // 400 OK
        Console.WriteLine($"Part 1: Unique AntiNodes: {GetAntiNodes(frequencies, boundary, GetPart1AntiNodePoints)}");
        
        // 1280 - OK 
        Console.WriteLine($"Part 2: Unique AntiNodes: {GetAntiNodes(frequencies, boundary, GetPart2AntiNodePoints)}");
    }

    delegate IEnumerable<Point> GetAndiNodePoints((Point a, Point b) ap, Boundary b);

    static int GetAntiNodes(Dictionary<char, List<Point>> frequencies, Boundary boundary, GetAndiNodePoints getAntiNodePoints)
    {
        return frequencies.Where(o => o.Value.Count > 1)
            .SelectMany(o => GetAntennaPermutations(o.Value).SelectMany(o => getAntiNodePoints(o, boundary)))
            .Distinct()
            .Count();
    }

    static (Dictionary<char, List<Point>>, Boundary) GetInput(string[] input)
    {
        Boundary boundary = (input[0].Length - 1, input.Length - 1);
            
        var dict = new Dictionary<char, List<Point>>();

        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                char c = input[y][x]; 
                
                if(c == '.')
                    continue;

                if (dict.TryGetValue(c, out var value))
                {
                    value.Add((x, y));
                }
                else
                {
                    dict.Add(c, [(x, y)]);
                }
            }
        }

        return (dict, boundary);
    }

    static List<(Point a, Point b)> GetAntennaPermutations(List<Point> points)
    {
        // number of 2 point permutations = n*(n-1)
        List<(Point a, Point b)> pairs = new List<(Point a, Point b)>(points.Count*(points.Count-1));
        
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i+1; j < points.Count; j++)
            {
                pairs.Add((points[i], points[j]));
            }
        }

        return pairs;
    }
    
    static IEnumerable<Point> GetPart1AntiNodePoints((Point a, Point b) ap, Boundary boundary)
    {
        var dx = ap.a.x - ap.b.x;
        var dy = ap.a.y - ap.b.y;

        var a = (ap.a.x + dx, ap.a.y + dy);
        var b = (ap.b.x - dx, ap.b.y - dy);

        if (IsInside(a, boundary))
            yield return a;
            
        if (IsInside(b, boundary))
            yield return b;
    }
    
    static IEnumerable<Point> GetPart2AntiNodePoints((Point a, Point b) ap, Boundary boundary)
    {
        yield return ap.a;
        yield return ap.b;
        
        var dx = ap.a.x - ap.b.x;
        var dy = ap.a.y - ap.b.y;

        var a = ap.a;
        var b = ap.b;

        while (IsInside(a, boundary))
        {
            yield return a;
            
            a.x += dx;
            a.y += dy;
        }
        
        while (IsInside(b, boundary))
        {
            yield return b;
            
            b.x -= dx;
            b.y -= dy;
        }
    }

    static bool IsInside(Point p, Boundary b)
    {
        return p.x >= 0 && p.x <= b.xb && p.y >= 0 && p.y <= b.yb;
    }
}