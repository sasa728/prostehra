using Godot;
using System;

public partial class StartButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Připojíme signál k metodě pro ukončení aplikace
		this.Pressed += OnStartButtonPressed;
		
	}

	// Funkce zavolána při stisknutí tlačítka
	private void OnStartButtonPressed()
	{
		
	GetTree().ChangeSceneToFile("res://scenes/battle.tscn");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
