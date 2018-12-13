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
    // Day 13

    var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day13-input.txt");
    var input = File.ReadAllLines(inputname).ToList();

    // Build the map
    for (var y = 0; y < input.Count; y++)
    {
        var row = input[y];
        for (var x = 0; x < input[0].Length; x++)
        {
            if (row[x] == ' ')
                continue;
            
            Node node = NewOrExistingNode(x, y);
            
            Node e, s;
            switch (row[x])
            {
                case '-':
                    e = NewOrExistingNode(x + 1, y, west: node);
                    node.East = e;
                    node.TrackType = NodeType.Straight;
                    break;
                case '|':
                    s = NewOrExistingNode(x, y + 1, north: node);
                    node.South = s;
                    node.TrackType = NodeType.Straight;
                    break;
                case '/': // Could either be (S <-> E) or (N <-> W)
                    if (node.North != null)
                    {
                        // Nothing to do. The E node is not connected to this one.
                    }
                    else
                    {
                        e = NewOrExistingNode(x + 1, y, west: node);
                        s = NewOrExistingNode(x, y + 1, north: node);
                        node.East = e;
                        node.South = s;
                    }
                    node.TrackType = NodeType.Corner;
                    break;
                case '\\': // Could either be (N <-> E) or (S <-> W)
                    if (node.West != null)
                    {
                        s = NewOrExistingNode(x, y + 1, north: node);
                        node.South = s;
                    }
                    else
                    {
                        e = NewOrExistingNode(x + 1, y, west: node);
                        node.East = e;
                    }
                    node.TrackType = row[x] == '|' ? NodeType.Straight : NodeType.Corner;
                    break;
                case '+':
                    // The north and west nodes will already be filled in
                    e = NewOrExistingNode(x + 1, y, west: node);
                    s = NewOrExistingNode(x, y + 1, north: node);
                    node.TrackType = NodeType.Intersection;
                    node.East = e;
                    node.South = s;
                    break;
                case '>': // Add cart and a '-' node
                    Carts.Add(new Cart(x, y, Direction.East));
                    e = NewOrExistingNode(x + 1, y, west: node);
                    node.East = e;
                    node.TrackType = NodeType.Straight;
                    break;
                case '<': // Add cart and a '-' node
                    Carts.Add(new Cart(x, y, Direction.West));
                    e = NewOrExistingNode(x + 1, y, west: node);
                    node.East = e;
                    node.TrackType = NodeType.Straight;
                    break;
                case '^': // Add cart and a '|' node
                    Carts.Add(new Cart(x, y, Direction.North));
                    s = NewOrExistingNode(x, y + 1, north: node);
                    node.South = s;
                    node.TrackType = NodeType.Straight;
                    break;
                case 'v': // Add cart and a '|' node
                    Carts.Add(new Cart(x, y, Direction.South));
                    s = NewOrExistingNode(x, y + 1, north: node);
                    node.South = s;
                    node.TrackType = NodeType.Straight;
                    break;
            }
        }
    }

    // Drive around it
    var collision = false;
    var carts = Carts.OrderBy(c => c.Y).ThenBy(c => c.X).Skip(9).Take(1);
    while (!collision)
    {
        //var carts = Carts.OrderBy(c => c.Y).ThenBy(c => c.X);
        foreach (var cart in carts)
        {
            cart.Move(Map);
            // Has it collided
            if (Carts.Count(c => c.X == cart.X && c.Y == cart.Y) > 1)
            {
                // Collision
                Console.WriteLine($"Collision at ({cart.X},{cart.Y})");
                collision = true;
                break;
            }
        }
    }
}

public Dictionary<(int x, int y), Node> Map = new Dictionary<(int x, int y), Node>();
public List<Cart> Carts = new List<Cart>();

public Node NewOrExistingNode(int x, int y, Node north = null, Node east = null, Node south = null, Node west = null)
{
    Node result;
    var key = (x,y);
    if (Map.ContainsKey(key))
    {
        result = Map[key];
    }
    else
    {
        result = new Node(x, y);
        Map.Add(key, result);
    }
    
    if (north != null) result.North = north;
    if (east != null) result.East = east;
    if (south != null) result.South = south;
    if (west != null) result.West = west;
    return result;
}

public enum Direction
{
    North = 0,
    East = 90,
    South = 180,
    West = 270,    
}

public enum NodeType
{
    Straight,
    Corner,
    Intersection,
}

public enum Turn
{
    Left = -90,
    Straight = 0,
    Right = 90,
    None = 360,
}

public class Cart
{
    private int _initialX;
    private int _initialY;
    public int X { get; private set; }
    public int Y { get; private set; }
    public Direction Facing { get; private set; } = Direction.North;
    public Turn LastTurn { get; private set; } = Turn.None;
    
    public Cart(int initialX, int initialY, Direction facing)
    {
        _initialX = initialX;
        _initialY = initialY;
        X = initialX;
        Y = initialY;
        Facing = facing;
    }
    
    private Direction MakeTurn(Direction currentDirection, Turn turn)
    {
        return (Direction)((360 + (int)currentDirection + (int)turn) % 360);
    }
    
    public void Move(Dictionary<(int x, int y), Node> map)
    {        
        switch (Facing)
        {
            case Direction.North:
                Y -= 1;
                break;
            case Direction.East:
                X += 1;
                break;
            case Direction.South:
                Y += 1;
                break;
            case Direction.West:
                X -= 1;
                break;
        }

        // Turn the cart if the new node requires it
        Node node = null;
        try
        {
            node = map[(X, Y)];
        }
        catch (Exception ex)
        {
            $"Error moving cart".Dump();
            this.Dump();
            ex.Dump();
        }
        switch (node.TrackType)
        {
            case NodeType.Intersection:
                Turn turn;
                switch (LastTurn)
                {
                    case Turn.None:
                    case Turn.Right:
                        turn = Turn.Left;
                        break;
                    case Turn.Straight:
                        turn = Turn.Right;
                        break;
                    case Turn.Left:                    
                        turn = Turn.Straight;
                        break;
                    default:
                        turn = Turn.Straight;
                        break;
                }
                Facing = MakeTurn(Facing, turn);
                LastTurn = turn;
                break;
            case NodeType.Corner:
                if (Facing == Direction.North || Facing == Direction.South)
                    Facing = node.East != null ? Direction.East : Direction.West;
                else // Direction == East or West
                    Facing = node.North != null ? Direction.North : Direction.South;
                break;
            
            case NodeType.Straight:
            default:
                break;
        }
    }
}

public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    public NodeType TrackType { get; set; } = NodeType.Straight;
    public Node North { get; set; }
    public Node East { get; set; }
    public Node South { get; set; }
    public Node West { get; set; }
    
    public Node(int x, int y)
    {
        X = x;
        Y = y;
    }
}
