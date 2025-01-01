// https://adventofcode.com/2024/day/9

class Program
{
    static void Main()
    {
        //string input = "2333133121414131402"; // 1928
        string input = System.IO.File.ReadAllText("input.txt"); // 6399153661894
        
        Console.WriteLine($"Part 1. Using Array. Sum: {CalculateUsingArray.Calculate(input)}");
        Console.WriteLine($"Part 1. Using LinkedList. Sum: {CalculateUsingLinkedList.Calculate(input)}");
    }
}

public static class CalculateUsingLinkedList
{
    public static long Calculate(string input)
    {
        var data = GetDecodedDiskData(input);

        CompactData(ref data);

        return Calculate(ref data);
    }
    
    static long Calculate(ref LinkedList<int> data)
    {
        int index = 0;
        long sum = 0;
        var node = data.First;
        
        while (node.Value >= 0)
        {
            sum += node.Value * index;

            index++;
            node = node.Next;
        }

        return sum;
    }

    static void CompactData(ref LinkedList<int> data)
    {
        var first = data.First;
        var last = data.Last;

        while (first != last && first!.Previous != last)
        {
            if (first.Value >= 0)
            {
                first = first.Next;
                continue;
            }

            if (last.Value == -1)
            {
                last = last.Previous;
                continue;
            }

            (last.Value, first.Value) = (first.Value, last.Value);
        }
    }

    static LinkedList<int> GetDecodedDiskData(string input)
    {
        LinkedList<int> data = new LinkedList<int>();

        int index = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];

            if (i % 2 == 0)
            {
                for (int j = 0; j < (c - '0'); j++)
                {
                    data.AddLast(index);
                }

                index++;
            }
            else
            {
                if(c == '0')
                    continue;

                for (int j = 0; j < (c - '0'); j++)
                {
                    data.AddLast(-1);
                }
            }
        }

        return data;
    }
}

public static class CalculateUsingArray
{
    public static long Calculate(string input)
    {
        var data = GetDecodedDiskData(input);

        CompactData(ref data);

        return Calculate(ref data);
    }

    private static void CompactData(ref int[] data)
    {
        int i = 0;
        int j = data.Length-1; 

        while (i <= j)
        {
            if (data[i] != -1) // -1 means empty
            {
                i++;
                continue;
            }

            if (data[j] == -1)
            {
                j--;
                continue;
            }

            (data[j], data[i]) = (data[i], data[j]);
        }
    }

    private static long Calculate(ref int[] data)
    {
        long sum = 0;
        int i = 0;

        while (data[i] != -1)
        {
            sum += i * data[i];
            i++;
        }

        return sum;
    }

    private static int[] GetDecodedDiskData(string input)
    {
        List<int> data = new List<int>();
        
        int index = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            
            if (i % 2 == 0)
            {
                for (int j = 0; j < (c - '0'); j++)
                {
                    data.Add(index);
                }
                
                index++;
            }
            else
            {
                if(c == '0')
                    continue;
                
                for (int j = 0; j < (c - '0'); j++)
                {
                    data.Add(-1); // -1 means empty
                }                
            }
        }

        return data.ToArray();
    }
}

