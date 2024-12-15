using Spectre.Console;
using System.Linq;

namespace AdventOfCode;

public class Day12 : BaseDay
{
    private readonly List<string> _plot;
    private Dictionary<char, List<(int i, int j)>> _plotLocations;
    Dictionary<(int i, int j), List<(int i, int j)>> _tree;


    public Day12()
    {
        _plot = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                _plot.Add(line);
            }
        }

        // Create a dictionary of plot types to the first location we see
        _plotLocations = _plot.SelectMany((line, i) =>
                line.Select((ch, j) => (PlotType: ch, Coordinates: (i, j)))
            )
            .GroupBy(x => x.PlotType)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Coordinates).ToList()
            );

        // Build a tree
        _tree = new Dictionary<(int i, int j), List<(int i, int j)>>();
        for (int i = 0; i < _plot.Count; i++)
        {
            for (int j = 0; j < _plot[i].Length; j++)
            {
                (int i, int j) currentCoordinate = (i, j);
                char ch = _plot[i][j];
                foreach (var direction in directions)
                {
                    (int i, int j) newCoord = (currentCoordinate.i + direction.i, currentCoordinate.j + direction.j);
                    if (IsInGrid(newCoord))
                    {
                        // Only add if it's the same character
                        if (_plot[newCoord.i][newCoord.j] == ch)
                        {
                            if (_tree.ContainsKey(currentCoordinate))
                            {
                                _tree[currentCoordinate].Add(newCoord);
                            }
                            else
                            {
                                _tree.Add(currentCoordinate, new List<(int i, int j)> { newCoord });
                            }
                        }
                    }
                }
            }
        }
    }

    List<(int i, int j)> directions = new List<(int i, int j)>
        {
            (-1, 0),    // Up
            (1, 0),     // Down
            (0, -1),    // Left
            (0, 1),     // Right
        };

    private bool IsInGrid((int i, int j) loc) => 
        loc.i >= 0 && loc.j >= 0 && loc.i < _plot.Count && loc.j < _plot[loc.i].Length;

    private HashSet<(int i, int j)> _globalVisited = new();

    private HashSet<(int i, int j)> FloodFill((int i, int j) currentCoord)
    {
        if (_globalVisited.Contains(currentCoord))
        {
            return null;
        }

        Queue<(int i, int j)> coordQueue = new Queue<(int i, int j)>();
        HashSet<(int i, int j)> visited = new();
        coordQueue.Enqueue(currentCoord);
        visited.Add(currentCoord);
        while (coordQueue.Count > 0) 
        {
            currentCoord = coordQueue.Dequeue();

            foreach (var dir in directions)
            {
                (int i, int j) next = (currentCoord.i + dir.i, currentCoord.j + dir.j);

                if (visited.Contains(next) || _globalVisited.Contains(next))
                {

                    continue;
                }

                if (!visited.Contains(next) &&
                    _tree.ContainsKey(currentCoord) &&
                    _tree[currentCoord].Contains(next))
                {
                    visited.Add(next);
                    coordQueue.Enqueue(next);
                }
            }
        }

        var visitedList = visited.ToList();
        visitedList.Sort((x, y) => y.i.CompareTo(x.i));

        _globalVisited.UnionWith(visited);

        return visited;
    }

    private int getPerimeterSize(HashSet<(int i, int j)> region)
    {
        int perimeter = 0;

        foreach (var (i, j) in region)
        {
            foreach(var dir in directions)
            {
                (int i, int j) neighbour = (i + dir.i, j + dir.j);
                if (!region.Contains(neighbour))
                {
                    perimeter++;
                }
            }
        }

        return perimeter;
    }


    private int getSides(HashSet<(int i, int j)> region)
    {
        int[] dR = { 0, 1, 0, -1 };
        int[] dC = { 1, 0, -1, 0 };

        int totalSides = region
            .SelectMany(cell =>
                Enumerable.Range(0, 4)
                    .Select(i =>
                    (
                        Cell: cell,
                        Direction: i,
                        NewCoord: (cell.i + dR[i], cell.j + dC[i])
                    ))
                    .Where(x =>
                        !IsInGrid(x.NewCoord) ||
                        !region.Contains(x.NewCoord))
                    .Select(x =>
                    {
                        // Check the cell 90 degrees counter-clockwise
                        int previousDirection = (x.Direction - 1 + 4) % 4;
                        (int i, int j) new90CC = (x.Cell.i + dR[previousDirection], x.Cell.j + dC[previousDirection]);
                        (int i, int j) newCorner = (x.NewCoord.Item1 + dR[previousDirection], x.NewCoord.Item2 + dC[previousDirection]);

                        // Is this the beginning of an edge?
                        bool isBeginEdge = !IsInGrid(new90CC) || !region.Contains(new90CC);
                        // Is it a concave edge?
                        bool isConcaveBeginEdge = IsInGrid(newCorner) && region.Contains(newCorner);

                        return isBeginEdge || isConcaveBeginEdge;
                    })
            )
            .Count(x => x);

        return totalSides;
    }

    public override ValueTask<string> Solve_1()
    {
        //HashSet<HashSet<(int i, int j)>> fills = new();
        //for (int i = 0; i < _plot.Count; i++) 
        //{
        //    for (int j = 0; j < _plot[i].Length; j++)
        //    {
        //        if(_globalVisited.Contains((i, j)))
        //        {
        //            continue;
        //        }

        //        HashSet<(int i, int j)> region = FloodFill((i, j));
        //        fills.Add(region);
        //    }
        //}

        //// Calculate cost
        //var total = 0;
        //foreach (var fill in fills) 
        //{
        //    int area = fill.Count;
        //    int perimeter = getPerimeterSize(fill);
        //    total += area * perimeter;
        //}

        return new($"{1}");
    }

    public override ValueTask<string> Solve_2()
    {
        HashSet<HashSet<(int i, int j)>> fills = new();
        for (int i = 0; i < _plot.Count; i++)
        {
            for (int j = 0; j < _plot[i].Length; j++)
            {
                if (_globalVisited.Contains((i, j)))
                {
                    continue;
                }

                HashSet<(int i, int j)> region = FloodFill((i, j));
                fills.Add(region);
            }
        }

        // Calculate cost
        var total = 0;
        foreach (var fill in fills)
        {
            int area = fill.Count;
            var sides = getSides(fill);
            total += area * sides;
        }

        return new($"{total}");
    }
}
