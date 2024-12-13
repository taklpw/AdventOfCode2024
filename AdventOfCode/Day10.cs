using Spectre.Console;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AdventOfCode;

public class Day10 : BaseDay
{
    private List<List<int>> _heightMap;


    public Day10()
    {
        _heightMap = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                _heightMap.Add(line.Select(c => int.Parse(c.ToString())).ToList());
            }
        }
    }

    private bool IsInGrid((int i, int j) loc) =>
        loc.i >= 0 && loc.j >= 0 && loc.i < _heightMap.Count && loc.j < _heightMap[0].Count;


    List<List<(int, int)>> FindPaths((int i, int j) startCoordinate, Dictionary<(int i, int j), List<(int i, int j)>> validMoves, bool distinct)
    {
        var validPaths = new List<List<(int i, int j)>>();
        var foundEnds = new List<(int i, int j)>();

        // Using a stack because who needs recursion
        var pathStack = new Stack<List<(int i, int j)>>();
        pathStack.Push(new List<(int i, int j)> { startCoordinate });

        var visited = new HashSet<(int i, int j)>();

        while (pathStack.Count > 0)
        {
            var path = pathStack.Pop();
            var currentCoordinate = path[path.Count - 1];

            // Finished path and the 9 hasn't been seen before 
            if (distinct)
            {

                if (_heightMap[currentCoordinate.i][currentCoordinate.j] == 9)
                {
                    foundEnds.Add(currentCoordinate);
                    validPaths.Add(path);
                    continue;
                }
            }
            else
            {
                if (_heightMap[currentCoordinate.i][currentCoordinate.j] == 9 && !foundEnds.Contains(currentCoordinate))
                {
                    foundEnds.Add(currentCoordinate);
                    validPaths.Add(path);
                    continue;
                }
            }

            visited.Add(currentCoordinate);

            // Dead path
            if (!validMoves.ContainsKey(currentCoordinate))
            {
                continue;
            }

            foreach (var nextCoordinate in validMoves[currentCoordinate])
            {
                // Don't go forever, but allow a fair amount of backtracking
                if (path.Count > _heightMap.Count * _heightMap[0].Count * 2)
                {
                    continue;
                }

                // Don't immediately backtrack if we've just visited
                if (path.Count > 1 && path[path.Count - 2] == nextCoordinate)
                {
                    continue;
                }

                // New path
                var newPath = new List<(int i, int j)>(path) { nextCoordinate };
                pathStack.Push(newPath);
            }
        }

        return validPaths;
    }

    public override ValueTask<string> Solve_1()
    {
        List<(int i, int j)> directions = new List<(int i, int j)>
        {
            (-1, 0),    // Up
            (1, 0),     // Down
            (0, -1),    // Left
            (0, 1),     // Right
        };

        var rows = _heightMap.Count;
        var cols = _heightMap[0].Count;

        // Find all 0 coordinates
        List<(int i, int j)> startCoordinates = _heightMap.Select((line, j) =>
            line.Select((height, i) =>
                _heightMap[i][j] == 0 ? (i,j) : (-1,-1)
            ).Where(item => item != (-1,-1)))
            .SelectMany(coord => coord).ToList();

        // Build tree
        var validMoves = _heightMap
            .SelectMany((row, i) => row
                .Select((elevation, j) => (elevation, i, j))
                // Only add to list when it's not the end
                .Where(x => x.elevation != 9)
                // Create dictionary
                .Select(x => (coordinate: (x.i, x.j), elevation: x.elevation))
                .SelectMany(x => directions
                    .Select(direction => (
                        orginalCoordinate: x.coordinate,
                        originalElevation: x.elevation,
                        newCoordinate: (x.coordinate.i + direction.i, x.coordinate.j + direction.j)
                    ))
                    .Where(move => IsInGrid((move.newCoordinate.Item1, move.newCoordinate.Item2)))
                    .Select(move => (
                        move.orginalCoordinate,
                        move.originalElevation,
                        newCoordinate: move.newCoordinate,
                        newElevation: _heightMap[move.newCoordinate.Item1][move.newCoordinate.Item2]
                    ))
                    .Where(move => move.newElevation == move.originalElevation + 1)
                )
            )
            .GroupBy(x => x.orginalCoordinate)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.newCoordinate).ToList()
            );

        var validPaths = new Dictionary<(int i, int j), List<List<(int i, int j)>>>();
        var total = 0;
        foreach (var startCoordinate in startCoordinates) 
        {
            var paths = FindPaths(startCoordinate, validMoves, false);
            validPaths.Add(startCoordinate, paths);
            total += paths.Count;
        }

        return new($"{total}");
    }

    public override ValueTask<string> Solve_2()
    {
        List<(int i, int j)> directions = new List<(int i, int j)>
        {
            (-1, 0),    // Up
            (1, 0),     // Down
            (0, -1),    // Left
            (0, 1),     // Right
        };

        var rows = _heightMap.Count;
        var cols = _heightMap[0].Count;

        // Find all 0 coordinates
        List<(int i, int j)> startCoordinates = _heightMap.Select((line, j) =>
            line.Select((height, i) =>
                _heightMap[i][j] == 0 ? (i, j) : (-1, -1)
            ).Where(item => item != (-1, -1)))
            .SelectMany(coord => coord).ToList();

        // Build tree
        var validMoves = _heightMap
            .SelectMany((row, i) => row
                .Select((elevation, j) => (elevation, i, j))
                // Only add to list when it's not the end
                .Where(x => x.elevation != 9)
                // Create dictionary
                .Select(x => (coordinate: (x.i, x.j), elevation: x.elevation))
                .SelectMany(x => directions
                    .Select(direction => (
                        orginalCoordinate: x.coordinate,
                        originalElevation: x.elevation,
                        newCoordinate: (x.coordinate.i + direction.i, x.coordinate.j + direction.j)
                    ))
                    .Where(move => IsInGrid((move.newCoordinate.Item1, move.newCoordinate.Item2)))
                    .Select(move => (
                        move.orginalCoordinate,
                        move.originalElevation,
                        newCoordinate: move.newCoordinate,
                        newElevation: _heightMap[move.newCoordinate.Item1][move.newCoordinate.Item2]
                    ))
                    .Where(move => move.newElevation == move.originalElevation + 1)
                )
            )
            .GroupBy(x => x.orginalCoordinate)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.newCoordinate).ToList()
            );

        var validPaths = new Dictionary<(int i, int j), List<List<(int i, int j)>>>();
        var total = 0;
        foreach (var startCoordinate in startCoordinates)
        {
            var paths = FindPaths(startCoordinate, validMoves, true);
            validPaths.Add(startCoordinate, paths);
            total += paths.Count;
        }

        return new($"{total}");
    }
}
