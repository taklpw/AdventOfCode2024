using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day03 : BaseDay
{
    private readonly string _input;
    private string _corruptedMemory;

    public Day03()
    {
        _corruptedMemory = "";
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                _corruptedMemory += line;
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        string pattern = @"mul\((\d+)\,(\d+)\)";
        Regex rg = new Regex(pattern);
        MatchCollection matches = rg.Matches(_corruptedMemory);
        int total = matches.Select(match =>
        {
            return int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
        }).Sum();
        return new($"{total}");
    }

    public override ValueTask<string> Solve_2()
    {
        string pattern = @"mul\(\d+\,\d+\)|(do\(\))|(don\'t\(\))";
        Regex rg = new Regex(pattern);
        MatchCollection matches = rg.Matches(_corruptedMemory);
        var total = matches.Select((match, index) => new { Match = match, Index = index })
            // Keep a summation over the entire match collection
            .Aggregate(
                new { Total = 0, IsSumming = true },
                (accumulator, current) =>
                {
                    switch (current.Match.Value)
                    {
                        case "do()":
                            return new { accumulator.Total, IsSumming = true };
                        case "don't()":
                            return new { accumulator.Total, IsSumming = false };
                    }

                    if (current.Match.Value.StartsWith("mul") && accumulator.IsSumming)
                    {
                        Regex mulRegex = new Regex(@"mul\((\d+)\,(\d+)\)");
                        Match mulMatch = mulRegex.Match(current.Match.Value);

                        var x = int.Parse(mulMatch.Groups[1].Value);
                        var y = int.Parse(mulMatch.Groups[2].Value);
                        return new { Total = accumulator.Total + x * y, accumulator.IsSumming };
                    }

                    return accumulator;
                }
            ).Total;
        return new($"{total}");
    }
}
