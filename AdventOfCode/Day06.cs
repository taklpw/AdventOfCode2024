﻿using Spectre.Console;
using System.Linq;

namespace AdventOfCode;

public class Day06 : BaseDay
{
    private readonly string _input;
    private List<string> _guardMap;


    public Day06()
    {
        _guardMap = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                _guardMap.Add(line);
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        // Directions: Up (^), Right (>), Down (v), Left (<)
        (int vert, int horz)[] Directions = { (-1, 0), (0, 1), (1, 0), (0, -1) };

        // Find initial position and direction
        var (startI, startJ, initialDirIndex) = Enumerable.Range(0, _guardMap.Count)
            // Flatten everything out to coordinates and character
            .SelectMany(i => Enumerable.Range(0, _guardMap[i].Length)
                .Select(j => (i, j, ch: _guardMap[i][j]))
            )
            // Wherever we find a direction, that's the start point and direction
            .First(x => "^>v<".Contains(x.ch));

        var initialDir = initialDirIndex switch
        {
            '^' => 0,
            '>' => 1,
            'v' => 2,
            '<' => 3,
            _ => throw new InvalidOperationException("Invalid direction")
        };

        var visited = TraverseMap(startI, startJ, initialDir)
            .ToHashSet();

        return new($"{visited.Count}");

        // Functional generator method to traverse the map
        IEnumerable<(int i, int j)> TraverseMap(int startI, int startJ, int startDirIndex)
        {
            int dirIndex = startDirIndex;
            (int i, int j) currentPos = (startI, startJ);

            yield return currentPos;

            while (true)
            {
                var (nextI, nextJ) = (
                    currentPos.i + Directions[dirIndex].vert,
                    currentPos.j + Directions[dirIndex].horz
                );

                // Check map boundaries and exit if out
                if (nextI < 0 || nextI >= _guardMap.Count ||
                    nextJ < 0 || nextJ >= _guardMap[0].Length)
                {
                    break;
                }

                // Handle obstacle
                if (_guardMap[nextI][nextJ] == '#')
                {
                    dirIndex = (dirIndex + 1) % 4;
                    (nextI, nextJ) = (
                        currentPos.i + Directions[dirIndex].vert,
                        currentPos.j + Directions[dirIndex].horz
                    );

                    // Exit if new position is out of bounds or still an obstacle
                    if (nextI < 0 || nextI >= _guardMap.Count ||
                        nextJ < 0 || nextJ >= _guardMap[0].Length ||
                        _guardMap[nextI][nextJ] == '#')
                    {
                        break;
                    }   
                }

                currentPos = (nextI, nextJ);
                yield return currentPos;
            }
        }
    }

    public override ValueTask<string> Solve_2()
    {
       var loopCount = _guardMap
              .SelectMany((row, i) => row
                  .Select((cell, j) => (I: i, J: j, Cell: cell)))
              .Where(x => x.Cell == '.')
              .Count(x => guardIsInLoop(CreateModifiedMap(x.I, x.J)))
              .ToString();

        return new($"{loopCount}");
    }

    private bool guardIsInLoop(List<char[]> modifiedMap)
    {
        var directions = new[] { (-1, 0), (0, 1), (1, 0), (0, -1) };
        var visitedConfigurations = new HashSet<(int, int, int)>();
        (int i, int j, int directionIndex) = FindInitialPositionAndDirection(modifiedMap);

        while (true)
        {
            if (!visitedConfigurations.Add((i, j, directionIndex))) 
            {
                return true;
            }   

            if (IsOutOfBounds(modifiedMap, i + directions[directionIndex].Item1, j + directions[directionIndex].Item2))
            {
                return false;
            }

            var (nextI, nextJ) = (
                i + directions[directionIndex].Item1,
                j + directions[directionIndex].Item2
            );

            if (modifiedMap[nextI][nextJ] == '#')
            {
                directionIndex = (directionIndex + 1) % 4;
                continue;
            }

            (i, j) = (nextI, nextJ);
        }
    }

    private (int, int, int) FindInitialPositionAndDirection(List<char[]> modifiedMap) 
    {    
        // Find initial position and direction
        var(startI, startJ, initialDirection) = Enumerable.Range(0, _guardMap.Count)
        // Flatten everything out to coordinates and character
        .SelectMany(i => Enumerable.Range(0, _guardMap[i].Length)
            .Select(j => (i, j, ch: _guardMap[i][j]))
        )
        // Wherever we find a direction, that's the start point and direction
        .First(x => "^>v<".Contains(x.ch));

        var initialDirectionIndex = initialDirection switch
        {
            '^' => 0,
            '>' => 1,
            'v' => 2,
            '<' => 3,
            _ => throw new InvalidOperationException("Invalid direction")
        };

        return (startI, startJ, initialDirectionIndex);
    }

    private bool IsOutOfBounds(List<char[]> map, int i, int j) =>
    i < 0 || i >= map.Count || j < 0 || j >= map[0].Length;

    private List<char[]> CreateModifiedMap(int obstacleI, int obstacleJ)
    {
        var modifiedMap = _guardMap.Select(row => row.ToArray()).ToList();
        modifiedMap[obstacleI][obstacleJ] = '#';
        return modifiedMap;
    }
}
