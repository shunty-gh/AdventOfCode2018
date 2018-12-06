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
    // Day 6

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day06-input.txt");
    var input = File.ReadAllLines(inputname)
    //var input = new List<string> { "1, 1", "1, 6", "8, 3", "3, 4", "5, 5", "8, 9" }  // Test input => Part1 = 17
        .Select(s => (int.Parse(s.Substring(0, s.IndexOf(','))), int.Parse(s.Substring(s.IndexOf(',') + 1))))
        .ToList<(int x, int y)>();

    int min_x = input.Min(p => p.x),
        min_y = input.Min(p => p.y),
        max_x = input.Max(p => p.x),
        max_y = input.Max(p => p.y);
    $"X: {min_x}..{max_x}; Y: {min_y}..{max_y}".Dump();

    var closest = new Dictionary<(int x, int y), (int x, int y)>();
    foreach (var x in Enumerable.Range(min_x, max_x - min_x + 1))
    {
        foreach (var y in Enumerable.Range(min_y, max_y - min_y + 1))
        {
            var close = input.Select(p => (Math.Abs(p.x - x) + Math.Abs(p.y - y), p))
                .GroupBy(p => p.Item1)
                .OrderBy(g => g.Key)
                .FirstOrDefault();
            if (close != null && close.Count() == 1)
            {
                closest.Add((x, y), close.First().p);
            }
        }
    }

    // Exclude any coord where any location it is closest to is on the boundary as this means it will have an infinte area
    var part1 = closest.GroupBy(c => c.Value)
        .Where(g => g.All(p => p.Key.x > min_x && p.Key.y > min_y && p.Key.x < max_x && p.Key.y < max_y))
        .OrderByDescending(g => g.Count())
        .Select(g => new { g.Key, Count = g.Count() })
        .First();
    Console.WriteLine($"Part 1: Co-ordinate {part1.Key} with {part1.Count} locations");

    // Part 2
    var dists = new Dictionary<(int x, int y), List<((int x, int y) point, int dist)>>();
    Enumerable.Range(min_x, max_x - min_x + 1).ToList().ForEach(x =>
        Enumerable.Range(min_y, max_y - min_y + 1).ToList().ForEach(y =>
            dists[(x, y)] = input.Select(p => (p, Math.Abs(p.x - x) + Math.Abs(p.y - y))).ToList()));

    var part2 = dists.Select(kvp =>
        kvp.Value
            .Sum(v => v.dist))
            .Count(r => r < 10000);
    Console.WriteLine($"Part 2: {part2}");
}
