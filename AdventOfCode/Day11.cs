using Spectre.Console;
using System.Linq;

namespace AdventOfCode;

public class Day11 : BaseDay
{
    private readonly List<long> _input;

    public Day11()
    {
        _input = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                _input = Array.ConvertAll(line.Split(' '), long.Parse).ToList();
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        int loops = 25;

        // Copy input list
        List<long> stones = _input.Select(x => x).ToList();

        int loopsDone = 0;
        while (loopsDone < loops) 
        {
            stones = stones.Select(stone =>
            {
                List<long> newStones = new();
                int digitCount = (int)Math.Floor(Math.Log10(stone) + 1);
                if (stone == 0)
                {
                    newStones.Add(1);
                }
                else if (digitCount % 2 == 0)
                {
                    // Get left and right portions of the stone
                    // Calculate the divisor to split the number
                    long divisor = (long)Math.Pow(10, digitCount / 2);

                    // Split the number mathematically
                    int leftHalf = (int)(stone / divisor);
                    int rightHalf = (int)(stone % divisor);

                    newStones.Add(leftHalf);
                    newStones.Add(rightHalf);
                }
                else
                {
                    // multiply by 2024
                    newStones.Add(stone * 2024);
                }
                return newStones;
            }).SelectMany(x => x).ToList();

            loopsDone++;
        }
        return new($"{stones.Count}");
    }

    // Precalculate divisors
    private static readonly long[] _divisors =
    {
            (long) Math.Pow(10, 1 / 2),
            (long) Math.Pow(10, 2 / 2),
            (long) Math.Pow(10, 3 / 2),
            (long) Math.Pow(10, 4 / 2),
            (long) Math.Pow(10, 5 / 2),
            (long) Math.Pow(10, 6 / 2),
            (long) Math.Pow(10, 7 / 2),
            (long) Math.Pow(10, 8 / 2),
            (long) Math.Pow(10, 9 / 2),
            (long) Math.Pow(10, 10 / 2),
            (long) Math.Pow(10, 11 / 2),
            (long) Math.Pow(10, 12 / 2),
            (long) Math.Pow(10, 13 / 2),
            (long) Math.Pow(10, 14 / 2),
            (long) Math.Pow(10, 15 / 2),
            (long) Math.Pow(10, 16 / 2),
            (long) Math.Pow(10, 17 / 2),
            (long) Math.Pow(10, 18 / 2),
            (long) Math.Pow(10, 19 / 2),
            (long) Math.Pow(10, 20 / 2),
    };

    public static (long, long) splitInt(long num, long digitCount)
    {
        long divisor = _divisors[digitCount];
        long leftHalf = (int)(num / divisor);
        long rightHalf = (int)(num % divisor);
        return (leftHalf, rightHalf);
    }

    public static long intLength(long num) => (long)Math.Floor(Math.Log10(num) + 1);

    static Dictionary<(long, int), long> cache = new Dictionary<(long, int), long>();
    public static long GetSpan(long stone, int blinks)
    {
        if (cache.ContainsKey((stone, blinks)))
        {
            return cache[(stone, blinks)];
        }

        if (blinks == 0)
        {
            return 1;
        }

        if (stone == 0)
        {
            return GetSpan(1, blinks - 1);
        }
        else if (intLength(stone) % 2 == 0)
        {
            var len = intLength(stone);
            var splitNum = splitInt(stone, len);

            long result = GetSpan(splitNum.Item1, blinks - 1) + GetSpan(splitNum.Item2, blinks - 1);
            cache[(stone, blinks)] = result;
            return result;
        }
        else
        {
            long result = GetSpan(2024 * stone, blinks - 1);
            cache[(stone, blinks)] = result;
            return result;
        }
    }

    public override ValueTask<string> Solve_2()
    {
        int blinks = 75;
        long total = 0;
        foreach (var stone in _input)
        {
            total += GetSpan(stone, blinks);
        }

        // Return the final count as a string
        return new($"{total}");
    }
}
