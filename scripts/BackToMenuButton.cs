using Godot;
using System;

public partial class BackToMenuButton: Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Připojíme signál k metodě pro ukončení aplikace
		this.Pressed += OnBackToMenuButtonPressed;
		
	}

	// Funkce zavolána při stisknutí tlačítka
	private void OnBackToMenuButtonPressed()
	{
		
	GetTree().ChangeSceneToFile("res://scenes/menu.tscn");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
