using Godot;
using System;

public partial class CreditsButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Připojíme signál k metodě pro ukončení aplikace
		this.Pressed += OnCreditsButtonPressed;
		
	}

	// Funkce zavolána při stisknutí tlačítka
	private void OnCreditsButtonPressed()
	{
		
	GetTree().ChangeSceneToFile("res://scenes/credits.tscn");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
