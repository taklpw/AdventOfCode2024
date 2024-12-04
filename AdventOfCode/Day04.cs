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
            (1, 0),     // Vertical up
            (-1, 0),    // Vertical down
            (1, 1),     // Diagonal up right
            (1, -1),    // Diagonal up left
            (-1, 1),    // Diagonal down right
            (-1, -1),   // Diagonal down left
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


    private bool IsMasCrossAtPosition(List<string> grid, int startRow, int startCol, int rowStep, int colStep)
    {
        // Can we make a cross MAS at the current position
        // M . S
        //   A
        // M . S

        // Positions for the cross pattern
        var crossPositions = new[]
        {
            (startRow, startCol),                               // First M
            (startRow + rowStep, startCol + colStep),           // A
            (startRow + 2 * rowStep, startCol + 2 * colStep),   // S
            (startRow + 3 * rowStep, startCol + 3 * colStep)    // Final M
        };

        // Check if all positions are within grid bounds
        return crossPositions.All(pos =>
            pos.Item1 >= 0 &&
            pos.Item1 < grid.Count &&
            pos.Item2 >= 0 &&
            pos.Item2 < grid[0].Length
        )
        // Check for full MAS cross pattern
        && new[]
        {
            grid[crossPositions[0].Item1][crossPositions[0].Item2] == 'M',
            grid[crossPositions[1].Item1][crossPositions[1].Item2] == 'A',
            grid[crossPositions[2].Item1][crossPositions[2].Item2] == 'S',
            grid[crossPositions[3].Item1][crossPositions[3].Item2] == 'M'
        }.All(x => x);
    }



    public override ValueTask<string> Solve_2()
    {
        var directions = new List<(int rowDir, int colDir)>()
        {
            (0, 1),   // Horizontal
            (1, 0),   // Vertical
            (1, 1),   // Diagonal up right
            (1, -1)   // Diagonal up left
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
                    IsMasCrossAtPosition(_wordSearch, row, col, direction.rowDir, direction.colDir) ||
                    IsMasCrossAtPosition(_wordSearch, row, col, -direction.rowDir, -direction.colDir)
                )
            );
        });

        return new($"{targetCount}");
    }
}
