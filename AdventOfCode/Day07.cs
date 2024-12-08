using Spectre.Console;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode;

public class Day07 : BaseDay
{
    private List<(long answer, List<long> components)> _equations;

    public Day07()
    {
        _equations = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                var answer = long.Parse(line.Split(':')[0]);
                var components = line.Split(':')[1].Split(' ').Skip(1).Select(long.Parse).ToList();
                _equations.Add((answer, components));
            }
        }
    }

    private long EvaluateLeftToRight(List<long> terms, string operators)
    {
        long total = terms[0];
        long result = terms.Select((term, i) => 
        {
            var operatorIndex = i - 1;
            if (i == 0)
            {
                return 0;
            }
            long nextTerm = terms[i];
            if (operators[operatorIndex] == '+')
            {
                total += nextTerm;
            }
            else if (operators[operatorIndex] == '*')
            {
                total *= nextTerm;
            }
            else if (operators[operatorIndex] == '|')
            {
                total = long.Parse(total.ToString() + nextTerm.ToString());
            }
            return total;
        }).Sum(x => x);


        return total;
    }

    public override ValueTask<string> Solve_1()
    {
        long total = 0;
        foreach (var entry in _equations)
        {
            long target = entry.answer;
            List<long> terms = entry.components;
        
            // Generate all combinations of '+' and '*' operators between terms
            var operators = new[] { '+', '*' };

            // Number of possible combinations
            int operatorsCount = terms.Count - 1;
            var operatorCombinations =  Enumerable.Range(0, (int)Math.Pow(operators.Length, operatorsCount)) 
                 .Select(index => Enumerable.Range(0, operatorsCount)
                         .Select(i => operators[(index / (long)Math.Pow(operators.Length, operatorsCount - 1 - i)) % operators.Length])
                         .Aggregate("", (acc, op) => acc + op));

            IEnumerable<long> validResults = operatorCombinations.Select(op => EvaluateLeftToRight(terms, op));

            bool sumsCorrectly = validResults.Where(result => result == target).Any();

            if (sumsCorrectly) 
            {
                total += target;
            }
        }

        return new($"{total}");
    }


    public override ValueTask<string> Solve_2()
    {
        long total = 0;
        foreach (var entry in _equations)
        {
            long target = entry.answer;
            List<long> terms = entry.components;

            // Generate all combinations of '+' and '*' and '|' operators between terms
            var operators = new[] { '+', '*', '|' };

            // Number of possible combinations
            int operatorsCount = terms.Count - 1;
            var operatorCombinations = Enumerable.Range(0, (int)Math.Pow(operators.Length, operatorsCount))
                 .Select(index => Enumerable.Range(0, operatorsCount)
                         .Select(i => operators[(index / (long)Math.Pow(operators.Length, operatorsCount - 1 - i)) % operators.Length])
                         .Aggregate("", (acc, op) => acc + op));

            IEnumerable<long> validResults = operatorCombinations.Select(op => EvaluateLeftToRight(terms, op));

            bool sumsCorrectly = validResults.Where(result => result == target).Any();

            if (sumsCorrectly)
            {
                total += target;
            }
        }

        return new($"{total}");
    }
}
