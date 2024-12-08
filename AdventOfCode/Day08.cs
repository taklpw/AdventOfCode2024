using Spectre.Console;
using System.Collections.Immutable;

namespace AdventOfCode;

public class Day08 : BaseDay
{
    private List<string> _antennaMap;


    public Day08()
    {
        _antennaMap = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                _antennaMap.Add(line);
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        // Find each unique frequency
        IEnumerable<char> frequencies = _antennaMap.SelectMany(line => line).Where(ch => ch != '.').Distinct();

        List<(int i, int j)> antinodes = new();
        foreach (char frequency in frequencies) 
        {
            // Find the coordinates of each antenna
            List<(int i, int j)> frequencyCoords = _antennaMap.Select((line, j) =>
                line.Select((ch, i) =>
                    ch.Equals(frequency) ? (i, j) : (-1, -1)).Where(i => i.Item1 != -1 || i.Item2 != -1)
            ).SelectMany(coord => coord).ToList();

            // Calculate the antinodes
            for(int i = 0; i < frequencyCoords.Count(); i++)
            {
                for (int j = 0; j < frequencyCoords.Count(); j++)
                {
                    // Don't compare an antenna to itself
                    if (i == j)
                    {
                        continue;
                    }

                    var p1 = frequencyCoords[i];
                    var p2 = frequencyCoords[j];

                    // Calculate slope m and intercept b
                    (int rise, int run) distance = (p1.j - p2.j, p1.i - p2.i);

                    // Calculate new coordinates for the antinodes (make sure they're away from the other antenna
                    (int i, int j) antinode1 = (p1.i - distance.run, p1.j - distance.rise) != p2 ?
                        (p1.i - distance.run, p1.j - distance.rise) :
                        (p1.i + distance.run, p1.j + distance.rise);
                    (int i, int j) antinode2 = (p2.i - distance.run, p2.j - distance.rise) != p1 ?
                        (p2.i - distance.run, p2.j - distance.rise) :
                        (p2.i + distance.run, p2.j + distance.rise);

                    // Only add if they're in the map
                    if (antinode1.i >= 0 && antinode1.i < _antennaMap[0].Count() &&
                        antinode1.j >= 0 && antinode1.j < _antennaMap.Count)
                    {
                        antinodes.Add(antinode1);
                    }
                    if (antinode2.i >= 0 && antinode2.i < _antennaMap[0].Count() &&
                        antinode2.j >= 0 && antinode2.j < _antennaMap.Count)
                    {
                        antinodes.Add(antinode2);
                    }
                }
            }
        }
        var uniqueAntinodes = antinodes.ToHashSet();

        return new($"{uniqueAntinodes.Count}");
    }

    public override ValueTask<string> Solve_2()
    {
        // Find each unique frequency
        IEnumerable<char> frequencies = _antennaMap.SelectMany(line => line).Where(ch => ch != '.').Distinct();
        IEnumerable<(int x, int y)> antinodes = frequencies.SelectMany(frequency =>
        {
            // Find the coordinates of each antenna
            List<(int i, int j)> frequencyCoords = _antennaMap.Select((line, j) =>
                line.Select((ch, i) =>
                    ch.Equals(frequency) ? (i, j) : (-1, -1)).Where(i => i.Item1 != -1 || i.Item2 != -1)
            ).SelectMany(coord => coord).ToList();

            return frequencyCoords.SelectMany((p1, i) =>
                frequencyCoords.Where((_, j) => i != j)
                .SelectMany(p2 =>
                {
                    // Calculate slope
                    (int rise, int run) distance = (p1.j - p2.j, p1.i - p2.i);
                    (int rise, int run) absDistance = (Math.Abs(distance.rise), Math.Abs(distance.run));

                    // Find the greatest common divisor of the rise and run
                    static int calculateGCD(int a, int b)
                    {
                        while (b != 0)
                        {
                            int temp = b;
                            b = a % b;
                            a = temp;
                        }
                        return a;
                    }
                    int gcd = calculateGCD(absDistance.rise, absDistance.run);
                    // Smallest steps that can be taken
                    (int reducedRise, int reducedRun) = (distance.rise / gcd, distance.run / gcd);

                    // Find line bounds
                    int minI = Math.Min(p1.i, p2.i);
                    int maxI = Math.Max(p1.i, p2.i);
                    int minJ = Math.Min(p1.j, p2.j);
                    int maxJ = Math.Max(p1.j, p2.j);

                    return Enumerable.Range(0, _antennaMap[0].Count())
                        .SelectMany(x => Enumerable.Range(0, _antennaMap.Count)
                        .Where(y =>
                            // Is this point on the line?
                            (x - p1.i) * reducedRise == (y - p1.j) * reducedRun &&
                            // Point is outside line segment
                            !(x > minI && x < maxI && y > minJ && y < maxJ) &&
                            // Point is wihtin the map
                            x >= 0 && x < _antennaMap[0].Count() &&
                            y >= 0 && y < _antennaMap.Count
                        ).Select(y => (x, y))
                    );
                })
                );
        });
        var uniqueAntinodes = antinodes.ToHashSet();
        return new($"{uniqueAntinodes.Count}");
    }
}
