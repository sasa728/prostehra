using Godot;
using System;

public partial class MainMenuButton : Button
{
	public override void _Ready()
	{
		this.Pressed += OnMainMenuButtonPressed;
		
	}
	private void OnMainMenuButtonPressed()
	{
		
	GetTree().ChangeSceneToFile("res://scenes/menu.tscn");

	}
	public override void _Process(double delta)
	{
	}
}
