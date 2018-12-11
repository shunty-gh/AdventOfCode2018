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
    // Day 11

    var input = 9221;
    //var input = 18; // Part 1 expect 29, Part 2 => 90,269,16 
    //var input = 42; // Part 1 expect 30, Part 2 => 232,251,12
    System.Diagnostics.Debug.Assert(-5 == PowerLevel(122, 79, 57));
    System.Diagnostics.Debug.Assert(0 == PowerLevel(217, 196, 39));
    System.Diagnostics.Debug.Assert(4 == PowerLevel(101, 153, 71));
    
    ((int x, int y) topleft, int level) largest = ((0, 0), 0);
    var levels = new Dictionary<(int x, int y), int>();
    foreach (var y in Enumerable.Range(1, 300))
    {
        foreach (var x in Enumerable.Range(1, 300))
        {
            levels[(x, y)] = PowerLevel(x, y, input);
        }
        
        if (y >= 3)
        {
            foreach (var x1 in Enumerable.Range(3, 298))
            {
                (int X, int Y) topleft = (x1 - 2, y - 2);
                var level = 0;
                for (var yoffset = 0; yoffset < 3; yoffset++)
                {
                    for (var xoffset = 0; xoffset < 3; xoffset++)
                    {
                        try
                        {
                            level += levels[(topleft.X + xoffset, topleft.Y + yoffset)];
                        }
                        catch (Exception ex)
                        {
                            $"Error at x = {x1}, y = {y}\nError: {ex.Message}".Dump();
                        }
                    }
                }
                if (level > largest.level)
                {
                    largest = (topleft, level);
                    //largest.Dump();
                }
            }
            //largest.Dump($"Largest after Y = {y}");
        }
    }

    Console.WriteLine($"Part 1: Max power of {largest.level} at {largest.topleft}");

    // Part 2
    // This is slooow brute force. There's bound to be a form of short circuit that I haven't realised yet
    // but this works in under 2 minutes so it'll do for now.
    (int x, int y, int size, int level) part2 = (largest.topleft.x, largest.topleft.y, 3, largest.level);
    foreach (var y in Enumerable.Range(1, 300))
    {
        foreach (var x in Enumerable.Range(1, 300))
        {
            var maxside = 300 - Math.Max(x, y) + 1;
            // Ignore squares of size 1, 2, 3 as we already have the max of the 3s and the 1s and 2s will be smaller
            if (maxside < 4)
                continue;
                
            // Work out the size == 4
            var level = levels[(x, y)] + levels[(x + 1, y)] + levels[(x + 2, y)] + levels[(x + 3, y)]
                      + levels[(x, y + 1)] + levels[(x + 1, y + 1)] + levels[(x + 2, y + 1)] + levels[(x + 3, y + 1)]
                      + levels[(x, y + 2)] + levels[(x + 1, y + 2)] + levels[(x + 2, y + 2)] + levels[(x + 3, y + 2)]
                      + levels[(x, y + 3)] + levels[(x + 1, y + 3)] + levels[(x + 2, y + 3)] + levels[(x + 3, y + 3)];
            if (level > part2.level)
            {
                part2 = (x, y, 4, level);
            }
            // Add the extra left and bottom edges for each increment rather than recalculating the whole square
            for (var side = 5; side <= maxside; side++)
            {
                for (var dy = 0; dy < side; dy++)
                {
                    level += levels[(x + side - 1, y + dy)];
                }
                for (var dx = 0; dx < side - 1; dx++)
                {
                    level += levels[(x + dx, y + side - 1)];
                }
                if (level > part2.level)
                {
                    part2 = (x, y, side, level);
                }
            }
        }
    }
    
    Console.WriteLine($"Part 2: Max power of {part2.level} at ({part2.x},{part2.y}) with size of {part2.size}x{part2.size}");
}

public int PowerLevel(int x, int y, int serial)
{
    var rackid = x + 10;
    return (((((rackid * y) + serial) * rackid) % 1000) / 100) - 5;
}