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

    int dayNumber = 5, partNumber = 1;
    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day{dayNumber.ToString("00")}-part{partNumber}.txt");
    var input = File.ReadAllText(inputname).ToArray();
    //var input = "dabAcCaCBAcCcaDA".ToArray(); // Test input -> part1 = 10; part2 = 4;

    // Part 1
    var part1 = React(input);
    part1.Dump($"Part 1 result string has {part1.Length} chars)");

    // Part 2
    var part2 = part1;
    var removedchar = ' ';
    const string Alphabet = "ABVDEFGHIJKLMNOPQRSUVWXYZ";
    foreach (var ch in Alphabet)
    {
        var p2 = part1.ToCharArray(); // OR var p2 = input.ToArray(); doesn't make a difference
        // Blank out all occurrences of the current alpha char then 
        // re-run the reaction on the remaining characters
        for (var index = 0; index < p2.Length; index++)
        {
            if ((p2[index] == ch) || (p2[index] == ch + CharDiff))
            {
                p2[index] = ' ';
            }
        }
        var tmp = React(p2);
        //$"{ch} : {tmp.Length} - {tmp}".Dump();
        if (tmp.Length < part2.Length)
        {
            part2 = tmp;
            removedchar = ch;
        }
    }
    part2.Dump($"Part 2 result string has {part2.Length} chars by removing character \"{removedchar}\")");
}

const int CharDiff = 32; // 'a' - 'A';

public string React(char[] input)
{
    var len = input.Length;
    var reaction = true;
    while (reaction) // Keep looping through the array until there's no reaction
    {
        var index1 = 0;
        var index2 = 0;
        reaction = false;

        while (index1 < len) // Find a matching pair then move on through the array
        {
            while (index1 < len && input[index1] == ' ') { index1++; }
            if (index1 >= len) break;
            var ch1 = input[index1];
            index2 = index1 + 1;
            while (index2 < len && input[index2] == ' ') { index2++; }
            if (index2 >= len) break;
            var ch2 = input[index2];

            if (Math.Abs(ch1 - ch2) == CharDiff)
            {
                //$"Ch1 {ch1}; Ch2 {ch2}".Dump();
                reaction = true;
                input[index1] = ' ';
                input[index2] = ' ';
                index1 = index2 + 1;
            }
            else
            {
                index1 = index2;
            }
        }
        //string.Join("", input).Dump();
    }
    return string.Join("", input.Where(c => c != ' '));
}
