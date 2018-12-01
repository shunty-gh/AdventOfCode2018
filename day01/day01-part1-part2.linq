<Query Kind="Program" />

void Main()
{
	int dayNumber = 1, partNumber = 1;
	var input = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day{dayNumber.ToString("00")}-part{partNumber}.txt");
	var changes = File.ReadAllLines(input)
		.Select(i => int.Parse(i))
		.ToList(); 
	int current = 0, part1 = 0, part2 = 0;
	var seen = new HashSet<int>();
	bool found = false, firsttime = true;
	while (!found)
	{
		foreach (var change in changes)
		{
			current += change;
			if (seen.Contains(current))
			{
				found = true;
				part2 = current;
				if (!firsttime)
					break;
			}
			else
			{
				seen.Add(current);
			}
		}
		// After first full looop through...
		if (firsttime)
		{
			firsttime = false;
			part1 = current;
		}
	}

	part1.Dump("Part 1: Final frequency (after first full loop through)");
	part2.Dump("Part 2: First repeated frequency");
}
