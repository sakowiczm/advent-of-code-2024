// https://adventofcode.com/2024/day/12

using System.Diagnostics;

using Plot = (int row, int col);

var input = File.ReadAllLines("input.txt");
var garden = new Garden(input);
var regions = garden.GetRegions().ToList();

int count = 0;
foreach (var region in regions)
{
    Console.WriteLine($"Lp. {count++,3}. Region: {region.Plant}, Area: {region.Area}, Perimeter: {region.Perimeter}, Sides: {region.Sides}");
}

var price = regions.Sum(r => r.FencePrice);
var bulkPrice = regions.Sum(r => r.BulkFencePrice);

Console.WriteLine($"Regions count: {price} and {bulkPrice}");


record Garden(string[] Map)
{
    private readonly (int row, int col)[] _deltas = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    public int Rows { get; } = Map.Length;
    public int Cols { get; } = Map[0].Length;

    bool IsIn(int row, int col) => col >=0 && col < Cols && row >=0 && row < Rows;

    IEnumerable<Plot> GetAdjacentPlots(Plot plot)
    {
        return _deltas.Select(d => (row: plot.row + d.row, col: plot.col + d.col))
            .Where(p => IsIn(p.row, p.col))
            .Select(p => new Plot(p.row, p.col));
    }

    Region GetRegion(Plot plot)
    {
        var plant = Map[plot.row][plot.col];
        var region = new Region(plant);
        
        //Console.WriteLine($"Region Starting Point: {plot.row}, {plot.col}, {plant}");

        Queue<Plot> queue = new Queue<Plot>();
        queue.Enqueue(plot);

        int count = 0;

        while (queue.Any())
        {
            var p = queue.Dequeue();

            if(!region.Plots.Add(p))
                continue;

            foreach (var pp in GetAdjacentPlots(p))
            {
                //Console.WriteLine($"Lp. {count++,4}, Plot: {pp.row}, {pp.col}, {Map[pp.row][pp.col]}");

                if (Map[pp.row][pp.col] == region.Plant && !region.Plots.Contains(pp))
                    queue.Enqueue(pp);
            }
        }
        
        return region;
    }

    public IEnumerable<Region> GetRegions()
    {
        var visited = new HashSet<Plot>();

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                var plot = new Plot(row, col);
                
                if(visited.Contains(plot))
                    continue;

                var region = GetRegion(plot);
                
                foreach(var p in region.Plots)
                    visited.Add(p);
                
                yield return region;
            }
        }
    }
}

[DebuggerDisplay("Region: {Plant}, Plots: {Plots.Count}")]
record Region(char Plant)
{
    private readonly (int row, int col)[] _deltas = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    
    public HashSet<Plot> Plots { get; set; } = new();

    public int Area => Plots.Count;
    public int Perimeter => GetPerimeter();
    public int Sides => GetSides();
    public int FencePrice => Area * Perimeter;
    public int BulkFencePrice => Area * Sides;

    private int GetPerimeter()
    {
        return Plots.Sum(p => 4 - _deltas.Select(d => new Plot(p.row + d.row, p.col + d.col)).Count(p => Plots.Contains(p)));
    }

    // count of angles == count of sides
    int GetSides()
    {
        (int minRow, int maxRow) = GetBounds(Plots.Select(o => o.row).Distinct());
        (int minCol, int maxCol) = GetBounds(Plots.Select(o => o.col).Distinct());

        int angles = 0;
        
        for (int row = minRow-1; row < maxRow + 1; row++)
        {
            for (int col = minCol-1; col < maxCol + 1; col++)
            {
                angles += GetAngles(row, col);
            }
        }
        
        return angles;
    }

    // get window of 2x2 plot and count angles
    int GetAngles(int row, int col)
    {
        var a = new Plot(row, col);
        var b = new Plot(row + 1, col);
        var c = new Plot(row, col + 1);
        var d = new Plot(row + 1, col + 1);
        
        var count =  new[] {a,b,c,d}.Count(w => Plots.Contains(w));

        return count switch
        {
            1 or 3 => 1,
            2 => IsDiagonal() ? 2 : 0,
            _ => 0
        };

        bool IsDiagonal()
        {
            return Plots.Contains(a) && Plots.Contains(d) || Plots.Contains(b) && Plots.Contains(c);
        }
    }

    static (int min, int max) GetBounds(IEnumerable<int> values)
    {
        var sorted = values.OrderBy(o => o).ToList();
        return (sorted.First(), sorted.Last());
    }
}
