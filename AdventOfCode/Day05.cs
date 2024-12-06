﻿using Spectre.Console;

namespace AdventOfCode;

public class Day05 : BaseDay
{
    private readonly string _input;
    private Dictionary<int, List<int>> _orderingRules; // prior -> latter
    private List<List<int>> _pagesToProduce;

    public Day05()
    {
        _orderingRules = new();
        _pagesToProduce = new();
        using (StreamReader r = new StreamReader(InputFilePath))
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                if (line.Contains('|'))
                {
                    int prior = int.Parse(line.Split('|')[0]);
                    int latter = int.Parse(line.Split('|')[1]);
                    if (_orderingRules.ContainsKey(prior))
                    {
                        _orderingRules[prior].Add(latter);
                    }
                    else
                    {
                        _orderingRules.Add(prior, new List<int>{ latter });
                    }
                }
                else if (line.Contains(','))
                {
                    _pagesToProduce.Add(line.Split(',').Select(s => Convert.ToInt32(s)).ToList());
                }
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        var invalidPageIndices = Enumerable.Range(0, _pagesToProduce.Count)
        .Where(i => _pagesToProduce[i]
            .Select((currentPage, j) =>
                _orderingRules.ContainsKey(currentPage) &&
                _pagesToProduce[i].Take(j)
                    .Intersect(_orderingRules[currentPage])
                    .Any())
            .Any(x => x));

        var validPageIndices = Enumerable.Range(0, _pagesToProduce.Count)
            .Except(invalidPageIndices);

        var totalMiddleValues = validPageIndices
            .Sum(validPageIndex => _pagesToProduce[validPageIndex][_pagesToProduce[validPageIndex].Count / 2]);

        return new($"{totalMiddleValues}");
    }

    public override ValueTask<string> Solve_2()
    {

        return new($"{1}");
    }
}
