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
    Thread.Sleep(500);
    //return;

    // Live
    var iteration = 0;
    while (iterations == 0 || iteration < iterations)
    {
        iteration++;
        Evolve(cells);
        DrawWorld(cells, iteration);
        Thread.Sleep(500);
    }

    var trees = cells.Count(c => c.Value.State == LandscapeState.Trees);
    var lumberyards = cells.Count(c => c.Value.State == LandscapeState.LumberYard);
    var part1 = trees * lumberyards;
    Console.WriteLine($"Part 1: {trees} trees x {lumberyards} lumberyards = {part1}");
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