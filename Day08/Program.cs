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

        var antiNodes = new HashSet<Point>();
        
        // Part 1
        foreach (var frequency in frequencies.Where(o => o.Value.Count > 1))
        {
            var antennaPoints = GetAntennaPermutations(frequency.Value);
            
            //DebugAntennaPoints(frequency, antennaPoints);
        
            foreach (var ap in antennaPoints)
            {
                var (aa, ab) = GetAntiNodePoints(ap);
        
                //DebugAntiNodes(aa, boundary, ab, frequency, ap);
        
                if(CheckBoundary(aa, boundary))
                    antiNodes.Add(aa);
                
                if(CheckBoundary(ab, boundary))
                    antiNodes.Add(ab);
            }
        }
        
        // 400 OK
        Console.WriteLine($"Part 1: Unique AntiNodes: {antiNodes.Count}");
        
        // Part 2
        antiNodes.Clear();
        
        foreach (var frequency in frequencies.Where(o => o.Value.Count > 1))
        {
            var antennaPoints = GetAntennaPermutations(frequency.Value);

            foreach (var ap in antennaPoints)
            {
                foreach (var p in GetAntiNodePointsPart2(ap, boundary))
                {
                    if (CheckBoundary(p, boundary))
                        antiNodes.Add(p);
                }
            }
        }
        
        // 1280 - OK 
        Console.WriteLine($"Part 2: Unique AntiNodes: {antiNodes.Count}");
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
    
    private static (Point aa, Point ab) GetAntiNodePoints((Point a, Point b) antennaPoints)
    {
        // Define points A and B
        double x1 = antennaPoints.a.x, y1 = antennaPoints.a.y;
        double x2 = antennaPoints.b.x, y2 = antennaPoints.b.y;

        // Calculate the length of the vector AB
        int length = (int)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        
        // Define distance d
        int d = length;
        //Console.WriteLine($"Distance: {d}");

        // Calculate the unit vector in the direction of AB
        double ux = (x2 - x1) / length;
        double uy = (y2 - y1) / length;
        //Console.WriteLine($"Direction vector: ({ux},{uy})");

        // Calculate the coordinates of the point P1 which is distance d from A
        int px1 = (int)(x1 - d * ux);
        int py1 = (int)(y1 - d * uy);

        // Calculate the coordinates of the point P2 which is distance d from B
        int px2 = (int)(x2 + d * ux);
        int py2 = (int)(y2 + d * uy);

        return ((px1, py1), (px2, py2));
    }
    
    private static IEnumerable<Point> GetAntiNodePointsPart2((Point a, Point b) antennaPoints, Boundary boundary)
    {
        // Define points A and B
        double x1 = antennaPoints.a.x, y1 = antennaPoints.a.y;
        double x2 = antennaPoints.b.x, y2 = antennaPoints.b.y;

        // Calculate the length of the vector AB
        int length = (int)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

        // Calculate the unit vector in the direction of AB
        double ux = (x2 - x1) / length;
        double uy = (y2 - y1) / length;
        //Console.WriteLine($"Direction vector: ({ux},{uy})");

        // Define distance d
        int d = length;
        //Console.WriteLine($"Distance: {d}");

        // assuming the grid is square
        for (int i = 1; i*d-1 <= boundary.xb; i++)
        {
            // Calculate the coordinates of the point P1 which is distance d from A
            int px1 = (int)(x1 - d * i * ux);
            int py1 = (int)(y1 - d * i * uy);

            // Calculate the coordinates of the point P2 which is distance d from B
            int px2 = (int)(x2 + d * i * ux);
            int py2 = (int)(y2 + d * i * uy);

            yield return (px1, py1);
            yield return (px2, py2);
            // antenna points are also antiNodes
            yield return antennaPoints.a;
            yield return antennaPoints.b;
        }
    }    

    static bool CheckBoundary(Point p, Boundary b)
    {
        return p.x >= 0 && p.x <= b.xb && p.y >= 0 && p.y <= b.yb;
    }

    private static void DebugAntennaPoints(KeyValuePair<char, List<Point>> frequency, List<(Point a, Point b)> antennaPoints)
    {
        //Console.WriteLine($"Frequency: {frequency.Key}, Points No: {string.Join(',', frequency.Value)}");
        Console.WriteLine($"Frequency: {frequency.Key}, Permutations No: {antennaPoints.Count}");
        Console.WriteLine($"Frequency: {frequency.Key}, Permutations No: {string.Join(',', antennaPoints)}");
    }

    private static void DebugAntiNodes(Point aa, Boundary boundary, Point ab, KeyValuePair<char, List<Point>> frequency, (Point a, Point b) ap)
    {
        bool ai = false;
        bool bi = false;
        if(CheckBoundary(aa, boundary))
            ai = true;
                
        if(CheckBoundary(ab, boundary))
            bi = true;
                
        Console.WriteLine($"Frequency: {frequency.Key}, Points: {string.Join(',', ap.a, ap.b)}, AntiNodes: {string.Join(',', aa, ab)}, Inside: ({ai}, {bi})");
    }

}