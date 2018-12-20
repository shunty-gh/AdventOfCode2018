<Query Kind="Program" />

void Main()
{
    // Advent of Code 2018 https://adventofcode.com/2018
    // Day 19

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day20-input.txt");
    var input = File.ReadAllText(inputname);
    // *** Test data ***
    //var input = "^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$"; // Part 1 => 18
    //var input = "^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$"; // Part 1 => 23
    //var input = "^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$"; // Part 1 => 31
    // ***
                             
    BuildMap(input);

    var paths = ShortestPaths();
    var part1 = paths.Max(p => p.Value.Count);
    Console.WriteLine($"Part 1: {part1}");

    var part2 = paths.Count(p => p.Value.Count >= 1000);
    Console.WriteLine($"Part 2: {part2}");

    State();
}

public Dictionary<(int X, int Y), Node> Map = new Dictionary<(int X, int Y), Node>();

public (int X, int Y) North((int X, int Y) current) => (current.X, current.Y - 1);
public (int X, int Y) South((int X, int Y) current) => (current.X, current.Y + 1);
public (int X, int Y) West((int X, int Y) current) => (current.X - 1, current.Y);
public (int X, int Y) East((int X, int Y) current) => (current.X + 1, current.Y);

public bool DoorNorth((int X, int Y) point) => Map.ContainsKey(North(point)) && IsDoor(Map[North(point)].Content);
public bool DoorSouth((int X, int Y) point) => Map.ContainsKey(South(point)) && IsDoor(Map[South(point)].Content);
public bool DoorWest((int X, int Y) point) => Map.ContainsKey(West(point)) && IsDoor(Map[West(point)].Content);
public bool DoorEast((int X, int Y) point) => Map.ContainsKey(East(point)) && IsDoor(Map[East(point)].Content);

public Node AddRoom((int x, int y) point) => AddOrUpdate(point.x, point.y, NodeContent.Room);
public Node AddWall((int x, int y) point) => AddOrUpdate(point.x, point.y, NodeContent.Wall);
public Node AddDoorNS((int x, int y) point) => AddOrUpdate(point.x, point.y, NodeContent.DoorNS);
public Node AddDoorWE((int x, int y) point) => AddOrUpdate(point.x, point.y, NodeContent.DoorWE);
public bool IsDoor(NodeContent content) => content == NodeContent.DoorNS || content == NodeContent.DoorWE;

public void Initialise()
{
    Map.Clear();
    AddRoom((0, 0));
}

public Dictionary<Node, List<Node>> ShortestPaths()
{
    var q = new Queue<(Node Node, List<Node> Path)>();
    q.Enqueue((Map[(0,0)], new List<Node>()));
    var shortestpaths = new Dictionary<Node, List<Node>>();
    
    while (q.Count > 0)
    {
        var item = q.Dequeue();
        var node = item.Node;

        // Find all exits from this room. Add each destination to the queue to be processed 
        // unless we've been there before.
        Node dest;
        List<Node> newpath;
        foreach ((int dx, int dy) in new(int, int)[] { (0, -1), (-1, 0), (0, 1), (1, 0) })
        {
            var k1 = (node.X + dx, node.Y + dy);
            if (Map.ContainsKey(k1) && IsDoor(Map[k1].Content))
            {
                var k2 = (node.X + dx + dx, node.Y + dy + dy);
                dest = Map[k2];
                
                // Is this the shortest route to this point
                if (!shortestpaths.ContainsKey(dest) || (item.Path.Count + 1 < shortestpaths[dest].Count))
                {
                    // Add it to the queue for further processing if we haven't already seen it
                    if (!item.Path.Contains(dest))
                    {
                        newpath = item.Path.ToList();
                        newpath.Add(dest);
                        q.Enqueue((dest, newpath));

                        shortestpaths[dest] = newpath;
                    }
                }
            }
        }
    }
    return shortestpaths;
}

public void BuildMap(string input)
{
    var index = 0;
    (int X, int Y) current = (0, 0);
    var complete = false;
    var splits = new List<((int X, int Y) Point, int Index)>();
    Initialise();
    while (!complete)
    {
        index++;
        char ch = input[index];
        (int, int) next;
        switch (ch)
        {
            case 'N':
                // There's a door north 1 step and a room 2 steps
                next = North(current);
                AddDoorWE(next);
                next = North(next);
                AddRoom(next);
                current = next;
                break;
            case 'E':
                next = East(current);
                AddDoorNS(next);
                next = East(next);
                AddRoom(next);
                current = next;
                break;
            case 'S':
                next = South(current);
                AddDoorWE(next);
                next = South(next);
                AddRoom(next);
                current = next;
                break;
            case 'W':
                next = West(current);
                AddDoorNS(next);
                next = West(next);
                AddRoom(next);
                current = next;
                break;
            case '(':
                splits.Add(((current.X, current.Y), index));
                break;
            case ')':
                // Remove the last split
                splits.RemoveAt(splits.Count - 1);
                break;
            case '|':
                // Back up to the previous split point
                current = splits.Last().Point;
                break;
            case '$':
                complete = true;
                break;
            default:
                throw new Exception($"Unexpected regex character '{ch}' at index {index}");
        }
    }
    System.Diagnostics.Debug.Assert(splits.Count == 0, $"List of splits should be empty but it has {splits.Count} items");
}

public Node AddOrUpdate(int x, int y, NodeContent content)
{
    Node result;
    if (Map.ContainsKey((x, y)))
    {
        result = Map[(x, y)];
        if (result.Content == NodeContent.Unknown)
            result.Content = content;

        if (result.Content != content)
            throw new Exception($"Unexpected node content at ({x},{y}). Expected {content} but found {result.Content}");
    }
    else
    {
        result = new Node(x, y, content);
        Map.Add(result.Point, result);
    }
    return result;
}

public enum NodeContent
{
    Unknown,
    Wall,
    DoorNS,
    DoorWE,
    Room,
}

public class Node
{
    public int X { get; } = 0;
    public int Y { get; } = 0;
    public (int X, int Y) Point => (X, Y);
    public NodeContent Content { get; set; } = NodeContent.Unknown;
    
    public Node(int x, int y, NodeContent content = NodeContent.Unknown)
    {
        X = x;
        Y = y;
        Content = content;
    }
}

public void State()
{
    var minx = Map.Min(m => m.Key.X) - 1;
    var miny = Map.Min(m => m.Key.Y) - 1;
    var maxx = Map.Max(m => m.Key.X) + 1;
    var maxy = Map.Max(m => m.Key.Y) + 1;

    Console.WriteLine("");
    foreach (var y in Enumerable.Range(miny, maxy - miny + 1))
    {
        Console.Write("  ");
        foreach (var x in Enumerable.Range(minx, maxx - minx + 1))
        {
            if (x == 0 && y == 0)
            {
                Console.Write('X');
                continue;
            }

            var key = (x,y);
            if (Map.ContainsKey(key))
            {
                var c = Map[key].Content;
                Console.Write(c == NodeContent.DoorNS 
                    ? '|' 
                    : c == NodeContent.DoorWE
                        ? '-'
                        :  c == NodeContent.Room 
                            ? '.' 
                            : c == NodeContent.Wall ? '#' : '#');
            }
            else
            {
                Console.Write('#');
            }
        }
        Console.WriteLine("");
    }
}