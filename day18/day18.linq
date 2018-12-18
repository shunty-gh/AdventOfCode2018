<Query Kind="Program" />

void Main()
{
    // Advent of Code 2018 https://adventofcode.com/2018
    // Day 18

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day18-input.txt");
    var input = File.ReadAllLines(inputname).ToList();
    //var input = TestData();

    // Set up
    var iterations = 10; // Set to max number or 0 for infinite
    var cells = CreateInitialState(input);

    // Draw the initial state
    DrawWorld(cells, 0);
    Thread.Sleep(250);
    //return;

    // Part 1
    var iteration = 0;
    while (iterations == 0 || iteration < iterations)
    {
        iteration++;
        Evolve(cells);
        DrawWorld(cells, iteration);
        Thread.Sleep(250);
    }

    var trees = cells.Count(c => c.Value.State == LandscapeState.Trees);
    var lumberyards = cells.Count(c => c.Value.State == LandscapeState.LumberYard);
    var part1 = trees * lumberyards;
    Console.WriteLine($"Part 1: {trees} trees x {lumberyards} lumberyards = {part1}");

    // Part 2
    // Repeat until the state stabilises then find the repeat frequency. Get
    // resource values for each item in the repeating sequence. Find the index that
    // matches (target value % frequency).
    iterations = 0;
    cells.Clear();
    cells = CreateInitialState(input);
    List<(int, int, LandscapeState)> snapshotbase = null;
    var snapshotat = 500; // Arbitrary number at which point we assume it's settled into a rythmn
    var periodfound = false;
    var period = 0;
    iteration = 0;
    var resourcevalues = new List<(int Iteration, int Value)>();
    while (iterations == 0 || iteration < iterations)
    {
        iteration++;
        Evolve(cells);
        
        // Take a snapshot at an arbitrary point
        if (iteration == snapshotat)
        {
            snapshotbase = Snapshot(cells);
        }
        // Loop until we find an iteration that matches our original snapshot
        if (iteration > snapshotat && !periodfound)
        {
            var snapshot = Snapshot(cells);
            if (SnapshotsEqual(snapshotbase, snapshot))
            {
                periodfound = true;
                period = iteration - snapshotat;
                Console.WriteLine($"Period found: {period}");
            }            
        }

        // Once found, generate resource values for each possibility
        if (periodfound)
        {
            // Adding on one more than needed ( <= as opposed to < ) just so 
            // we can check that the last value in the list matches the first
            for (var pindex = 0; pindex <= period; pindex++) 
            {
                resourcevalues.Add((iteration, GetResourceValue(cells)));
                iteration++;
                Evolve(cells);
            }
            // No need to carry on once we have all possible outcomes
            break;
        }
    }

    //resourcevalues.Dump();

    var target = 1_000_000_000;
    var rvindex = ((target - resourcevalues.First().Iteration) % period);
    var part2 = resourcevalues[rvindex].Value;
    Console.WriteLine($"Part 2: {part2}");
}

public int GetResourceValue(Dictionary<(int, int), Cell> cells)
{
    var trees = cells.Count(c => c.Value.State == LandscapeState.Trees);
    var lumberyards = cells.Count(c => c.Value.State == LandscapeState.LumberYard);
    return trees * lumberyards;
}

public bool SnapshotsEqual(List<(int, int, LandscapeState)> s1, List<(int, int, LandscapeState)> s2)
{
    if (s1.Count != s2.Count)
        return false;
    
    for (var index = 0; index < s1.Count; index++)
    {
        var i1 = s1[index];
        var i2 = s2[index];
        if (i1.Item1 != i2.Item1 || i1.Item2 != i2.Item2 || i1.Item3 != i2.Item3)
            return false;        
    }
    return true;
}

public enum LandscapeState
{
    None,
    Open,       // '.'
    Trees,      // '|'
    LumberYard, // '#
}

public Dictionary<(int, int), Cell> CreateInitialState(List<string> input)
{
    var cells = new Dictionary<(int x, int y), Cell>();
    for (var y = 0; y < input.Count; y++)
    {
        var line = input[y];
        for (var x = 0; x < line.Length; x++)
        {
            var c = line[x];
            if (c == '.')
                CreateCell(cells, x, y, LandscapeState.Open);
            else if (c == '|')
                CreateCell(cells, x, y, LandscapeState.Trees);
            else if (c == '#')
                CreateCell(cells, x, y, LandscapeState.LumberYard);
            else
                throw new Exception($"Unexpected item in the world: \"{c}\" in line {y} at column {x}");
        }
    }
    return cells;
}

public Cell CreateCell(Dictionary<(int, int), Cell> cells, int x, int y, LandscapeState state = LandscapeState.Open)
{
    var result = new Cell { X = x, Y = y, State = state};
    cells.Add((x, y), result);
    return result;
}

public (int Open, int Trees, int Lumberyards) GetNeighbourCount(Dictionary<(int x, int y), Cell> cells, int x, int y)
{
    (int Open, int Trees, int Lumberyards) = (0,0,0);
    foreach ((int dx, int dy) in new(int, int)[] { (0, -1), (-1, -1), (-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1) })
    {
        var key = (x + dx, y + dy);
        if (cells.ContainsKey(key))
        {
            var state = cells[key].State;
            if (state == LandscapeState.Open)
                Open++;
            else if (state == LandscapeState.Trees)
                Trees++;
            else if (state == LandscapeState.LumberYard)
                Lumberyards++;
        }
    }
    return (Open, Trees, Lumberyards);
}

public void Evolve(Dictionary<(int x, int y), Cell> cells)
{
    var max_x = cells.Max(kvp => kvp.Key.x);
    var max_y = cells.Max(kvp => kvp.Key.y);
    var min_x = cells.Min(kvp => kvp.Key.x);
    var min_y = cells.Min(kvp => kvp.Key.y);

    for (var y = min_y; y <= max_y; y++)
    {
        for (var x = min_x; x <= max_x; x++)
        {
            Cell cell = cells[(x, y)];
            var neighbours = GetNeighbourCount(cells, x, y);

            // Rules
            // An open acre will become filled with trees if three or more 
            // adjacent acres contained trees. Otherwise, nothing happens.
            if (cell.State == LandscapeState.Open)
            {
                if (neighbours.Trees >= 3)
                    cell.NextState = LandscapeState.Trees;
                else 
                    cell.NextState = cell.State;
            }
            
            // An acre filled with trees will become a lumberyard if three 
            // or more adjacent acres were lumberyards. Otherwise, nothing 
            // happens.
            else if (cell.State == LandscapeState.Trees)
            {
                if (neighbours.Lumberyards >= 3)
                    cell.NextState = LandscapeState.LumberYard;
                else
                    cell.NextState = cell.State;
            }
            
            // An acre containing a lumberyard will remain a lumberyard if 
            // it was adjacent to at least one other lumberyard and at least 
            // one acre containing trees. Otherwise, it becomes open.
            else if (cell.State == LandscapeState.LumberYard)
            {
                if (neighbours.Lumberyards < 1 || neighbours.Trees < 1)
                    cell.NextState = LandscapeState.Open;
                else
                    cell.NextState = cell.State;
            }
        }
    }
    // Update all cells so that we apply all their NextState values all at once
    foreach (var cell in cells)
        cell.Value.State = cell.Value.NextState;
}

public List<(int, int, LandscapeState)> Snapshot(Dictionary<(int x, int y), Cell> cells)
{
    return cells
        .OrderBy(c => c.Key.y)
        .ThenBy(c => c.Key.x)
        .Select(c => (c.Key.x, c.Key.y, c.Value.State))
        .ToList();
}

public char StateToChar(LandscapeState state) => state == LandscapeState.LumberYard ? '#' : state == LandscapeState.Trees ? '|' : '.';
public void DrawWorld(Dictionary<(int x, int y), Cell> cells, int iteration)
{
    if (cells.Count == 0)
        return;
        
    var max_x = cells.Max(kvp => kvp.Key.x);
    var max_y = cells.Max(kvp => kvp.Key.y);
    var min_x = cells.Min(kvp => kvp.Key.x);
    var min_y = cells.Min(kvp => kvp.Key.y);
    Util.ClearResults();

    Console.WriteLine("");
    Console.WriteLine($"Iteration {iteration}");
    Console.WriteLine("");
    for (var y = min_y; y <= max_y; y++)
    {
        Console.Write("  ");
        for (var x = min_x; x <= max_x; x++)
        {
            if (cells.ContainsKey((x,y)))
            {
                var cell = cells[(x,y)];
                Console.Write(StateToChar(cell.State));
            }
            else
            {
                Console.Write(StateToChar(LandscapeState.Open));
            }
        }
        Console.WriteLine("");
    }
}

public class Cell
{
    public int X { get; set; }
    public int Y { get; set; }
    public LandscapeState State { get; set; }
    public LandscapeState NextState { get; set; }
}

public List<string> TestData()
{
    return new List<string>
    {
        ".#.#...|#.",
        ".....#|##|",
        ".|..|...#.",
        "..|#.....#",
        "#.#|||#|#|",
        "...#.||...",
        ".|....|...",
        "||...#|.#|",
        "|.||||..|.",
        "...#.|..|.",
    };
}