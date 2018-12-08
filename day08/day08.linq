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
	// Day 8

	var inputname = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"day08-input.txt");
	var input = File.ReadAllText(inputname)
	//var input = "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2"  // Part 1 => 138
		.Split(new char[] {' '})
		.Select(s => int.Parse(s))
        .ToList<int>();

	var mtotal = 0;
	var stack = new Stack<Node>();
	var root = new Node(null);
	stack.Push(root);
	int index = 0;
	while (stack.Any())
	{
		var node = stack.Pop();
		if (node.ChildCount < 0)
		{
			node.ChildCount = input[index];
			node.MetaCount = input[index + 1];
			index += 2;
		}
		
		if (node.ChildCount > node.Children.Count)
		{
			stack.Push(node);
			for (var nindex = 0; nindex < node.ChildCount; nindex++)
			{
				var newnode = new Node(node);
				node.Children.Add(newnode);
				stack.Push(newnode);
			}
		}
		else
		{
			//Read in the metadata elements
			for (var mindex = 0; mindex < node.MetaCount; mindex++)
			{
				var meta = input[index];
				node.Metadata.Add(meta);
				mtotal += meta;
				index++;
			}
		}
	}
	Console.WriteLine($"Part 1: {mtotal}");
}

public class Node
{
	public Node Parent { get; }
	public int ChildCount { get; set; } = -1;
	public int MetaCount { get; set; } = -1;
	public List<Node> Children { get; } = new List<Node>();
	public List<int> Metadata { get; } = new List<int>();
	
	public Node(Node parent)
	{
		Parent = parent;
	}
}