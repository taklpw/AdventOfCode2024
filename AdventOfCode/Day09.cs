namespace AdventOfCode;

public class Day09 : BaseDay
{
    private readonly string _input;


    public Day09()
    {
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                _input = line;
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        List<int> fileBlock = new();
        for (int i = 0; i < _input.Length; i++) 
        {
            if (i % 2 == 0)
            {
                // File
                int fileLength = int.Parse(_input[i].ToString());
                int id = i / 2;
                fileBlock.AddRange(Enumerable.Repeat(id, fileLength));
            }
            else
            {
                // Free space
                int freeSpaceLength = int.Parse(_input[i].ToString());
                fileBlock.AddRange(Enumerable.Repeat(-1, freeSpaceLength));
            }
        }

        // Compact from right most
        List<int> compactFileBlock = new List<int>(Enumerable.Repeat(-1, fileBlock.Count));
        for (int i=0; i < fileBlock.Count; i++)
        {
            // If there's free space
            if (fileBlock[i] == -1)
            {
                // Search from the back of the string to find the first available ID
                for (int j = fileBlock.Count - 1; j >= 0; j--)
                {
                    // If there's a file
                    if (fileBlock[j] != -1)
                    {
                        compactFileBlock[i] = fileBlock[j];
                        fileBlock[j] = -1;
                        break;
                    }
                }
            }
            else
            {
                // Otherwise keep it where it is
                compactFileBlock[i] = fileBlock[i];
                fileBlock[i] = -1;
            }
        }


        long checksum = CalculateChecksum(compactFileBlock);
        return new($"{checksum}");
    }

    private long CalculateChecksum(List<int> files) => files
        .Where(fileId => fileId != -1)
        .Select((fileId, i) => fileId * i)
        .Sum();
    

    public override ValueTask<string> Solve_2()
    {
        Dictionary<int, (int startIndex, int length)> idToIndexStartAndLength = new Dictionary<int, (int startIndex, int length)>();
        List<int> fileBlock = new();
        for (int i = 0; i < _input.Length; i++)
        {
            if (i % 2 == 0)
            {
                // File
                int fileLength = int.Parse(_input[i].ToString());
                int id = i / 2;

                idToIndexStartAndLength.Add(id, (fileBlock.Count, fileLength));
                fileBlock.AddRange(Enumerable.Repeat(id, fileLength));
            }
            else
            {
                // Free space
                int freeSpaceLength = int.Parse(_input[i].ToString());
                fileBlock.AddRange(Enumerable.Repeat(-1, freeSpaceLength));
            }
        }

        // Compact in left to right in whole chunks, in descending file id
        List<int> compactFileBlock = new List<int>(Enumerable.Repeat(-1, fileBlock.Count));
        int fileIdToTryMove = idToIndexStartAndLength.Keys.Max();

        int currentFileBlockIndex = 0;
        while(currentFileBlockIndex < fileBlock.Count)
        {
            if (fileBlock[currentFileBlockIndex] == -1)
            {
                // Free space is here, how long is it?
                int freeSpaceLength = 0;
                for (int j = currentFileBlockIndex; j < fileBlock.Count; j++)
                {
                    if (fileBlock[j] != -1)
                    {
                        freeSpaceLength = j - currentFileBlockIndex;
                        break;
                    }
                }

                // Find the most appropriate file to put in there in descending order
                for (int j = fileIdToTryMove; j >= 0; j--)
                {
                    int idLength = idToIndexStartAndLength[j].length;
                    if (idLength <= freeSpaceLength)
                    {
                        // Move it in
                        compactFileBlock.InsertRange(currentFileBlockIndex, Enumerable.Repeat(j, idLength));
                        compactFileBlock.RemoveRange(currentFileBlockIndex + idLength, idLength);
                        currentFileBlockIndex += idLength;
                        break;
                    }
                    else
                    {
                        fileIdToTryMove--;
                    }

                    if (j == 0)
                    {
                        // We haven't found something to put in this slot
                        currentFileBlockIndex++;
                    }
                }
            }
            else
            {
                compactFileBlock[currentFileBlockIndex] = fileBlock[currentFileBlockIndex];
                currentFileBlockIndex++;
            }
        }

        return new($"{CalculateChecksum(compactFileBlock)}");
    }
}
