// https://adventofcode.com/2024/day/1

var (left, right) = ReadInput();

var totalDistance = left.Order()
    .Zip(right.Order(), (l, r) => Math.Abs(l - r))
    .Sum();


var index = left
    .GroupBy(v => v, (value, count) => (value: value, count: count.Count()))
    .ToDictionary(p => p.value, p => p.count);

int similarityScore = right
    .Where(index.ContainsKey)
    .Sum(r => r * index[r]);

Console.WriteLine(totalDistance); // 765748
Console.WriteLine(similarityScore); // 27732508


static (List<int> left, List<int> right) ReadInput(string file = @".\input.txt")
{
    (List<int> left, List<int> right) = (new(), new());

    using (TextReader reader = new StreamReader(file))
    {
        //while (reader.ReadLine() is string line)
        // https://www.jetbrains.com/help/resharper/ConvertTypeCheckToNullCheck.html
        // https://www.reddit.com/r/csharp/comments/wae89g/rider_suggestion_use_not_null_pattern_instead_of/?rdt=55489
        
        // todo: investigate
        while (reader.ReadLine() is { } line)
        {
            var values = line.Split(' ', 2);
            left.Add(int.Parse(values[0]));
            right.Add(int.Parse(values[1]));
        }
    }

    return (left, right); 
}
