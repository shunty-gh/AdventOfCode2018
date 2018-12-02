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
	int dayNumber = 2, partNumber = 1;
	var input = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day{dayNumber.ToString("00")}-part{partNumber}.txt");
	var ids = File.ReadAllLines(input)
	//var ids = GetTestInput()
		.ToList();
	int three = 0, two = 0;
	var idcounts = new Dictionary<char, int>();	
	foreach(var id in ids)
	{
		idcounts.Clear();
		foreach (var ch in id)
		{
			if (idcounts.ContainsKey(ch))
				idcounts[ch] = idcounts[ch] + 1;
			else
				idcounts[ch] = 1;
		}
		var has2 = idcounts.Any(c => c.Value == 2);
		var has3 = idcounts.Any(c => c.Value == 3);
		if (has2) two++;
		if (has3) three++;
	}
	
	var part1 = two * three;
	part1.Dump("Part 1");
	
	var index = 0;
	var part2 = "";
	foreach (var id in ids)
	{
		index++;
		//Check if it *nearly* matches any of the remaining ids
		foreach (var id2 in ids.Skip(index))
		{
			if (id.Length != id2.Length)
				continue;
			var diffs = id.Select((ch, i) => ch != id2[i] ? new Tuple<char, int>(ch, i) : null)
				.Where(t => t != null)
				.ToList();
			if (diffs.Count() == 1)
			{
				// Found
				$"Found {id} and {id2} have diff {diffs[0]}".Dump();
				part2 = id.Substring(0, diffs[0].Item2) + id.Substring(diffs[0].Item2 + 1);
				break;
			}
		}
	}
	Clipboard.SetText(part2);
	part2.Dump("Part 2");
}

public List<string> GetTestInput()
{
	return new List<string>
	{
		"abcde",
		"fghij",
		"klmno",
		"pqrst",
		"fguij",
		"axcye",
		"wvxyz",
	};
}