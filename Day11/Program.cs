// https://adventofcode.com/2024/day/11

//var input = "125 17";
var input = "0 7 198844 5687836 58 2478 25475 894";


// Part 1

var stones = input.Split(" ").Select(o => Convert.ToInt64(o));
Print(stones, 0);

int blinks = 25;
for (int i = 1; i <= blinks; i++)
{
    stones = ApplyRules(stones);
    //Print(stones, i);
}

Console.WriteLine($"Part 1. Count: {stones.Count()}");


// Part 2

// Many numbers repeat - keep stone only once with count of stones it 'produces'
// To understand it better draw it as a tree with incremented count of stones 
// that are created during each blik

// init cache
stones = input.Split(" ").Select(o => Convert.ToInt64(o));

Dictionary<long, long> state = new Dictionary<long, long>(); 
foreach (var stone in stones)  
{
    state.Add(stone, 1);
}

blinks = 75;
for (int i = 0; i < blinks; i++)
{
    Dictionary<long, long> newState = new Dictionary<long, long>();
    foreach (var stone in state)
    {
        Blink(stone.Key, stone.Value, newState);
    }
    
    state = newState;
}

Console.WriteLine($"Part 2. Count: {state.Sum(o => o.Value)}");

void Blink(long stone, long count, Dictionary<long, long> newState)
{
    void AddOrUpdateCount(long key, long value)
    {
        if (!newState.TryAdd(key, value))
        {
            newState[key] += value;
        }        
    }

    if (stone == 0)
    {
        AddOrUpdateCount(1, count);
        return;
    }

    int len = stone.ToString().Length;  
    if (len % 2 == 0)  
    {        
        int l = len / 2;
    
        var a = Convert.ToInt64(stone.ToString()[..l]);
        var b = Convert.ToInt64(stone.ToString()[l..]);
    
        AddOrUpdateCount(a, count);
        AddOrUpdateCount(b, count);
        return;
    }
    
    AddOrUpdateCount(stone * 2024, count);
} 

IEnumerable<long> ApplyRules(IEnumerable<long> stones)
{
    foreach (var stone in stones)
    {
        // Rule 1
        if (stone == 0)
        {
            yield return 1;
            continue;
        }
        
        // Rule 2
        int len = stone.ToString().Length;  
        if (len % 2 == 0)  
        {        
            int l = len / 2;
    
            yield return Convert.ToInt64(stone.ToString()[..l]);
            yield return Convert.ToInt64(stone.ToString()[l..]);
            continue;
        }        
        
        // Rule 3
        yield return stone * 2024;
    }
}

void Print(IEnumerable<long> stones, int blink)
{
    Console.WriteLine($"Blink: {blink}. Stones: " + string.Join(" ", stones));
}





