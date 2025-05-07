using Godot;
using System;

public partial class StartButton : Button
{
	public override void _Ready()
	{
		this.Pressed += OnStartButtonPressed;
		
	}

	// Funkce zavolána při stisknutí tlačítka
	private void OnStartButtonPressed()
	{
		
	GetTree().ChangeSceneToFile("res://scenes/battle.tscn");

	}
	public override void _Process(double delta)
	{
	}
}
