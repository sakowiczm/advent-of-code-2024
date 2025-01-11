// https://adventofcode.com/2024/day/13

using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var machines = input.Where(l => !string.IsNullOrWhiteSpace(l))
    .Chunk(3)
    .Select(o => string.Join(Environment.NewLine, o))
    .Select(Machine.Create)
    .ToList();

foreach (var machine in machines)
{
    var cost = machine.GetPressCount();

    Console.WriteLine(cost.HasValue
        ? $"Machine: {machine}, A: {cost.Value.a}, B: {cost.Value.b}. Cost: {machine.GetMinimalCost()}"
        : $"Machine: {machine}, No Solution.");
}

var p1TokenCount = machines.Sum(o => o.GetMinimalCost());
var p2TokenCount = machines.Select(o => o.CorrectConversionError()).Sum(o => o.GetMinimalCost());
 
Console.WriteLine($"P1 Cost: {p1TokenCount}");
Console.WriteLine($"P2 Cost: {p2TokenCount}");

record Machine(int Ax, int Ay, int Bx, int By, long PrizeX, long PrizeY)
{
    // todo: refactor not to return null
    
    public (long a, long b)? GetPressCount()
    {
        // Solving Linear Equations Using Determinants (Rozwiązanie układu równań z dwiema niewiadomymi metodą wyznaczników)
        // https://www.mathnstuff.com/math/algebra/adeterm.htm
        var determinant = Ax * By - Ay * Bx;
        
        if (determinant == 0)
            return null;
        
        var a = (PrizeX * By - PrizeY * Bx) / determinant;
        var b = (PrizeY * Ax - PrizeX * Ay) / determinant;
        
        // if (a > 100 || b > 100)
        //     return null;
        
        // check if is valid
        if (a * Ax + b * Bx != PrizeX || a * Ay + b * By != PrizeY)
             return null;
        
        return (a: a, b: b);
    }

    public long GetMinimalCost()
    {
        var pressCount = GetPressCount();
        return pressCount.HasValue ? pressCount.Value.a * 3 + pressCount.Value.b : 0;
    }
    
    public static Machine Create(string data)
    {
        var pattern = """
                      Button A: X\+(?<ax>[0-9]+), Y\+(?<ay>[0-9]+)
                      Button B: X\+(?<bx>[0-9]+), Y\+(?<by>[0-9]+)
                      Prize: X=(?<x>[0-9]+), Y=(?<y>[0-9]+)
                      """;

        var m = Regex.Match(data, pattern, RegexOptions.Multiline);

        var ax = int.Parse(m.Groups["ax"].Value);
        var ay = int.Parse(m.Groups["ay"].Value);
        var bx = int.Parse(m.Groups["bx"].Value);
        var by = int.Parse(m.Groups["by"].Value);
        var x = int.Parse(m.Groups["x"].Value);
        var y = int.Parse(m.Groups["y"].Value);
        
        return new Machine(ax, ay, bx, by, x, y);
    }

    public Machine CorrectConversionError()
    {
        return this with
        {
            PrizeX = PrizeX + 10000000000000,
            PrizeY = PrizeY + 10000000000000
        };
    }
}
