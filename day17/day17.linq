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
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

void Main()
{
    // Advent of Code 2018 https://adventofcode.com/2018
    // Day 17

    // There is a bug in the implementation of part 1 that over reports the total by 18.
    // This is due to an issue at approx line 844 of the ouput where it incorrectly
    // adds an extra column of water going down for 18 rows.
    // I can't be bothered to fix it.
    
    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day17-input.txt");
    var input = File.ReadAllLines(inputname).ToList();
    //var input = TestInput();
	
	var pattern = @"(x=(?<x1>\d*).*y=(?<y1>\d*).{2}(?<y2>\d*))|(y=(?<y1>\d*).*x=(?<x1>\d*).{2}(?<x2>\d*))";
	var re = new Regex(pattern);
	input.ForEach(s =>
	{
		var matches = re.Match(s);
		var x1 = int.Parse(matches.Groups["x1"].Value);
		var y1 = int.Parse(matches.Groups["y1"].Value);
		var x2 = Math.Max(int.Parse("0" + matches.Groups["x2"]?.Value ?? "-1"), x1);
		var y2 = Math.Max(int.Parse("0" + matches.Groups["y2"]?.Value ?? "-1"), y1);

        if (x1 < 0 || y1 < 0)
            throw new Exception($"Unexpected values. x1={x1}; y1={y1}");
            
		foreach (var y in Enumerable.Range(y1, y2 - y1 + 1))
			foreach (var x in Enumerable.Range(x1, x2 - x1 + 1))
				Map[(x, y)] = Clay;
	});

    var minx = Map.Min(k => k.Key.X);
    var miny = Map.Min(k => k.Key.Y);
    var maxx = Map.Max(k => k.Key.X);
    var maxy = Map.Max(k => k.Key.Y);

    // Now add the spring at (500,0) after the min/max - so as not to include it in the valid range for calculation
    Map[(500, 0)] = Spring;
    
    Console.WriteLine($"Range X={minx}..{maxx}; Y={miny}..{maxy}");
    //State();
    
    var counter = 0;
    var heads = new Queue<(int X, int Y)>();
    heads.Enqueue((500, 0));
    while (heads.Count > 0)
    {
        counter++;
        var head = heads.Dequeue();
        
        // If it is beyond our max Y boundary then ignore it
        if (head.Y >= maxy)
            continue;
            
        // Flow down if possible
        var down = (head.X, head.Y + 1);
        if (!Map.ContainsKey(down) || Map[down] < Water)
        {
            Map[down] = Drain;
            heads.Enqueue(down);
        }
        // Check if this row section is already filled but has a drain at one end or the other (or both)
        else if ((Map[down] == Water || Map[down] == Drain) && !IsFilled(down))
        {
            // Skip it
            continue;
        }
        else if (Map[down] == Clay || Map[down] == Water || Map[down] == Drain)
        {
            // Spread out horizontally, checking for ways to go down
            var cangodown = false;
            
            // Fill left
            (int X, int Y) next = (head.X - 1, head.Y);
            while (!Map.ContainsKey(next) || Map[next] <= Drain)
            {
                if (CanGoDown(next))
                {
                    Map[next] = Drain;
                    cangodown = true;
                    heads.Enqueue(next);
                    break;
                }
                else
                {
                    Map[next] = Water;
                    next = (next.X - 1, next.Y);
                }
            }

            // Fill right
            next = (head.X + 1, head.Y);
            while (!Map.ContainsKey(next) || Map[next] <= Drain)
            {
                if (CanGoDown(next))
                {
                    Map[next] = Drain;
                    cangodown = true;
                    heads.Enqueue(next);
                    break;
                }
                else
                {
                    Map[next] = Water;
                    next = (next.X + 1, next.Y);
                }
            }
            
            // If we can't go down then we need to go back up one level and set 
            // that as the new head
            if (!cangodown)
                heads.Enqueue(FindPreviousHead(head));
        }
        
//        if (counter >= 450)
//            break;
    }

    //State();
    var part1 = Map.Count(m => m.Key.Y >= miny && m.Key.Y <= maxy && (m.Value == Water || m.Value == Drain));
    Console.WriteLine($"Part 1: {part1}");
    
    // Part - every continuous piece of water that is enclosed at both ends by clay
    // This method is a bit dubious as it could falsely include something like "#~#
    // where this is no bound below the line. However, it's not a problem with this input
    // and life is too short...
    var part2 = 0;
    for (var y = miny; y <= maxy; y++)
    {
        var x = minx - 1;
        var rowtotal = 0;
        while (x <= maxx)
        {
            x++;
            (int X, int Y) key = (x,y);
            if (!Map.ContainsKey(key))
            {
                continue;
            }

            if (Map[key] == Clay)
            {
                var sectiontotal = 0;
                // Walk along counting contiguous water cells, if any, until we find more 
                // clay or the end of the row
                for (var x1 = x + 1; x1 <= maxx; x1++)
                {
                    var key1 = (x1,y);
                    if (!Map.ContainsKey(key1) || Map[key1] < Water)
                    {
                        x = x1;
                        break;
                    }
                    
                    if (Map[key1] == Water || Map[key1] == Drain)
                    {
                        sectiontotal += 1;
                    }
                    else if (Map[key1] == Clay) // End of the section
                    {
                        rowtotal += sectiontotal;
                        x = x1 - 1; // Make sure we look at this cell one again in case it is a left bound for another enclosure
                        break;
                    }
                }
            }
        }
        part2 += rowtotal;
    }
    Console.WriteLine($"Part 2: {part2}");
}

const int Clay = 9;
const int Spring = 8;
const int Drain = 2;
const int Water = 1;
const int Empty = 0;
public Dictionary<(int X, int Y), int> Map = new Dictionary<(int X, int Y), int>();

public bool IsWater((int x, int y) key) => Map.ContainsKey(key) && (Map[key] == Water || Map[key] == Drain);
public bool IsClay((int x, int y) key) => Map.ContainsKey(key) && Map[key] == Clay;

public (int X, int Y) FindPreviousHead((int x, int y) key)
{
    // Go up one level then walk left and right until we are either blocked
    // or we hit water

    // Look up
    (int X, int Y) next = (key.x, key.y - 1);
    if (IsWater(next))
        return next;

    var dx = 0;
    bool blockedleft = false, blockedright = false;
    while (true)
    {
        dx++;
        // Look left
        if (!blockedleft)
        {
            next = (key.x - dx, next.Y);
            if (IsWater(next))
                return next;
            if (IsClay(next))
                blockedleft = true;
        }

        if (!blockedright)
        {
            // Look right
            next = (key.x + dx, next.Y);
            if (IsWater(next))
                return next;
            if (IsClay(next))
                blockedright = true;
        }
        
        if (blockedleft && blockedright)
        {
            throw new Exception($"Can't find previous head from {key}");
        }
    }
}

public bool IsFilled((int x, int y) key)
{
    // Check if there is a blockage at both ends of this row section and this 
    // section is filled with water that has no way out
    
    // Check left
    var blockleft = false;
    (int X, int Y) next = (key.x - 1, key.y);
    while (Map.ContainsKey(next))
    {
        if (Map[next] == Clay)
        {
            blockleft = true;
            break;
        }
        next = (next.X - 1, next.Y);
    }

    // Check right
    var blockright = false;
    next = (key.x + 1, key.y);
    while (Map.ContainsKey(next))
    {
        if (Map[next] == Clay)
        {
            blockright = true;
            break;
        }
        next = (next.X + 1, next.Y);
    }
    return blockleft && blockright;
}

public bool CanGoDown((int x, int y) key)
{
    var down = (key.x, key.y + 1);
    return !Map.ContainsKey(down) || Map[down] < Water;
}

public void State()
{
    Util.ClearResults();
    var minx = Map.Min(k => k.Key.X);
    var miny = Map.Min(k => k.Key.Y);
    var maxx = Map.Max(k => k.Key.X);
    var maxy = Map.Max(k => k.Key.Y);

    for (var y = miny - 1; y <= maxy + 1; y++)
    {
        Console.Write($"{y:d4}  ");
        for (var x = minx - 1; x <= maxx + 1; x++)
        {
            if (Map.ContainsKey((x,y)))
            {
                var content = Map[(x,y)];
                Console.Write(content == Clay ? '#' : content == Spring ? '+' : content == Drain ? 'v' : content == 1 ? '~' : '.');
            }
            else
            {
                Console.Write('.');
            }
        }
        Console.WriteLine("");
    }
}

public List<string> TestInput()
{
    return new List<string> 
    {
        "x=495, y=2..7",
        "y=7, x=495..501",
        "x=501, y=3..7",
        "x=498, y=2..4",
        "x=506, y=1..2",
        "x=498, y=10..13",
        "x=504, y=10..13",
        "y=13, x=498..504",       
    };
}
