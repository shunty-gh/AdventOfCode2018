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
    // Day 4
    
	int dayNumber = 4, partNumber = 1;
	var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day{dayNumber.ToString("00")}-part{partNumber}.txt");
	var input = File.ReadAllLines(inputname)
        .OrderBy(s => s)
        .ToList();
    
    var pattern = @"\[(?<dt>\d{4}-\d{2}-\d{2}\s\d{2}:\d{2})\]\s(?<action>(Guard #(?<guardid>\d*) begins shift|wakes up|falls asleep))";
    var re = new Regex(pattern);
    
    (int GuardId, int SleepStart) current = (-1, -1);
    var sleeptimes = new Dictionary<int, List<int>>();

    // Parse each input line and add the minutes asleep for each guard to a list
    foreach (var item in input)
    {
        var match = re.Match(item);
        var dt = DateTime.Parse(match.Groups["dt"].Value);
        var action = match.Groups["action"].Value;
        var guardid = action.StartsWith("Guard") ? int.Parse(match.Groups["guardid"].Value) : -1;

        // Only 3 actions "wakes...", "falls...", "Guard..."
        if (action.StartsWith("falls"))
        {
            current.SleepStart = dt.Minute;
        }
        else // either "wakes" or changes guard
        {
            if (current.SleepStart >= 0)
            {                
                // Add sleeping minutes to the list for this guard
                var sleepminutes = dt.Minute - current.SleepStart;
                if (sleeptimes.ContainsKey(current.GuardId))
                    sleeptimes[current.GuardId].AddRange(Enumerable.Range(current.SleepStart, sleepminutes));
                else
                    sleeptimes[current.GuardId] = new List<int>(Enumerable.Range(current.SleepStart, sleepminutes));
                
                current.SleepStart = -1;
            }
            
            if (guardid >= 0) // Changing of the guard
            {
                current.GuardId = guardid;
            }
        }
    }
    
    // Part 1
    // Guard who is asleep for the most minutes
    var sleepiest = sleeptimes.OrderByDescending(kvp => kvp.Value.Count).First();
    var sleepfreq = sleepiest.Value.GroupBy(m => m).OrderByDescending(g => g.Count());
    var part1 = sleepiest.Key * sleepfreq.First().Key;
    part1.Dump("Part 1");
    
    // Part 2
    // Guard who is asleep most often on a given minute
    (int GuardId, int Minute, int Count) mostoftenasleep = (0, 0, 0);
    foreach (var kvp in sleeptimes)
    {
        var sleepeistminute = kvp.Value
            .GroupBy(m => m)
            .OrderByDescending(g => g.Count())
            .Select(g => new { Minute = g.Key, Count = g.Count() })
            .First();
        if (sleepeistminute.Count > mostoftenasleep.Count)
        {
            mostoftenasleep = (kvp.Key, sleepeistminute.Minute, sleepeistminute.Count);
        }
    }
    var part2 = mostoftenasleep.GuardId * mostoftenasleep.Minute;
    part2.Dump("Part 2");
}
