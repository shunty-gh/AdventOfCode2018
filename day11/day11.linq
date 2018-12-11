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
    //var input = 18; // expect 29
    //var input = 42; // expect 30
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
}

public int PowerLevel(int x, int y, int serial)
{
    var rackid = x + 10;
    return (((((rackid * y) + serial) * rackid) % 1000) / 100) - 5;
}