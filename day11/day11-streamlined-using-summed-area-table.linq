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

    var serial = 9221;
    //var serial = 18; // Part 1 => 33,45; Part 2 => 90,269,16 
    //var serial = 42; // Part 1 => 21,61; Part 2 => 232,251,12
    
    (int x, int y, int level) part1 = (0, 0, 0);
    (int x, int y, int size, int level) part2 = (0, 0, 0, 0);   
    // Build a "summed area table". 
    // Don't actually need a table of power levels - a summed area table will suffice.
    // https://en.wikipedia.org/wiki/Summed-area_table
    var sat = new Dictionary<(int x, int y), int>();
    foreach (var y in Enumerable.Range(1, 300))
    {
        foreach (var x in Enumerable.Range(1, 300))
        {
            sat[(x, y)] = PowerLevel(x, y, serial)
                + (y > 1 ? sat[(x, y - 1)] : 0)
                + (x > 1 ? sat[(x - 1, y)] : 0)
                - ((x > 1 && y > 1) ? sat[(x - 1, y - 1)] : 0);
        }
    }
    
    for (var side = 1; side <= 300; side++)
    {
        foreach (var y in Enumerable.Range(1, 300 - side + 1))
        {
            foreach (var x in Enumerable.Range(1, 300 - side + 1))
            {
                var a = x > 1 && y > 1 ? sat[(x - 1, y - 1)] : 0; // top left bound
                var b = y > 1 ? sat[(x + side - 1, y - 1)] : 0;   // top right
                var c = x > 1 ? sat[(x - 1, y + side - 1)] : 0;   // bottom left
                var d = sat[(x + side - 1, y + side - 1)];        // bottom right

                var level = a + d - b - c;
                // Part 1 check
                if (side == 3 && level > part1.level)
                {
                    part1 = (x, y, level);
                }
                // Part 2 check
                if (level > part2.level)
                {
                    part2 = (x, y, side, level);
                }
            }
        }
    }

    Console.WriteLine($"Part 1: ({part1.x},{part1.y}) with max power of {part1.level}");
    Console.WriteLine($"Part 2: ({part2.x},{part2.y},{part2.size}) with max power of {part2.level}");
}

public int PowerLevel(int x, int y, int serial)
{
    var rackid = x + 10;
    return (((((rackid * y) + serial) * rackid) % 1000) / 100) - 5;
}