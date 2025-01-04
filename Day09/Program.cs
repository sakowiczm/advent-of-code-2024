// https://adventofcode.com/2024/day/9

class Program
{
    static void Main()
    {
        //string input = "2333133121414131402"; // P1 1928 P2 2858
        string input = System.IO.File.ReadAllText("input.txt"); // P1 6399153661894 P2 6421724645083
        
        Console.WriteLine($"Part 1. Using Array. Output: {CalculateUsingArray.Calculate(input)}");
        Console.WriteLine($"Part 1. Using LinkedList. Output: {CalculateUsingLinkedList.Calculate(input)}");
        
        Console.WriteLine($"Part 2. Using LinkedList. Output: {CalculateUsingLinkedListPart2.Calculate(input)}");
    }
}

public static class CalculateUsingLinkedListPart2
{
    record Data(int Id, int Length);
    record File(int Id, int Length) : Data(Id, Length);
    record Empty(int Length) : Data(-1, Length);
    
    public static long Calculate(string input)
    {
        var data = GetDecodedDiskData(input);

        CompactData(ref data);

        return Calculate(ref data);
    }
    
    static long Calculate(ref LinkedList<Data> data)
    {
        int index = 0;
        long sum = 0;
        var node = data.First;
        
        while (node.Next != null)
        {
            if (node.Value is File)
            {
                for (int i = 0; i < node.Value.Length; i++)
                {
                    sum += node.Value.Id * index;
                    index++;
                }
            }
            else
            {
                // empty
                index += node.Value.Length;
            }
            
            node = node.Next;
        }
        
        return sum;
    }

    static void CompactData(ref LinkedList<Data> data)
    {
        int fileId = GetNextFile(data.Last).Value.Id;
        LinkedListNode<Data> file = data.Last;
        
        while (fileId >= 0)
        {
            file = GetFileWithId(file, fileId);

            var space = GetNextSpace(data.First, file);
            
            if(space != null)
                Swap(ref data, space, file);
            
            fileId--;
        }
        
    }
    
    static void Swap(ref LinkedList<Data> data, LinkedListNode<Data> space, LinkedListNode<Data> file)
    {
        if (space.Value.Length >= file.Value.Length)
        {
            int el = space.Value.Length;
            int fl = file.Value.Length;

            // move file from end to the empty space
            data.AddBefore(space, file.Value);
            // empty the space at the end
            file.Value = new Empty(fl);
                
            // some empty space left at the beginning
            if (el != fl)
            {
                space.Value = new Empty(el - fl);
            }
            else // if equal
            {
                data.Remove(space);
            }                  
        }
    }

    static LinkedListNode<Data>? GetNextSpace(LinkedListNode<Data> startNode, LinkedListNode<Data> endNode)
    {
        if (startNode.Value is Empty && startNode.Value.Length == endNode.Value.Length)
            return startNode;

        while (startNode.Value is File || (startNode.Value is Empty && startNode.Value.Length < endNode.Value.Length))
        {
            if (startNode.Next == null || startNode == endNode || startNode.Next == endNode)
                return null;
            
            startNode = startNode.Next;
        }

        return startNode;
    }

    static LinkedListNode<Data> GetFileWithId(LinkedListNode<Data> node, int fileId)
    {
        if (node.Value is File && node.Value.Id == fileId)
            return node;
        
        while (node.Value is Empty || (node.Value is File && node.Value.Id != fileId))
        {
            node = node.Previous;
        }
        
        return node;
    }
    
    static LinkedListNode<Data> GetNextFile(LinkedListNode<Data> data)
    {
        if (data.Value is File)
            return data;

        while (data.Value is Empty)
        {
            data = data.Previous;
        }

        return data;
    }

    static LinkedList<Data> GetDecodedDiskData(string input)
    {
        LinkedList<Data> data = new LinkedList<Data>();

        int index = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];

            if (i % 2 == 0)
            {
                data.AddLast(new File(index, c - '0'));
                index++;
            }
            else
            {
                if(c == '0')
                    continue;

                data.AddLast(new Empty(c - '0'));
            }
        }

        return data;
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

