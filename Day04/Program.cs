// https://adventofcode.com/2024/day/4

string[] input = File.ReadAllLines("input.txt");

// Part 1

// XMAS is here 18 times - horizontal, vertical, diagonal, written backwards - can overlap
// string[] input =
// {
//     "MMMSXXMASM",
//     "MSAMXMSMSA",
//     "AMXSXMAAMM",
//     "MSAMASMSMX",
//     "XMASAMXAMM",
//     "XXAMMXXAMA",
//     "SMSMSASXSS",
//     "SAXAMASAAA",
//     "MAMMMXMMMM",
//     "MXMXAXMASX"
// };

// Part 1 correct value is 2545
Console.WriteLine($"Part 1: {Part1(input)}");
Console.WriteLine($"Part 1 V2: {Part1V2(input)}");

// Part 2

// X-MAS appears 9 times.
// string[] input =
// {
//     ".M.S......",
//     "..A..MSMS.",
//     ".M.S.MAA..",
//     "..A.ASMSM.",
//     ".M.S.M....",
//     "..........",
//     "S.S.S.S.S.",
//     ".A.A.A.A..",
//     "M.M.M.M.M.",
//     ".........."
// };

// Part 2: 1886
Console.WriteLine($"Part 2: {Part2(input)}");


int Part1(string[] input)
{
    List<string> values = new List<string>();
    values.AddRange(input); // horizontal 
    values.AddRange(GetVerticals(input));
    values.AddRange(GetDiagonals(input));
    values.AddRange(GetAntiDiagonals(input));

    return values.Select(o => FindValue(o, "XMAS") + FindValue(o, "SAMX")).Sum();
}

int Part1V2(string[] input)
{
    int count = 0;
    
    (int x, int y)[] directions =
    {
        (-1, -1), (0, -1), (+1, -1),
        (-1,  0),          (+1,  0),
        (-1, +1), (0, +1), (+1, +1)
    };
    
    // for each input character
    for (int x = 0; x < input.Length; x++)
    {
        for (int y = 0; y < input.Length; y++)
        {
            // for each direction
            foreach (var direction in directions)
            {
                if(HasXmas(input, x, y, direction))
                    count++;                
            }
        }
    }

    return count;
}

bool HasXmas(string[] input, int x, int y, (int x, int y) direction)
{
    for (int i = 0; i < "XMAS".Length; i++)
    {
        int dx = x + direction.x * i;
        int dy = y + direction.y * i;
        
        if (!(dx >= 0 && dx < input.Length && dy >= 0 && dy < input.Length))
            return false;

        if(input[dx][dy] != "XMAS"[i])
            return false;
    }
    
    return true;
}

int FindValue(string input, string searchValue)
{
    int count = 0;   
    int startIndex = 0;

    while (startIndex <= input.Length - searchValue.Length)
    {
        int index = input.IndexOf(searchValue, startIndex);

        if (index == -1)
            break;
        
        count++;
        startIndex = index + 1;
    }

    return count;
}

IEnumerable<string> GetDiagonals(string[] input)
{
    // assumption input is a square
    string output = GetDiagonal(input, 0, 0);
        
    if(output.Length >= 4)
        yield return output;

    for (int i = 1; i < input.Length; i++)
    {
        output = GetDiagonal(input, i, 0);
        
        if(output.Length >= 4)
            yield return output;
        
        output = GetDiagonal(input, 0, i);
        
        if(output.Length >= 4)
            yield return output;
    }
}

IEnumerable<string> GetAntiDiagonals(string[] input)
{
    return GetDiagonals(Reverse(input));
}

string GetDiagonal(string[] input, int x, int y)
{
    char[] result = new char[input.Length];

    for (int i = 0; i < input.Length; i++)
    {
        int dx = x + i;
        int dy = y + i;
        
        if(dx < input.Length && dy < input.Length)
            result[i] = input[dx][dy];
    }

    return new string(result);
}

// Matrix transpose
static string[] GetVerticals(string[] input)
{
    int rows = input.Length;
    int cols = input[0].Length;

    string[] result = new string[cols];

    for (int j = 0; j < cols; j++)
    {
        char[] newRow = new char[rows];
        for (int i = 0; i < rows; i++)
        {
            newRow[i] = input[i][j];
        }
        result[j] = new string(newRow);
    }

    return result;
}

string[] Reverse(string[] input)
{
    string[] result = new string[input.Length];

    for (int i = 0; i < input.Length; i++)
    {
        var o = input[i].ToCharArray();
        Array.Reverse(o);
        
        result[i] = new string(o);
    }
    
    return result;
}

string ReverseV2(string input)
{
    char[] reversed = new char[input.Length];

    for (int i = 0; i < input.Length; i++)
    {
        reversed[i] = input[input.Length - 1 - i];
    }
    
    return new string(reversed);
}

int Part2(string[] input)
{
    string[] expected = ["MASMAS", "SAMSAM", "MASSAM", "SAMMAS"];
    int count = 0;
    
    // assuming square - one less
    for (int x = 1; x < input.Length - 1; x++)
    {
        for (int y = 1; y < input.Length - 1; y++)
        {
            if (input[x][y] != 'A' && input[x][y] != 'a') continue;
            
            if(expected.Contains(GetX(input, x, y)))
                count++;
        }
    }
    
    return count;
}

string GetX(string[] input, int x, int y)
{
    char a = input[x - 1][y - 1];
    char b = input[x][y];
    char c = input[x + 1][y + 1];
    char d = input[x + 1][y - 1];
    char e = input[x - 1][y + 1];

    return new string(new[] { a, b, c, d, b, e });
}