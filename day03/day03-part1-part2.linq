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
	int dayNumber = 3, partNumber = 1;
	var input = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day{dayNumber.ToString("00")}-part{partNumber}.txt");
	var claimsinput = File.ReadAllLines(input)
		.ToList();
    
    var pattern = @"#(?<id>\d*)\s@\s(?<left>\d*),(?<top>\d*):\s(?<width>\d*)x(?<height>\d*)";
    var re = new Regex(pattern);
    var map = new Dictionary<(int, int), int>();
    var claims = new List<(string id, int left, int top, int width, int height)>();
    foreach (var claim in claimsinput)
    {
        var matches = re.Match(claim);
        if (!matches.Success)
            throw new Exception("Invalid input (or invalid regex string)!");
            
        var id = matches.Groups["id"].Value;
        var l = int.Parse(matches.Groups["left"].Value);
        var t = int.Parse(matches.Groups["top"].Value);
        var w = int.Parse(matches.Groups["width"].Value);
        var h = int.Parse(matches.Groups["height"].Value);
        claims.Add((id, l, t, w, h)); // Saves re-running the Regex in part 2
        
        foreach (var i in Enumerable.Range(l, w))
        {
            foreach (var j in Enumerable.Range(t, h))
            {
                var key = (i, j);
                if (map.ContainsKey(key))
                    map[key] = map[key] + 1;
                else
                    map[key] = 1;
            }
        }
    }
    
    var overlaps = map.Where(m => m.Value > 1).Count();
    overlaps.Dump("Part 1");
    
    // Go through the claims again, now that the map is filled
    // and check if the claim is the only one in its area
    var overlap = false;
    foreach (var claim in claims)
    {
        overlap = false;
        foreach (var i in Enumerable.Range(claim.left, claim.width))
        {
            if (overlap)
                continue;
                
            foreach (var j in Enumerable.Range(claim.top, claim.height))
            {
                var key = (i, j);
                if (map[key] != 1)
                {
                    overlap = true;
                    continue;
                }
            }
        }

        if (!overlap)
        {
            claim.id.Dump("Part 2");
            break; // This assumes there's only one match
        }
    }
}
