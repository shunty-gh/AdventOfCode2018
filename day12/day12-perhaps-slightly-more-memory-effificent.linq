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
        .ToDictionary(t => KeyHash(t.Item1), t => t.Item2);
    //var (initialstate, notes) = TestInput(); // Part 1 => 325
    
    // For part 2:
    /*
    Run the sim for 500 (= 5 x 10^^2) gens with the variable showState == true and
    we can see that the population settles down at about 100 generations.
    
    Then turn off showState and run for 5000 gens and then 50000. See the pattern.
    We can see that the result follows a pattern: 31 followed by (exp - 2) x 0 followed by 655
    50 billion is 10 zeros so the result is 31 then (10 - 2) x 0 then 655
    ie Part 2 = 3100000000655 for this input
    I'm sure there's some form of population theory or somesuch that explains
    this but I don't know what.
    */
    var showState = true;
    var gen = 0;
    var maxgen = 500;
    var state1 = initialstate.Select(c => c == '#').ToArray();
    Array.Resize(ref state1, state1.Length * 2);
    var states = new bool[][] { state1, new bool[state1.Length] };
    var state = states[0];
    var newstate = states[1];
    var flip = false;
    var zeroindex = 0;
    var newindex = 0;
    while (gen < maxgen)
    {
        gen++;
        var minindex = state.Select((b, i) => (b, i)).Where(x => x.b).Min(x => x.i);
        var maxindex = state.Select((b, i) => (b, i)).Where(x => x.b).Max(x => x.i);
        zeroindex = zeroindex - minindex + 2;
        newindex = 0;
        for (var index = minindex - 2; index < maxindex + 2; index++)
        {
            var key = KeyHash(state, index - 2);
            newstate[newindex] = notes.ContainsKey(key) && notes[key];
            newindex++;
        }
        // Clear remaining items in newstate
        for (var nsi = newindex; nsi < newstate.Length; nsi++)
            newstate[nsi] = false;

        // Resize if necessary
        if ((maxindex - minindex) > (state.Length - 5))
        {
            "Reallocating".Dump();
            Array.Resize(ref states[0], states[0].Length * 2);
            Array.Resize(ref states[1], states[1].Length * 2);
        }
        
        // Swap states
        state = flip ? states[0] : states[1];
        newstate = flip ? states[1] : states[0];
        flip = !flip;

        if (showState)
        {
            $"{gen:0000}: {new string('.', Math.Abs(zeroindex))}{string.Join("", state.Select(b => b ? '#' : '.'))})".Dump();
        }
    }
    
    var part1 = state.Select((b, i) => b == true ? i - zeroindex : 0).Sum(i => i);
    Console.WriteLine($"Result: {part1}");
}

public int KeyHash(string src)
{
    return KeyHash(src.Select(c => c == '#').ToArray());
}

public int KeyHash(bool[] src, int startIndex = 0)
{
    var len = src.Length;
    return (startIndex >= 0 && startIndex < len && src[startIndex] ? 1 : 0)
        + (startIndex >= -1 && startIndex < len - 1 && src[startIndex + 1] ? 2 : 0)
        + (startIndex >= -2 && startIndex < len - 2 && src[startIndex + 2] ? 4 : 0)
        + (startIndex >= -3 && startIndex < len - 3 && src[startIndex + 3] ? 8 : 0)
        + (startIndex >= -4 && startIndex < len - 4 && src[startIndex + 4] ? 16 : 0);
}

public (string, Dictionary<int, bool>) TestInput()
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
        .ToDictionary(t => KeyHash(t.Item1), t => t.Item2);
    
    return (init, notes);
}