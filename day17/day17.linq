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

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day17-input.txt");
    var input = File.ReadAllLines(inputname).ToList();
	
	var pattern = @"x=(?<x1>\d*).*y=(?<y1>\d*).{2}(?<y2>\d*)";
	var re = new Regex(pattern);
	var map = new Dictionary<(int,int), int>();
	input.ForEach(s =>
	{
		if (re.IsMatch(pattern))
		{
			var matches = re.Match(s);
			var x1 = int.Parse(matches.Groups["x1"].Value);
			var y1 = int.Parse(matches.Groups["y1"].Value);
			var x2 = Math.Max(int.Parse(matches.Groups["x2"]?.Value ?? "-1"), x1);
			var y2 = Math.Max(int.Parse(matches.Groups["y2"]?.Value ?? "-1"), y1);

			foreach (var y in Enumerable.Range(y1, y2 - y1 + 1))
				foreach (var x in Enumerable.Range(x1, x2 - x1 + 1))
					map.Add((x, y), 9);
		}
	});

	map.Dump();
}