// https://adventofcode.com/2024/day/2

// Part 1 - correct 213
// Part 2 - correct 285

var reports = GetReports();
var safe = reports.Count(r => r.Check());
Console.WriteLine($"Safe: {safe}");

// foreach (var report in reports)
// {
//     Console.WriteLine(report.ToString());
// }

return;

IEnumerable<Report> GetReports(string fileName = @"input\input.txt")
{
    return File.ReadLines(fileName).Select(line => new Report(line));
}

public class Report
{
    private List<int> Levels { get; }
    
    public Report(string levels)
    {
        var stringLevels = levels.Split(' ').ToList();
        Levels = stringLevels.Select(o => int.Parse(o)).ToList();
    }

    public bool Check()
    {
        //return Check(Levels) || GetProblemDampenedLevels().Any(o => Check(o));
        return Check(Levels) || Expand(Levels).Any(o => Check(o));
    }
    
    private bool Check(List<int> levels)
    {
        if (levels.Count < 3)
            return true;
        
        int diff = Math.Sign(levels[1] - levels[0]);
        
        for (int i = 0; i < levels.Count-1; i++)
        {
            int r = levels[i + 1] - levels[i];
            
            if (Math.Abs(r) <= 3 && Math.Abs(r) >= 1 && diff == Math.Sign(r))
            {
                continue;
            }
            
            return false;
        }        
        
        return true;
    }    

    // Alternative approach
    private bool Check01(List<int> levels)
    {

        if (levels.Count < 3)
            return true;
        
        Func<int, int, bool> diff = levels[0] < levels[1] ? (x, y) => x >= y : (x, y) => x <= y;
        
        for (int i = 0; i < levels.Count-1; i++)
        {
            if (Math.Abs(levels[i] - levels[i + 1]) > 3 || diff(levels[i], levels[i + 1]))
            {
                return false;
            }
        }        
        
        return true;
    }
    
    // remove every potential level
    private IEnumerable<List<int>> GetProblemDampenedLevels()
    {
        for (var i = 0; i < Levels.Count; i++)
        {
            var tmp = new List<int>(Levels);
            tmp.RemoveAt(i);
        
            yield return tmp;
        }
    }
    
    private IEnumerable<List<int>> Expand(List<int> values) =>
        new[] {values}.Concat(Enumerable.Range(0, values.Count).Select(i => ExceptAt(values, i)));
    
    private List<int> ExceptAt(List<int> values, int index) =>
        values.Take(index).Concat(values.Skip(index + 1)).ToList();
    

    public override string ToString()
    {
        return "Levels: " + string.Join(", ", Levels) + $", Safe: { Check() }";
    }
}