using Godot;
using System;

public partial class CreditsButton : Button
{
	public override void _Ready()
	{
		// Připojíme signál k metodě pro ukončení aplikace
		this.Pressed += OnCreditsButtonPressed;
		
	}
	private void OnCreditsButtonPressed()
	{
		
	GetTree().ChangeSceneToFile("res://scenes/credits.tscn");

	}
	public override void _Process(double delta)
	{
	}
}
