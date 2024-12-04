using Spectre.Console;

namespace AdventOfCode;

public class Day04 : BaseDay
{
    private readonly string _input;
    private List<string> _wordSearch;

    public Day04()
    {
        _wordSearch = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                _wordSearch.Add(line);
            }
        }
    }

    private bool IsTargetAtPosition(List<string> grid, string target, int startRow, int startColumn, int rowDir, int columnDir)
    {
        bool isTargetAtPosition =
            Enumerable.Range(0, target.Length)
            .All(i =>
            {
                int currentRow = startRow + i * rowDir;
                int currentColumn = startColumn + i * columnDir;

                bool isInGrid = currentRow >= 0 &&
                       currentRow < grid.Count &&
                       currentColumn >= 0 &&
                       currentColumn < grid[0].Count();

                return isInGrid && grid[currentRow][currentColumn] == target[i];
            });
        return isTargetAtPosition;
    }

    public override ValueTask<string> Solve_1()
    {
        const string target = "XMAS";

        var directions = new List<(int rowDir, int colDir)>()
        {
            (0, 1),     // Horizontal right
            (0, -1),    // Horizontal left
            (1, 0),     // Vertical down
            (-1, 0),    // Vertical up
            (1, 1),     // Diagonal down right
            (1, -1),    // Diagonal down left
            (-1, 1),    // Diagonal up right
            (-1, -1),   // Diagonal up left
        };

        int targetCount = directions.Sum(direction => 
        {
            return
            // Go along row
            Enumerable.Range(0, _wordSearch.Count())
            .Sum(row =>
                // Go along column
                Enumerable.Range(0, _wordSearch[0].Length)
                .Count(col => 
                    IsTargetAtPosition(_wordSearch, target, row, col, direction.rowDir, direction.colDir)
                )
            );
        });

        return new($"{targetCount}");
    }

    public override ValueTask<string> Solve_2()
    {
        int targetCount = _wordSearch
            .SelectMany((row, i) =>
                // Is the current line the possible centre of a 3x3 grid?
                i >= 1 && i < _wordSearch.Count - 1 ?
                // Is the current string index the possble centre of a 3x3 grid?
                row.Select((_, j) => (i, j)).Where(t => t.j > 0 && t.j < row.Length - 1) :
                // If not, give a nothing enumerable
                Enumerable.Empty<(int i, int j)>())
            // If we're in a 3x3 grid position and the centre square is A we're in business 
            .Where(pos => _wordSearch[pos.i][pos.j] == 'A')
            .Select(pos =>
            {
                // Create sub-grid
                List<string> subgrid = new List<string>()
                {
                    _wordSearch[pos.i-1].Substring(pos.j-1, 3),
                    _wordSearch[pos.i].Substring(pos.j-1, 3),
                    _wordSearch[pos.i+1].Substring(pos.j-1, 3),
                };

                bool downRight =
                    (subgrid[0][0] == 'M' && subgrid[2][2] == 'S') ||
                    (subgrid[0][0] == 'S' && subgrid[2][2] == 'M');

                bool downLeft =
                    (subgrid[0][2] == 'M' && subgrid[2][0] == 'S') ||
                    (subgrid[0][2] == 'S' && subgrid[2][0] == 'M');

                return downLeft && downRight;
            })
            .Count(result => result);
        return new($"{targetCount}");
    }
}
