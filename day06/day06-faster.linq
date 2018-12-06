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
    
    // Test input => Part1 = 17; Part2 = 16 when limit is 32
    //var limit = 32;
    //var input = new List<string> { (1, 1), (1, 6), (8, 3), (3, 4), (5, 5), (8, 9) };
    
    var limit = 10000;
    var input = File.ReadAllLines(inputname)
        .Select(s => (int.Parse(s.Substring(0, s.IndexOf(','))), int.Parse(s.Substring(s.IndexOf(',') + 1))))
        .ToList<(int x, int y)>();

    int min_x = input.Min(p => p.x),
        min_y = input.Min(p => p.y),
        max_x = input.Max(p => p.x),
        max_y = input.Max(p => p.y);
    //$"X: {min_x}..{max_x}; Y: {min_y}..{max_y}".Dump();

    var locations = new List<((int x, int y) location, (int x, int y) closest, int total_distances, bool infinite)>();
    foreach (var x in Enumerable.Range(min_x, max_x - min_x + 1))
    {
        foreach (var y in Enumerable.Range(min_y, max_y - min_y + 1))
        {
            var total_dist = 0; // For part 2 - total of distances to all coords
            var mindist = -1;
            var closest = (-1, -1);
            foreach (var p in input)
            {
                var dist = Math.Abs(p.x - x) + Math.Abs(p.y - y);
                if (mindist < 0 || dist < mindist) // Is this the best so far
                {
                    mindist = dist;
                    closest = p;
                }
                else if (dist == mindist) // There's more than one coord at this distance so ignore them
                {
                    closest = (-1, -1);
                }                    
                
                total_dist += dist;
            }
            
            // If (x,y) is on the boundary then the closest calculated coord will have an infinite area
            var inf = x == min_x || x == max_x || y == min_y || y == max_y;
            
            locations.Add(((x,y), closest, total_dist, inf));
        }
    }
    
    var part1 = locations
        .GroupBy(l => l.closest)
        .Where(g => g.All(x => !x.infinite) && g.Key.x >= 0)
        .Max(g => g.Count());
    var part2 = locations.Count(l => l.total_distances < limit);
    Console.WriteLine($"Part 1: {part1}");
    Console.WriteLine($"Part 2: {part2}");
}
