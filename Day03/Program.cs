// https://adventofcode.com/2024/day/3

using System.Text.RegularExpressions;

//string input = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))mul(12,43)";
//string input = "don't()xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))mul(12,43)";
//string input = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))mul(12,43)don't()";
//string input = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]don't()then(mul(11,8)mul(8,5))mul(12,43)";
//string input = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]don't()then(mul(11,8)do()mul(8,5))mul(12,43)";
//string input = "xmul(2,4)don't()%&mul[3,7]do()!@^do_not_mul(5,5)+mul(32,64]don't()then(mul(11,8)do()mul(8,5))mul(12,43)don't()";
//string input = "don't()don't()";

string input = string.Concat(File.ReadLines("input.txt"));

Console.WriteLine("Length: " + input.Length);

// todo: how to write below using spans?
// https://learn.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay

var sum = GetAllowedSubstrings(input).Select(o => GetSum(o)).Sum();
Console.WriteLine("Sum: " + sum);


IEnumerable<string> GetAllowedSubstrings(string ainput)
{
    int j = 0, k = 0;

    do
    {
        j = ainput.IndexOf("don't()", j, StringComparison.InvariantCultureIgnoreCase);

        if (j == -1)
        {
            yield return ainput.Substring(k, ainput.Length - k);
            continue;
        }
    
        yield return ainput.Substring(k, j - k);
    
        k = ainput.IndexOf("do()", j, StringComparison.InvariantCultureIgnoreCase);

        if (k == -1)
            yield return "";

        j = k;

    } while (j > 0 && k > 0);
}

int GetSum(string binput)
{
    int i = 0, sum = 0;

    do
    {
        i = binput.IndexOf("mul(", i, StringComparison.InvariantCultureIgnoreCase);
    
        if(i == -1)
            continue;

        var tmp = i + 12 > binput.Length ? binput.Substring(i, binput.Length - i) : binput.Substring(i, 12);

        // can compile regex
        sum += Regex.Matches(tmp, @"(?<mul>mul)\((?<a>\d{1,3}),(?<b>\d{1,3})\)")
            .Select(o => int.Parse(o.Groups["a"].Value) * int.Parse(o.Groups["b"].Value)).Sum();

        i += 4;

    } while (i != -1 && i < binput.Length-1);

    return sum;
}
