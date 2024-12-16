using Spectre.Console;
using System.Linq;

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

    public override ValueTask<string> Solve_1()
    {
        const int width = 11;//101;
        const int height = 7;//103;
        const int maxTime = 100;

        int widthMax = width - 1;
        int heightMax = height - 1;

        foreach (var state in _initialStates)
        {
            // Calculate where it will be in 100 steps (without boundaries)
            (int x, int y) finalPositionUnbound = (state.pos.x + (state.vel.x * maxTime), state.pos.y + (state.vel.y * maxTime));
            int boundX = state.vel.x >= 0 ? finalPositionUnbound.x % width : finalPositionUnbound.x % widthMax + widthMax;
            int boundY = state.vel.y >= 0 ? finalPositionUnbound.y % height : finalPositionUnbound.y % heightMax + heightMax;

            Console.WriteLine($"x:{boundX}, y:{boundY}");
        }

        return new($"{1}");
    }

    public override ValueTask<string> Solve_2()
    {

        return new($"{1}");
    }
}
