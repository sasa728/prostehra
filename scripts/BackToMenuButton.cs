using Godot;
using System;

public partial class BackToMenuButton: Button
{
	public override void _Ready()
	{
		this.Pressed += OnBackToMenuButtonPressed;
		
	}

	private void OnBackToMenuButtonPressed()
	{
		
	GetTree().ChangeSceneToFile("res://scenes/menu.tscn");

	}
	public override void _Process(double delta)
	{
	}
}
