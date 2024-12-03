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
        var validReports = _reports
            .Where(levels =>
                // Generate indicies for levels
                Enumerable.Range(0, levels.Count)
                // Check if excluding any one index makes the report valid
                .Any(skipIndex =>
                    (levels
                    // Skip the level where the index matches the currently skipped index
                    .Where((level, index) => index != skipIndex)
                    .OrderBy(level => level)
                    // Check if the sorted list with the removed index is the same as the list with the removed index
                    .SequenceEqual(levels.Where((level, index) => index != skipIndex))
                    ||
                    // Again but decending
                    levels
                    .Where((level, index) => index != skipIndex)
                    .OrderByDescending(level => level)
                    .SequenceEqual(levels.Where((level, index) => index != skipIndex)))

                    // Using and and here this time since we're skipping indicies
                    &&

                    // Check if the level is also within range
                    levels
                    .Where((level, index) => index != skipIndex)
                    .Zip(
                        // Generate reports where the skipped index is uhh skipped
                        levels
                        .Where((level, index) => index != skipIndex)
                        .Skip(1),
                        // Apply the difference function
                        (current, next) => Math.Abs(next - current))
                    .All(absdiff => absdiff >= 1 && absdiff <= 3)
                )
            );

        return new($"{validReports.Count()}");
    }
}
