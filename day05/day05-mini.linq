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
    // Day 5

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day05-part1.txt");
    var input = File.ReadAllText(inputname);

    // Part 1
    var part1 = React(input);
    part1.Length.Dump($"Part 1");

    // Part 2
    var part2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".AsEnumerable()
        .Min(c => 
            React(part1.Replace(c.ToString(), "").Replace(c.ToString().ToLower(), ""))
            .Length);    
    part2.Dump("Part 2");
}

public string React(string input)
{
    var sb = new StringBuilder(input.Length);
    foreach (var c in input)
    {
        var prev = sb.Length > 0 ? sb[sb.Length - 1] : c;
        if (prev == c || char.ToUpper(c) != char.ToUpper(prev))
            sb.Append(c);
        else
            sb.Remove(sb.Length - 1, 1);            
    }
    return sb.ToString();
}
