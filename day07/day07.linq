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
    // Day 7

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day07-input.txt");
    var input = File.ReadAllLines(inputname)
    //var input = TestInput()  // Part 1 => "CABDFE"; Part 2 => 15s
        .Select(s => (s[5], s[36]))
        .ToList<(char A, char B)>();

    // Build the graph
    var graph = new Dictionary<char, List<char>>(); // The list represents "key depends on items in list"
    foreach (var step in input)
    {
        if (!graph.ContainsKey(step.A))
            graph[step.A] = new List<char>();
            
        if (graph.ContainsKey(step.B))
            graph[step.B].Add(step.A);
        else
            graph[step.B] = new List<char> { step.A };
    }

    // Part 1
    var complete = new List<char>();
    // First node is one that has no dependencies and is the lowest alphabetically
    var node = graph.Where(kvp => kvp.Value.Count == 0)
        .OrderBy(kvp => kvp.Key)
        .First();
    while (node.Key != default(char))
    {
        complete.Add(node.Key);
        
        // Now find the lowest char that hasn't already been used and has all its dependencies fulfilled
        node = graph
            .Where(kvp => !complete.Contains(kvp.Key))
            .Where(kvp => kvp.Value.All(c => complete.Contains(c)))
            .OrderBy(kvp => kvp.Key)
            .FirstOrDefault();
    }
    
    var part1 = string.Join("", complete);
    Clipboard.SetText(part1);
    Console.WriteLine($"Part 1: {part1}");

    // Part 2
    const int maxWorkers = 5; // For test input = 2
    const int baseWorkTime = 60; // For test input = 0
    var workers = new List<(char step, int startTime, int totalTime)>();
    complete.Clear();
    int count = 0;
    while (true)
    {
        // Find any that have completed. Add them to the completed list and remove them from the work list
        var finished = workers
            .Where(w => count - w.startTime == w.totalTime)
            .OrderBy(w => w.step)
            .Select(w => w.step);
        complete.AddRange(finished);
        workers.RemoveAll(w => finished.Contains(w.step));

        // Find work for idle hands
        while (workers.Count < maxWorkers)
        {
            node = graph
                .Where(kvp => !workers.Any(w => w.step == kvp.Key)) // Exclude any we're already working on
                .Where(kvp => !complete.Contains(kvp.Key))          // Exclude those already done
                .Where(kvp => kvp.Value.All(c => complete.Contains(c))) // Only those whose dependencies are all met
                .OrderBy(kvp => kvp.Key) // Want the lowest, alphabetically
                .FirstOrDefault();
            if (node.Key == default(char))
                break;
            var worktime = baseWorkTime + (node.Key - 'A' + 1);
            workers.Add((node.Key, count, worktime));
        }

        // If nobody's working then we must be all done
        if (workers.Count == 0)
            break;
        // Time marches on
        count++;
    }

    var part2 = string.Join("", complete);
    Console.WriteLine($"Part 2: {part2} took {count} seconds");
}

public string[] TestInput()
{
    return new string[] 
    {
        "Step C must be finished before step A can begin.",
        "Step C must be finished before step F can begin.",
        "Step A must be finished before step B can begin.",
        "Step A must be finished before step D can begin.",
        "Step B must be finished before step E can begin.",
        "Step D must be finished before step E can begin.",
        "Step F must be finished before step E can begin.",
    };
}