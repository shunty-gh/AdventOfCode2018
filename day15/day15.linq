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
    // Day 15

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day15-input.txt");
    var input = File.ReadAllLines(inputname).ToList();

    /* Test input expect:
		0 => 47 * 590 = 27730
		1 => 37 * 982 = 36334
		2 => 46 * 859 = 39514
		3 => 35 * 793 = 27755
		4 => 54 * 536 = 28944
		5 => 20 * 937 = 18740
    */
    //var input = TestInput(5);


    var part1 = BuildAndFight(input);
    Fighters.OrderBy(f => f.Y).ThenBy(f => f.X).Dump("Part 1");
    Console.WriteLine($"Part 1: {Fighters.Count} fighters left; {part1.Round} x {part1.HitSum} = {part1.Score}");
    //State();
    
    (int Score, int Round, int HitSum) part2 = (0,0,0);
    var elfpower = 3;
    while (part2.Score <= 0)
    {
        elfpower++;
        part2 = BuildAndFight(input, elfpower, true);
    }
    Fighters.OrderBy(f => f.Y).ThenBy(f => f.X).Dump("Part 2");
    Console.WriteLine($"Part 2: {Fighters.Count} fighters left with AttackPower {elfpower}; {part2.Round} x {part2.HitSum} = {part2.Score}");
}

public HashSet<(int X, int Y)> World = new HashSet<(int X, int Y)>();
public List<Fighter> Fighters = new List<Fighter>();

public bool IsOpen(int x, int y) => World.Contains((x, y)) && !Fighters.Any(f => f.X == x && f.Y == y);
public bool IsOpen((int x, int y) key) => IsOpen(key.x, key.y);

public (int Score, int Round, int HitSum) BuildAndFight(List<string> input, int ElfPower = 3, bool stopOnElfDeath = false)
{
    World.Clear();
    Fighters.Clear();
    BuildTheWorld(input);
    return Fight(ElfPower, stopOnElfDeath);
}

public void BuildTheWorld(List<string> input)
{
    for (var y = 0; y < input.Count; y++)
    {
        var row = input[y];
        for (var x = 0; x < input[0].Length; x++)
        {
            if (row[x] != '#')
            {
                World.Add((x, y));
                if (row[x] == 'G' || row[x] == 'E')
                    NewFighter(x, y, row[x]);
            }
        }
    }
}

public (int Score, int Round, int HitSum) Fight(int ElfPower, bool stopOnElfDeath)
{
    // Update Elf attack power
    Fighters.Where(f => f.IsElf).ToList().ForEach(f => f.AttackPower = ElfPower);
    
	var gcount = Fighters.Count(f => f.IsGoblin);
	var ecount = Fighters.Count(f => f.IsElf);
	var round = 0;
	
	while (gcount > 0 && ecount > 0)
	{
		//State();
		
		var fighters = Fighters
			.Where(f => f.HitPoints > 0) // Shouldn't be necessary
			.OrderBy(f => f.Y)
			.ThenBy(f => f.X)
			.ToList();		
		
		var turns = fighters.Count;
        foreach (var fighter in fighters)
		{
            turns--;
            
			Fighter fightWith = null;
			if (fighter.HitPoints <= 0)
				continue;
				
			// Identify targets
			var targets = Fighters
				.Where(f => f.HitPoints > 0 && fighter.IsElf ? f.IsGoblin : f.IsElf)
				.OrderBy(f => f.Y)
				.ThenBy(f => f.X)
				.ToList();
				
			if (targets.Count == 0)
				break;
                
            // Are we adjacent to one (or more) targets already. Target with the lowest HitPoints
            // will be chosen if > 1 target in range.
            fightWith = targets
                .Where(t => t.IsAdjacentTo(fighter))
                .OrderBy(t => t.HitPoints)
                .ThenBy(t => t.Y)
                .ThenBy(t => t.X)
                .FirstOrDefault();

            // Find open locations            
            var openlocs = new List<(int X, int Y)>();
            if (fightWith == null)
            {
                foreach (var target in targets)
                {
                    foreach ((int dx, int dy) in new(int, int)[] { (0, -1), (-1, 0), (0, 1), (1, 0) })
                    {
                        if (IsOpen(target.X + dx, target.Y + dy))
                        {
                            openlocs.Add((target.X + dx, target.Y + dy));
                        }
                    }
                }
            }
			
			// Move closer - find the shortest traversable path to a target and move one step along it
			if (fightWith == null && openlocs.Count > 0)
			{
                // Find the shortest paths to the open locations
				var q = new Queue<((int X, int Y) Key, List<(int X, int Y)> Path)>();			
				q.Enqueue(((fighter.X, fighter.Y), new List<(int X, int Y)>()));
				var pathlist = new List<List<(int X, int Y)>>();
                var bestpathtopoint = new Dictionary<(int X, int Y), (int Count, List<(int X, int Y)> Path)>();
				
                var minpath = -1;
				while (q.Count > 0)
				{
					var item = q.Dequeue();
                    var pcount = item.Path.Count;
                    if (minpath > 0 && pcount >= minpath)
                        continue;
                        
                    foreach ((int dx, int dy) in new(int, int)[] { (0, -1), (-1, 0), (0, 1), (1, 0) })
					{
						(int X, int Y) key = (item.Key.X + dx, item.Key.Y + dy);
                        
                        if (openlocs.Contains(key))
                        {
                            minpath = pcount + 1;
                            var np = item.Path.ToList();
                            np.Add(key);
                            pathlist.Add(np);
                        }                        
						else if (IsOpen(key) && !item.Path.Contains(key))
						{
                            if (bestpathtopoint.ContainsKey(key))
                            {
                                if (bestpathtopoint[key].Count < (pcount + 1))
                                {
                                    continue;
                                }
                                else 
                                {
                                    // Compare first step
                                    var b1 = bestpathtopoint[key].Path.First();
                                    var p1 = item.Path.First();
                                    if ((p1.Y > b1.Y) || ((p1.Y == b1.Y) && (p1.X >= b1.X)))
                                        continue;
                                }
                            }
                            
                            var np = item.Path.ToList();
                            np.Add(key);
                            bestpathtopoint[key] = (pcount + 1, np);
                            q.Enqueue((key, np));
						}
					}
				}

				// Move one step closer to the closest open location
                if (pathlist.Count > 0)
				{
					var minpathlen = pathlist.Min(p => p.Count);
					var closestopen = pathlist
						.Where(p => p.Count == minpathlen)
						.OrderBy(p => p.Last().Y)
						.ThenBy(p => p.Last().X)
						.ThenBy(p => p.First().Y)
						.ThenBy(p => p.First().X)
						.First();
					// Move to it
					fighter.X = closestopen.First().X;
					fighter.Y = closestopen.First().Y;

                    fightWith = targets
                        .Where(t => t.IsAdjacentTo(fighter))
                        .OrderBy(t => t.HitPoints)
                        .ThenBy(t => t.Y)
                        .ThenBy(t => t.X)
                        .FirstOrDefault();
                }
			}

			// Attack
			if (fightWith != null)
			{
				fighter.Attack(fightWith);
				if (fightWith.HitPoints <= 0)
				{
                    Fighters.Remove(fightWith);
                    if (fightWith.IsElf)
                    {
                        if (stopOnElfDeath)
                        {
                            return (0,0,0);
                        }
                        ecount--;
                    }
					else
						gcount--;
					
					if (gcount <= 0 || ecount <= 0)
						break;
				}
			}
        }
        if (turns == 0)
            round++;
    }

	var hitsum = Fighters.Sum(f => f.HitPoints);
	var score = round * hitsum;
    return (score, round, hitsum);	
}

public Fighter NewFighter(int x, int y, char fighterType)
{
	var ftype = fighterType == 'E' ? FighterType.Elf : fighterType == 'G' ? FighterType.Goblin : FighterType.None;
	if (ftype == FighterType.None)
		return null;
		
	var result = new Fighter(x, y, ftype);
	Fighters.Add(result);
	return result;
}

public enum FighterType
{
	None,
	Elf,
	Goblin,
}

public class Fighter
{
	public int X { get; set; }
	public int Y { get; set; }
	public FighterType FighterType { get; }
	public int AttackPower { get; set; } = 3;
	public int HitPoints { get; private set; } = 200;

	public Fighter(int x, int y, FighterType fighterType)
	{
		X = x;
		Y = y;
		FighterType = fighterType;
	}
	
	public void Attack(Fighter opponent)
	{
		opponent.HitPoints -= this.AttackPower;
	}
	
	public bool IsAt(int x, int y) => X == x && Y == y;
	public bool IsAdjacentTo(Fighter opponent) 
	{
		return IsAdjacentTo(opponent.X, opponent.Y);
	}
	
	public bool IsAdjacentTo(int x, int y)
	{
		var dx = Math.Abs(X - x);
		var dy = Math.Abs(Y - y);
		return (dx == 0 && dy == 1) || (dx == 1 && dy == 0);
	}
	
	public bool IsElf => FighterType == FighterType.Elf;
	public bool IsGoblin => FighterType == FighterType.Goblin;
}

public List<string> TestInput(int testNo = 0)
{
	var tests = new List<List<string>>
    {
        new List<string>
        {
			// Part 1 = 47 * 590 = 27730
			"#######",
            "#.G...#",
            "#...EG#",
            "#.#.#G#",
            "#..G#E#",
            "#.....#",
            "#######",
        },

        new List<string>
		{
			// Part 1 = 37 * 982 = 36334
			"#######",
			"#G..#E#",
			"#E#E.E#",
			"#G.##.#",
			"#...#E#",
			"#...E.#",
			"#######",
		},

		new List<string>
		{
			// Part 1 = 46 * 859 = 39514
			"#######",
			"#E..EG#",
			"#.#G.E#",
			"#E.##E#",
			"#G..#.#",
			"#..E#.#",
			"#######",
        },
        
        new List<string>
        {
			// Part 1 = 35 * 793 = 27755
			"#######",
            "#E.G#.#",
            "#.#G..#",
            "#G.#.G#",
            "#G..#.#",
            "#...E.#",
            "#######",
        },

        new List<string>
        {
			// Part 1 = 54 * 536 = 28944
			"#######",
            "#.E...#",
            "#.#..G#",
            "#.###.#",
            "#E#G#G#",
            "#...#G#",
            "#######",
        },

        new List<string>
        {
			// Part 1 = 20 * 937 = 18740
			"#########",
            "#G......#",
            "#.E.#...#",
            "#..##..G#",
            "#...##..#",
            "#...#...#",
            "#.G...G.#",
            "#.....G.#",
            "#########",
        },
    };

    return tests[testNo];
}

public void State()
{
	foreach (var y in Enumerable.Range(0, World.Max(w => w.Y) + 2))
    {
        var ptstr = "";
        Console.Write("  ");
        foreach (var x in Enumerable.Range(0, World.Max(w => w.X) + 2))
		{	
			if (World.Contains((x,y)))
			{
				if (Fighters.Any(f => f.X == x && f.Y == y))
				{
                    var fighter = Fighters.First(f => f.X == x && f.Y == y);
                    ptstr += $"{(fighter.IsElf ? "E:" : "G:")} {fighter.HitPoints}; ";
					Console.Write(Fighters.First(f => f.X == x && f.Y == y).IsElf ? 'E' : 'G');
				}
				else
				{
					Console.Write('.');
				}
			}
			else
			{
				Console.Write('#');
			}
		}
		Console.Write("  " + ptstr);
        Console.WriteLine("");
	}
	Console.WriteLine("");
}