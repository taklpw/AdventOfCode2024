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

    private string StringifyListofListofInt(List<List<int>> input)
    {
        string outString = "";
        foreach (var line in input)
        {
            foreach(var ch in line)
            {
                outString += $"{ch}";
            }
            outString += "\n";
        }
        return outString;
    }

    private long GetCompressedSize(string text)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(text);
        MemoryStream ms = new MemoryStream();
        using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
        {
            zip.Write(buffer, 0, buffer.Length);
        }
        return ms.Length;
        //ms.Position = 0;
        //MemoryStream outStream = new MemoryStream();

        //byte[] compressed = new byte[ms.Length];
        //ms.Read(compressed, 0, compressed.Length);

        //byte[] gzBuffer = new byte[compressed.Length + 4];
        //System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
        //System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
        //return Convert.ToBase64String(gzBuffer);
    }

    public override ValueTask<string> Solve_2()
    {
        const int width = 101;
        const int height = 103;
        const int maxTime = 100000;
        long minSize = -1;

        //for (int i = 0; i < maxTime; i++)
        int i = 1;
        while(true)
        {
            bool skipThisOne = false;
            List<List<int>> outGrid = Enumerable.Repeat(Enumerable.Repeat(0, width).ToList(), height).ToList();
            foreach (var state in _initialStates)
            {
                (int x, int y) newPoint = (SaneModulo(state.pos.x + state.vel.x * i, width), SaneModulo(state.pos.y + state.vel.y * i, height));
                outGrid[newPoint.y][newPoint.x]++;
                //if (outGrid[newPoint.y][newPoint.x] > 1)
                //{
                //    skipThisOne = true;
                //    break;
                //}
            }
            string grid = StringifyListofListofInt(outGrid);
            long size = GetCompressedSize(grid);

            if (minSize == -1 || size < minSize)
            {
                minSize = size;
                Console.WriteLine($"New Minsize! At {i} Seconds, Size: {minSize}");
            }

            //if (!skipThisOne)
            //{
            //    string stateString = StringifyListofListofInt(outGrid);
            //    System.IO.Directory.CreateDirectory("./day14_files");
            //    File.WriteAllText($"./day14_files/{i}.txt", stateString);
            //    return new($"{i}");
            //}
            i++;
        }
        return new($"{1}");
    }
}
