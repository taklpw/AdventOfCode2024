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
                levels
                .Zip(levels.Skip(1), (current, next) => current.CompareTo(next) < 0 || current.CompareTo(next) > 0)
                .All(isIncreasingorDecreasing => isIncreasingorDecreasing)
            )
            // Are within range
            .Where(levels =>
                levels
                .Zip(levels.Skip(1), (current, next) => Math.Abs(next - current))
                .Any(absdiff => absdiff > 1 && absdiff < 3)
            );
        return new($"{validReports.Count()}");
    }

    public override ValueTask<string> Solve_2()
    {

        return new($"");
    }
}
