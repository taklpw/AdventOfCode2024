namespace AdventOfCode;

public class Day02 : BaseDay
{
    private readonly string _input;
    private List<List<int>> _reports;


    public Day02()
    {
        _reports = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string levelsLine;
            while ((levelsLine = r.ReadLine()) != null)
            {
                _reports.Add(levelsLine.Split(" ").Select(int.Parse).ToList());
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        var validReports = _reports
            // Are increasing or decreasing
            .Where(levels =>
                // Increasing
                levels.SequenceEqual(levels.OrderBy(level => level)) ||
                // Decreasing
                levels.SequenceEqual(levels.OrderByDescending(level => level))
            )
            // Are within range
            .Where(levels =>
                levels
                .Zip(levels.Skip(1), (current, next) => Math.Abs(next - current))
                .All(absdiff => absdiff >= 1 && absdiff <= 3)
            );
        return new($"{validReports.Count()}");
    }

    public override ValueTask<string> Solve_2()
    {

        return new($"");
    }
}
