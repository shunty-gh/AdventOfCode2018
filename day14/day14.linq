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
    // Day 14

    var input = 165061;
    var recipes = new List<int>(input + 10) { 3, 7 };
    int current1 = 0, current2 = 1;
    int round = 0;
    bool p1 = false, p2 = false;

    // For part 2
    var check = input.ToString();
    //var check = 59414.ToString();  // => 2018
    var len = check.Length;
    var laststart = 0;
    
    while (!(p1 && p2))
    {
        round++;
        var score1 = recipes[current1];
        var score2 = recipes[current2];
        var newrecipes = score1 + score2;
        if (newrecipes < 10)
            recipes.Add(newrecipes);
        else
        {
            recipes.Add(newrecipes / 10);
            recipes.Add(newrecipes % 10);
        }
        current1 = (current1 + 1 + score1) % recipes.Count;
        current2 = (current2 + 1 + score2) % recipes.Count;

        if (!p1)
        {
            //var stopat = 2018;  // => 5941429882
            var stopat = input;
            if (recipes.Count > stopat + 10)
            {
                p1 = true;
                var part1 = string.Join("", recipes.Skip(stopat).Take(10));
                Console.WriteLine($"Part 1: {part1}");
            }
        }

        if (!p2)
        {
            // Do the last chars contain our input digits. Only check new possibilities,
            // not ones we've already checked
            var rlen = recipes.Count;
            if (rlen < len)
                continue;
            
            for (var index = laststart; index <= rlen - len; index++)
            {
                laststart = index;
                if (recipes[laststart] == (check[0] - '0') && recipes[laststart + 1] == (check[1] - '0') 
                    && recipes[laststart + 2] == (check[2] - '0') && recipes[laststart + 3] == (check[3] - '0'))
                {
                    var tmp = string.Join("", recipes.Skip(index).Take(len));
                    if (tmp == check)
                    {
                        p2 = true;
                        var part2 = index;
                        Console.WriteLine($"Part 2: {part2}");
                        break;
                    }
                }
            }
        }
    }
}