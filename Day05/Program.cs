// https://adventofcode.com/2024/day/5

// (int a, int b)[] rules =
// {
//     (47, 53),
//     (97, 13),
//     (97, 61),
//     (97, 47),
//     (75, 29),
//     (61, 13),
//     (75, 53),
//     (29, 13),
//     (97, 29),
//     (53, 29),
//     (61, 53),
//     (97, 53),
//     (61, 29),
//     (47, 13),
//     (75, 47),
//     (97, 75),
//     (47, 61),
//     (75, 61),
//     (47, 29),
//     (75, 13),
//     (53, 13),
// };
//
// // Sum 143
// int[][] input =
// [ // collection expression
//     new[] { 75, 47, 61, 53, 29 }, // OK
//     new[] { 97, 61, 53, 29, 13 }, // OK
//     new[] { 75, 29, 13 },         // OK  
//     new[] { 75, 97, 47, 61, 53 }, // not OK
//     new[] { 61, 13, 29 },         // not OK  
//     new[] { 97, 13, 75, 29, 47 }  // not OK 
// ];

var (rules, input) = GetInput("input.txt");

(HashSet<(int a, int b)> rules, int[][] input) GetInput(string fileName)
{
    var lines = File.ReadAllLines("input.txt");

    var rules = lines.TakeWhile(o => !string.IsNullOrWhiteSpace(o))
        .Select(o => o.Split('|'))
        .Select(o => (int.Parse(o[0]), int.Parse(o[1])))
        .ToHashSet();
    
    var input = lines.Skip(rules.Count + 1) 
        .Select(o => o.Split(',').Select(p => int.Parse(p)).ToArray())
        .ToArray();

    return (rules, input);
}


var afterRules = rules.GroupBy(pair => pair.a).ToDictionary(group => group.Key, group => group.Select(o => o.b).ToHashSet());
var beforeRules = rules.GroupBy(pair => pair.b).ToDictionary(group => group.Key, group => group.Select(o => o.a).ToHashSet());

// -1 = a < b
//  1 = a > b
//  0 = a == b
IComparer<int> comparer = Comparer<int>.Create((a, b) => rules.Contains((a, b)) ? -1 : rules.Contains((b, a)) ? 1 : 0);

// Correct sum: 4462
Console.WriteLine($"Part 1 V1. Sum: {input.Where(numbers => IsValid(numbers)).Sum(numbers => numbers[numbers.Length / 2])}");
Console.WriteLine($"Part 1 V2. Sum: {input.Where(numbers => IsValidV2(numbers)).Sum(numbers => numbers[numbers.Length / 2])}");
Console.WriteLine($"Part 1 V3. Sum: {input.Where(numbers => IsValidV3(numbers)).Sum(numbers => numbers[numbers.Length / 2])}");

// Correct sum: 6767
//Console.WriteLine($"Part 2 V1. Sum: {input.Where(numbers => !IsValid(numbers)).Select(o => Fix(o)).Sum(numbers => numbers[numbers.Length / 2])}");
//Console.WriteLine($"Part 2 V2. Sum: {input.Where(numbers => !IsValidV2(numbers)).Select(o => Fix(o)).Sum(numbers => numbers[numbers.Length / 2])}");
Console.WriteLine($"Part 2 V3. Sum: {input.Where(numbers => !IsValidV3(numbers)).Select(o => FixV2(o)).Sum(numbers => numbers[numbers.Length / 2])}");


bool IsValid(int[] pageNumbers)
{
    // check after & before
    return CheckDirection(pageNumbers, true) && CheckDirection(pageNumbers, false);
}

bool CheckDirection(int[] numbers, bool reverse)
{
    if(reverse)
        numbers = numbers.Reverse().ToArray();
    
    // we can skip first (last element)
    for(int i=1; i<numbers.Length; i++)
    {
        // todo: use HashSet
        
        var lr = reverse ? 
            rules.Where(o => o.a == numbers[i]).Select(o => o.b).ToList() : 
            rules.Where(o => o.b == numbers[i]).Select(o => o.a).ToList();

        // no rules try next
        if(lr.Count != 0)
        {
            // check every next element if it's in our lrules
            for(int j=i-1; j>=0; j--)
            {
                if (!lr.Contains(numbers[j]))
                    return false;
            }
        }
    }

    return true;
}

bool IsValidV2(int[] numbers)
{
    // create list of before & after numbers for each page number - and check both directions
    return CheckDirectionV2(numbers, beforeRules) && CheckDirectionV2(numbers.Reverse().ToArray(), afterRules);
}

bool CheckDirectionV2(int[] numbers, Dictionary<int, HashSet<int>> rules)
{
    // we can skip first (last element)
    for(int i=1; i<numbers.Length; i++)
    {
        if (rules.TryGetValue(numbers[i], out var elementRules))
        {
            for(int j=i-1; j>=0; j--)
            {
                if (!elementRules.Contains(numbers[j]))
                    return false;
            }
        }
    }

    return true;
}


// swap until there is no rules against it
int[] Fix(int[] numbers)
{
    while (true)
    {
        bool sorted = true;

        for (int i = 0; i < numbers.Length-1; i++)
        {
            if (rules.Contains((numbers[i+1],numbers[i])))
            {
                sorted = false;
                (numbers[i], numbers[i+1]) = (numbers[i+1], numbers[i]);
            }
        }

        if(sorted)
            return numbers;
    }
}

bool IsValidV3(int[] numbers)
{
    var pairs = new List<(int a, int b)>();
    
     // create pairs 
     for (int i = 0; i < numbers.Length; i++)
     {
         for (int j = i + 1; j < numbers.Length; j++)
         {
             pairs.Add((numbers[i], numbers[j]));        
         }
     }
    
     foreach (var pair in pairs)
     {
         if(comparer.Compare(pair.a, pair.b) > 0)
             return false;
     }
    
     return true;
     
    // the same using range operator & linq
    // var pairs = numbers.SelectMany((value, index) => numbers[(index + 1)..].Select(next => (value, next)));
    // return pairs.All(pair => comparer.Compare(pair.value, pair.next) <= 0);
}

int[] FixV2(int[] numbers)
{
    return numbers.Order(comparer).ToArray();
}