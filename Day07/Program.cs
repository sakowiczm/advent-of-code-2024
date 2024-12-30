// https://adventofcode.com/2024/day/7

using System.Diagnostics;

class Program
{
    static void Main()
    {
        var equationValues = File.ReadAllLines("input.txt").Select(o => GetEquationValues(o)).ToArray();
        
        Stopwatch timer = Stopwatch.StartNew();
        
        // Part 1
        long sum = 0;
        
        foreach (var ev in equationValues)
        {
            // number of permutations = operation characters ^ (length-1) i.e. 2^(length-1) 
            var canAdd = GetOperationPermutations(['+', '*'], ev.Values.Length - 1)
                .Any(p => ev.Total == GetEquationResult(ev.Values, p));
            
            if (canAdd)
            {
                sum += ev.Total;
            }
        }
        
        Console.WriteLine($"Part 1: Sum: {sum}");
        
        // Part 2
        sum = 0;
        
        foreach (var ev in equationValues)
        {
            // number of permutations 3^(length-1)
            var canAdd = GetOperationPermutations(['+', '*', '|'], ev.Values.Length - 1)
                .Any(p => ev.Total == GetEquationResult(ev.Values, p));
            
            if (canAdd)
            {
                sum += ev.Total;
            }
        }
        
        Console.WriteLine($"Part 2: Sum: {sum}");
        
        
        timer.Stop();
        
        Console.WriteLine($"Elapsed: {timer.ElapsedMilliseconds}");
        
    }
    
    private static long GetEquationResult(long[] values, string operators)
    {
        long result = values[0];
        
        for (int i = 0; i < values.Length - 1; i++)
        {
            if (operators[i] == '+')
            {
                result += values[i + 1];
                continue;
            }

            if (operators[i] == '*')
            {
                result *= values[i + 1];
                continue;
            }

            if (operators[i] == '|')
            {
                result = long.Parse(result.ToString() + values[i+1].ToString());
            }
        }

        return result;
    }    

    static (long Total, long[] Values) GetEquationValues(string input)
    {
       int delimiter = input.IndexOf(':');

       long total = long.Parse(input.Substring(0, delimiter));

       var values = input.Substring(delimiter + 2, input.Length - delimiter - 2)
           .Split(' ')
           .Select(o => long.Parse(o))
           .ToArray();

       return (Total: total, Values: values);
    }
    
    static List<string> GetOperationPermutations(char[] characters, int length)
    {
        List<string> permutations = new List<string>();
        permutations.Add("");

        for (int i = 0; i < length; i++)
        {
            List<string> newPermutations = new List<string>();

            foreach (string perm in permutations)
            {
                foreach (char c in characters)
                {
                    newPermutations.Add(perm + c);
                }
            }

            permutations = newPermutations;
        }

        return permutations;
    }

    // Slower than GetOperationPermutations
    static IEnumerable<string> GetOperationPermutationsUsingRecursion(char[] characters, int length)
    {
        return GetOperationPermutationsUsingRecursion(characters, new char[length], 0);
    }
    
    static IEnumerable<string> GetOperationPermutationsUsingRecursion(char[] characters, char[] currentString, int position)
    {
        if (position == currentString.Length)
        {
            yield return new string(currentString);
            yield break;
        }

        foreach (char c in characters)
        {
            currentString[position] = c;
            foreach (var permutation in GetOperationPermutationsUsingRecursion(characters, currentString, position + 1))
            {
                yield return new string(permutation);
            }
        }
    }
    
    static IEnumerable<string> GetOperationPermutationsUsingQueue(char[] characters, int length)
    {
        Queue<string> queue = new Queue<string>();
        queue.Enqueue("");

        while (queue.Count > 0)
        {
            string current = queue.Dequeue();

            if (current.Length == length)
            {
                yield return current;
            }
            else
            {
                foreach (char c in characters)
                {
                    queue.Enqueue(current + c);
                }
            }
        }
    }


}
