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
	// Day 9

	// Input = 459 players; last marble is worth 71320 points
	var part1 = DoIt(459, 71320);
	var part2 = DoIt(459, 7132000);
	Console.WriteLine($"Part 1: {part1}");
	Console.WriteLine($"Part 2: {part2}");
}

public Int64 DoIt(int playerCount, int marbleCount)
{
	var scores = new Dictionary<int, Int64>();
	foreach (var p in Enumerable.Range(1, playerCount)) { scores[p] = 0; }
	var player = 0;
	var root = new Node { Value = 0 };
	root.Next = root; root.Previous = root;
	var current = root;
	foreach (var marble in Enumerable.Range(1, marbleCount))
	{
		player = player < playerCount ? player + 1 : 1;

		if (marble % 23 == 0)
		{
			var nd = current.Previous.Previous.Previous.Previous.Previous.Previous.Previous;
			scores[player] += marble + nd.Value;
			// Remove the node
			nd.Previous.Next = nd.Next;
			nd.Next.Previous = nd.Previous;
			current = nd.Next;
			nd = null;
			continue;
		}
		
		// Insert a new node
		var node = current.Next.Next;
		var newnode = new Node { Next = node, Previous = node.Previous, Value = marble };
		node.Previous.Next = newnode;
		node.Previous = newnode;
		current = newnode;
	}

	return scores.Max(kvp => kvp.Value);
}

public class Node
{
	public Node Next { get; set; }
	public Node Previous { get; set; }
	public int Value { get; set; }
}
