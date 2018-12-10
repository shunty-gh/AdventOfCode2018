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
    // Day 10

    // Need a fixed pitch font in the LINQPad results window
    // to make this look reasonable.
    // See https://forum.linqpad.net/discussion/1150/how-do-i-get-monospaced-results
    // for how to do that.
    
    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day10-input.txt");
    var input = File.ReadAllLines(inputname)
        .Select(s => new Light
        {
            X = int.Parse(s.Substring(10, 6)),
            Y = int.Parse(s.Substring(18, 6)),
            dX = int.Parse(s.Substring(36, 2)),
            dY = int.Parse(s.Substring(40, 2)),
        })
//    var input = TestInput()  // Part 1 => "HI", Part 2 => 3
//        .Select(s => new Light 
//        {
//            X = int.Parse(s.Substring(10, 2)),
//            Y = int.Parse(s.Substring(13, 3)),
//            dX = int.Parse(s.Substring(28, 2)),
//            dY = int.Parse(s.Substring(32, 2)),
//        })
        .ToList<Light>();

    var moves = 0;
    while(true)
    {
        moves++;
        foreach (var p in input)
        {
            p.Move();
        }
        
        // Is it good enough to draw - ie are all the points close to at least one other
        if (input.All(p => input.Any(pp => p.IsNextTo(pp))))
        {
            Draw(input, moves);
            break;
        }
    }
}

public class Light
{
    public int X { get; set; }
    public int Y { get; set; }
    public int dX { get; set; }
    public int dY { get; set; }
    
    public void Move()
    {
        X += dX;
        Y += dY;
    }
    
    public bool IsXY(int x, int y)
    {
        return X == x && Y == y;
    }
    
    public bool IsNextTo(Light l)
    {
        return ((X != l.X) || (Y != l.Y)) && (Math.Abs(X - l.X) <= 1) && (Math.Abs(Y - l.Y) <= 1);
    }
}

public void Draw(IEnumerable<Light> points, int moveCount)
{
    var max_x = (int)points.Max(p => p.X);
    var max_y = (int)points.Max(p => p.Y);
    var min_x = (int)points.Min(p => p.X);
    var min_y = (int)points.Min(p => p.Y);

    Util.ClearResults();
    Console.WriteLine($"After {moveCount} moves");
    Console.WriteLine("");
    foreach (var y in Enumerable.Range(min_y - 3, max_y - min_y + 7))
    {
        foreach (var x in Enumerable.Range(min_x - 3, max_x - min_x + 7))
        {
            if (points.Any(p => p.IsXY(x, y)))
            {
                Console.Write('#');
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
        "position=< 9,  1> velocity=< 0,  2>",
        "position=< 7,  0> velocity=<-1,  0>",
        "position=< 3, -2> velocity=<-1,  1>",
        "position=< 6, 10> velocity=<-2, -1>",
        "position=< 2, -4> velocity=< 2,  2>",
        "position=<-6, 10> velocity=< 2, -2>",
        "position=< 1,  8> velocity=< 1, -1>",
        "position=< 1,  7> velocity=< 1,  0>",
        "position=<-3, 11> velocity=< 1, -2>",
        "position=< 7,  6> velocity=<-1, -1>",
        "position=<-2,  3> velocity=< 1,  0>",
        "position=<-4,  3> velocity=< 2,  0>",
        "position=<10, -3> velocity=<-1,  1>",
        "position=< 5, 11> velocity=< 1, -2>",
        "position=< 4,  7> velocity=< 0, -1>",
        "position=< 8, -2> velocity=< 0,  1>",
        "position=<15,  0> velocity=<-2,  0>",
        "position=< 1,  6> velocity=< 1,  0>",
        "position=< 8,  9> velocity=< 0, -1>",
        "position=< 3,  3> velocity=<-1,  1>",
        "position=< 0,  5> velocity=< 0, -1>",
        "position=<-2,  2> velocity=< 2,  0>",
        "position=< 5, -2> velocity=< 1,  2>",
        "position=< 1,  4> velocity=< 2,  1>",
        "position=<-2,  7> velocity=< 2, -2>",
        "position=< 3,  6> velocity=<-1, -1>",
        "position=< 5,  0> velocity=< 1,  0>",
        "position=<-6,  0> velocity=< 2,  0>",
        "position=< 5,  9> velocity=< 1, -2>",
        "position=<14,  7> velocity=<-2,  0>",
        "position=<-3,  6> velocity=< 2, -1>",      
    };
}