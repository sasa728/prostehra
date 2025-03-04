using Godot;
using System;

public partial class Battle : Control
{
	// Called when the node enters the scene tree for the first time.
	 public override void _Ready()
	{
		PrintTree(GetTree().CurrentScene);
	}

	private void PrintTree(Node node, string indent = "")
	{
		GD.Print(indent + node.Name + " (" + node.GetType().Name + ")");

		foreach (Node child in node.GetChildren())
		{
			PrintTree(child, indent + "  ");
		}
	}
}
