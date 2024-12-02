namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string _input;
    private List<int> _list1;
    private List<int> _list2;


    public Day01()
    {
        _list1 = new();
        _list2 = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                var nums = line.Split("   ");
                _list1.Add(Convert.ToInt32(nums[0]));
                _list2.Add(Convert.ToInt32(nums[1]));
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        _list1.Sort();
        _list2.Sort();
        int length = _list1.Count;
        int total = 0;
        for (int i = 0; i < length; i++) 
        {
            int distance = Math.Abs(_list1[i] - _list2[i]);
            total += distance;
        }
        return new($"{total}");
    }

    public override ValueTask<string> Solve_2()
    {
        _list1.Sort();
        _list2.Sort();

        int similarity = 0;
        for (int i = 0; i < _list1.Count; i++) 
        {
            int valueToFind = _list1[i];
            similarity += _list2.Where(x => x == valueToFind).Count() * valueToFind;
        }
        return new($"{similarity}");
    }
}
