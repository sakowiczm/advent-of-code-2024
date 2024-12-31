// https://adventofcode.com/2024/day/9


// char c = '4';
// int i = 3;
// // convert int into representing it char and char into representing it int
// var s = new string((char)('0' + i), c - '0');



record Data(int Id, int Length, bool IsEmpty);
record File(int Id, int Length) : Data(Id, Length, false);
record Empty(int Length) : Data(0, Length, true);

class Program
{
    static void Main()
    {
        //string input = "2333133121414131402";
        string input = System.IO.File.ReadAllText("input.txt");
        var data = GetDecodedDiskData(input);

        CompactData(ref data);

        // 6399153661894 OK
        Console.WriteLine($"Part 1. Sum: {Calculate(ref data)}");
    }

    static long Calculate(ref LinkedList<Data> data)
    {
        int index = 0;
        long sum = 0;
        var node = data.First;
        
        while (node.Value is File)
        {
            sum += node.Value.Id * index;
            
            index++;
            node = node.Next;
        }

        return sum;
    }

    static void CompactData(ref LinkedList<Data> data)
    {
        var first = data.First;
        var last = data.Last;

        while (first != last && first!.Previous != last)
        {
            if (first.Value is File)
            {
                first = first.Next;    
                continue;
            }

            if (last.Value is Empty)
            {
                last = last!.Previous;
                continue;
            }

            (last.Value, first.Value) = (first.Value, last.Value);
        }
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
                for (int j = 0; j < (c - '0'); j++)
                {
                    data.AddLast(new File(index, 1));    
                }
                
                index++;
            }
            else
            {
                if(c == '0')
                    continue;
                
                for (int j = 0; j < (c - '0'); j++)
                {
                    data.AddLast(new Empty(1));    
                }                
            }
        }

        return data;
    }
    
}


