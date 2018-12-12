<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationCore.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\WindowsBase.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xaml.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\UIAutomationTypes.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\System.Windows.Input.Manipulations.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\UIAutomationProvider.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Namespace>System.Windows</Namespace>
</Query>

void Main()
{
    // Advent of Code 2018 https://adventofcode.com/2018
    // Day 12

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day12-input.txt");
    var input = File.ReadAllLines(inputname);
    var initialstate = input.First().Substring("initial state: ".Length - 1).Trim();
    var notes = input.Skip(2)
        .Select(s => (s.Substring(0, 5), s.Substring(s.Length - 1) == "#"))
        .ToList<(string Pattern, bool Result)>();
    //var (initialstate, notes) = TestInput();
    
    var gen = 0;
    var maxgen = 20;
    var state = ".........." + initialstate;
    var zeroindex = 10;
    $"0: {state}".Dump();
    var newstate = new StringBuilder();
    while (gen < maxgen)
    {
        gen++;
        state = state + ".....";
        newstate.Clear();
        newstate.Append("..");
        for (var index = 2; index < state.Length - 2; index++)
        {
            var section = state.Substring(index - 2, 5);
            var match = notes.FirstOrDefault(n => n.Pattern == section);
            if (match.Pattern == section)
            {
                newstate.Append(match.Result ? '#' : '.');
            }
            else
            {
                newstate.Append('.');
            }
        }
        state = newstate.ToString().TrimEnd(new char[] {'.'});
        $"{gen}: {state}".Dump();        
    }
    
    var part1 = state.Select((c, i) => c == '#' ? i - zeroindex : 0).Sum(i => i);
    Console.WriteLine($"Part 1: {part1}");
}

public (string, List<(string Pattern, bool Result)>) TestInput()
{
    var init = "#..#.#..##......###...###";
    var src = new List<string>
    {
        "...## => #",
        "..#.. => #",
        ".#... => #",
        ".#.#. => #",
        ".#.## => #",
        ".##.. => #",
        ".#### => #",
        "#.#.# => #",
        "#.### => #",
        "##.#. => #",
        "##.## => #",
        "###.. => #",
        "###.# => #",
        "####. => #",
    };
    var notes = src
        .Select(s => (s.Substring(0, 5), s.Substring(s.Length - 1) == "#"))
        .ToList<(string Pattern, bool Result)>();
    
    return (init, notes);
}