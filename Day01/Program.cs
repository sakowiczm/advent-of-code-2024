// See https://aka.ms/new-console-template for more information

// https://adventofcode.com/2024/day/1

var (left, right) = ReadInput();

var totalDistance = left.Order()
    .Zip(right.Order(), (l, r) => Math.Abs(l - r))
    .Sum();

// my code 
// todo: if the same number is duplicated on the left side we will be calculating count multiple times
int similarityScore = left.Sum(i => right.Count(o => o == i) * i);

// others first approach

similarityScore = right
    .Where(x => left.Contains(x)) // O(n) * O(n) = O(n^2) - quadratic
    .GroupBy(x => x)
    .Sum(group => group.Key * group.Count());

// todo: how HashSet is created just once - why?

// this is wrong solution - duplicate numbers won't be taken into consideration - idea is that multiple addition replace multiplication 
// similarityScore = right
//     .Where(new HashSet<int>(left).Contains) // O(1) * O(n) = O(n) - linear time
//     .Sum();

var index = left
    .GroupBy(v => v, (value, count) => (value: value, count: count.Count()))
    //todo:  ToFrozenDictionary??
    .ToDictionary(p => p.value, p => p.count);

similarityScore = right
    .Where(index.ContainsKey)
    .Sum(r => r * index[r]);
        

// todo: learn how to use benchmark dot net?
        




    



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
