using Spectre.Console;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace AdventOfCode;

public class Day14 : BaseDay
{
    private readonly List<((int x, int y) pos, (int x, int y) vel)> _initialStates;


    public Day14()
    {
        _initialStates = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                var position = line.Split(' ')[0].Split('=')[1].Split(',').Select(int.Parse).ToList();
                var velocity = line.Split(' ')[1].Split('=')[1].Split(',').Select(int.Parse).ToList();
                (int x, int y) pos = (position[0], position[1]);
                (int x, int y) vel = (velocity[0], velocity[1]);
                _initialStates.Add((pos, vel));
            }
        }
    }

    private bool IsOutOfBounds(int x, int y, int width, int height) =>
        x < 0 || x > width || y < 0 || y > height;

    private int SaneModulo(int a, int b)
    {
        return ((a % b) + b) % b;
    }

    private int GetQuadrant((int x, int y) pos, int width, int height)
    {
        // Returns:
        // |-----------|
        // |     |     |
        // |  1  |  2  |
        // |     |     |
        // |-----|-----|
        // |     |     |
        // |  3  |  4  |
        // |_____|_____|

        int halfWidth = width / 2;
        int halfHeight = height / 2;
        if (pos.x == halfWidth || pos.y == halfHeight)
        {
            return 0;
        }
        else if (pos.x < halfWidth && pos.y < halfHeight) 
        {
            return 1;
        }
        else if (pos.x > halfWidth && pos.y < halfHeight)
        {
            return 2;
        }
        else if (pos.x < halfWidth && pos.y > halfHeight)
        {
            return 3;
        }
        else if (pos.x > halfWidth && pos.y > halfHeight)
        {
            return 4;
        }
        else
        {
            // Shouldn't happen, but just to make the compiler happy
            return -1;
        }
    }

    public override ValueTask<string> Solve_1()
    {
        const int width = 101;
        const int height = 103;
        const int maxTime = 100;
        Dictionary<int, int> quadrantValues = new Dictionary<int, int>() 
        {
            { -1, 0 },
            { 0, 0 },
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
        };

        foreach (var state in _initialStates)
        {
            (int x, int y) newPoint = (SaneModulo(state.pos.x + maxTime * state.vel.x, width), SaneModulo(state.pos.y + maxTime * state.vel.y, height));
            quadrantValues[GetQuadrant(newPoint, width, height)]++;
        }

        return new($"{quadrantValues[1] * quadrantValues[2] * quadrantValues[3] * quadrantValues[4]}");
    }

    private string Stringify2DArray(int[,] input, int width, int height)
    {
        string outString = "";
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                outString += $"{input[x, y]} ";
            }
            outString += "\n";
        }
        return outString;
    }

    public override ValueTask<string> Solve_2()
    {
        const int width = 101;
        const int height = 103;
        int minSafetyFactor = -1;

        int i = 1;
        while(true)
        {
            Dictionary<int, int> quadrantValues = new Dictionary<int, int>()
            {
                { -1, 0 },
                { 0, 0 },
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 0 },
            };
            int[,] outGrid = new int[width, height];

            foreach (var state in _initialStates)
            {
                (int x, int y) newPoint = (SaneModulo(state.pos.x + i * state.vel.x, width), SaneModulo(state.pos.y + i * state.vel.y, height));
                quadrantValues[GetQuadrant(newPoint, width, height)]++;
                outGrid[ newPoint.x, newPoint.y]++;
            }

            int safetyFactor = quadrantValues[1] * quadrantValues[2] * quadrantValues[3] * quadrantValues[4];

            if (minSafetyFactor == -1 || safetyFactor < minSafetyFactor)
            {
                minSafetyFactor = safetyFactor;
                Console.WriteLine($"New MinSafetyFactor! At {i} Seconds, Safety Factor: {safetyFactor}");
                string grid = Stringify2DArray( outGrid, width, height );
                Console.WriteLine( grid );
            }
            i++;
        }
    }
}
